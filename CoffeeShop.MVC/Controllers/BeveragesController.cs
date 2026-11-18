using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoffeeShop.Infrastructure;
using CoffeeShop.Infrastructure.Models;

namespace CoffeeShop.MVC.Controllers;

public class BeveragesController : Controller
{
    private readonly CoffeeShopContext _context;

    public BeveragesController(CoffeeShopContext context)
    {
        _context = context;
    }

    // GET: Beverages
    public async Task<IActionResult> Index()
    {
        return View(await _context.Beverages.ToListAsync());
    }

    // GET: Beverages/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var beverage = await _context.Beverages
            .FirstOrDefaultAsync(m => m.Id == id);
        if (beverage == null)
        {
            return NotFound();
        }

        return View(beverage);
    }

    // GET: Beverages/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Beverages/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Price,VolumeMl,IsAvailable")] BeverageModel beverage)
    {
        if (ModelState.IsValid)
        {
            _context.Add(beverage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(beverage);
    }

    // GET: Beverages/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var beverage = await _context.Beverages.FindAsync(id);
        if (beverage == null)
        {
            return NotFound();
        }
        return View(beverage);
    }

    // POST: Beverages/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,VolumeMl,IsAvailable")] BeverageModel beverage)
    {
        if (id != beverage.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(beverage);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BeverageExists(beverage.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(beverage);
    }

    // GET: Beverages/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var beverage = await _context.Beverages
            .FirstOrDefaultAsync(m => m.Id == id);
        if (beverage == null)
        {
            return NotFound();
        }

        return View(beverage);
    }

    // POST: Beverages/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var beverage = await _context.Beverages.FindAsync(id);
        if (beverage != null)
        {
            _context.Beverages.Remove(beverage);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool BeverageExists(int id)
    {
        return _context.Beverages.Any(e => e.Id == id);
    }
}
