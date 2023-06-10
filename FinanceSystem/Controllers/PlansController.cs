﻿using System;
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
using Microsoft.AspNetCore.Authorization;

namespace FinanceSystem.Controllers
{
	[Authorize(Roles ="Premium, Admin")]
    public class PlansController : Controller
    {
        private readonly FinanceSystemDbContext _context;
		UserManager<FinanceSystemUser> _userManager;
		public PlansController(FinanceSystemDbContext context, UserManager<FinanceSystemUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

		// GET: Plans
		public string GetIdUser()
		{
			System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var userId = _userManager.GetUserId(currentUser);
			return userId;
		}
		public async Task<IActionResult> Index()
        {
            var id = GetIdUser();
            var financeSystemDbContext = _context.Plan.Where(x => x.UserId == id).Include(p => p.User);
            return View(await financeSystemDbContext.ToListAsync());
        }

        // GET: Plans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userId = GetIdUser();
            if (id == null || _context.Plan == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }

            var plan = await _context.Plan
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plan == null)
            {
                return View("/Views/Shared/Error_404.cshtml"); 
			}

            return View(plan);
        }

        // GET: Plans/Create
        public IActionResult Create()
        {
           
            return View();
        }

        // POST: Plans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UserId,PlanDateStart,PlanDateEnd,Amount")] Plan plan)
        {
            string id = GetIdUser();
            plan.UserId= id;
            if (ModelState.IsValid)
            {
                _context.Add(plan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Set<FinanceSystemUser>(), "Id", "Id", plan.UserId);
            return View(plan);
        }

        // GET: Plans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Plan == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }
            var uid = GetIdUser();
            var plan = await _context.Plan.Where(x => x.User.Id == uid).FirstOrDefaultAsync(x => x.Id == id);
            if (plan == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }
            ViewData["UserId"] = new SelectList(_context.Set<FinanceSystemUser>(), "Id", "Id", plan.UserId);
            return View(plan);
        }

        // POST: Plans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UserId,PlanDateStart,PlanDateEnd,Amount")] Plan plan)
        {
            if (id != plan.Id)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }
                
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanExists(plan.Id))
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
            ViewData["UserId"] = new SelectList(_context.Set<FinanceSystemUser>(), "Id", "Id", plan.UserId);
            return View(plan);
        }

        // GET: Plans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Plan == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }

            var plan = await _context.Plan
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plan == null)
            {
                return View("/Views/Shared/Error_404.cshtml");
            }

            return View(plan);
        }

        // POST: Plans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Plan == null)
            {
                return Problem("Entity set 'FinanceSystemDbContext.Plan'  is null.");
            }
            var plan = await _context.Plan.FindAsync(id);
            if (plan != null)
            {
                _context.Plan.Remove(plan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePlanAmount()
        {
            // Lấy Plan từ database
            var plan = await _context.Plan.FirstOrDefaultAsync();

            if (plan != null)
            {
                // Lấy danh sách các transaction nằm trong khoảng thời gian của Plan và có Amount là số âm
                var transactions = await _context.Transactions
                    .Where(t => t.CreateDate >= plan.PlanDateStart && t.CreateDate <= plan.PlanDateEnd && t.Amount < 0)
                    .ToListAsync();

                // Tính tổng số tiền âm từ các transaction
                decimal negativeAmount = transactions.Sum(t => t.Amount);

                // Kiểm tra nếu tổng số tiền âm vượt quá giá trị của Amount Plan
                if (Math.Abs(negativeAmount) > plan.Amount)
                {
                    ModelState.AddModelError("", "Số tiền trong Transaction vượt quá giá trị của Amount Plan");
                    return View(); // Trả về view để hiển thị thông báo lỗi
                }

                // Cập nhật Amount trong Plan chỉ khi có số tiền âm
                if (negativeAmount < 0)
                {
                    // Cập nhật Amount trong Plan dựa trên tổng số tiền âm từ các transaction
                    plan.Amount += negativeAmount;

                    // Lưu thay đổi vào database
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index", "Plan"); // Chuyển hướng đến action Index của Plan
        }

        private bool PlanExists(int id)
        {
          return (_context.Plan?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
