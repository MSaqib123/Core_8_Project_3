
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;
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
            ShoppingCartVM vm = new ShoppingCartVM();
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            vm.ShoppingCartList = iUnit.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product");

            vm.OrderHeader = new();

            //___ Total price base on Quantity ___
            foreach (var cart in vm.ShoppingCartList)
            {
                double price = GetPriceBaseOnQuantity(cart);
                cart.Price = price;
                vm.OrderHeader.OrderTotal += (price * cart.Count);
            }
            return View(vm);
        }
        
        
        //_____ minus Cart ______
        public IActionResult minus(int cartId)
        {
            var cartFromDb = iUnit.ShoppingCart.Get(u => u.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                iUnit.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                iUnit.ShoppingCart.Update(cartFromDb);
            }
            
            iUnit.SaveChange();
            return RedirectToAction("Index");
        }
        //_____ plus Cart ______
        public IActionResult plus(int cartId)
        {
            var cartFromDb = iUnit.ShoppingCart.Get(u=>u.Id == cartId);
            cartFromDb.Count += 1;
            iUnit.ShoppingCart.Update(cartFromDb);
            iUnit.SaveChange();
            return RedirectToAction("Index");
        }
        //_____ remove Cart ______
        public IActionResult remove(int cartId)
        {
            var cartFromDb = iUnit.ShoppingCart.Get(u => u.Id == cartId);
            iUnit.ShoppingCart.Remove(cartFromDb);
            iUnit.SaveChange();
            return RedirectToAction("Index");
        }

        //_____ CheckOut Screen ______
        public IActionResult CheckOut()
        {
            ShoppingCartVM vm = new ShoppingCartVM();
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            vm.OrderHeader = new();
            vm.ShoppingCartList = iUnit.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product");

            //___ Getting login user Data ____
            vm.OrderHeader.ApplicationUser = iUnit.ApplicationUser.Get(u => u.Id == userId);
            vm.OrderHeader.Name = vm.OrderHeader.ApplicationUser.Name;
            vm.OrderHeader.PhoneNumber = vm.OrderHeader.ApplicationUser.PhoneNumber;
            vm.OrderHeader.StreetAddress = vm.OrderHeader.ApplicationUser.StreetAddress;
            vm.OrderHeader.City = vm.OrderHeader.ApplicationUser.City;
            vm.OrderHeader.State = vm.OrderHeader.ApplicationUser.State;
            vm.OrderHeader.PostCode = vm.OrderHeader.ApplicationUser.PostalCode;

            //___ Total price base on Quantity ___
            foreach (var cart in vm.ShoppingCartList)
            {
                double price = GetPriceBaseOnQuantity(cart);
                cart.Price = price;
                vm.OrderHeader.OrderTotal += (price * cart.Count);
            }
            return View(vm);
        }

        private double GetPriceBaseOnQuantity(ShoppingCart obj)
        {
            if (obj.Count <= 50)
            {
                return obj.Product.Price;
            }
            else
            {
                if (obj.Count <= 100)
                {
                    return obj.Product.Price50;
                }
                else
                {
                    return obj.Product.Price100;
                }
            }

        }


    }
}
