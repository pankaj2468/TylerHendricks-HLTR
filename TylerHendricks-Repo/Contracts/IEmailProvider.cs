using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;

namespace TylerHendricks_Repo.Contracts
{
    public interface IEmailProvider
    {
        void PlaintTextGmail(string templateName, string recipientEmail, string subject, string body);
        void TemplateGmail(string templateName, dynamic model);
        void TemplateGmailWithAttachment(string recipientEmail, string templateName, dynamic model);
    }
}
