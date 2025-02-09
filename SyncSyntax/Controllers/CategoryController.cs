using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SyncSyntax.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
