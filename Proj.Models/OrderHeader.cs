using Microsoft.AspNetCore.Mvc;
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
    public class OrderHeader
    {
        public int Id { get; set; }
        public string ApplicationUserID { get; set; }
        [ForeignKey("ApplicationUserID")]
        [ValidateNever]
        public virtual ApplicationUser ApplicationUser { get; set; }

        //__ Order ___
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public double OrderTotal { get; set; }

        //____ for Each order ___
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }

        //____ after shipping order ___
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }

        //____ For Company user ______
        //they can pay after  30 day  if clients are company
        public DateTime PaymentDate { get; set; }
        public DateOnly PaymentDueDate { get; set; }

        //____ Payment (creditCart , stripe) ______
        public string? PaymentIntentId { get; set; }


        //____ for Each Payment ___
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostCode { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
