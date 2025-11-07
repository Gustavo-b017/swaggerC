namespace BurgerApi.Domain;

/// <summary>
/// Tabela de junção Burger x Topping (N:N).
/// </summary>
public class BurgerTopping
{
    public int BurgerId { get; set; }
    public Burger Burger { get; set; } = default!;

    public int ToppingId { get; set; }
    public Topping Topping { get; set; } = default!;
}
