using ForumFuncionario.Api.Repository;
using ForumFuncionario.Api.Repository.Interface;
using ForumFuncionario.Api.Service;
using ForumFuncionario.Api.Service.Interface;

namespace ForumFuncionario.Api.Config.Extensions
{
    public static class DependencyInjectionConfigurationExtensions
    {
        public static void ConfigureDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            // Injeção de dependência para repositórios e serviços
            services.AddSingleton<IEmployeeRepository>(provider =>
                new EmployeeRepository(configuration.GetConnectionString("DefaultConnection")!));
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddHttpContextAccessor();
        }
    }
}
