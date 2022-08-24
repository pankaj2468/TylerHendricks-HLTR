using System;
using System.ComponentModel.DataAnnotations;

namespace TylerHendricks_Core.CustomValidator
{
    public class ValidateDateRange : ValidationAttribute
    {
        public int MinRangeDate { get; set; }
        public int MaxRangeDate { get; set; }

        public override bool IsValid(object value)
        {
            var startDate = DateTime.UtcNow.AddYears(-MaxRangeDate);
            var endDate = DateTime.UtcNow.AddYears(-MinRangeDate);
            if ((DateTime)value >= startDate && (DateTime)value <= endDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
