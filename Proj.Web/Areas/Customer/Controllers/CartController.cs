
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proj.Models.ViewModel;

namespace Proj.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            ShoppingCartVM vm = new ShoppingCartVM();
            return View();
        }
    }
}
