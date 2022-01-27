using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Configuration;
using System.Web.Configuration;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.PanelServerAppPool)]
    public class PanelServiciosWebController : BaseController
    {
        private readonly IConfiguracionProvider configuracion;
        private readonly ILogger log;
        private readonly IListaDeWorkflows servicioWorkflows;
        private readonly IServicioNotificarUsuario notificar;
        private readonly ZSDWS_SCATO servicioSap;
        private readonly IServicioComandos servicioComandos;

        public PanelServiciosWebController(IConfiguracionProvider configuracion, ILogger log, IServicioRepositorio servicio, IListaDeWorkflows servicioWorkflows,
            IServicioNotificarUsuario notificar, ZSDWS_SCATO servicioSap, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.configuracion = configuracion;
            this.log = log;
            this.servicioWorkflows = servicioWorkflows;
            this.notificar = notificar;
            this.servicioSap = servicioSap;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index()
        {
            var servidorPorDefecto = SetearVista();
            return Listar(servidorPorDefecto, "Index");
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string servidorNombre,string vista = "Listar")
        {
            try
            {
                var model = ObtenerServiciosDeCapaWeb();
                model.Servicios.AddRange(servicio.ListarEstadoDeServicios());
                model.Servicios.AddRange(servicioWorkflows.ListarEstadoDeServicios());
                model.Servicios = model.Servicios.GroupBy(x => x.Nombre).Select(x => x.First()).ToList();
                return View(vista, model);
            }
            catch (Exception e)
            {
                log.Error("No se pudo ejecutar el servicio" + e.Message);
                return View(vista, new PanelServiciosWebDto
                {
                    Error = "No se pudo ejecutar el servicio " + servidorNombre + ": " + e.Message
                });
            }
        }

        private PanelServiciosWebDto ObtenerServiciosDeCapaWeb()
        {
            var clientSection = (WebConfigurationManager.GetSection("system.serviceModel/client") as ClientSection);
            var model = new PanelServiciosWebDto {Servicios = new List<ServicioDto>()};
            foreach (
                ChannelEndpointElement cee in
                    clientSection.Endpoints.Cast<ChannelEndpointElement>()
                                 .Where(cee => !cee.Address.AbsoluteUri.StartsWith("net.msmq")))
            {
                model.Servicios.Add(new ServicioDto
                    {
                        Nombre = cee.Name,
                        Url = cee.Address.AbsoluteUri
                    });
            }
            return model;
        }

        private string SetearVista()
        {
            var elem = configuracion.AppSettings["HostsServiciosWeb"];
            var list = elem.Split('|').Select(x => x.Split('/')[2].Split('.')[0]).ToList();
            ViewBag.Servers = list.Select(x => new SelectListItem { Text = x, Value = x.ToString(CultureInfo.InvariantCulture), Selected = x == list.FirstOrDefault() }).ToList();
            
            return list.FirstOrDefault();
        }

        public bool Ping(string url)
        {
            var pingable = false;
            try
            {
                var myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Timeout = 3000;
                var response = (HttpWebResponse)myRequest.GetResponse();

                pingable = response.StatusCode == HttpStatusCode.OK;
            }
            catch (WebException e)
            {
                if ( e.Response != null && (int) ((HttpWebResponse) e.Response).StatusCode == 407)
                {
                    pingable = true;
                }
            }
            return pingable;
        }

        public string EnviarMail(string to,string from,string subj, string body)
        {
            var message = new MailMessage();


            message.To.Add(new MailAddress(to));
            if (!String.IsNullOrEmpty(from))
            {
                message.From = new MailAddress(from);
            }
            
            message.IsBodyHtml = true;
            message.Body = String.IsNullOrEmpty(body) ? "Test" : body;
            message.Subject = String.IsNullOrEmpty(subj) ? "Test" : subj;
            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                    delegate(object s

                             , X509Certificate certificate

                             , X509Chain chain

                             , SslPolicyErrors sslPolicyErrors)

                    { return true; };
                    smtpClient.Send(message);
                }
            }
            catch (Exception ex)
            {
                return GetExceptionDetails(ex);
            }
            return String.Format("Enviado: From:{0}  To:{1}  Subj:{2}  Body:{3}", message.From, message.To, message.Subject, message.Body);
        }

        public string GetExceptionDetails(Exception exception)
        {
            var properties = exception.GetType()
                                    .GetProperties();
            var fields = properties
                             .Select(property => new
                             {
                                 Name = property.Name,
                                 Value = property.GetValue(exception, null)
                             })
                             .Select(x => String.Format(
                                 "{0} = {1}",
                                 x.Name,
                                 x.Value != null ? x.Value.ToString() : String.Empty
                             ));
            return String.Join("\n", fields);
        }
        [DatosUsuario]
        public void ConsultarEstadoServiciosWeb(DatosUsuario datosUsuario)
        {
            var ping = new List<ServicioDto>();
            //Ping SAP
            try
            {
                var codigosDeCentroSap = servicio.ObtenerCodigoDeCentroPorId(datosUsuario.CentroId);
                var cupo = "MOL1111/11111111";
                log.Debug("ValidarCupoEnSap Generico Ping: {0}, centro: {1}", cupo, string.Join(",", codigosDeCentroSap));

                var response = servicioSap.Z_SDMF_RFC_Z2100(new Z_SDMF_RFC_Z2100Request
                {
                    Z_SDMF_RFC_Z2100 = new Z_SDMF_RFC_Z2100()
                    {
                        IM_CENTRO = new ZMPES5210[] { new ZMPES5210 { CENTRO = codigosDeCentroSap[0] } },
                        IM_CODIGO = new ZMPES5200[] { new ZMPES5200 { CODIGO = cupo } }
                    }
                });
            }
            catch (Exception e)
            {
                ping.Add(new ServicioDto { Nombre = "SAP SCATO", Estado = false });
            }
            //Ping AFIP
            var res = servicioComandos.Ejecutar(new ConsultarEstadoCtg { Ctg = "11111111", CentroId = datosUsuario.CentroId });
            if (res.HayErrores)
            {
                ping.Add(new ServicioDto { Nombre = "CTG Service", Estado = false });
            }
            //Ping CNRT
            var resultadoEscalables = servicioComandos.Ejecutar(new ConsultarEscalables { Patente = "ASD123", Usuario = datosUsuario.NombreUsuario }) as ResultadoEscalables;
            if (resultadoEscalables.HayErrores)
            {
                ping.Add(new ServicioDto { Nombre = "CNRT Service", Estado = false });
            }
            var resultadoCPe = servicioComandos.Ejecutar(new ConsultarCPEDummy());
            if (resultadoCPe.HayErrores)
            {
                ping.Add(new ServicioDto { Nombre = "CPE Service", Estado = false });
            }
            var centros = servicio.ListarCentros();
            foreach (var c in centros)
            {
                notificar.Notificar(new NotificacionDto
                {
                    Grupo = c.Id.ToString(),
                    Mensaje = ping.Count == 0 ? "Ok" : string.Join(", ", ping.Select(x => x.Nombre)),
                    TipoAlerta = TipoAlerta.NotificacionEstadoWeb
                });
            }
        }
    }
}
