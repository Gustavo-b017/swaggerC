using System.Collections.Generic;

namespace BurgerApi.Domain;

/// <summary>
/// Burger base com preço base e relação de coberturas (toppings).
/// </summary>
public class Burger
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal BasePrice { get; set; }

    public ICollection<BurgerTopping> BurgerToppings { get; set; } = new List<BurgerTopping>();
}
