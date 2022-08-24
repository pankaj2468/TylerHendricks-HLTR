using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [MaxLength(length: 256)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password), Display(Name = "Password")]
        [MaxLength(length: 30)]
        public string Password { get; set; }
        public string Role { get; set; }
        public int TimezoneOffSet { get; set; }
        public string IsDayLightSaving { get; set; }
    }
}
