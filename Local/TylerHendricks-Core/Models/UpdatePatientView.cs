using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class UpdatePatientView
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "required field")]
        [Display(Name = "First Name")]
        [MaxLength(length: 50)]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "required field")]
        [Display(Name = "Last Name")]
        [MaxLength(length: 50)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "required field")]
        [Display(Name = "DateOfBirth")]
        [DataType(DataType.Date)]
        public DateTime? Dob { get; set; }
        [Required(ErrorMessage = "required field")]
        [Display(Name = "Phone Number")]
        [MaxLength(length: 15)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^\\([0-9]{3}\\) [0-9]{3}-[0-9]{4}$", ErrorMessage = "Phone Number is invalid")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "required field")]
        public int StateId { get; set; }
        [Required(ErrorMessage = "required field")]
        [MaxLength(length: 100)]
        public string City { get; set; }
        [Required(ErrorMessage = "required field")]
        [MaxLength(length: 5)]
        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; }
        public string DobToLocal { get; set; }
    }
}
