using BurgerApi.Domain;

namespace BurgerApi.Utils;

/// <summary>
/// Extensões para cálculo de preços.
/// </summary>
public static class PriceExtensions
{
    /// <summary>
    /// Preço total = BasePrice + soma dos Toppings.
    /// </summary>
    public static decimal CalculatePrice(this Burger b)
    {
        var toppingsSum = b.BurgerToppings?.Sum(bt => bt.Topping.Price) ?? 0m;
        return b.BasePrice + toppingsSum;
    }
}
