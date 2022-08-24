using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class Signup
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [MaxLength(length: 256)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression("(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d@$!%*?&]{6,30}$", ErrorMessage = @"<div> 
    <p class='mb-0'>Your password must contain the following:</p>
	<ul style='margin-top: 1px;list-style: disc;padding-left: 30px;line-break: 25px;padding-top: -15px;'>
        <li>6 character minimum</li>
        <li>1 uppercase letter</li>
		<li>1 lowercase letter</li>
        <li>1 number</li>
        <ul>
    </ul>
</div>
")]
        [DataType(DataType.Password), Display(Name = "Change Password")]
        [MaxLength(length: 30)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password), Display(Name = "Confirm Change Password"), Compare("Password", ErrorMessage = "Current password does not match")]
        [MaxLength(length: 30)]
        public string ConfirmPassword { get; set; }
        [Required]
        public int StateId { get; set; }
        public bool TermCondition { get; set; }
        public int ConsultationCategoryId { get; set; }
        public string ConsultationId { get; set; }
        public string Role { get; set; }
        public int TimezoneOffSet { get; set; }
    }
}
