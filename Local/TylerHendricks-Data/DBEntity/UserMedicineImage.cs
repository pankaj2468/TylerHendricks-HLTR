using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class UserMedicineImage
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ConsultationId { get; set; }
        public string MedicineFile1 { get; set; }
        public string MedicineFile2 { get; set; }
        public string MedicineFile3 { get; set; }
        public bool? IsRecordDeleted { get; set; }
        public string AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
