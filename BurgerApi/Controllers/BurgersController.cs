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
[SwaggerTag("CRUD de Burgers e endpoints para montar/desmontar toppings.")]
public class BurgersController : ControllerBase
{
    private readonly IBurgerService _service;

    public BurgersController(IBurgerService service) => _service = service;

    /// <summary>Lista todos os burgers (com toppings e preço calculado).</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Listar burgers")]
    [ProducesResponseType(typeof(List<BurgerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BurgerDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    /// <summary>Obtém um burger por id.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Obter burger por id")]
    [ProducesResponseType(typeof(BurgerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BurgerDto>> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>Cria um novo burger (pode incluir toppings iniciais).</summary>
    /// <remarks>
    /// Exemplo:
    /// 
    /// ```json
    /// { "name": "Duplo", "basePrice": 15.00, "toppingIds": [1,2] }
    /// ```
    /// </remarks>
    [HttpPost]
    [SwaggerOperation(Summary = "Criar burger")]
    [ProducesResponseType(typeof(BurgerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BurgerDto>> Create([FromBody] CreateBurgerDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Atualiza nome e preço base.</summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Atualizar burger")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBurgerDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var ok = await _service.UpdateAsync(id, dto);
        return ok ? NoContent() : NotFound();
    }

    /// <summary>Exclui um burger.</summary>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Excluir burger")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }

    /// <summary>Adiciona um topping ao burger.</summary>
    [HttpPost("{id:int}/toppings/{toppingId:int}")]
    [SwaggerOperation(Summary = "Adicionar topping ao burger")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTopping(int id, int toppingId)
    {
        var ok = await _service.AddToppingAsync(id, toppingId);
        return ok ? NoContent() : NotFound();
    }

    /// <summary>Remove um topping do burger.</summary>
    [HttpDelete("{id:int}/toppings/{toppingId:int}")]
    [SwaggerOperation(Summary = "Remover topping do burger")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTopping(int id, int toppingId)
    {
        var ok = await _service.RemoveToppingAsync(id, toppingId);
        return ok ? NoContent() : NotFound();
    }
}
