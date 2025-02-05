using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RestauranteFinal.Models;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger (Apenas uma chamada de AddSwaggerGen)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Restaurante API",
        Description = "API para gestão de reservas de mesas em restaurantes",
        Contact = new OpenApiContact
        {
            Name = "Equipa Restaurante",
            Email = "contato@restaurantefinal.com",
            Url = new Uri("https://restaurantefinal.com")
        }
    });
});

// Configuração do banco de dados SQL Server
builder.Services.AddDbContext<ReservationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // REMOVENDO O COMENTÁRIO ERRADO

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware padrão
app.UseHttpsRedirection();
app.UseAuthorization();

// Configuração do Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurante API v1");
    c.RoutePrefix = string.Empty; // Swagger será acessado na raiz do site
});

// Mapeamento de controladores
app.MapControllers();

app.Run();
