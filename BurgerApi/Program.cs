using System.Reflection;
using BurgerApi.Data;
using BurgerApi.Mappings;
using BurgerApi.Services.Implementations;
using BurgerApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ===== ConnString: Railway -> fallback appsettings =====
string? host = Environment.GetEnvironmentVariable("MYSQLHOST") ?? Environment.GetEnvironmentVariable("MYSQL_HOST");
string? port = Environment.GetEnvironmentVariable("MYSQLPORT") ?? Environment.GetEnvironmentVariable("MYSQL_PORT");
string? db   = Environment.GetEnvironmentVariable("MYSQLDATABASE") ?? Environment.GetEnvironmentVariable("MYSQL_DATABASE");
string? user = Environment.GetEnvironmentVariable("MYSQLUSER") ?? Environment.GetEnvironmentVariable("MYSQL_USER");
string? pass = Environment.GetEnvironmentVariable("MYSQLPASSWORD") 
               ?? Environment.GetEnvironmentVariable("MYSQL_PASSWORD") 
               ?? Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD");

string connStr;
if (!string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(db) && !string.IsNullOrWhiteSpace(user))
{
    port ??= "3306";
    pass ??= "";
    connStr = $"Server={host};Port={port};Database={db};User ID={user};Password={pass};" +
              "AllowPublicKeyRetrieval=True;SslMode=Preferred;TreatTinyAsBoolean=false;";
}
else
{
    // Dev local
    connStr = builder.Configuration.GetConnectionString("BurgerDb")!;
}

// ===== DI =====
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(connStr, ServerVersion.AutoDetect(connStr), my =>
        my.EnableRetryOnFailure(10, TimeSpan.FromSeconds(2), null)));

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IBurgerService, BurgerService>();
builder.Services.AddScoped<IToppingService, ToppingService>();

builder.Services.AddControllers().AddJsonOptions(_ => { });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Burger API",
        Version = "v1",
        Description = "API didática para montar hambúrguer, com foco em documentação Swagger (CRUD + N:N)."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

// ===== CORS (sem barra final!) =====
const string CorsPolicy = "AllowFront";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(CorsPolicy, p => p
        .WithOrigins(
            "http://localhost:5173",
            "http://127.0.0.1:5173",
            "http://127.0.0.1:5500",
            "https://hamburgueria-ten-kappa.vercel.app",
            "https://swaggerc-production.up.railway.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// ===== Swagger sempre ligado =====
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

// Health simples na raiz (ajuda o Railway e o front)
app.MapGet("/", () => Results.Ok(new { ok = true, service = "BurgerApi", time = DateTime.UtcNow }));

// ===== Migração com try/catch (não derruba o app) =====
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbCtx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbCtx.Database.Migrate();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "[Startup] Falha ao aplicar migrações.");
    }
}

app.Run();
