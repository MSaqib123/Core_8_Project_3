
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models.ViewModel;
using System.Security.Claims;

namespace Proj.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork iUnit;
        ShoppingCartVM vm = new ShoppingCartVM();

        public CartController(IUnitOfWork _iUnit)
        {
            iUnit = _iUnit;
        }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            vm.ShoppingCartList = iUnit.ShoppingCart.GetAll(x=>x.ApplicationUserId == userId , includeProperties : "Product");
            return View();
        }
    }
}
