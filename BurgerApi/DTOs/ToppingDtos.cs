using System.ComponentModel.DataAnnotations;

namespace BurgerApi.DTOs;

public class CreateToppingDto
{
    [Required] public string Name { get; set; } = default!;
    [Range(0, 9999)] public decimal Price { get; set; }
}

public class UpdateToppingDto
{
    [Required] public string Name { get; set; } = default!;
    [Range(0, 9999)] public decimal Price { get; set; }
}

public class ToppingDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
}
