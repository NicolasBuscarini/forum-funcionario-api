using ForumFuncionario.Api.Config.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Adiciona servi�os ao container
builder.Services.AddControllers();
builder.Services.ConfigureLogging();
builder.Services.ConfigureCors();
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureDependencyInjection(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.ConfigureJwtAuthentication(builder.Configuration);

var app = builder.Build();

// Configura��o do pipeline HTTP
app.UseCors("AllowAllOrigins");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.RoutePrefix = string.Empty; // Define a p�gina principal como o Swagger
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
