using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class Notify
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public int StateId { get; set; }

        public string StateCode { get; set; }

    }
}
