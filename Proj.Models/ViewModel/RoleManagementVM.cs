using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj.Models.ViewModel
{
    public class RoleManagementVM
    {
        public ApplicationUser applicationUser { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
        public IEnumerable<SelectListItem> CompanyList { get; set; }

        //___ Alredy in  ApplicationUser ___
        //public string CurrentRole { get; set; }
    }
}
