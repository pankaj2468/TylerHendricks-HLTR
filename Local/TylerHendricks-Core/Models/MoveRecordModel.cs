using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class MoveRecordModel
    {
        public int TotalRows { get; set; }
        [Key]
        public int RowNo { get; set; }
        public int RowId { get; set; }
        public string PatientId { get; set; }
        public string ConsultationId { get; set; }
        public int Status { get; set; }
    }
}
