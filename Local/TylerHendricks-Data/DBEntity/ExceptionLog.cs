using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class ExceptionLog
    {
        public int Id { get; set; }
        public int? ErrorLine { get; set; }
        public string ErrorMessage { get; set; }
        public int? ErrorNumber { get; set; }
        public string ErrorProcedure { get; set; }
        public int? ErrorSeverity { get; set; }
        public int? ErrorState { get; set; }
        public DateTime? DateErrorRaised { get; set; }
    }
}
