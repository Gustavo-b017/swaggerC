using System.Reflection;
using BurgerApi.Data;
using BurgerApi.Mappings;
using BurgerApi.Services.Implementations;
using BurgerApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ==== DbContext (MySQL) – usa as variáveis recomendadas do Railway ====
// Tenta primeiro a URL completa (se existir), depois os campos separados.
// Se nada disso vier, cai no appsettings.json (dev/local).
string? BuildRailwayMySqlConnectionString()
{
    // 1) Tenta URL completa (MYSQL_URL ou MYSQL_PUBLIC_URL)
    var rawUrl = Environment.GetEnvironmentVariable("MYSQL_URL")
                 ?? Environment.GetEnvironmentVariable("MYSQL_PUBLIC_URL");
    if (!string.IsNullOrWhiteSpace(rawUrl))
    {
        try
        {
            var uri = new Uri(rawUrl);
            var ui  = uri.UserInfo.Split(':', 2);
            var urlUser = ui.Length > 0 ? ui[0] : "";
            var urlPass = ui.Length > 1 ? ui[1] : "";
            var urlHost = uri.Host;
            var urlPort = uri.Port > 0 ? uri.Port.ToString() : "3306";
            var urlDb   = uri.AbsolutePath.TrimStart('/');

            if (!string.IsNullOrWhiteSpace(urlHost) && !string.IsNullOrWhiteSpace(urlDb))
                return $"Server={urlHost};Port={urlPort};Database={urlDb};User Id={urlUser};Password={urlPass};CharSet=utf8mb4;TreatTinyAsBoolean=true;";
        }
        catch
        {
            // se não parsear, cai pro plano B
        }
    }

    // 2) Campos separados
    string? First(params string?[] xs) => xs.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));

    var envHost = First(Environment.GetEnvironmentVariable("MYSQLHOST"),
                        Environment.GetEnvironmentVariable("MYSQL_HOST"));
    var envPort = First(Environment.GetEnvironmentVariable("MYSQLPORT"),
                        Environment.GetEnvironmentVariable("MYSQL_PORT")) ?? "3306";
    var envDb   = First(Environment.GetEnvironmentVariable("MYSQL_DATABASE"),
                        Environment.GetEnvironmentVariable("MYSQLDATABASE"));
    var envUser = First(Environment.GetEnvironmentVariable("MYSQLUSER"),
                        Environment.GetEnvironmentVariable("MYSQL_USER"));
    var envPass = First(Environment.GetEnvironmentVariable("MYSQLPASSWORD"),
                        Environment.GetEnvironmentVariable("MYSQL_PASSWORD"));

    if (!string.IsNullOrWhiteSpace(envHost) &&
        !string.IsNullOrWhiteSpace(envDb) &&
        !string.IsNullOrWhiteSpace(envUser))
    {
        return $"Server={envHost};Port={envPort};Database={envDb};User Id={envUser};Password={envPass};CharSet=utf8mb4;TreatTinyAsBoolean=true;";
    }

    // 3) Fallback dev/local
    return builder.Configuration.GetConnectionString("BurgerDb");
}

var conn = BuildRailwayMySqlConnectionString();

// Log leve (sem senha) p/ diagnóstico em deploy
try
{
    var safe = new System.Data.Common.DbConnectionStringBuilder { ConnectionString = conn! };
    if (safe.ContainsKey("Password")) safe["Password"] = "***";
    Console.WriteLine($"[DB] Using: {safe.ConnectionString}");
}
catch { /* ignore */ }

// Ajuste a versão se seu MySQL na Railway for diferente (geralmente 8.0.x)
var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseMySql(conn!, serverVersion, o => o.EnableRetryOnFailure(5));
});

// ==== AutoMapper & Services ====
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IBurgerService, BurgerService>();
builder.Services.AddScoped<IToppingService, ToppingService>();

// ==== Controllers + Swagger ====
builder.Services.AddControllers();
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

// ==== CORS (coloque seu domínio do front aqui) ====
const string CorsPolicy = "AllowFront";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(CorsPolicy, p => p
        .WithOrigins(
            "http://localhost:5173",
            "http://127.0.0.1:5173",
            "http://127.0.0.1:5500",
            "http://localhost:3000",
            "https://hamburgueria-ten-kappa.vercel.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// ==== Swagger sempre ligado (projeto didático) ====
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Burger API v1");
    c.DisplayRequestDuration();
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
});

// Em produção (container), não force HTTPS redirection
if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors(CorsPolicy);
app.MapControllers();

// ==== Bind da porta do Railway (PORT) ====
var portEnv = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(portEnv))
{
    app.Urls.Add($"http://0.0.0.0:{portEnv}");
    Console.WriteLine($"[Kestrel] Listening on 0.0.0.0:{portEnv}");
}

// ==== Migrações com retry simples ====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    for (var i = 1; i <= 10; i++)
    {
        try { db.Database.Migrate(); break; }
        catch (Exception ex) when (i < 10)
        {
            Console.WriteLine($"[Migrate] Tentativa {i} falhou: {ex.Message}. Aguardando 2s...");
            await Task.Delay(2000);
        }
    }
}

app.Run();
