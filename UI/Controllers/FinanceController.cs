using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    public class FinanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
