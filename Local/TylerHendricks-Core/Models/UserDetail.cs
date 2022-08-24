using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class UserDetail
    {
        [Key]
        public int Id { set; get; }

        [Required]
        [EmailAddress]
        public string EmailId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }
        public string Rolle { get; set; }
        

    }
}
