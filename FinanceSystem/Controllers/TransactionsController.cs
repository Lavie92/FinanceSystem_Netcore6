﻿using System;
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

namespace FinanceSystem.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly FinanceSystemDbContext _db;
        private readonly IWebHostEnvironment _env;
        UserManager<IdentityUser> _userManager;
        public TransactionsController(FinanceSystemDbContext context, IWebHostEnvironment env, UserManager<IdentityUser> userManager)
        {
            _db = context;
            _env = env;
            _userManager = userManager;
        }


        // GET: Transactions
        public ActionResult Index()
        {
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
            if (!selectedIncome.Value)
            {
                transaction.Amount *= -1;
            }
            if (ModelState.IsValid)
            {
                _db.Add(transaction);
                await _db.SaveChangesAsync();
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
