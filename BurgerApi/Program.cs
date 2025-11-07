using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using BurgerApiPT.Data;
using BurgerApiPT.Models;

var builder = WebApplication.CreateBuilder(args);

// Serviços essenciais
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("BurgerDb"));

// CORS simples (permite qualquer origem, header e método)
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Burger API (PT) — Adicionais",
        Version = "v1",
        Description = "API didática em .NET 8 para CRUD de Adicionais (itens extras do hambúrguer)."
    });
});

var app = builder.Build();

// Swagger 
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// IMPORTANTe: CORS antes dos controllers
app.UseCors();

app.MapControllers();

Seed(app);
app.Run();

static void Seed(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Adicionais.Any()) return;

    db.Adicionais.AddRange(
        new Adicional { Nome = "Queijo Cheddar", Preco = 3.50m, Ativo = true },
        new Adicional { Nome = "Bacon",          Preco = 4.00m, Ativo = true },
        new Adicional { Nome = "Cebola Crispy",  Preco = 2.50m, Ativo = true },
        new Adicional { Nome = "Picles",         Preco = 1.50m, Ativo = false }
    );
    db.SaveChanges();
}
