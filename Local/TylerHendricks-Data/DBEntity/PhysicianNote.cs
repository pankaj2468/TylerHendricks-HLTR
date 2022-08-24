using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class PhysicianNote
    {
        public int Id { get; set; }
        public string ConsultationId { get; set; }
        public string PhysicianId { get; set; }
        public string PatientId { get; set; }
        public string FilePath { get; set; }
        public bool IsFile { get; set; }
        public bool IsRecordDeleted { get; set; }
        public string AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
