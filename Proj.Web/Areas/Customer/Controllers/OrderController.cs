using Microsoft.AspNetCore.Mvc;
using Proj.DataAccess.Repository.IRepository;

namespace Proj.Web.Areas.Customer.Controllers
{
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
        public IActionResult GetAll()
        {
            var list = iUnit.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = list });
        }

        #endregion
    }
}
