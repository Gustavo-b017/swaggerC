using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BurgerApi.DTOs;

/// <summary>
/// DTO para criação de Burger.
/// </summary>
public class CreateBurgerDto
{
    [Required] public string Name { get; set; } = default!;
    [Range(0, 9999)] public decimal BasePrice { get; set; }

    /// <summary>Opcional: toppings iniciais.</summary>
    public List<int>? ToppingIds { get; set; }
}

/// <summary>
/// DTO para atualização de Burger.
/// </summary>
public class UpdateBurgerDto
{
    [Required] public string Name { get; set; } = default!;
    [Range(0, 9999)] public decimal BasePrice { get; set; }
}

/// <summary>
/// DTO de leitura (inclui preço calculado e lista de toppings).
/// </summary>
public class BurgerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal BasePrice { get; set; }
    public decimal CalculatedPrice { get; set; }
    public List<ToppingDto> Toppings { get; set; } = new();
}
