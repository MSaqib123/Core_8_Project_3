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
        [HttpGet]
        public IActionResult RoleManagement(string id)
        {
            return View();
        }


        //_______________________ APis _______________________
        #region Apis work
        [HttpGet]
        public IActionResult GetAll()
        {
            var list = db.ApplicationUsers.Include(x=>x.Company).ToList();

            var userRoles = db.UserRoles.ToList();
            var roles = db.Roles.ToList(); 

            //________ Fixing Nullable values for datatable _____
            foreach (var item in list)
            {
                var roleId = userRoles.FirstOrDefault(x => x.UserId == item.Id).RoleId;
                item.Role = roles.FirstOrDefault(x => x.Id == roleId).Name;

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


        [HttpPost]
        public IActionResult LockUnlock([FromBody]string? id)
        {
            var rec = db.ApplicationUsers.FirstOrDefault(x=>x.Id == id);
            if(rec == null)
            {
                return Json(new { success = true, message = "Deleted Successfully" });
            }

            if (rec.LockoutEnd != null && rec.LockoutEnd > DateTime.Now)
            {
                rec.LockoutEnd = DateTime.Now;
            }
            else
            {
                rec.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" });
        }

        #endregion
    }
}
