using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class Notes
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public bool IsFile { get; set; }
        public string DateAdd { get; set; }
    }
}
