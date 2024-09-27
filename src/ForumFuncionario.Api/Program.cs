using ForumFuncionario.Api.Config.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao container
builder.Services.AddControllers();
builder.Services.ConfigureLogging();
builder.Services.ConfigureCors();
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureDependencyInjection(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.ConfigureJwtAuthentication(builder.Configuration);

var app = builder.Build();

// Configuração do pipeline HTTP
app.UseCors("AllowAllOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
