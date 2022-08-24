using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;
using TylerHendricks_Core.ViewModel;
using TylerHendricks_Data.DBEntity;
namespace TylerHendricks_Repo.Contracts
{
    public interface IPatient: IDisposable
    {
        string getName();
        Task<int> TotalQuestions(int consultationCategoryId);
        Task<int> AttemptQuestions(string userId, int consultationCategoryId, string consultationId);
        Task<Questions> CurrentQuestion(string userId, int consultationCategoryId,string consultationId);
        Task<Questions> QuestionsDetail(string userId = "", int QuestionId = 0, bool QuestionResponse = true);
        Task<CrudResult> UpdateQuestionState(string userId, string previousId, string answer,string consultationId, int consultationCategoryId, string consultationMappingdId);
        Task<string> GetPopupText(int? QuestionId);
        Task<string> GetAnswer(string userId, int consultationCategoryId, string questionMappingId);
        Task<CrudResult> AddConsulationCategory(string userId, int consultationCategoryId);
        Task<List<MedicineForm>> GetMedicineForms();
        Task<List<MedicineUnit>> GetMedicineUnit();
        Task<List<MedicineFrequency>> GetMedicineFrequency();
        Task<CrudResult> AddMedicineDose(IEnumerable<MedicineDoseModal> modals, string userId, int consultationCategoryId, string consultationId,bool IsMedicine);
        Task<CrudResult> SetPreviousQuestion(string userId, int consultationCategoryId, string consultationId,bool isBack);
        Task<List<Medication>> GetMedication(int medicationCategoryId);
        Task<List<MedicationCategory>> GetMedicationCategories();
        Task<CrudResult> SaveMedicineImage(List<string> fileName, string userId,string consultationId);
        Task<ChooseYourMedicationModel> GetSelectedMedication(string consultationId);
        Task<CrudResult> SaveRetakeMedicine(List<string> fileName, string userId, int consultationCategoryId);
        Task<CrudResult> SavePaymentDetails(string userId, int consultationCategoryId, string txnId, string consultationId);
        Task<CrudResult> SavePaymentDetails(int userRowId, string invoiceNumber);
        Task<bool> ConsultationComplete(string consultationId);
        Task<List<OrderHistoryModel>> GetOrderHistory(string userId, TimeZoneConfig zoneConfig);
        Task<CrudResult> SaveShippingDetail(QuestionNaireViewModel question);
        Task<MessagesViewModel> GetMessageDetail(string UserId, int ConsultationCategoryId, TimeZoneConfig TimeZone);
        Task<int> GetFirstQuestion(int ConsultationCategoryId);
        Task<int> GetStateId(string UserId); 
        Task<string> GetProviderDetail(int StateId);
        Task<CrudResult> SaveMessages(UserChat userChat);
        Task<CrudResult> SavePayment(PaymentInfo payment);
        Task<int> CheckMedi(string UserId, string ConsultationId, int ConsultationCategoryId);
        Task<CrudResult> UpdatePatientOrderStatus(string UserId, int StatusId, int ConsultationCategoryId);
        Task<bool> VerifyEmail(string Email);
        Task<CrudResult> SendOTP(string UserId, string Email);
        Task<bool> QuestionCountIsOne(string userId,int consultationCategoryId);
        Task<CrudResult> SetConsultation(string userId,string consultationId,int consulationCategoryId);
        Task<List<ConsultationCategoryView>> GetConsultation();
        Task<List<ConsultationView>> GetConsultation(string userId);
        Task<Consultation> GetConsultation(string userId, int consultationCategoryId);
        Task<List<ConsultationView>> GetRecentConsultation(string userId);
        Task<bool> UserConsultationStatus(string userId);
        Task<bool> GetCompleteConsultationStatus(string userId);
        Task<CrudResult> SetOrderIdConsultation(string orderId, string consultationId);
        Task<CrudResult> SetOrderIdChat(string orderId, string userId);
        Task<List<ConsultationCustomMedication>> GetMedications(int consultationCategoryId);
        Task<List<ConsultationCategoryDetailView>> GetDetailViews(int consultationCategoryId,bool IsHomeDelivery);
        Task<bool> IsConsultationMedicationExist(int? medicationId);
        Task<bool> IsConsultationDetailExist(int? detailId);
        Task<CrudResult> SaveTempConsultationMedication(string consultationId, string userId, int medicationId);
        Task<CrudResult> SaveTempConsultationDetail(string consultationId, string userId, int detailId);
        Task<CrudResult> SavePharmacyDetail(PharmacyInformationModel model, string consultationId, string userId);
    }
}
 