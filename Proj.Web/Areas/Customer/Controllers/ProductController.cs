using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;
using Proj.Utility;
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
            s.Product.ProductImages = _iUnit.ProductImage.GetAll(x=>x.ProductId == id).ToList();
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
                //______________ Bug of Entity _________________ without update the product will update this is issue
                //_iUnit.ShoppingCart.Update(cartFromDb);

                //_____________ again adding update 
                _iUnit.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                _iUnit.ShoppingCart.Add(obj);
            }
            _iUnit.SaveChange();

            var cartCount = _iUnit.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count();
            HttpContext.Session.SetInt32(
                SD.SessionCart,
                cartCount
            );

            return RedirectToAction(nameof(Detail));
        }
    }
}
