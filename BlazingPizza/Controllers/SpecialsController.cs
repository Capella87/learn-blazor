using Microsoft.AspNetCore.Mvc;
using BlazingPizza;
using BlazingPizza.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza.Controllers;

[Route("specials")]
[ApiController]
public class SpecialsController : Controller
{
    private readonly PizzaStoreContext _db;

    public SpecialsController(PizzaStoreContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<PizzaSpecial>>> GetSpecials()
    {
        return (await _db.SpecialPizzas.ToListAsync()).OrderByDescending(s => s.BasePrice).ToList();
    }
}
