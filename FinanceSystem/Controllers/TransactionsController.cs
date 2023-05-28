using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinanceSystem.Data;
using FinanceSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FinanceSystem.Areas.Identity.Data;
using System.Numerics;

namespace FinanceSystem.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly FinanceSystemDbContext _db;
        private readonly IWebHostEnvironment _env;
        UserManager<FinanceSystemUser> _userManager;
        public TransactionsController(FinanceSystemDbContext context, IWebHostEnvironment env, UserManager<FinanceSystemUser> userManager)
        {
            _db = context;
            _env = env;
            _userManager = userManager;
        }


        // GET: Transactions
        public ActionResult Index()
        {
            var uId = _userManager.GetUserId(User);
            var userInfo =  _db.UserInfors.Find(uId);

            if (userInfo != null)
            {
                if (userInfo.FirstName == null || userInfo.LastName == null || userInfo.Image == null)
                {
                    // Chuyển hướng đến trang chỉnh sửa thông tin
                    return RedirectToAction("Edit", "UserInfors", new { id = userInfo.Id });
                }
            }
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var userId = _userManager.GetUserId(currentUser);
            var viewModel = new TransactionViewModel
            {
                Categories = _db.Categories.Where(x => x.UserId == userId).ToList(),
                Transactions = _db.Transactions.Where(x => x.Wallet.UserId == userId).ToList()
            };
            return View(viewModel);
        }
        public IActionResult FilterCategory(int? cate)
        {
            var list = new List<Transaction>();
            if (cate == null || cate == 0)
            {
                list = _db.Transactions.ToList();
            }
            else
            {
                list = _db.Transactions.Where(x => x.CategoryId == cate).ToList();
            }
            var viewModel = new TransactionViewModel
            {
                Transactions = list,
                Categories = _db.Categories.ToList()
            };
            var totalAmount = viewModel.Transactions.Sum(x => x.Amount);
            TempData["TotalAmount"] = totalAmount;
            return PartialView("_FilterCategory", viewModel);
        }
        public IActionResult Create()
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var userId = _userManager.GetUserId(currentUser);
            ViewData["CategoryId"] = new SelectList(_db.Categories.Where(x => x.UserId == userId), "Id", "Name");
            ViewData["WalletId"] = new SelectList(_db.Wallets.Where(x => x.UserId == userId), "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Create([Bind("TransactionId,WalletId,CategoryId,Amount,CreateDate,Image,Income,Note, Plan")] Transaction transaction)
        //    {                       
        //        System.Security.Claims.ClaimsPrincipal currentUser = this.User;
        //        var userId = _userManager.GetUserId(currentUser);
        //        transaction.ImageFile = Request.Form.Files["ImageFile"];
        //        if (transaction.ImageFile != null && transaction.ImageFile.Length > 0)
        //        {
        //            var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
        //            if (!Directory.Exists(uploadsFolder))
        //            {
        //                Directory.CreateDirectory(uploadsFolder);
        //            }
        //            var fileName = transaction.ImageFile.FileName;
        //            var filePath = Path.Combine(uploadsFolder, fileName);
        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await transaction.ImageFile.CopyToAsync(stream);
        //            }
        //            var relativePath = Path.GetRelativePath(_env.WebRootPath, filePath);
        //            var imageUrl = "/" + relativePath.Replace("\\", "/");
        //            filePath = imageUrl;
        //            transaction.Image = filePath;
        //        }

        //        bool? selectedIncome = transaction.Income;
        //        if (!selectedIncome.Value)
        //        {
        //            transaction.Amount *= -1;
        //        }
        //        if (ModelState.IsValid)
        //        {
        //            var wallet = _db.Wallets.FirstOrDefault(x => x.Id == transaction.WalletId);
        //            wallet.Balance -= transaction.Amount;
        //_db.Add(transaction);

        //            await _db.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            // Thêm transaction vào database
        //            _db.Add(transaction);
        //            await _db.SaveChangesAsync();

        //            // Lấy plan từ database
        //            var plan = await _db.Plan.FirstOrDefaultAsync();

        //            // Kiểm tra số tiền vượt quá giới hạn của Amount trong plan
        //            if (Math.Abs(transaction.Amount) > plan.Amount)
        //            {
        //                TempData["ErrorMessage"] = "Số tiền trong Transaction vượt quá giá trị của Amount Plan";
        //                return RedirectToAction(nameof(Index));
        //            }

        //            return RedirectToAction(nameof(Index));
        //        }
        //        return View(transaction);

        //    }
        public async Task<IActionResult> Create([Bind("TransactionId,WalletId,CategoryId,Amount,CreateDate,Image,Income,Note")] Transaction transaction)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var userId = _userManager.GetUserId(currentUser);
            transaction.ImageFile = Request.Form.Files["ImageFile"];
            if (transaction.ImageFile != null && transaction.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var fileName = transaction.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await transaction.ImageFile.CopyToAsync(stream);
                }
                var relativePath = Path.GetRelativePath(_env.WebRootPath, filePath);
                var imageUrl = "/" + relativePath.Replace("\\", "/");
                filePath = imageUrl;
                transaction.Image = filePath;
            }

            bool? selectedIncome = transaction.Income;
            if (selectedIncome.HasValue && !selectedIncome.Value)
            {
                transaction.Amount *= -1;
            }

            if (ModelState.IsValid)
            {
                var plans = await _db.Plan.ToListAsync();

                // Kiểm tra xem giao dịch có nằm trong khoảng thời gian của plan hay không
                bool isWithinAnyPlan = false;

                foreach (var plan in plans)
                {
                    if (!selectedIncome.HasValue || (selectedIncome.HasValue && !selectedIncome.Value && DateTime.Compare(transaction.CreateDate, plan.PlanDate) >= 0 && DateTime.Compare(transaction.CreateDate, plan.PlanDateEnd) <= 0))
                    {
                        // Tính tổng số tiền âm từ các giao dịch thuộc cùng khoảng thời gian với transaction hiện tại
                        decimal negativeAmount = await _db.Transactions
                            .Where(t => t.Amount < 0 && t.CreateDate >= plan.PlanDate && t.CreateDate <= plan.PlanDateEnd)
                            .SumAsync(t => t.Amount);

                        // Tính số tiền còn lại của Amount trong plan
                        decimal remainingAmount = plan.Amount + negativeAmount;

                        // Kiểm tra xem số tiền của giao dịch có lớn hơn số tiền còn lại trong plan hay không
                        if (Math.Abs(transaction.Amount) > remainingAmount)
                        {
                            TempData["ErrorMessage"] = "Số tiền bạn tiêu trong kế hoạch đề ra trong khoảng thời gian này đã quá mức cho phép rồi. Bạn đã tiêu vượt so với dự kiến " + (-remainingAmount).ToString("N0") + "đ";
                        }
                        {
                            var wallet = _db.Wallets.FirstOrDefault(x => x.Id == transaction.WalletId);
                            wallet.Balance -= transaction.Amount;

                            _db.Add(transaction);
                            await _db.SaveChangesAsync();

                            isWithinAnyPlan = true;
                            break;
                        }
                    }
                }

                if (!isWithinAnyPlan)
                {
                    var wallet = _db.Wallets.FirstOrDefault(x => x.Id == transaction.WalletId);
                    wallet.Balance -= transaction.Amount;

                    _db.Add(transaction);
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(transaction);
        }

            public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var userId = _userManager.GetUserId(currentUser);
            var transaction = await _db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_db.Categories, "Id", "Name", transaction.CategoryId);
            ViewData["WalletId"] = new SelectList(_db.Wallets, "Id", "Name", transaction.WalletId);
            TempData["Image"] = transaction.Image;
            transaction.Amount= Math.Abs(transaction.Amount);
            return PartialView("Edit", transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionId,WalletId,CategoryId,Amount,CreateDate,Income,Image,Note")] Transaction transaction)
        {
            if (id != transaction.TransactionId)
            {
                return BadRequest();
            }
            //transaction.WalletId = 10;
            if (ModelState.IsValid)
            {
                transaction.ImageFile = Request.Form.Files["ImageFile"];
                if (transaction.ImageFile != null && transaction.ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(transaction.ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await transaction.ImageFile.CopyToAsync(stream);
                    }
                    transaction.Image = "/Images/" + fileName;
                }

                bool? selectedIncome = transaction.Income;
                if (!selectedIncome.Value)
                {
                    transaction.Amount *= -1;
                }
                try
                {
                    _db.Update(transaction);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionId))
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

            ViewData["CategoryId"] = new SelectList(_db.Categories, "CategoryId", "CategoryName", transaction.CategoryId);
            ViewData["WalletId"] = new SelectList(_db.Wallets, "WalletId", "WalletName", transaction.WalletId);
            return PartialView("Edit", transaction);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            try
            {
                _db.Transactions.Remove(transaction);
                await _db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi xảy ra khi xóa sản phẩm: " + ex.Message });
            }
        }



        private bool TransactionExists(int id)
        {
            return (_db.Transactions?.Any(e => e.TransactionId == id)).GetValueOrDefault();
        }
    }
}
