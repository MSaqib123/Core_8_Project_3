using AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proj.DataAccess.Data;
using Proj.DataAccess.Repository.IRepository;
using Proj.Models;
using Proj.Models.ViewModel;
using Proj.Utility;

namespace Proj.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController: Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> userManager;

        public UserController(
            ApplicationDbContext _db,
            UserManager<IdentityUser> _userManager
            )
        {
            db= _db;
            userManager = _userManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult RoleManagement(string userId)
        {
            RoleManagementVM vm = new RoleManagementVM();

            vm.applicationUser = db.ApplicationUsers.Include(x=>x.Company).Where(x => x.Id == userId).FirstOrDefault();

            //Roles Projection
            vm.RoleList = db.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Name
            });

            //Companies Projection
            vm.CompanyList = db.Companies.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Name
            });

            //Current Role 
            var roleId = db.UserRoles.FirstOrDefault(x => x.UserId == userId).RoleId;
            vm.applicationUser.Role = db.Roles.FirstOrDefault(x => x.Id == roleId).Name;

            return View(vm);
        }


        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM vm)
        {
            //Current Role 
            var roleId = db.UserRoles.FirstOrDefault(x => x.UserId == vm.applicationUser.Id).RoleId;

            //old role
            string oldRole = db.Roles.FirstOrDefault(u=>u.Id == roleId).Name;

            if (!(vm.applicationUser.Role == oldRole))
            {
                //a role was updated
                ApplicationUser applicationUser = db.ApplicationUsers.FirstOrDefault(x => x.Id == vm.applicationUser.Id);
                if (vm.applicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = vm.applicationUser.CompanyId;
                }
                if(oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                db.SaveChanges();

                //__ Removing old role __
                userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                //___ add new role ___
                userManager.AddToRoleAsync(applicationUser, vm.applicationUser.Role).GetAwaiter().GetResult();

            }

            return RedirectToAction(nameof(Index));
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
