using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proj.DataAccess.Data;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;
using Proj.Utility;

namespace Proj.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController: Controller
    {
        private readonly ApplicationDbContext db;

        public UserController(ApplicationDbContext _db)
        {
            db= _db;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        //_______________________ APis _______________________
        #region Apis work
        [HttpGet]
        public IActionResult GetAll()
        {
            var list = db.ApplicationUsers.Include(x=>x.Company).ToList();
            foreach (var item in list)
            {
                if (item.Company == null)
                {
                    item.Company = new Company() { Name = "-" };
                }
                
                if (item.PhoneNumber == null)
                {
                    item.PhoneNumber = "-";
                }
            }
            return Json(new {data= list });
        }


        [HttpDelete]
        public IActionResult DeleteRecord(int? id)
        {
            return Json(new { success = true, message = "Deleted Successfully" });
        }

        #endregion
    }
}
