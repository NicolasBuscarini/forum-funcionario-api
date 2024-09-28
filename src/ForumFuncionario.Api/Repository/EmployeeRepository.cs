using Dapper;
using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Repository.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ForumFuncionario.Api.Repository
{
    public class EmployeeRepository(string connectionString) : IEmployeeRepository
    {
        public async Task<IEnumerable<Employee>> GetEmployeesByMonthAsync()
        {
            using IDbConnection dbConnection = new SqlConnection(connectionString);
            var sql = @"SELECT 
                                RA_NOME AS nome,  
                                DAY(RA_NASC) AS Dia,  
                                MONTH(RA_NASC) AS Mes,
                                CASE
                                    WHEN RA_FILIAL = '0101' THEN 'ANÁPOLIS-GO' 
                                    WHEN RA_FILIAL = '0103' THEN 'SÃO PAULO-SP'
                                    WHEN RA_FILIAL = '0201' THEN 'S.B.C-SP'
                                    ELSE 'OUTRO'
                                END AS filial, 
                                CTT_DESC01 AS Descricao
                            FROM SRA010 AS SRA 
                            INNER JOIN CTT010 AS CTT  
                                ON SUBSTRING(RA_FILIAL, 1, 2) = SUBSTRING(CTT_FILIAL, 1, 2) 
                                AND RA_CC = CTT_CUSTO 
                                AND CTT.D_E_L_E_T_ <> '*'
                            WHERE RA_SITFOLH IN ('', 'A', 'F') 
                                AND SRA.D_E_L_E_T_ <> '*'
                                AND MONTH(RA_NASC) = MONTH(GETDATE()) 
                            ORDER BY RA_FILIAL, DAY(RA_NASC)";

            return await dbConnection.QueryAsync<Employee>(sql);
        }
    }
}
