namespace ForumFuncionario.Api.Config.Extensions
{
    public static class LoggingConfigurationExtensions
    {
        public static void ConfigureLogging(this IServiceCollection services)
        {
            var builder = WebApplication.CreateBuilder();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(); // Adiciona logging no console
        }
    }
}
