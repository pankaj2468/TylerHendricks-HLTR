using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.IO;
using TylerHendricks_Repo.Contracts;

namespace TylerHendricks_Repo.Services
{
    public class EmailProvider : IEmailProvider
    {
        private readonly IServiceProvider _sp;

        public EmailProvider(IServiceProvider sp)
        {
            _sp = sp;
        }

        public async void PlaintTextGmail(string templateName, string recipientEmail, string subject, string body)
        {
            try
            {
                using (var scope = _sp.CreateScope())
                {
                    using (var common = new Common())
                    {
                        if ((await common.GetNotification(templateName).ConfigureAwait(true)).Status)
                        {
                            var Mailer = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
                            var email = Mailer
                                    .To(recipientEmail)
                                    .Subject(subject)
                                    .Body(body, true);
                            await email.SendAsync().ConfigureAwait(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async void TemplateGmail(string templateName, dynamic model)
        {
            try
            {
                using (var scope = _sp.CreateScope())
                {
                    using (var common = new Common())
                    {
                        if ((await common.GetNotification(templateName).ConfigureAwait(true)).Status)
                        {
                            var Mailer = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
                            var email = Mailer
                                    .To(model.GetType().GetProperty("recipientEmail").GetValue(model, null))
                                    .Subject(model.GetType().GetProperty("subject").GetValue(model, null))
                                    .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/EmailTemplate/{ model.GetType().GetProperty("templateName").GetValue(model, null)}.html", model);
                            await email.SendAsync();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async void TemplateGmailWithAttachment(string recipientEmail, string templateName, dynamic model)
        {
            try
            {
                using (var scope = _sp.CreateScope())
                {
                    using (var common = new Common())
                    {
                        if ((await common.GetNotification(templateName).ConfigureAwait(true)).Status)
                        {

                            var Mailer = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
                            var email = Mailer
                                    .To(recipientEmail)
                                    .Subject("Test email from Fluent")
                                    .AttachFromFilename($"{Directory.GetCurrentDirectory()}/wwwroot/EmailTemplate/{templateName}.cshtml")
                                    .UsingCultureTemplateFromFile($"{Directory.GetCurrentDirectory()}/wwwroot/EmailTemplate/{templateName}.cshtml", model, CultureInfo.CurrentUICulture);
                            await email.SendAsync();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
