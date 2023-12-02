using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;
using System.Security.Claims;

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
        [Authorize]
        [HttpPost]
        public IActionResult Detail(ShoppingCart obj)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            obj.ApplicationUserId = userId.ToString();

            //____ checking Cart productCart exist in DB _____
            var cartFromDb = _iUnit.ShoppingCart.Get(x=>x.ApplicationUserId == userId && x.productId == obj.productId);
            if (cartFromDb != null)
            {
                cartFromDb.Count += obj.Count;
                _iUnit.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                _iUnit.ShoppingCart.Add(obj);
            }
            _iUnit.SaveChange();
            return RedirectToAction(nameof(Detail));



        }
    }
}
