using System.Collections.Generic;
using TylerHendricks_Core.Models;
namespace TylerHendricks_Core.ViewModel
{
    public class ChooseYourConsultationViewModel
    {
        public List<ConsultationView> consultationViews { get; set; }
        public ConsultationView consultationView { get; set; }
    }
}
