namespace BlazingPizzaEight.Extensions;

public static class OrderApiExtensions
{
    public static IEndpointRouteBuilder MapPizzaApi(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("specials", static async (PizzaStoreContext db) =>
        {
            var specials = await db.Specials.ToListAsync();
            return TypedResults.Ok(specials.OrderByDescending(s => s.BasePrice));
        });

        var orders = builder.MapGroup("orders");
        orders.MapGet("", static async (PizzaStoreContext db) =>
        {
            var orders = await db.Orders
                .Include(o => o.DeliveryAddress)
                .Include(o => o.Pizzas).ThenInclude(p => p.Special)
                .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
                .OrderByDescending(o => o.CreatedTime)
                .ToListAsync();

            return TypedResults.Ok(orders.Select(o => OrderWithStatus.FromOrder(o)).ToList());
        });

        orders.MapPost("", static async (PizzaStoreContext db, Order order) =>
        {
            order.CreatedTime = DateTime.Now;

            // Enforce existence of Pizza.SpecialId and Topping.ToppingId
            // in the database - prevent the submitter from making up
            // new specials and toppings
            foreach (var pizza in order.Pizzas)
            {
                pizza.SpecialId = pizza.Special?.Id ?? 0;
                pizza.Special = null!;
            }

            db.Orders.Attach(order);
            await db.SaveChangesAsync();

            return order.OrderId;
        });

        orders.MapGet("{orderId}", static async (PizzaStoreContext db, int orderId) =>
        {
            var order = await db.Orders
                .Where(o => o.OrderId == orderId)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.Pizzas).ThenInclude(p => p.Special)
                .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
                .SingleOrDefaultAsync();

            if (order is null)
            {
                return Results.NotFound();
            }

            return TypedResults.Ok(OrderWithStatus.FromOrder(order));
        });

        return builder;
    }
}
