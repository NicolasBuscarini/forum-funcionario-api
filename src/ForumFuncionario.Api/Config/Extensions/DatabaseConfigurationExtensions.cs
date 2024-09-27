using ForumFuncionario.Api.Context;
using Microsoft.EntityFrameworkCore;

namespace ForumFuncionario.Api.Config.Extensions
{
    public static class DatabaseConfigurationExtensions
    {
        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("EntityConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)); // Configuração do provedor de banco de dados

            // Executa migrações pendentes, se existirem
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                {
                    if (dbContext.Database.GetPendingMigrations().Any())
                    {
                        dbContext.Database.Migrate();
                    }
                }
            }
        }
    }
}
