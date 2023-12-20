using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;
using Proj.Models.ViewModel;
using Proj.Utility;
using Stripe;
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
