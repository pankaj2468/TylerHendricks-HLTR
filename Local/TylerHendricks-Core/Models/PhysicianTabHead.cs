using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class PhysicianTabHead
    {
        public uint ReadyToPrescribe { get; set; }
        public uint FurtherReviewNeeded { get; set; }
        public uint PendingResponse { get; set; }
        public uint AllPatients { get; set; }
        public uint Denied { get; set; }
        public uint PotentialDuplicates { get; set; }
        public uint Responded { get; set; }
        public uint NotCurrentlyServiceInYourArea { get; set; }
    }
}
