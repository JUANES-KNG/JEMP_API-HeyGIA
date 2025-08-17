using JEMP_API_HeyGIA.Data;
using JEMP_API_HeyGIA.Services;
using JEMP_API_HeyGIA.Services.RuletaApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Controladores
builder.Services.AddControllers();

// 2. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Base de datos (Se escogio SQLite por simplicidad)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=ruleta.db"));

// 4. Servicios (inyección de dependencias)
builder.Services.AddScoped<IRuletaService, RuletaService>();

var app = builder.Build();

// Middleware de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Seguridad y autorización (aunque aquí no se usa JWT)
app.UseAuthorization();

// Rutas
app.MapControllers();

app.Run();
