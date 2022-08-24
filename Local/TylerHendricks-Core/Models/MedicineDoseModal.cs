using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class MedicineDoseModal
    {
        public string DrugName { get; set; }
        public string Dose { get; set; }
        public int MedicationUnitId { get; set; }
        public int MedicationFormId { get; set; }
        public int MedicationFrequencyId { get; set; }
        public string  MedicalCondition { get; set; }     
    }
}
