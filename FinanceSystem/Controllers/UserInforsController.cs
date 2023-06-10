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
    public class UserInforsController : Controller
    {
        private readonly FinanceSystemDbContext _context;
        UserManager<FinanceSystemUser> _userManager;
        private readonly SignInManager<FinanceSystemUser> _signInManager;
        public UserInforsController(FinanceSystemDbContext context, UserManager<FinanceSystemUser> userManager, SignInManager<FinanceSystemUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public string GetIdUser()
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var userId = _userManager.GetUserId(currentUser);
            return userId;
        }
        // GET: UserInfors
        public async Task<IActionResult> Index()
        {
            var userId = GetIdUser();
            var financeSystemDbContext = _context.UserInfors.Where(x => x.Id == userId);
            return View(await financeSystemDbContext.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_context.Set<FinanceSystemUser>(), "Id", "Id");
            return View();
        }

        // POST: UserInfors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Gender,Birthdate,Image")] UserInfor userInfor)
        {
            var userId = GetIdUser();
            userInfor.Id = userId;
            if (ModelState.IsValid)
            {
                _context.Add(userInfor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id"] = new SelectList(_context.Set<FinanceSystemUser>(), "Id", "Id", userInfor.Id);
            return View(userInfor);
        }

        // GET: UserInfors/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.UserInfors == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }
            var userId = GetIdUser();

            var userInfor = _context.UserInfors.FirstOrDefault(x => x.Id == userId && x.Id == id);
            if (userInfor == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }
            var viewModel = new AccountSettingViewModel
            {
                
            };
            TempData["Image"] = userInfor.Image;
            return View(userInfor);
        }

        // POST: UserInfors/Edit/5
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
            var image = userInfor.Image;
            if (ModelState.IsValid)
            {
                userInfor.ImageFile = Request.Form.Files["ImageFile"];
                if (userInfor.ImageFile != null && userInfor.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(userInfor.ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await userInfor.ImageFile.CopyToAsync(stream);
                    }
                    userInfor.Image = "/Images/" + fileName;
                    TempData["Image"] = userInfor.Image;
                }
                else
                {
                    userInfor.Image = image;
                    TempData["Image"] = userInfor.Image;
                }
                _context.Update(userInfor);
                await _context.SaveChangesAsync();

            }
            ViewData["Id"] = new SelectList(_context.Set<FinanceSystemUser>(), "Id", "Id", userInfor.Id);
            return RedirectToAction("Index", "Transactions");
        }

        // GET: UserInfors/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.UserInfors == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }

            var userInfor = await _context.UserInfors
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userInfor == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }

            return View(userInfor);
        }

        // POST: UserInfors/Delete/5
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
