# Burger API (PT) — CRUD de Adicionais com Swagger

## Requisitos
- .NET 8 SDK instalado (`dotnet --version`);
- (Opcional) Visual Studio Code.

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
## Organização do código
- `Program.cs` — configuração geral, Swagger e seed de dados.
- `Data/AppDbContext.cs` — EF Core InMemory.
- `Models/Adicional.cs` — entidade com validações.
- `Controllers/AdicionaisController.cs` — CRUD completo com anotações Swagger.

## Licença
MIT
