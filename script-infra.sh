#!/usr/bin/env bash
set -e

echo "======================================================"
echo " 🚀 Iniciando script de infraestrutura Nebulohub (.NET + SQL Server)"
echo "======================================================"

### ==========================================
### Variáveis
### ==========================================
RG="rg-nebulohub"
LOCATION="eastus2"

# SQL Server
SQL_SERVER="sqlserver-nebulohub"
DBNAME="nebuluhubdb"
DB_USER="adminsql"
DB_PASSWORD="SuaSenhaForte123!"

# App Service
APP_PLAN="planNebulohub"
APP_NAME="webapp-nebulohub"
RUNTIME="DOTNETCORE:8.0"

### ==========================================
### Grupo de Recursos
### ==========================================
echo ">> Verificando grupo de recursos..."
if [ "$(az group exists --name $RG)" = true ]; then
    echo "✔️ Grupo de recursos já existe: $RG"
else
    echo "🆕 Criando grupo de recursos: $RG..."
    az group create --name "$RG" --location "$LOCATION" >/dev/null
    echo "✔️ Criado!"
fi

### ==========================================
### SQL Server + Banco
### ==========================================
echo ">> Verificando SQL Server..."
if az sql server show --name "$SQL_SERVER" --resource-group "$RG" >/dev/null 2>&1; then
    echo "✔️ SQL Server já existe: $SQL_SERVER"
else
    echo "🆕 Criando SQL Server..."
    az sql server create \
        --name "$SQL_SERVER" \
        --resource-group "$RG" \
        --location "$LOCATION" \
        --admin-user "$DB_USER" \
        --admin-password "$DB_PASSWORD" \
        --enable-public-network true \
        --minimal-tls-version 1.2 >/dev/null
    echo "✔️ SQL Server criado!"
fi

echo ">> Verificando banco de dados..."
if az sql db show --name "$DBNAME" --server "$SQL_SERVER" --resource-group "$RG" >/dev/null 2>&1; then
    echo "✔️ Banco já existe: $DBNAME"
else
    echo "🆕 Criando banco de dados: $DBNAME..."
    az sql db create \
        --name "$DBNAME" \
        --server "$SQL_SERVER" \
        --resource-group "$RG" \
        --service-objective Basic \
        --backup-storage-redundancy Local >/dev/null
    echo "✔️ Banco criado!"
fi

echo ">> Configurando firewall para acesso público..."
az sql server firewall-rule create \
    --name AllowAll \
    --server "$SQL_SERVER" \
    --resource-group "$RG" \
    --start-ip-address 0.0.0.0 \
    --end-ip-address 255.255.255.255 \
    --output none || echo "✔️ Regra de firewall já existe"

### ==========================================
### App Service Plan
### ==========================================
echo ">> Verificando App Service Plan..."
if az appservice plan show --name "$APP_PLAN" --resource-group "$RG" >/dev/null 2>&1; then
    echo "✔️ App Service Plan já existe: $APP_PLAN"
else
    echo "🆕 Criando App Service Plan..."
    az appservice plan create \
        --name "$APP_PLAN" \
        --resource-group "$RG" \
        --is-linux \
        --sku F1 >/dev/null
    echo "✔️ Plano criado!"
fi

### ==========================================
### Web App
### ==========================================
echo ">> Verificando Web App..."
if az webapp show --name "$APP_NAME" --resource-group "$RG" >/dev/null 2>&1; then
    echo "✔️ Web App já existe: $APP_NAME"
else
    echo "🆕 Criando Web App (.NET)..."
    az webapp create \
        --name "$APP_NAME" \
        --resource-group "$RG" \
        --plan "$APP_PLAN" \
        --runtime "$RUNTIME" >/dev/null
    echo "✔️ Web App criado!"
fi

### ==========================================
### Configurar Connection String
### ==========================================
CONNECTION_STRING="Server=tcp:${SQL_SERVER}.database.windows.net,1433;Database=${DBNAME};User ID=${DB_USER};Password=${DB_PASSWORD};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

echo ">> Configurando connection string no Web App..."
az webapp config connection-string set \
    --name "$APP_NAME" \
    --resource-group "$RG" \
    --settings DefaultConnection="$CONNECTION_STRING" \
    --connection-string-type SQLAzure

### ==========================================
### Reiniciar Web App
### ==========================================
echo ">> Reiniciando Web App..."
az webapp restart --name "$APP_NAME" --resource-group "$RG"

echo "======================================================"
echo " 🎉 Infraestrutura .NET + SQL Server criada com sucesso!"
echo "======================================================"
echo "Web App: https://${APP_NAME}.azurewebsites.net"
echo "Banco: ${DBNAME}"
echo "Servidor SQL: ${SQL_SERVER}.database.windows.net"
echo "Usuário: ${DB_USER}"
echo "======================================================"
