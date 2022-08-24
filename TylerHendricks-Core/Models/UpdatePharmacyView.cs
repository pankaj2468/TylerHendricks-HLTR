using System.ComponentModel.DataAnnotations;

namespace TylerHendricks_Core.Models
{
    public class UpdatePharmacyView
    {
        [Required(ErrorMessage = "required field")]
        public int PharmacyId { get; set; }

        [Required(ErrorMessage = "required field")]
        [MaxLength(length: 200)]
        public string PharmacyName { get; set; }
        [Required(ErrorMessage = "required field")]
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
        [MaxLength(length: 100)]
        public string AddressLine1 { get; set; }
        [MaxLength(length: 100)]
        public string AddressLine2 { get; set; }
        [Required(ErrorMessage = "required field")]
        [MaxLength(length: 5)]
        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; }
    }
}
