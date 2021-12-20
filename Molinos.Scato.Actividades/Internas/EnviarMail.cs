using System;
using System.Activities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{

    public class EnviarMail : CodeActivity<bool>
    {
        [RequiredArgument]
        public InArgument<string> To { get; set; }
        public InArgument<string> From { get; set; }
        [RequiredArgument]
        public InArgument<string> Subject { get; set; }
        public InArgument<Collection<Attachment>> Attachments { get; set; }
        public InArgument<string> CC { get; set; }
        public InArgument<IDictionary<string, string>> Tokens { get; set; }
        [RequiredArgument]
        public InArgument<string> Body { get; set; }
        public InArgument<Guid> InstanceId { get; set; }
        public OutArgument<string> Error { get; set; }

        protected override bool Execute(CodeActivityContext context)
        {
            var firmaProvider = context.GetExtension<IFirmaProvider>();
            var servComando = context.GetExtension<IServicioComandos>();
            var instanceId = InstanceId.Get(context);
                        
            try
            {
                var message = new MailMessage();

                var emails = To.Get(context).Split(';');
                foreach (var email in emails)
                {
                    if (!string.IsNullOrEmpty(email))
                    {
                        message.To.Add(new MailAddress(email));
                    }
                }
                var cc = CC.Get(context);
                if (cc != null)
                {
                    emails = cc.Split(';');
                    foreach (var email in emails)
                    {
                        if (!string.IsNullOrEmpty(email))
                        {
                            message.CC.Add(new MailAddress(email));
                        }
                    }
                }
                var from = From.Get(context);
                if (from != null)
                {
                    message.From = new MailAddress(from);
                }
                var attachments = Attachments.Get(context);
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        message.Attachments.Add(attachment);
                    }
                }
                var body = Body.Get(context);
                var subject = Subject.Get(context);
                if ((Tokens.Get(context) != null) && (Tokens.Get(context).Count > 0))
                {
                    var t = Tokens.Get(context);
                    var firma = firmaProvider.ObtenerFirmaSinLogo();
                    t.Add("[Firma]", firma.Descripcion);

                    foreach (string key in t.Keys)
                    {
                        body = body.Replace(key, t[key]);
                        subject = subject.Replace(key, t[key]);
                    }
                }
                message.Subject = Regex.Replace(subject, @"\t|\n|\r", "");
                message.Body = body;
                message.IsBodyHtml = true;

                using (var smtpClient = new SmtpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                    delegate(object s

                             , X509Certificate certificate

                             , X509Chain chain

                             , SslPolicyErrors sslPolicyErrors)

                    { return true; };
                    smtpClient.Send(message);
                    Error.Set(context, string.Empty);
                    return true;
                }
            }
            catch (Exception ex)
            {
                servComando.Ejecutar(new CrearControlRecorrido()
                    {
                        Dto = new ControlRecorridoDto()
                            {
                                Actividad = "EnviarMailCatch",
                                Comentario = ex.StackTrace,
                                WorkflowInstanceId = instanceId
                            }
                    });
                servComando.Ejecutar(new CrearControlRecorrido()
                {
                    Dto = new ControlRecorridoDto()
                    {
                        Actividad = "EnviarMailCatch",
                        Comentario = ex.InnerException != null ? ex.InnerException.Message : "no hay inner",
                        WorkflowInstanceId = instanceId
                    }
                });
                servComando.Ejecutar(new CrearControlRecorrido()
                {
                    Dto = new ControlRecorridoDto()
                    {
                        Actividad = "EnviarMailCatch",
                        Comentario = "Mensaje: " + ex.Message,
                        WorkflowInstanceId = instanceId
                    }
                });
                Error.Set(context, ex.StackTrace);
                return false;
            }
        }
    }
}
