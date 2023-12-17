using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;
using Proj.Utility;

namespace Proj.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork iUnit;
        public OrderController(IUnitOfWork _iUnit)
        {
            iUnit = _iUnit;
        }

        public IActionResult Index()
        {
            return View();
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
