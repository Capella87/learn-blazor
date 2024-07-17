using BlazingPizza;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza.Data;

public class PizzaStoreContext : DbContext
{
    public DbSet<PizzaSpecial> SpecialPizzas { get; set; }

    public PizzaStoreContext(DbContextOptions<PizzaStoreContext> options) : base(options) { }
}
