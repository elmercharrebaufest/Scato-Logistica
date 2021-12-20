//using HtmlAgilityPack;
//using Molinos.Scato.Dominio.Comandos;
//using Molinos.Scato.Dominio.Entidades;
//using Molinos.Scato.Repositorio;
//using Molinos.Scato.Servicios.Conversiones;
//using Molinos.Scato.Servicios.Orquestador;
//using Ninject.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;

//namespace Molinos.Scato.Servicios.Procesamiento
//{
//    public class ProcesadorActualizarEstadoPuerto : ProcesadorComando<ActualizarEstadoPuerto>
//    {
//        IServicioOrquestador servicioOrquestador;
//        public ProcesadorActualizarEstadoPuerto(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOrquestador servicioOrquestador)
//            : base(repositorio, conversor, log)
//        {
//            this.servicioOrquestador = servicioOrquestador;
//        }

//        public override Resultado Ejecutar(ActualizarEstadoPuerto comando)
//        {
//            var estado = ObtenerAlturaRio() ?? new EstadoPuerto();
//            ObtenerVelocidadViento(estado);
//            if (estado != null)
//            {
//                Repositorio.Agregar(estado);
//            }
//            Repositorio.GuardarCambios();
//            return new Resultado();
//        }

//        private EstadoPuerto ObtenerAlturaRio()
//        {
//            try
//            {
//                WebClient webClient = new WebClient();
//                string page = webClient.DownloadString("https://contenidosweb.prefecturanaval.gob.ar/alturas/");

//                var doc = new HtmlDocument();
//                doc.LoadHtml(page);
//                var labels = new string[] { "Fecha Hora:", "Puerto:", "Ultimo Registro:" };
//                var sloRow = doc.DocumentNode.SelectSingleNode("//table[@class='table table-hover fpTable']")
//                    .Descendants("tr")
//                    .Select(tr =>
//                        tr.ChildNodes.Where(y => labels.Contains(y.GetAttributeValue("data-label", "")))
//                            .Select(td => td.InnerText.Trim()).ToList()).Where(x => x.Count > 0 && x[0] == "SAN LORENZO").FirstOrDefault();
//                var fecha = DateTime.ParseExact(sloRow[2], "dd/MMM/yy - HHmm",
//                                           System.Globalization.CultureInfo.InvariantCulture);
//                return sloRow != null ? new EstadoPuerto { AlturaDelRio = sloRow[1], FechaAlturaRio = fecha } : null;
//            }
//            catch(Exception e)
//            {
//                Log.Error(e, "Error al obtener la altura del rio del viento");

//                return null;
//            }
//        }

//        private void ObtenerVelocidadViento(EstadoPuerto estadoPuerto)
//        {
//            try
//            {
//                var respuesta = (ResultadoMeteorologica)servicioOrquestador.Ejecutar(
//                new EjecutarEstacionMeteorologica { CodigoDispositivo = "ESTMETSLO" });

//                foreach (var dato in respuesta.Imagenes)
//                {
//                    if (dato.Descripcion == "VIENTO")
//                    {
//                        estadoPuerto.VelocidadViento = dato.Detalle[1];
//                        estadoPuerto.DireccionViento = dato.Detalle[3];

//                    }
//                    if (dato.Descripcion == "FECHA")
//                    {
//                        estadoPuerto.FechaVelocidadViento = DateTime.ParseExact(dato.Detalle[1], "dd-MM-yyyy HH:mm",
//                                           System.Globalization.CultureInfo.InvariantCulture);
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                Log.Error(e, "Error al obtener la velocidad del viento");
//            }
//        }
//    }
//}