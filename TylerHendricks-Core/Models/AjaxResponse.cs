using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class AjaxResponse
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool Succeeded { get; set; }
        public object Data { get; set; }
    }
}
