using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;
using Proj.Models.ViewModel;
using Proj.Utility;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Claims;

namespace Proj.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork iUnit;
        [BindProperty]
        public OrderVM orderVM { get; set; }
        public OrderController(IUnitOfWork _iUnit)
        {
            iUnit = _iUnit;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            orderVM = new OrderVM();
            orderVM.OrderHeader = iUnit.OrderHeader.Get(x => x.Id == orderId);
            orderVM.OrderDetail = iUnit.OrderDetail.GetAll(x => x.OrderHeaderId == orderId, includeProperties:"Product").ToList();
            return View(orderVM);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Employee +","+SD.Role_Admin)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHdr = iUnit.OrderHeader.Get(x=>x.Id == orderVM.OrderHeader.Id);
            orderHdr.Name = orderVM.OrderHeader.Name;
            orderHdr.Name = orderVM.OrderHeader.PhoneNumber;
            orderHdr.Name = orderVM.OrderHeader.StreetAddress;
            orderHdr.Name = orderVM.OrderHeader.City;
            orderHdr.Name = orderVM.OrderHeader.State;
            orderHdr.Name = orderVM.OrderHeader.PostCode;
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
            {
                orderHdr.Carrier = orderVM.OrderHeader.Carrier;
            }
            
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
            {
                orderHdr.Carrier = orderVM.OrderHeader.TrackingNumber;
            }
            iUnit.OrderHeader.Update(orderHdr);
            iUnit.SaveChange();

            TempData["Success"] = "Order Updated Successfully";
            
            return RedirectToAction(nameof(Details), new {orderId=orderHdr.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Employee + " , " + SD.Role_Admin)]
        public IActionResult StartProcess()
        {
            iUnit.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.StatusInProcess);
            iUnit.SaveChange();
            TempData["Success"] = "Order Details Update Successfully";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }
        
        [HttpPost]
        [Authorize(Roles = SD.Role_Employee + " , " + SD.Role_Admin)]
        public IActionResult ShipOrder()
        {
            var orderHdr = iUnit.OrderHeader.Get(x => x.Id == orderVM.OrderHeader.Id);
            orderHdr.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            orderHdr.Carrier = orderVM.OrderHeader.Carrier;
            orderHdr.OrderStatus = SD.StatusShipped;
            orderHdr.ShippingDate = DateTime.Now;
            if(orderHdr.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHdr.PaymentDueDate = DateTime.Now.AddDays(30);
            }
            //iUnit.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.StatusShipped);
            iUnit.OrderHeader.Update(orderHdr);
            iUnit.SaveChange();
            TempData["Success"] = "Order Details Update Successfully";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Employee + " , " + SD.Role_Admin)]
        public IActionResult CancelOrder()
        {
            var orderHdr = iUnit.OrderHeader.Get(x => x.Id == orderVM.OrderHeader.Id);
            if(orderHdr.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHdr.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                iUnit.OrderHeader.UpdateStatus(orderHdr.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                iUnit.OrderHeader.UpdateStatus(orderHdr.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            iUnit.SaveChange();
            TempData["Success"] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }

        [ActionName("Details")]
        [HttpPost]
        public IActionResult Details_Pay_NOW()
        {
            orderVM.OrderHeader = iUnit.OrderHeader.Get(x => x.Id == orderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            orderVM.OrderDetail = iUnit.OrderDetail.GetAll(x => x.OrderHeaderId == orderVM.OrderHeader.Id, includeProperties: "Product").ToList();

            //its regular Customer account and need to Capture payment
            //stripe logic
            var domain = "http://localhost:5003/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"Admin/Order/PaymentConfirmation?orderHeaderId={orderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/Order/details?orderId={orderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            //___ iterate all item in strape __
            foreach (var item in orderVM.OrderDetail)
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
            
            iUnit.OrderHeader.UpdateStripePaymentId(orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            iUnit.SaveChange();

            //____ Redirect ____
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = iUnit.OrderHeader.Get(u => u.Id == orderHeaderId, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                //this is Company Order
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    iUnit.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    iUnit.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    iUnit.SaveChange();
                }
            }
            //___________ Removing All Showiping Cart List for DB _________
            List<ShoppingCart> shopingCart = iUnit.ShoppingCart.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserID).ToList();
            iUnit.ShoppingCart.RemoveRange(shopingCart);
            iUnit.SaveChange();

            return View(orderHeaderId);
        }

        //_______________________ APis _______________________
        #region Apis work
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> list;
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                list = iUnit.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                list = iUnit.OrderHeader.GetAll(x=>x.ApplicationUserID == userId,includeProperties: "ApplicationUser").ToList();
            }
            

            switch (status)
            {
                case "pending":
                    list = list.Where(x => x.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;

                case "inprocess":
                    list = list.Where(x => x.OrderStatus == SD.StatusInProcess);
                    break;

                case "completed":
                    list = list.Where(x => x.PaymentStatus == SD.StatusShipped);
                    break;

                case "approved":
                    list = list.Where(x => x.PaymentStatus == SD.StatusApproved);
                    break;

                default:
                    break;

            }
            return Json(new { data = list });
        }

        #endregion
    }
}
