using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class EnvironmentalResource
    {
        public PayeezyConfig PayeezyCredentials { get; set; }
        public AWSS3Config AWSS3Credentials { get; set; }
    }
}
