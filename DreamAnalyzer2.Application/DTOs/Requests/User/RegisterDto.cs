using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DreamAnalyzer2.Application.DTOs.Requests.User
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is requierd")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long"), MaxLength(50, ErrorMessage = "Username must be less than or equal to 50 characters")]
        public string Username { get; set; } = string.Empty;
       
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$", ErrorMessage ="Password must have at least 1 uppercase letter and 1 digit")]
        public string Password { get; set; } = string.Empty;
    }
}
