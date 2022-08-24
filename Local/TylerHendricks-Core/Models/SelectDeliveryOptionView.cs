using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class SelectDeliveryOptionView
    {
        [Required(ErrorMessage = "Please select an option.")]
        public bool isHomeDelivery { get; set; }
    }
}
