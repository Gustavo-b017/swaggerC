# Burger API (Swagger-Driven • .NET 8 • MySQL)

API didática para montar hambúrguer (CRUD + relacionamento N:N), com foco em **documentação no Swagger**.

## Stack
- ASP.NET Core Web API (.NET 8)
- EF Core 9 + Pomelo MySQL
- AutoMapper
- Swashbuckle (Swagger + Annotations)
- CORS pronto para `http://localhost:5173` e `http://localhost:3000`

## Requisitos
- .NET SDK 8+
- MySQL (XAMPP) rodando em `localhost:3306`

## Configuração

**appsettings.json**
```json
{
  "ConnectionStrings": {
    "BurgerDb": "Server=localhost;Port=3306;Database=burgerdb;User Id=root;Password=;CharSet=utf8mb4;TreatTinyAsBoolean=true;"
  },
  "AllowedHosts": "*"
}
