using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class ForgotModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(length: 256)]
        public string Email { get; set; }
        public string Role { get; set; }
    }

}
