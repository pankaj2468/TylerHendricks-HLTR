using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class InitialRegister
    {
        public int TotalRecords { get; set; }
        public string UserId { get; set; }
        [Key]
        public string ConsultationId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RegistrationDate { get; set; }
        public string QuestionNaire { get; set; }
    }
}
