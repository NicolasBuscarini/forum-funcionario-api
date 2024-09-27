using Dapper;
using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Repository.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ForumFuncionario.Api.Repository
{
    public class AuthRepository(string connectionString) : IAuthRepository
    {
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            using IDbConnection dbConnection = new SqlConnection(connectionString);
            var sql = @"SELECT * FROM SYS_USR WHERE USR_CODIGO = @Username AND USR_PSWMD5 = @Password";

            return await dbConnection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
        }
    }
}
