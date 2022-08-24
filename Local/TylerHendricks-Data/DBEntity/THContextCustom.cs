using Microsoft.EntityFrameworkCore;
using TylerHendricks_Core.Models;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class THContext: DbContext
    {
        public virtual DbSet<PatientInformationModel> Sp_PatientRecords { get; set; }
        public virtual DbSet<PatientSqlChart> SP_PATIENT_CHART { get; set; }
        public virtual DbSet<CheckMediResult> CheckMedi { get; set; }
        public virtual DbSet<MoveRecordModel> SP_GETRowNumber { get; set; }
        public virtual DbSet<MoveRecordModel> GET_PATIENT { get; set; }
        public virtual DbSet<MoveRecordModel> GET_PATIENT_ROWNO { get; set; }
        public virtual DbSet<AdminPatientDataView> ADMIN_PATIENT_RECORD { get; set; }
        public virtual DbSet<OrderHistoryModel> PATIENT_REQUEST_HISTORY { get; set; }
        public virtual DbSet<InitialRegister> GetInitialRegisters { get; set; }
    }
}
