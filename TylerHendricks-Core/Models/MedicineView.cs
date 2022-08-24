using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class MedicineView
    {
        public string DrugName { get; set; }
        public string Dose { get; set; }
        public int? UnitId { get; set; }
        public int? FormId { get; set; }
        public int? FrequencyId { get; set; }
        public string MedicalCondition { get; set; }
    }
}
