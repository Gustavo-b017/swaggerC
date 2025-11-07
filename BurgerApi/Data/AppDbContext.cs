using BurgerApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BurgerApi.Data;

/// <summary>
/// DbContext da aplicação (MySQL).
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Burger> Burgers => Set<Burger>();
    public DbSet<Topping> Toppings => Set<Topping>();
    public DbSet<BurgerTopping> BurgerToppings => Set<BurgerTopping>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Chave composta da N:N
        modelBuilder.Entity<BurgerTopping>()
            .HasKey(bt => new { bt.BurgerId, bt.ToppingId });

        modelBuilder.Entity<BurgerTopping>()
            .HasOne(bt => bt.Burger)
            .WithMany(b => b.BurgerToppings)
            .HasForeignKey(bt => bt.BurgerId);

        modelBuilder.Entity<BurgerTopping>()
            .HasOne(bt => bt.Topping)
            .WithMany(t => t.BurgerToppings)
            .HasForeignKey(bt => bt.ToppingId);

        // Seed simples (opcional)
        modelBuilder.Entity<Topping>().HasData(
            new Topping { Id = 1, Name = "Queijo", Price = 3.00m },
            new Topping { Id = 2, Name = "Bacon", Price = 4.50m },
            new Topping { Id = 3, Name = "Alface", Price = 1.50m }
        );

        modelBuilder.Entity<Burger>().HasData(
            new Burger { Id = 1, Name = "Clássico", BasePrice = 12.00m }
        );

        modelBuilder.Entity<BurgerTopping>().HasData(
            new BurgerTopping { BurgerId = 1, ToppingId = 1 } // Clássico + Queijo
        );
    }
}
