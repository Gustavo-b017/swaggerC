using System.Collections.Generic;

namespace BurgerApi.Domain;

/// <summary>
/// Topping (cobertura/ingrediente) com preço avulso.
/// </summary>
public class Topping
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }

    public ICollection<BurgerTopping> BurgerToppings { get; set; } = new List<BurgerTopping>();
}
