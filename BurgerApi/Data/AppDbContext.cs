using Microsoft.EntityFrameworkCore;
using BurgerApiPT.Models;

namespace BurgerApiPT.Data
{
    /// <summary>
    /// Contexto de dados da aplicação usando EF Core InMemory.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Coleção de Adicionais.
        /// </summary>
        public DbSet<Adicional> Adicionais => Set<Adicional>();
    }
}
