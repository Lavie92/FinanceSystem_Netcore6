using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinanceSystem.Data;
using FinanceSystem.Models;
using FinanceSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace FinanceSystem.Controllers
{
    public class WalletsController : Controller
    {
        private readonly FinanceSystemDbContext _context;
        UserManager<IdentityUser> _userManager;
        public WalletsController(FinanceSystemDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

                // GET: Wallets
        public async Task<IActionResult> Index()
        {
            var userId = GetIdUser();
            var financeSystemDbContext = _context.Wallets.Where(x => x.UserId == userId);
            return View(await financeSystemDbContext.ToListAsync());
        }

        // GET: Wallets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Wallets == null)
            {
                return NotFound();
            }
            var userId = GetIdUser();
            var wallet = await _context.Wallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (wallet == null)
            {
                return NotFound();
            }

            return View(wallet);
        }

        // GET: Wallets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Wallets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Balance,Name")] Wallet wallet)
        {
            var userId = GetIdUser();
            wallet.UserId = userId;
            if (ModelState.IsValid)
            {
                _context.Add(wallet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wallet);
        }

        // GET: Wallets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Wallets == null)
            {
                return NotFound();
            }
            var userId = GetIdUser();
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }
            return View(wallet);
        }

        // POST: Wallets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Balance,Name")] Wallet wallet)
        {
            var userId = GetIdUser();
            if (id != wallet.Id)
            {
                return NotFound();
            }
            wallet.UserId = userId;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wallet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WalletExists(wallet.Id))
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
            ViewData["UserId"] = new SelectList(_context.Set<FinanceSystemUser>(), "Id", "Id", wallet.UserId);
            return View(wallet);
        }

        // GET: Wallets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Wallets == null)
            {
                return NotFound();
            }

            var userId = GetIdUser();
            var wallet = await _context.Wallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wallet == null)
            {
                return NotFound();
            }

            return View(wallet);
        }

        // POST: Wallets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Wallets == null)
            {
                return Problem("Entity set 'FinanceSystemDbContext.Wallets'  is null.");
            }
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet != null)
            {
                _context.Wallets.Remove(wallet);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WalletExists(int id)
        {
          return (_context.Wallets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public string GetIdUser() {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var userId = _userManager.GetUserId(currentUser);
            return userId;
        }
    }
}
