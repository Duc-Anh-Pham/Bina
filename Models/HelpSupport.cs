using System.ComponentModel.DataAnnotations;

namespace Bina.Models
{
    public class HelpSupport
    {
        [Key]
        [Required]
        public int HelpSupportID { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; }

        public string? Comments { get; set; }
    }
}