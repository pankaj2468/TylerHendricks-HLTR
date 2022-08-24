using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;
using TylerHendricks_Core.ViewModel;
using TylerHendricks_Data.DBEntity;

namespace TylerHendricks_Repo.Contracts
{
    public interface  IPhysician: IDisposable
    {     
        Task<List<ProviderDashboard>> GetInformationModels(int recordType, int stateId, TimeZoneConfig timeZone, string sortColumn, string sortColumnDirection, string searchValue, int pageSize, int start, int skip);
        Task<PatientChartViewModel> GetPatientChart(int recordType, int stateId, TimeZoneConfig timezone, int rowNo,string userId, string consultationId);
        Task<PatientChartViewModel> GetPatient(string userId);
        Task<List<Notify>> GetNotify();
        Task<CrudResult> SaveChat(string patientId, string physicianId, string consultationId, string message);
        Task<CrudResult> UpdateOrderStatus(string consultationId, string userId, int statusId);
        Task<CrudResult> SaveNote(string userId, string patientId, string filePath);
        Task<CrudResult> UpdatePatientOrderStatus(string patientId,string consultationId, int statusId);
        Task<CrudResult> DeleteNotes(int id);
        Task<MoveRecordModel> FindRowNumber(string patientId,string consultationId, int status, int stateId);
        Task<UpdatePatientView> GetPatientDetails(string patientId);
        Task<UpdatePharmacyView> GetPharmacyDetails(string consultationId);
        Task<CrudResult> UpdatePharmacyInfo(UpdatePharmacyView pharmacyView);
        Task<List<MedicineView>> GetMedicineDetails(string consultationId);
        Task<CrudResult> UpdateMedicineInfo(List<MedicineView> medicineViews, string patientId, string userId,string consultationId);
        Task<MoveRecordModel> GetPatientByConsultationId(int stateId, string consultationId);
        Task<MoveRecordModel> GetPatientByRowNo(int stateId, int rowNo,int recordType);
        DataTable ToDataTable<T>(List<T> listItems);
        Task<bool> MoveToAllPatient(string consultationId);
    }
}
