using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;

namespace Proj.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _iUnit;
        public ProductController(IUnitOfWork iUnit)
        {
            _iUnit = iUnit;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            ShoppingCart s = new ShoppingCart();
            s.Product = _iUnit.Product.Get(x => x.Id == id, includeProperties: "Category");
            s.productId = id;
            return View(s);
        }
        [HttpPost]
        public IActionResult Detail(ShoppingCart obj)
        {
            return View();
        }
    }
}
