using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class PharmacyInformationModel
    {
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Pharmacy Name")]
        [MaxLength(length:200)]
        public string PharmacyName { get; set; }
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        [RegularExpression("^\\([0-9]{3}\\) [0-9]{3}-[0-9]{4}$", ErrorMessage = "Phone Number is invalid")]
        [MaxLength(length: 15)]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Address Line1")]
        [MaxLength(length: 100)]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line2")]
        [MaxLength(length: 100)]
        public string AddressLine2 { get; set; }
        [Required(ErrorMessage = "Required")]
        [MaxLength(length: 100)]
        public string City { get; set; }
        [Required(ErrorMessage = "Required")]
        public int State { get; set; }
        [Required(ErrorMessage = "Required")]
        [MaxLength(length: 5)]
        [MinLength(length: 5, ErrorMessage = "Zip code must be 5 digits")]
        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; }
    }
}
