using System;
using System.Collections.Generic;
using System.Text;
using TylerHendricks_Core.Models;

namespace TylerHendricks_Core.ViewModel
{
    public class MessagesViewModel
    {
        public List<UserChat> userChats { get; set; }
        public bool? IsRequestedPhotoId { get; set; }
        public bool? IsRequestedSelfie { get; set; }
        public bool? IsRequestedMedication { get; set; }
        public bool StartChat { get; set; }
        public bool ChatPayment { get; set; }
    }
}
