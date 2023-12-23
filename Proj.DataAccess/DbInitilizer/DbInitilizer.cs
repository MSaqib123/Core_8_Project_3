using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proj.DataAccess.Data;
using Proj.Models;
using Proj.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj.DataAccess.DbInitilizer
{
    public class DbInitilizer : IDbInitilizer
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext db;
        public DbInitilizer(
            UserManager<IdentityUser> _userManager,
            RoleManager<IdentityRole> _roleManager,
            ApplicationDbContext _db
            )
        {
            userManager = _userManager;
            roleManager = _roleManager;
            db = _db;

        }
        public void Initilize()
        {
            //___ migration if they are not Applied
            try
            {
                if(db.Database.GetPendingMigrations().Count() > 0)
                {
                    db.Database.Migrate();
                }
            }catch(Exception ex) { }

            //Create role if they are not created
            if (!roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                //if roles are not created , then we will create admin user as well
                userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "Admin@dotnetmastery.com",
                    Email = "admin@dotnetmastery.com",
                    Name = "Admin",
                    PhoneNumber = "123423",
                    StreetAddress = "Karachi",
                    State = "Sindh",
                    PostalCode = "123",
                    City = "Karachi"
                }, "Admin.9090").GetAwaiter().GetResult();

                ApplicationUser user = db.ApplicationUsers.FirstOrDefault(x => x.Email == "Admin@dotnetmastery.com");
                userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

            }
            return;
        }
    }
}
