


using Dapper;
using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Repository.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ForumFuncionario.Api.Repository
{
    public class UserProtheusRepository(string connectionString) : IUserProtheusRepository
    {
        public async Task<UserProtheus?> GetUserProtheusByUsernameAsync(string username)
        {
            using IDbConnection dbConnection = new SqlConnection(connectionString);
            var sql = @"SELECT 
                            USR_CODIGO AS Username, 
                            USR_ID AS ProtheusId,
                            USR_EMAIL AS EMAIL
                        FROM SYS_USR 
                        WHERE D_E_L_E_T_ <> '*' 
                        AND USR_CODIGO = @username";

            // QuerySingleOrDefaultAsync retorna um único registro ou null
            return await dbConnection.QuerySingleOrDefaultAsync<UserProtheus>(sql, new { username });
        }
    }
}
