using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Bina.Helper.Attributes
{
    public class EmailAddressAttribute : ValidationAttribute
    {
        private readonly Regex _regex = new Regex(@"^[\w\.-]+@(gmail|edu|org)\.com$", RegexOptions.IgnoreCase);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            string email = value.ToString();

            if (_regex.IsMatch(email))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"Please enter a valid email address with the domain @gmail.com, @edu.com, or @org.com.");
        }
    }
}
