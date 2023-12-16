
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;
using Proj.Models.ViewModel;
using Proj.Utility;
using System.Security.Claims;

namespace Proj.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork iUnit;
        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }

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
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM.OrderHeader = new();
            shoppingCartVM.ShoppingCartList = iUnit.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product");

            //___ Getting login user Data ____
            shoppingCartVM.OrderHeader.ApplicationUser = iUnit.ApplicationUser.Get(u => u.Id == userId);
            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
            shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
            shoppingCartVM.OrderHeader.PostCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            //___ Total price base on Quantity ___
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                double price = GetPriceBaseOnQuantity(cart);
                cart.Price = price;
                shoppingCartVM.OrderHeader.OrderTotal += (price * cart.Count);
            }
            return View(shoppingCartVM);
        }

        [HttpPost]
        [ActionName("CheckOut")]
        public IActionResult CheckOutPost()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM.ShoppingCartList = iUnit.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product");

            /*___________________ Shopping Header _______________________*/
            #region Header
            //___ Getting login user Data ____
            shoppingCartVM.OrderHeader.ApplicationUser = iUnit.ApplicationUser.Get(u => u.Id == userId);

            //___ Total price base on Quantity ___
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                double price = GetPriceBaseOnQuantity(cart);
                cart.Price = price;
                shoppingCartVM.OrderHeader.OrderTotal += (price * cart.Count);
            }
            //___ Company ____
            if(shoppingCartVM.OrderHeader.ApplicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //its Customer
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                //its Company
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            iUnit.OrderHeader.Add(shoppingCartVM.OrderHeader);
            iUnit.SaveChange();
            #endregion

            /*___________________ Shopping Details _______________________*/
            #region Detail
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.productId,
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                iUnit.OrderDetail.Add(orderDetail);
                iUnit.SaveChange();
            }
            #endregion

            //_____ Adding Stripe Proccessing ____________
            if(shoppingCartVM.OrderHeader.ApplicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //its regular Customer account and need to Capture payment
                //stripe logic
            }

            //return View(shoppingCartVM);
            return RedirectToAction(nameof(OrderConfirmation),new {id = shoppingCartVM.OrderHeader.Id});
        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
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
