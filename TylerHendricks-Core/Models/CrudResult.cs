using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class CrudResult
    {
        public string ErroCode { get; set; }
        public string ErrorMassage { get; set; }
        public bool Succeeded { get; set; }
        public bool Failed { get; set; }
        public string InnerErrorMessage { get; set; }
        public string Data { get; set; }
    }
}
