using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoffeeShop.Infrastructure;
using CoffeeShop.Infrastructure.Models;

namespace CoffeeShop.MVC.Controllers;

public class BaristasController : Controller
{
    private readonly CoffeeShopContext _context;

    public BaristasController(CoffeeShopContext context)
    {
        _context = context;
    }

    // GET: Baristas
    public async Task<IActionResult> Index()
    {
        return View(await _context.Baristas.ToListAsync());
    }

    // GET: Baristas/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var barista = await _context.Baristas
            .FirstOrDefaultAsync(m => m.Id == id);
        if (barista == null)
        {
            return NotFound();
        }

        return View(barista);
    }

    // GET: Baristas/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Baristas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FullName,ExperienceYears,Shift,HourlyRate,IsCertified")] BaristaModel barista)
    {
        if (ModelState.IsValid)
        {
            _context.Add(barista);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(barista);
    }

    // GET: Baristas/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var barista = await _context.Baristas.FindAsync(id);
        if (barista == null)
        {
            return NotFound();
        }
        return View(barista);
    }

    // POST: Baristas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,ExperienceYears,Shift,HourlyRate,IsCertified")] BaristaModel barista)
    {
        if (id != barista.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(barista);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BaristaExists(barista.Id))
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
        return View(barista);
    }

    // GET: Baristas/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var barista = await _context.Baristas
            .FirstOrDefaultAsync(m => m.Id == id);
        if (barista == null)
        {
            return NotFound();
        }

        return View(barista);
    }

    // POST: Baristas/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var barista = await _context.Baristas.FindAsync(id);
        if (barista != null)
        {
            _context.Baristas.Remove(barista);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool BaristaExists(int id)
    {
        return _context.Baristas.Any(e => e.Id == id);
    }
}
