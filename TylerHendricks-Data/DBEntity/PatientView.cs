using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class PatientView
    {
        public int RowId { get; set; }
        public string UserId { get; set; }
        public string ConsultationId { get; set; }
        public int? ConsultationCategoryId { get; set; }
        public string Name { get; set; }
        public string Dob { get; set; }
        public int StateId { get; set; }
        public string StateCode { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string ProductName { get; set; }
        public int? Refills { get; set; }
        public string OrderStatusType { get; set; }
        public int OrderStatusTypeId { get; set; }
        public string ConsultationCategory { get; set; }
        public int? Status { get; set; }
    }
}
