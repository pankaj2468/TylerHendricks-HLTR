using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TylerHendricks_Core.Models;

namespace TylerHendricks_Core.ViewModel
{
    public class QuestionNaireViewModel
    {
        public ChooseYourMedicationModel chooseYourMedicationModel { get; set; }
        public PharmacyInformationModel pharmacyInformationModel { get; set; }
        public FinishAccountSetupModel finishAccountSetupModel { get; set; }
        public string UserId { get; set; }
        public bool? IsMedicationDelivery { get; set; } = false;
        public int ConsultationCategoryId { get; set; }
        public string ConsultationId { get; set; }
        public int? MedicationId { get; set; }
        public int? DetailId { get; set; }
    }
}
