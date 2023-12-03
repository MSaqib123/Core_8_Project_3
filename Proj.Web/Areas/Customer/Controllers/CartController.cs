
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models.ViewModel;

namespace Proj.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork iUnit;
        public CartController(IUnitOfWork _iUnit)
        {
            iUnit = _iUnit;
        }
        public IActionResult Index()
        {
            ShoppingCartVM vm = new ShoppingCartVM();
            vm.ShoppingCartList = iUnit.ShoppingCart.GetAll();
            return View();
        }
    }
}
