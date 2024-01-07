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
        //private readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUnitOfWork iUnit;

        public UserController(
            //ApplicationDbContext _db,
            UserManager<IdentityUser> _userManager,
            RoleManager<IdentityRole> _roleManager,
            IUnitOfWork _iUnit
            )
        {
            //db= _db;
            userManager = _userManager;
            roleManager = _roleManager;
            iUnit = _iUnit;
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

            //db base
            //vm.applicationUser = db.ApplicationUsers.Include(x=>x.Company).Where(x => x.Id == userId).FirstOrDefault();

            //unit base
            vm.applicationUser = iUnit.ApplicationUser.Get(x => x.Id == userId , includeProperties : "Company");

            //Roles Projection
            vm.RoleList = roleManager.Roles.Select(u => new SelectListItem//db
            {
                Text = u.Name,
                Value = u.Name
            });

            //Companies Projection
            vm.CompanyList = iUnit.Company.GetAll().Select(u => new SelectListItem//db.Companies.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }) ;

            //Current Role 
            //db
            //var roleId = db.UserRoles.FirstOrDefault(x => x.UserId == userId).RoleId;

            //iunit
            vm.applicationUser.Role = userManager.GetRolesAsync(iUnit.ApplicationUser.Get(u=>u.Id == userId)).GetAwaiter().GetResult().FirstOrDefault();
            return View(vm);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM vm)
        {
            //Current Role 
            //var roleId = db.UserRoles.FirstOrDefault(x => x.UserId == vm.applicationUser.Id).RoleId;

            //old role
            string oldRole = vm.applicationUser.Role = userManager.GetRolesAsync(iUnit.ApplicationUser.Get(u => u.Id == vm.applicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();

            //Admin can not Change Company Directly ---> he has to change   Role 1st
            ApplicationUser applicationUser = iUnit.ApplicationUser.Get(x => x.Id == vm.applicationUser.Id);

            if (!(vm.applicationUser.Role == oldRole))
            {
                //a role was updated
                if (vm.applicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = vm.applicationUser.CompanyId;
                }
                if(oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                iUnit.ApplicationUser.Update(applicationUser);
                iUnit.SaveChange();

                //__ Removing old role __
                userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                //___ add new role ___
                userManager.AddToRoleAsync(applicationUser, vm.applicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if(oldRole == SD.Role_Company && applicationUser.CompanyId != vm.applicationUser.CompanyId)
                {
                    applicationUser.CompanyId = vm.applicationUser.CompanyId;
                    iUnit.ApplicationUser.Update(applicationUser);
                    iUnit.SaveChange();

                }
            }


            return RedirectToAction(nameof(Index));
        }


        //_______________________ APis _______________________
        #region Apis work
        [HttpGet]
        public IActionResult GetAll()
        {
            //var list = db.ApplicationUsers.Include(x=>x.Company).ToList();
            var list = iUnit.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            //var userRoles = db.UserRoles.ToList();
            //var roles = db.Roles.ToList(); 

            //________ Fixing Nullable values for datatable _____
            foreach (var item in list)
            {
                //var roleId = userrole.FirstOrDefault(x => x.UserId == item.Id).RoleId;
                //item.Role = roles.FirstOrDefault(x => x.Id == roleId).Name;
                item.Role = userManager.GetRolesAsync(item).GetAwaiter().GetResult().FirstOrDefault();

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
            //var rec = db.ApplicationUsers.FirstOrDefault(x=>x.Id == id);
            var rec = iUnit.ApplicationUser.Get(x => x.Id == id);
            if (rec == null)
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
            //db.SaveChanges();
            iUnit.ApplicationUser.Update(rec);
            iUnit.SaveChange();
            return Json(new { success = true, message = "Deleted Successfully" });
        }

        #endregion
    }
}
