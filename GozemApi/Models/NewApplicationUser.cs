using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

namespace GozemApi.Models {
    public class NewApplicationUser
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Invalid Phone number")]
        public string PhoneNumber { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "{0} should range from {2} to {1} characters in length")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}