using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_Project1.Models
{
    public class OrderHeader
    {
        public int Id { get; set;}
        public string ApplicationUserId { get; set;}
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set;}
        [Required]
        public DateTime OrderDate { get; set;}
        [Required]
        public DateTime ShippingDate { get; set;}
        [Required]
        public double OrderTotal { get; set;}
        public string TrackingNumber { get; set;}
        public string Carrier { get; set;}
        public string OrderStatus { get; set;}
        public string PaymentStatus { get; set;}
        public DateTime PaymentDate { get; set;}
        public DateTime PaymentDueDate { get; set;}
        public String TransactionId { get; set;}
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Required]
        [Display(Name="Phone Number")]
        public String PhoneNumber { get; set; }



    }
}
