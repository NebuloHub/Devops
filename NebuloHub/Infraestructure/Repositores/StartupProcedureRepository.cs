using System.Data;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace NebuloHub.Infraestructure.Repositories
{
    public class StartupProcedureRepository
    {
        private readonly string _connectionString;

        public StartupProcedureRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleNebuloHub");
        }

        public async Task<string> AnalisarStartupAsync(string cnpj)
        {
            using var connection = new OracleConnection(_connectionString);
            using var command = new OracleCommand("pkg_funcao2_validacao.analisar_startup", connection);
            command.CommandType = CommandType.StoredProcedure;

            var retorno = new OracleParameter("RETURN_VALUE", OracleDbType.Clob, ParameterDirection.ReturnValue);
            command.Parameters.Add(retorno);
            command.Parameters.Add("p_cnpj", OracleDbType.Varchar2, cnpj, ParameterDirection.Input);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            if (retorno.Value is OracleClob clob)
                return clob.Value;

            return string.Empty;
        }
    }
}
