using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinanceSystem.Data;
using FinanceSystem.Models;

namespace FinanceSystem.Controllers
{
    public class TargetSavingsController : Controller
    {
        private readonly FinanceSystemDbContext _context;

        public TargetSavingsController(FinanceSystemDbContext context)
        {
            _context = context;
        }

        // GET: TargetSavings
        public async Task<IActionResult> Index()
        {
              return _context.TargetSaving != null ? 
                          View(await _context.TargetSaving.ToListAsync()) :
                          Problem("Entity set 'FinanceSystemDbContext.TargetSaving'  is null.");
        }

        // GET: TargetSavings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TargetSaving == null)
            {
                return NotFound();
            }

            var targetSaving = await _context.TargetSaving
                .FirstOrDefaultAsync(m => m.Id == id);
            if (targetSaving == null)
            {
                return NotFound();
            }

            return View(targetSaving);
        }

        // GET: TargetSavings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TargetSavings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SaveDateStart,SaveDateEnd,TargetAmount,CurrentBalance")] TargetSaving targetSaving)
        {
            if (ModelState.IsValid)
            {
                _context.Add(targetSaving);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(targetSaving);
        }

        // GET: TargetSavings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TargetSaving == null)
            {
                return NotFound();
            }

            var targetSaving = await _context.TargetSaving.FindAsync(id);
            if (targetSaving == null)
            {
                return NotFound();
            }
            return View(targetSaving);
        }

        // POST: TargetSavings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,SaveDateStart,SaveDateEnd,TargetAmount,CurrentBalance")] TargetSaving targetSaving)
        {
            if (id != targetSaving.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(targetSaving);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TargetSavingExists(targetSaving.Id))
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
            return View(targetSaving);
        }

        // GET: TargetSavings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TargetSaving == null)
            {
                return NotFound();
            }

            var targetSaving = await _context.TargetSaving
                .FirstOrDefaultAsync(m => m.Id == id);
            if (targetSaving == null)
            {
                return NotFound();
            }

            return View(targetSaving);
        }

        // POST: TargetSavings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TargetSaving == null)
            {
                return Problem("Entity set 'FinanceSystemDbContext.TargetSaving'  is null.");
            }
            var targetSaving = await _context.TargetSaving.FindAsync(id);
            if (targetSaving != null)
            {
                _context.TargetSaving.Remove(targetSaving);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TargetSavingExists(int id)
        {
          return (_context.TargetSaving?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
