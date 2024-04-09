using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecomm.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        [ValidateNever]
        public string? ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public double OrderTotal { get; set; }
        public string? TrackingId { get; set; }
        
        public string? OrderStatus { get; set; }

        [Required]
        [MinLength(16, ErrorMessage = "Card Number should be 16 characters")]
        [MaxLength(255)]
        [DisplayName("Card Number")]
        public string CardNumber { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Card CSV should be 3 characters")]
        [MaxLength(3, ErrorMessage = "Card CSV should be 3 characters")]
        [DisplayName("CSV")]
        public string CardCSV { get; set; }
        [Required]
        [DisplayName("Expiry Date")]
        public DateOnly CardExpiryDate { get; set; }

        public string? PaymentTransactionId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        [RegularExpression(@"^972\d{9}$", ErrorMessage = "Invalid Phone Number. It should start with '972' followed by 9 digits.")]
        public string? PhoneNumber { get; set; }

    }
}
