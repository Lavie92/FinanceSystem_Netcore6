using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinanceSystem.Data;
using FinanceSystem.Models;
using Microsoft.AspNetCore.Identity;
using FinanceSystem.Areas.Identity.Data;

namespace FinanceSystem.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly FinanceSystemDbContext _context;
        UserManager<FinanceSystemUser> _userManager;
        public CategoriesController(FinanceSystemDbContext context, UserManager<FinanceSystemUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public string GetIdUser()
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var userId = _userManager.GetUserId(currentUser);
            return userId;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var userId = GetIdUser();
            return _context.Categories != null ? 
                          View(await _context.Categories.Where(x => x.UserId == userId).ToListAsync()) :
                          Problem("Entity set 'FinanceSystemDbContext.Categories'  is null.");
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userId = GetIdUser();
            if (id == null || _context.Categories == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (category == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Image")] Category category)
        {
            var userId = GetIdUser();
            category.UserId = userId;
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Image")] Category category)
        {
            var userId = GetIdUser();
            category.UserId = userId;
            if (id != category.Id)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return View("/Views/Shared/Error_404.cshtml");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }
            var userId = GetIdUser();

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (category == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'FinanceSystemDbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
