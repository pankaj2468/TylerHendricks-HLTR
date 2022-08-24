using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TemplateName { get; set; }
        public string Description { get; set; }
        public bool IsActived { get; set; }
        public bool IsRecordDeleted { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
