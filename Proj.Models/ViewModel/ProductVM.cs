using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj.Models.ViewModel
{
    public class ProductVM
    {
        public Product Product_obj { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> categoryList_obj { get; set; }
    }
}
