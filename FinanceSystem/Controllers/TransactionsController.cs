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
using System.Net.Mail;
using System.Net;
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
    
     
        private async Task<OkResult> SendInsufficientFundsEmail(string recipientEmail, decimal remainingAmount, string startDate, string endDate, string Name)
        {
            // Cấu hình thông tin người gửi
            string senderEmail = "hutechfinancesystem@gmail.com";
            string senderPassword = "cjblsplncbgzuzrx";
            string senderDisplayName = "Your App";

            // Cấu hình máy chủ SMTP
            string smtpHost = "smtp.gmail.com";
            int smtpPort = 587;

            // Tạo đối tượng MailMessage
            MailMessage message = new MailMessage();
            message.From = new MailAddress(senderEmail, senderDisplayName);
            message.To.Add(new MailAddress(recipientEmail));
            message.Subject = "Thông báo hết tiền";
            message.Body = $"Số tiền bạn tiêu trong kế hoạch '{Name}' từ ngày {startDate} đến ngày {endDate} đã vượt qua mức cho phép. Số tiền vượt quá là {(-remainingAmount).ToString("N0")} đồng.";
            // Khởi tạo đối tượng SmtpClient
            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtpClient.EnableSsl = true;

            // Gửi email
            await smtpClient.SendMailAsync(message);
            return Ok();
        }
        private async Task SendSuccessSavingsEmail(string recipientEmail, string savingName)
        {
            // Cấu hình thông tin người gửi
            string senderEmail = "hutechfinancesystem@gmail.com";
            string senderPassword = "cjblsplncbgzuzrx";
            string senderDisplayName = "Your App";

            // Cấu hình máy chủ SMTP
            string smtpHost = "smtp.gmail.com";
            int smtpPort = 587;

            // Tạo đối tượng MailMessage
            MailMessage message = new MailMessage();
            message.From = new MailAddress(senderEmail, senderDisplayName);
            message.To.Add(new MailAddress(recipientEmail));
            message.Subject = "Thông báo tiết kiệm thành công";
            message.Body = $"Chúc mừng! Bạn đã hoàn thành tiết kiệm '{savingName}' thành công.";

            // Khởi tạo đối tượng SmtpClient
            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtpClient.EnableSsl = true;

            // Gửi email
            await smtpClient.SendMailAsync(message);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,WalletId,CategoryId,Amount,CreateDate,Image,Income,Note")] Transaction transaction)
        {
			System.Security.Claims.ClaimsPrincipal currentUser = this.User;
			var userId = _userManager.GetUserId(currentUser);
			var targetSavings = await _db.TargetSaving.OrderBy(ts => ts.SaveDateStart).ToListAsync();
            bool targetAchieved = false; // Biến để theo dõi trạng thái mục tiêu đã đạt được

            foreach (var targetSaving in targetSavings)
            {
                if (transaction.Income && DateTime.Compare(transaction.CreateDate, targetSaving.SaveDateStart) >= 0 && DateTime.Compare(transaction.CreateDate, targetSaving.SaveDateEnd) <= 0)
                {
                    decimal savingAmount = transaction.Amount * 0.05m; // Lấy 5% số tiền giao dịch

                    // Giảm 5% số tiền giao dịch
                    transaction.Amount -= savingAmount;

                    if (!targetAchieved && targetSaving.CurrentBalance < targetSaving.TargetAmount)
                    {
                        targetSaving.CurrentBalance += savingAmount;

                        if (targetSaving.CurrentBalance >= targetSaving.TargetAmount)
                        {
                            var currentLoggedInUser = await _userManager.GetUserAsync(User);
                            string recipientEmail = currentLoggedInUser.Email;
                            string savingName = targetSaving.Name;

                            await SendSuccessSavingsEmail(recipientEmail, savingName);
                            TempData["SuccessMessage"] = "Tiết kiệm thành công! Bạn đã đạt được mục tiêu tiết kiệm"+ savingName;
                            targetAchieved = true; // Đánh dấu mục tiêu đã đạt được
                        }
                        if (targetSaving.CurrentBalance > targetSaving.TargetAmount)
                        {
                            decimal excessAmount = targetSaving.CurrentBalance - targetSaving.TargetAmount;
                            targetSaving.CurrentBalance -= excessAmount;
                            transaction.Amount += excessAmount;
                        }
                        _db.Update(targetSaving);
                        break; // Ngừng vòng lặp sau khi hoàn thành mục tiêu
                    }
                }
            }

            // Thực hiện các bước xử lý khi không phải giao dịch thu nhập (expense)
            
              
                transaction.ImageFile = Request.Form.Files["ImageFile"];
                if (transaction.ImageFile != null && transaction.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                    Directory.CreateDirectory(uploadsFolder);
                    var fileName = transaction.ImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await transaction.ImageFile.CopyToAsync(stream);
                    }
                    var relativePath = Path.GetRelativePath(_env.WebRootPath, filePath);
                    var imageUrl = "/" + relativePath.Replace("\\", "/");
                    transaction.Image = imageUrl;
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
                    if (!selectedIncome.HasValue || (selectedIncome.HasValue && !selectedIncome.Value && DateTime.Compare(transaction.CreateDate, plan.PlanDateStart) >= 0 && DateTime.Compare(transaction.CreateDate, plan.PlanDateEnd) <= 0))
                    {
                        // Tính tổng số tiền âm từ các giao dịch thuộc cùng khoảng thời gian với transaction hiện tại
                        decimal negativeAmount = await _db.Transactions
                            .Where(t => t.Amount < 0 && t.CreateDate >= plan.PlanDateStart && t.CreateDate <= plan.PlanDateEnd && t.Wallet.UserId == userId)
                            .SumAsync(t => t.Amount);

                        // Tính số tiền còn lại của Amount trong plan
                        decimal remainingAmount = plan.Amount + negativeAmount;

                        // Kiểm tra xem số tiền của giao dịch có lớn hơn số tiền còn lại trong plan hay không
                        if ((Math.Abs(transaction.Amount) - negativeAmount) > plan.Amount)
                        {
                            var currentLoggedInUser = await _userManager.GetUserAsync(User);
                            var userEmail = currentLoggedInUser.Email;

                            string startDate = plan.PlanDateStart.ToString("dd/MM/yyyy");
                            string endDate = plan.PlanDateEnd.ToString("dd/MM/yyyy");
                            string planName = plan.Name;
                            remainingAmount += transaction.Amount;
                            await SendInsufficientFundsEmail(userEmail, remainingAmount, startDate, endDate, planName);

                            TempData["ErrorMessage"] = "Số tiền bạn tiêu trong kế hoạch '" + planName + "' từ ngày " + startDate + " đến ngày " + endDate + " đã quá mức cho phép rồi. Bạn đã tiêu vượt so với dự kiến " + (-remainingAmount).ToString("N0") + "đ";
                            var wallet = _db.Wallets.FirstOrDefault(x => x.Id == transaction.WalletId);
                            wallet.Balance -= transaction.Amount;

                            _db.Add(transaction);
                            await _db.SaveChangesAsync();

                            isWithinAnyPlan = true;
							return RedirectToAction(nameof(Index));

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
