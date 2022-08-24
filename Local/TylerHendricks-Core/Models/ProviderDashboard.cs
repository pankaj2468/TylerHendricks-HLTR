using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class ProviderDashboard
    {
        public int RowNo { get; set; }
        public int RowId { get; set; }
        public int TotalRows { get; set; }
        public string PatientId { get; set; }
        public string ConsultationId { get; set; }
        public int ConsultationCategoryId { get; set; }
        public string ConsultationCateory { get; set; }
        public string Name { get; set; }
        public string DOB { get; set; }
        public string StateCode { get; set; }
        public string Submitted { get; set; }
        public string RequestedRx { get; set; }
        public int Refills { get; set; }
        public string Status { get; set; }
        public string WaitInQuene { get; set; }
        public string TabStatus { get; set; }
        public string QueryString { get; set; }
    }
}
