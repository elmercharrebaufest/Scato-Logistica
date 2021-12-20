using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEnvioMail : ProcesadorComando<EnvioMail>
    {
        public ProcesadorEnvioMail(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(EnvioMail comando)
        {
            var resultado = new Resultado();
            try
            {
                var message = new MailMessage();

                foreach (var email in comando.Destinatarios)
                {
                    message.To.Add(new MailAddress(email));
                }
                
                if (comando.Origen != null)
                {
                    message.From = new MailAddress(comando.Origen);
                }

                if (comando.Attachment != null)
                {
                    message.Attachments.Add(new Attachment(new MemoryStream(comando.Attachment), comando.AttachmentName));
                }
                if (comando.Attachment2 != null)
                {
                    message.Attachments.Add(new Attachment(new MemoryStream(comando.Attachment2), comando.AttachmentName2));
                }

                message.Subject = comando.Titulo;
                message.Body = comando.Cuerpo;
                message.IsBodyHtml = true;
                using (var smtpClient = new SmtpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                    delegate (object s

                                , X509Certificate certificate

                                , X509Chain chain

                                , SslPolicyErrors sslPolicyErrors)

                    { return true; };
                    smtpClient.Send(message);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al enviar mail");
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
