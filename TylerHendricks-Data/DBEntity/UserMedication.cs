using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class UserMedication
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ConsultationId { get; set; }
        public int ConsultationCategoryId { get; set; }
        public string DrugName { get; set; }
        public string Dose { get; set; }
        public int? MedicationUnitId { get; set; }
        public int? MedicationFormId { get; set; }
        public int? MedicationFrequencyId { get; set; }
        public string MedicalCondition { get; set; }
        public bool? IsMedicine { get; set; }
        public bool? IsRecordDeleted { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
