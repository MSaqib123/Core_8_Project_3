using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Discription { get; set; }
        [Required]
        public string ISBN { get; set; }
        public string Author { get; set; }
        [Required]
        [Display(Name = "List Price")]
        [Range(1,1000)]
        public double ListPrice { get; set; }

        [Required]
        [Display(Name = "Price for 1-50")]
        [Range(1,1000)]
        public double Price { get; set; }
        
        [Required]
        [Display(Name = "Price for 50+")]
        [Range(1,1000)]
        public double Price50 { get; set; }
        
        [Required]
        [Display(Name = "Price for 100+")]
        [Range(1,1000)]
        public double Price100 { get; set; }


        [Display(Name = "Product Category")]
        [ValidateNever]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }


        //_______ Removing Image _________
        //[ValidateNever]
        //[Display(Name = "Product Image")]
        //public string ImageUrl { get; set; }

        //_______ Adding Multiple Images _________
        // 1 to many    1product has manyImages
        [ValidateNever]
        public List<ProductImage> ProductImages{ get; set; }
    }
}
