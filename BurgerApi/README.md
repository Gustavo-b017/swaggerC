# Burger API (PT) — CRUD de Adicionais com Swagger

API didática, mínima e **100% em português** para CRUD de *Adicionais* (antes chamados de **Toppings**), com foco na documentação **Swagger**.

## Objetivos do redesign
- Renomear completamente o conceito **Topping → Adicional** (classe, rotas, comentários e Swagger);
- Código **simples**, **enxuto** e **bem comentado**, próprio para quem está começando em C#;
- Swagger configurado e rico em informações (sumário, descrições, exemplos via anotações);
- Evitar complexidade de banco: **EF Core InMemory** (sem migrações).

## Requisitos
- .NET 8 SDK instalado (`dotnet --version`);
- (Opcional) Visual Studio Code + extensões C#.

## Como executar
```bash
dotnet restore
dotnet run
```

Abra o navegador em: **https://localhost:5001/swagger** (ou **http://localhost:5000/swagger**).

## Endpoints principais
- `GET /api/adicionais` — lista; `?somenteAtivos=true` filtra só os ativos
- `GET /api/adicionais/{id}` — detalhe
- `POST /api/adicionais` — cria
- `PUT /api/adicionais/{id}` — atualiza
- `DELETE /api/adicionais/{id}` — exclui

### Exemplo de corpo (POST/PUT)
```json
{
  "id": 0,
  "nome": "Queijo Cheddar",
  "preco": 3.5,
  "ativo": true
}
```

## Mapeamento de renomeação (seu projeto antigo)
| Antes (EN) | Agora (PT)  |
|------------|-------------|
| Topping    | Adicional   |
| ToppingId  | Id          |
| Name       | Nome        |
| Price      | Preco       |
| IsAvailable| Ativo       |

> Se você já tinha **MySQL/XAMPP**, pode manter esta API em memória só para a documentação e depois trocar `UseInMemoryDatabase` por `UseMySql` ou `UseSqlServer`. O código dos controllers muda pouco.

## Organização do código
- `Program.cs` — configuração geral, Swagger e seed de dados.
- `Data/AppDbContext.cs` — EF Core InMemory.
- `Models/Adicional.cs` — entidade com validações.
- `Controllers/AdicionaisController.cs` — CRUD completo com anotações Swagger.

## Licença
MIT
