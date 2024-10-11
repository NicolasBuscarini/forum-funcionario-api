using ForumFuncionario.Api.Context;
using ForumFuncionario.Api.Repository;
using ForumFuncionario.Api.Repository.Interface;
using ForumFuncionario.Api.Service;
using ForumFuncionario.Api.Service.Interface;
using Microsoft.AspNetCore.Identity;

namespace ForumFuncionario.Api.Config.Extensions
{
    public static class DependencyInjectionConfigurationExtensions
    {
        public static void ConfigureDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            // Injeção de dependência para repositórios e serviços
            services.AddSingleton<IEmployeeRepository>(provider =>
                new EmployeeRepository(configuration.GetConnectionString("DefaultConnection")!));
            services.AddSingleton<IUserProtheusRepository>(provider =>
                new UserProtheusRepository(configuration.GetConnectionString("DefaultConnection")!));

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IRamalRepository, RamalRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IRamalService, RamalService>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, SmtpEmailService>();

            services.AddIdentity<UserApp, IdentityRole<int>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();


            services.AddHttpContextAccessor();
        }
    }
}
