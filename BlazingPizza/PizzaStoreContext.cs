using BlazingPizza;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza.Data;

public class PizzaStoreContext : DbContext
{
    public DbSet<Pizza> Pizzas { get; set; }

    public DbSet<Topping> Toppings { get; set; }

    public DbSet<PizzaSpecial> SpecialPizzas { get; set; }

    public DbSet<Order> Orders { get; set; }

    public PizzaStoreContext(DbContextOptions<PizzaStoreContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuring a many-to-many special => topping relationship that is friendly for serialization
        modelBuilder.Entity<PizzaTopping>().HasKey(pst => new { pst.PizzaId, pst.ToppingId });
        modelBuilder.Entity<PizzaTopping>().HasOne<Pizza>().WithMany(ps => ps.Toppings);
        modelBuilder.Entity<PizzaTopping>().HasOne(pst => pst.Topping).WithMany();
    }
}
