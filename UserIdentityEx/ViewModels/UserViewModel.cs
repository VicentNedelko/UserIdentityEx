using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UserIdentityEx.Models;

namespace UserIdentityEx.ViewModels
{
    public class UserViewModel
    {
        public string userId { get; set; }

        [Required(ErrorMessage = "Email required.")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [UIHint("email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age required.")]
        [Display(Name = "Age")]
        [Range(10, 100)]
        public int Age { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Pass")]
        [DataType(DataType.Password)]
        [UIHint("password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Pass Confirm")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passes are not equal.")]
        [UIHint("password")]
        public string PasswordConfirm { get; set; }
    }
}
