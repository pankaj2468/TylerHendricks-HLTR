using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TylerHendricks_Core.CustomValidator;

namespace TylerHendricks_Core.Models
{
    public class AccountSetupED
    {
        [Required(ErrorMessage = "Required")]
        [Display(Name = "First Name")]
        [DataType(DataType.Text)]
        [MaxLength(length: 50)]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Last Name")]
        [DataType(DataType.Text)]
        [MaxLength(length: 50)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Required")]
        [ValidateDateRange(MinRangeDate = 18, MaxRangeDate = 75, ErrorMessage = "Please enter a valid date")]
        [Display(Name = "DateOfBirth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Phone Number")]
        [MaxLength(length: 15)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^\\([0-9]{3}\\) [0-9]{3}-[0-9]{4}$", ErrorMessage = "Phone Number is invalid")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Address Line1")]
        [DataType(DataType.Text)]
        [MaxLength(length: 100)]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line2")]
        [DataType(DataType.Text)]
        [MaxLength(length: 100)]
        public string AddressLine2 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Text)]
        [MaxLength(length: 100)]
        public string City { get; set; }
        [Required(ErrorMessage = "Required")]
        public int State { get; set; }
        [Required(ErrorMessage = "Required")]
        [MaxLength(length: 5)]
        [MinLength(length: 5, ErrorMessage = "Zip code must be 5 digits")]
        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; }
        [Required(ErrorMessage = "Please select a consultation option")]
        public int DetailId { get; set; }
    }
}
