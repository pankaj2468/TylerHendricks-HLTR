using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class AWSS3Config
    {
        public string BucketName { get; set; }
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string ImageDirectory { get; set; }
        public string EmailTemplateDirectory { get; set; }
    }
}
