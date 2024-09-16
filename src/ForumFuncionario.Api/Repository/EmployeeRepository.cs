using Dapper;
using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Repository.Interface;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ForumFuncionario.Api.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByMonthAsync()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT RA_NOME AS nome, RA_NASC AS Dt_Nasc, RA_FILIAL AS filial
                        FROM SRA010
                        WHERE RA_SITFOLH IN ('', 'A', 'F') AND D_E_L_E_T_ <> '*'
                        AND MONTH(RA_NASC) = MONTH(GETDATE())";

                return await dbConnection.QueryAsync<Employee>(sql);
            }
        }
    }
}