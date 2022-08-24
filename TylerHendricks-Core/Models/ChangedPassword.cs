using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class ChangedPassword
    {
        [Required, DataType(DataType.Password), Display(Name = "Current Password"), MaxLength(length: 30)]
        public string CurrentPassword { set; get; }
        [Required, DataType(DataType.Password), Display(Name = "New Password"), MaxLength(length: 30),MinLength(length:4)]
        [RegularExpression("(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d@$!%*?&]{6,30}$", ErrorMessage = "Password should contain minimum 6 character at least one uppercase letter, one lowercase letter and one number")]
        public string ChangePassword { set; get; }
        [Required, DataType(DataType.Password), Display(Name = "Confirm New Password"), Compare("ChangePassword", ErrorMessage = "Confirm new password does not match"), MaxLength(length: 30), MinLength(length: 6)]
        public string ConfirmChangePassword { set; get; }
    }
}
