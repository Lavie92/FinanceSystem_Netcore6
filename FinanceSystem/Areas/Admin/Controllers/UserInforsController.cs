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
using Microsoft.AspNetCore.Authorization;

namespace FinanceSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
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
        [HttpGet("UserInfors")]
        public async Task<IActionResult> Index()
        {
			var userViewModels = new List<UserViewModel>();
            var users = await _userManager.Users.ToListAsync();
            foreach (var item in users)
            {
                var userViewModel = new UserViewModel
                {
                    User = item,
                    UserInfor = _context.UserInfors.FirstOrDefault(x => x.Id == item.Id),
                    Wallets = _context.Wallets.Where(x => x.UserId == item.Id).ToList(),
                    Balance = _context.Wallets.Where(x => x.UserId == item.Id).Sum(x => x.Balance),
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
                return View("/Views/Shared/Error_404.cshtml");
            }
			
			var transaction = _context.Transactions.Where(x => x.Wallet.UserId == id).ToList();
			ViewBag.Total = _context.Wallets.Where(x => x.UserId == id).Sum(x => x.Balance);
			ViewBag.Image = _context.UserInfors.FirstOrDefault(x => x.Id == id).Image;
			ViewBag.Id = id;
            ViewBag.Name = _context.UserInfors.FirstOrDefault(x => x.Id == id).LastName;
			return View(transaction);
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
                return View("/Views/Shared/Error_404.cshtml");
            }

            var userInfor = await _context.UserInfors.FindAsync(id);
            if (userInfor == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
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
                return View("/Views/Shared/Error_404.cshtml");
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
                        return View("/Views/Shared/Error_404.cshtml");
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
                return View("/Views/Shared/Error_404.cshtml");
            }

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }
            else
            {
				var result = await _userManager.DeleteAsync(user);
				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
                else
                {
					return View("/Views/Shared/Error_404.cshtml");
				}
			}

            return View(user);
        }

        // POST: Admin/UserInfors/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    if (_context.UserInfors == null)
        //    {
        //        return Problem("Entity set 'FinanceSystemDbContext.UserInfors'  is null.");
        //    }
        //    var userInfor = await _context.UserInfors.FindAsync(id);
        //    if (userInfor != null)
        //    {
        //        _context.UserInfors.Remove(userInfor);
        //    }
            
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool UserInforExists(string id)
        {
          return (_context.UserInfors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
