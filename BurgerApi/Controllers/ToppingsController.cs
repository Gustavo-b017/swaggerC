using BurgerApi.DTOs;
using BurgerApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BurgerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Operações de CRUD para Toppings (ingredientes).")]
public class ToppingsController : ControllerBase
{
    private readonly IToppingService _service;

    public ToppingsController(IToppingService service) => _service = service;

    /// <summary>Lista todos os toppings.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Listar toppings", Description = "Retorna a lista de todos os toppings.")]
    [ProducesResponseType(typeof(List<ToppingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ToppingDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    /// <summary>Obtém um topping por id.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Obter topping por id")]
    [ProducesResponseType(typeof(ToppingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ToppingDto>> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>Cria um novo topping.</summary>
    /// <remarks>
    /// Exemplo de body:
    /// 
    /// ```json
    /// { "name": "Tomate", "price": 1.20 }
    /// ```
    /// </remarks>
    [HttpPost]
    [SwaggerOperation(Summary = "Criar topping")]
    [ProducesResponseType(typeof(ToppingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ToppingDto>> Create([FromBody] CreateToppingDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Atualiza um topping existente.</summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Atualizar topping")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateToppingDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var ok = await _service.UpdateAsync(id, dto);
        return ok ? NoContent() : NotFound();
    }

    /// <summary>Exclui um topping.</summary>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Excluir topping")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
