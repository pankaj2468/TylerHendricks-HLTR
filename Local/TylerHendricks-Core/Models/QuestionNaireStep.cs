using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class QuestionNaireStep
    {
        [Required(ErrorMessage = "Please select a medication option")]
        [Range(1, 100, ErrorMessage = "Please select a medication option")]
        public int Id { get; set; }
    }
}
