using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;
using Proj.Models.ViewModel;
using Proj.Utility;
using Stripe.Checkout;
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

                //_____ Removing cart from Session _______
                HttpContext.Session.SetInt32(SD.SessionCart, iUnit.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
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
            var cartFromDb = iUnit.ShoppingCart.Get(u => u.Id == cartId);
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

            //_____ Removing cart from Session _______
            HttpContext.Session.SetInt32(SD.SessionCart, iUnit.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);

            return RedirectToAction("Index");
        }

        //_____ CheckOut Screen ______
        public IActionResult CheckOut()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM = new ShoppingCartVM();
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
        public IActionResult CheckOutPOST()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM.ShoppingCartList = iUnit.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product");

            //___ Getting login user Data ____
            //_____ Error Due to navigation Property
            //shoppingCartVM.OrderHeader.ApplicationUser = iUnit.ApplicationUser.Get(u => u.Id == userId);
            ApplicationUser applicationUser = iUnit.ApplicationUser.Get(u => u.Id == userId);

            /*___________________ Shopping Header _______________________*/
            #region Header
            //___ Order Date ___
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserID = userId;


            //___ Total price base on Quantity ___
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                double price = GetPriceBaseOnQuantity(cart);
                cart.Price = price;
                shoppingCartVM.OrderHeader.OrderTotal += (price * cart.Count);
            }
            //___ Company ____
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
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

            //_____ Adding Stripe Proccessing Logic ____________
            #region Stripe Proccess
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //its regular Customer account and need to Capture payment
                //stripe logic
                var domain = "http://localhost:5003/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                //___ iterate all item in strape __
                foreach (var item in shoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)item.Price * 100,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title,
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new Stripe.Checkout.SessionService();
                Session session = service.Create(options);
                //_____ Note ______
                //IntentId  will be  = null here 
                // IntentId only  get when   payment is successlfuly done in strape
                iUnit.OrderHeader.UpdateStripePaymentId(shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                iUnit.SaveChange();

                //____ Redirect ____
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            #endregion

            //return View(shoppingCartVM);
            return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVM.OrderHeader.Id });
        }
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = iUnit.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //this is Customer Order
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if(session.PaymentStatus.ToLower() == "paid")
                {
                    iUnit.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    iUnit.OrderHeader.UpdateStatus(id, SD.StatusApproved,SD.PaymentStatusApproved);
                    iUnit.SaveChange();
                }
            }
            //___________ Removing All Showiping Cart List for DB _________
            List<ShoppingCart> shopingCart = iUnit.ShoppingCart.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserID).ToList();
            iUnit.ShoppingCart.RemoveRange(shopingCart);
            iUnit.SaveChange();

            return View(id);
        }
        //___ Next ___
        //proccess stripe integration for register account at
        //https://stripe.com/
        //Acd Testing Api to test Stripe

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
