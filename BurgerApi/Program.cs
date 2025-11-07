using System.Reflection;
using BurgerApi.Data;
using BurgerApi.Mappings;
using BurgerApi.Services.Implementations;
using BurgerApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// DbContext (MySQL)
var conn = builder.Configuration.GetConnectionString("BurgerDb");
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseMySql(conn, ServerVersion.AutoDetect(conn));
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Services
builder.Services.AddScoped<IBurgerService, BurgerService>();
builder.Services.AddScoped<IToppingService, ToppingService>();

// Controllers + Swagger
builder.Services.AddControllers()
    .AddJsonOptions(o => { /* mant�m padr�o; ideal pro front depois */ });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Burger API",
        Version = "v1",
        Description = "API did�tica para montar hamb�rguer, com foco em documenta��o Swagger (CRUD + N:N)."
    });

    // XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// CORS (deixa pronto pro front depois: Vite/React/Next)
const string CorsPolicy = "AllowLocalhostFront";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(CorsPolicy, p =>
        p.WithOrigins(
            "http://localhost:5173",
            "http://127.0.0.1:5173",
            "http://127.0.0.1:5500",
            "https://hamburgueria-ten-kappa.vercel.app/",
            "http://localhost:3000"
        )
         .AllowAnyHeader()
         .AllowAnyMethod());
});

var app = builder.Build();

// Swagger sempre ligado neste projeto did�tico
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Burger API v1");
    c.DisplayRequestDuration();
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
});

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.MapControllers();

// Aplica migra��es no start (conveniente pra dev)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
