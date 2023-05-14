using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceSystem.Models;
using Microsoft.AspNetCore.Identity;
using FinanceSystem.Data;
using FinanceSystem.Areas.Identity.Data;

namespace FinanceSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly FinanceSystemDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReportController(FinanceSystemDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Report
        public IActionResult Report()
        {
            return View();
        }

        public async Task<IActionResult> GetData()
        {
            var user = await _userManager.GetUserAsync(User);
            //string id = user.Id;
            string id = "1";

            var data = await _context.Transactions
             //.Where(t => t.CreateDate.HasValue && t.Amount.HasValue && t.Income.HasValue && t.Wallet.UserId == id)
             .Where(t => t.CreateDate != null && t.Amount != null && t.Income != null)

             .Select(t => new
             {
                 Date = t.CreateDate.ToString("yyyy-MM-dd"),
                 Income = t.Income,
                 Amount = t.Amount  
             })
             .ToListAsync();


            var result = new List<object>();

            result.Add(new { name = "Income", data = data.Where(d => d.Income).Select(d => new string[] { d.Date, d.Amount.ToString() }).ToArray() });
            result.Add(new { name = "Expense", data = data.Where(d => !d.Income).Select(d => new string[] { d.Date, d.Amount.ToString() }).ToArray() });

            return Json(result);

        }

        public IActionResult ReportCategory()
        {
            return View();
        }


        public async Task<IActionResult> GetDataCategory()
        {
            var user = await _userManager.GetUserAsync(User);
            //string id = user.Id;
            string id = "1";

            var data = await _context.Transactions.Where(x => x.WalletId == x.WalletId)
                    .GroupBy(t => t.Category.Name)
                    .Select(g => new { name = g.Key, count = g.Count() })
                    .ToListAsync();

            return Json(data);
        }
    }
}