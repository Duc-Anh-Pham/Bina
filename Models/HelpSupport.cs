using System.ComponentModel.DataAnnotations;

namespace Bina.Models
{
    public class HelpSupport
    {
        [Key]
        public int HelpSupportID { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Your title cannot be left blank")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Your messages cannot be left blank")]
        public string? UserMessages { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}