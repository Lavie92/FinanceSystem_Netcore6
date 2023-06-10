using FinanceSystem.Areas.Identity.Data;
using FinanceSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FinanceSystem.Models.Order;
using FinanceSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace FinanceSystem.Controllers
{
	[Authorize]
	public class PremiumController : Controller
	{
        private readonly FinanceSystemDbContext _context;
        UserManager<FinanceSystemUser> _userManager;
        private IMomoService _momoService;
		public PremiumController(IMomoService momoService, FinanceSystemDbContext context,
        UserManager<FinanceSystemUser> userManager)
		{
			_context = context;
			_userManager = userManager;
			_momoService = momoService;
		}
		public IActionResult Index()
		{
			System.Security.Claims.ClaimsPrincipal currentUser = this.User;


			if (currentUser.IsInRole("Premium"))
			{
				TempData["IsPremium"] = "Tài khoản bạn đã là Premium!";
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreatePaymentUrl(OrderInfoModel model)
		{
			var price = 50000;
			model.Amount = price;
            var currentUser = await _userManager.GetUserAsync(User);
			model.FullName = currentUser.UserName;
			model.OrderInfo = "Nâng cấp tài khoản premium";
            var response = await _momoService.CreatePaymentAsync(model);
			return Redirect(response.PayUrl);
		}

		[HttpGet]
		public async Task<IActionResult> PaymentCallBack()
		{
            var currentUser = await _userManager.GetUserAsync(User);
            IdentityResult roleresult = await _userManager.AddToRoleAsync(currentUser, "Premium");
			var msg = "Chúc mừng bạn đã nâng cấp tài khoản premium thành công, vui lòng đăng nhập lại để sử dụng dịch vụ!!";
			TempData["UpgradeSuccess"] = msg;
            await _userManager.UpdateSecurityStampAsync(currentUser);
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account", new {area = "Identity"});
        }
	}
}
