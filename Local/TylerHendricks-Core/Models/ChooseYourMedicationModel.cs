using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class ChooseYourMedicationModel
    {
        
        public string MedicationName { get; set; }
        public string MedicationUnit { get; set; }
        public string MedicationQuantity { get; set; }
        public int Refills { get; set; }
        public string Description { get; set; }
        public decimal MedicationPrice { get; set; }
       
    }
}
