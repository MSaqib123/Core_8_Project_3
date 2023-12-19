using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;
using Proj.Models.ViewModel;
using Proj.Utility;
using Stripe.Climate;

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
        [Authorize(SD.Role_Employee +","+SD.Role_Admin)]
        public IActionResult UpdateOrderDetail()
        {
            //orderVM.OrderHeader = iUnit.OrderHeader.Get(x => x.Id == orderId);
            //orderVM.OrderDetail = iUnit.OrderDetail.GetAll(x => x.OrderHeaderId == orderId, includeProperties:"Product").ToList();
            //return View(orderVM);
            return RedirectToAction("Index");
        }



        //_______________________ APis _______________________
        #region Apis work
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> list = iUnit.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
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
