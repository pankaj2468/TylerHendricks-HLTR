using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class UpdateEmailModel
    {
        [Required(ErrorMessage ="required field")]
        [MaxLength(length:500)]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }
        public bool IsValidEmail { get; set; }
        [MaxLength(length: 10)]
        public string OTP { get; set; }
        public bool IsOTPValid { get; set; }
        public Dictionary<string,string> Error { get; set; }
    }
}
