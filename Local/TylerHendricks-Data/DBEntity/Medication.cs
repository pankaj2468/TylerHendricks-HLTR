using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class Medication
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MedicationCatgoryId { get; set; }
        public bool IsActive { get; set; }
        public bool IsRecordDeleted { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
