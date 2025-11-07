using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using BurgerApiPT.Data;
using BurgerApiPT.Models;

namespace BurgerApiPT.Controllers
{
    /// <summary>
    /// Endpoints de CRUD para Adicionais (itens extras do hambúrguer).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AdicionaisController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdicionaisController(AppDbContext db) => _db = db;

        /// <summary>Lista todos os adicionais.</summary>
        /// <remarks>Use o parâmetro <c>somenteAtivos=true</c> para filtrar apenas os disponíveis.</remarks>
        [HttpGet]
        [SwaggerOperation(Summary = "Listar adicionais", Description = "Retorna a lista de adicionais. Use 'somenteAtivos=true' para filtrar apenas os ativos.")]
        [ProducesResponseType(typeof(IEnumerable<Adicional>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Adicional>>> Get([FromQuery] bool somenteAtivos = false)
        {
            var query = _db.Adicionais.AsQueryable();
            if (somenteAtivos)
                query = query.Where(a => a.Ativo);

            var itens = await query.OrderBy(a => a.Nome).ToListAsync();
            return Ok(itens);
        }

        /// <summary>Obtém um adicional por Id.</summary>
        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Detalhar adicional", Description = "Retorna um adicional específico pelo seu Id.")]
        [ProducesResponseType(typeof(Adicional), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Adicional>> GetById(int id)
        {
            var adicional = await _db.Adicionais.FindAsync(id);
            return adicional is null ? NotFound() : Ok(adicional);
        }

        /// <summary>Cadastra um novo adicional.</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Criar adicional", Description = "Cria um novo adicional. Campos obrigatórios: Nome e Preco.")]
        [ProducesResponseType(typeof(Adicional), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Adicional>> Post([FromBody] Adicional dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            _db.Adicionais.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        /// <summary>Atualiza um adicional existente.</summary>
        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualizar adicional", Description = "Atualiza os dados de um adicional existente.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] Adicional dto)
        {
            if (id != dto.Id) return BadRequest("O Id do caminho não corresponde ao do corpo da requisição.");
            if (!await _db.Adicionais.AnyAsync(a => a.Id == id)) return NotFound();

            _db.Entry(dto).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Exclui um adicional.</summary>
        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Excluir adicional", Description = "Remove um adicional pelo Id.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var adicional = await _db.Adicionais.FindAsync(id);
            if (adicional is null) return NotFound();

            _db.Adicionais.Remove(adicional);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
