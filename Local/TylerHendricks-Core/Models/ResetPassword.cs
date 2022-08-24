using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
   public class ResetPasswordModel
    {
        [Required]
        public string TokenId { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        [RegularExpression("(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d@$!%*?&]{6,30}$", ErrorMessage = "Password should contain minimum 6 character at least one uppercase letter, one lowercase letter and one number")]
        [DataType(DataType.Password), Display(Name = "Change Password")]
        [MaxLength(length: 30)]
        public string ChnagePass { get; set; }
        [Required]
        [DataType(DataType.Password), Display(Name = "Confirm Password"), Compare("ChnagePass", ErrorMessage = "Current password does not match")]
        [MaxLength(length: 30)]
        public string ConfirmChnagePass { get; set; }
    }
}
