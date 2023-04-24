using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.Controllers
{
    public class TransactionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
