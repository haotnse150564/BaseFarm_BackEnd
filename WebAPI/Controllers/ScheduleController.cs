using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class ScheduleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
