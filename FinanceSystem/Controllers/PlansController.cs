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

namespace FinanceSystem.Controllers
{
    public class PlansController : Controller
    {
        private readonly FinanceSystemDbContext _context;

        public PlansController(FinanceSystemDbContext context)
        {
            _context = context;
        }

        // GET: Plans
        public async Task<IActionResult> Index()
        {
            var financeSystemDbContext = _context.Plan.Include(p => p.User);
            return View(await financeSystemDbContext.ToListAsync());
        }

        // GET: Plans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Plan == null)
            {
                return NotFound();
            }

            var plan = await _context.Plan
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        // GET: Plans/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Set<FinanceSystemUser>(), "Id", "Id");
            return View();
        }

        // POST: Plans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UserId,PlanDate,PlanDateEnd,Amount")] Plan plan)
        {
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
                return NotFound();
            }

            var plan = await _context.Plan.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Set<FinanceSystemUser>(), "Id", "Id", plan.UserId);
            return View(plan);
        }

        // POST: Plans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UserId,PlanDate,PlanDateEnd,Amount")] Plan plan)
        {
            if (id != plan.Id)
            {
                return NotFound();
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
                        return NotFound();
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
                return NotFound();
            }

            var plan = await _context.Plan
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plan == null)
            {
                return NotFound();
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
                    .Where(t => t.CreateDate >= plan.PlanDate && t.CreateDate <= plan.PlanDateEnd && t.Amount < 0)
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
