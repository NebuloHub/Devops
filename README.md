# NebuloHub

### NebuloHub = O hub onde novas ideias nascem

Nebulo vem de “Nebulosa” — estruturas cósmicas que simbolizam nascimento, criação e potencial explosivo.
É uma metáfora perfeita para startups, que também nascem pequenas e podem se tornar gigantes.

Hub representa conexão, comunidade e ponto central de encontro.

NebuloHub é uma plataforma inteligente dedicada à descoberta, avaliação e conexão de startups.
Ao se cadastrar, cada startup seleciona suas habilidades e características principais, formando um perfil único dentro do ecossistema.

Com base em avaliações públicas e no desempenho de startups semelhantes, o NebuloHub utiliza Inteligência Artificial para estimar o potencial de sucesso de cada negócio. Usuários comuns podem criar contas, acessar um feed interativo, visualizar startups, deixar avaliações em estrelas e registrar comentários, contribuindo para a formação de uma comunidade ativa e colaborativa.

Assim como estrelas surgem dentro de nebulosas, o NebuloHub funciona como um ambiente onde novas ideias ganham forma, visibilidade e direção — guiadas por dados, tecnologia e avaliação coletiva.


---

Este projeto é a solução desenvolvida para o Global Solution de "Devops" da FIAP. usando .NET 

**Integrantes:**
* Vicenzo Massao - 554833 - 2TDSPM
* Erick Alves - 556862 - 2TDSPM
* Luiz Heimberg - 556864 - 2TDSPX

**Link do Repositório GitHub:**


**Link do Vídeo da Apresentação:**
[Link do Video de Devops]()


---


# 🤖 API RESTful

Este é um projeto de uma API RESTful desenvolvida em **ASP.NET Core**, armazena os dados que serão necessario para o projeto, como as startups e usuarios.
O sistema simula uma plataforma de controle de dados, com integração a banco de dados Oracle e uso de validações robustas via **FluentValidation**.

---

## 📌 Rotas Disponíveis

Todas as rotas estão disponíveis no controlador, por Exemplo: usuario, startup, avaliacao, habilidade, possui

| Método | Rota                   | Descrição                             |
|--------|------------------------|---------------------------------------|
| GET    | `/api/v2/habilidade`      | Retorna todos as habilidade por pagina  |
| GET    | `/api/v2/habilidade/{id}`   | Retorna uma habilidade por ID            |
| POST   | `/api/v2/habilidade`       | Cria uma nova habilidade                  |
| PUT    | `/api/v2/habilidade/{id}`   | Atualiza uma habilidade existente         |
| DELETE | `/api/v2/habilidade/{id}`   | Remove uma habilidade do sistema          |



---

## 🏗️ Justificativa da Arquitetura

O projeto foi desenvolvido utilizando **arquitetura em camadas**, com inspiração em **Clean Architecture**, para garantir separação de responsabilidades, fácil manutenção e escalabilidade:

- **Domain** → contém as entidades, enums e regras de negócio principais.  
- **Application** → concentra os DTOs, validações com FluentValidation e casos de uso (Use Cases).  
- **Infrastructure** → responsável pela persistência dos dados, configuração do **Entity Framework Core** e integração com **Oracle Database**.  
- **API** → camada de apresentação, expondo os endpoints REST por meio de controllers.  

Essa abordagem permite **maior testabilidade**, **baixo acoplamento** e facilita futuras mudanças ou integrações.

---

## 🧰 Tecnologias Utilizadas

- **.NET 8.0**
- **.NET 8 Runtime**
- **Entity Framework Core 8**
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.EntityFrameworkCore.Design`
  - `Microsoft.EntityFrameworkCore.Tools`
  - `Microsoft.EntityFrameworkCore.Proxies`
- **Oracle.EntityFrameworkCore** — Suporte ao Oracle Database  
- **FluentValidation.AspNetCore** — Validação de dados  
- **Swagger (Swashbuckle.AspNetCore + Filters + Annotations)** — Documentação da API  
- **AutoMapper** — Mapeamento entre entidades e DTOs  
