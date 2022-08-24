using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class State
    {
        [Required(ErrorMessage ="Please select your state")]
        public int Id { get; set; }
        public string StateName { get; set; }
        public bool Status { get; set; }
    }
}
