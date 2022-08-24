using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.ViewModel
{
    public class SessionManager
    {
        [RegularExpression("^[1-9][0-9]*$", ErrorMessage = "Please select a state")]
        public int StateId { get; set; }
        public int ConsultationCategoryId { get; set; }
        public string ConsultationId { get; set; }
        public bool? MessagePayment { get; set; }

    }
}
