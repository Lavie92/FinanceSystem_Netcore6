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
using FinanceSystem.Areas.Admin.Models;

namespace FinanceSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserInforsController : Controller
    {
        private readonly FinanceSystemDbContext _context;
        private readonly UserManager<FinanceSystemUser> _userManager;
        public UserInforsController(FinanceSystemDbContext context, UserManager<FinanceSystemUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/UserInfors
        public async Task<IActionResult> Index()
        {
			var userViewModels = new List<UserViewModel>();
			var fUser = _context.Users.FirstOrDefault();
			var users = await _userManager.Users.Include(u => u.UserInfor).Include(u => u.Wallets).ToListAsync();
			var firstUser = (FinanceSystemUser)fUser;
			foreach (var user in users)
			{
                var userViewModel = new UserViewModel
                {
                    User = user,
                    UserInfor = user.UserInfor,
                    Wallets = user.Wallets.ToList(),
                    FirstUser = firstUser
				};

				userViewModels.Add(userViewModel);
			}
            return View(userViewModels);
		}


        // GET: Admin/UserInfors/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.UserInfors == null)
            {
                return NotFound();
            }
			var userViewModels = new List<UserViewModel>();

			var users = await _userManager.Users.Where(x => x.Id == id).Include(u => u.UserInfor).Include(u => u.Wallets).ToListAsync();
            TempData["Image"] = _context.UserInfors.FirstOrDefault(x => x.Id == id).Image;
            ViewData["UserId"] = id;
            foreach (var user in users)
			{
				var userViewModel = new UserViewModel
				{
					User = user,
					UserInfor = user.UserInfor,
					Wallets = user.Wallets.ToList(),
				};

				userViewModels.Add(userViewModel);
			}
			if (userViewModels == null)
            {
                return NotFound();
            }

            return PartialView("_Details", userViewModels);
        }

        // GET: Admin/UserInfors/Create
        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_context.Set<FinanceSystemUser>(), "Id", "Id");
            return View();
        }

        // POST: Admin/UserInfors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Gender,Birthdate,Image")] UserInfor userInfor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userInfor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id"] = new SelectList(_context.Set<FinanceSystemUser>(), "Id", "Id", userInfor.Id);
            return View(userInfor);
        }

        // GET: Admin/UserInfors/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.UserInfors == null)
            {
                return NotFound();
            }

            var userInfor = await _context.UserInfors.FindAsync(id);
            if (userInfor == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.Set<FinanceSystemUser>(), "Id", "Id", userInfor.Id);
            return View(userInfor);
        }

        // POST: Admin/UserInfors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,Gender,Birthdate,Image")] UserInfor userInfor)
        {
            if (id != userInfor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userInfor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserInforExists(userInfor.Id))
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
            ViewData["Id"] = new SelectList(_context.Set<FinanceSystemUser>(), "Id", "Id", userInfor.Id);
            return View(userInfor);
        }

        // GET: Admin/UserInfors/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.UserInfors == null)
            {
                return NotFound();
            }

            var userInfor = await _context.UserInfors
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userInfor == null)
            {
                return NotFound();
            }

            return View(userInfor);
        }

        // POST: Admin/UserInfors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.UserInfors == null)
            {
                return Problem("Entity set 'FinanceSystemDbContext.UserInfors'  is null.");
            }
            var userInfor = await _context.UserInfors.FindAsync(id);
            if (userInfor != null)
            {
                _context.UserInfors.Remove(userInfor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserInforExists(string id)
        {
          return (_context.UserInfors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
