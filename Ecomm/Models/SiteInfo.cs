using System.ComponentModel.DataAnnotations;

namespace Ecomm.Models
{
    public class SiteInfo
    {
        public int Id { get; set; }
        [Display(Name = "Website Name")]
        public string? WebsiteName { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? EmailAddress { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
        
    }
}
