using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCTGWebService;
using Molinos.Scato.Servicios.AfipWebService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarDestinatarioCTG : ProcesadorComando<ConsultarDestinatarioCTG>
    {
        const int cLongitudMaximaCupo = 16; //Se establece el limite maximo de caracteres para el campo cupo de la consulta de CTG.
        private CTGServicePortType serviceAfipCTG;
        private IAccesoWsCtg accesoWsCtg;
        public ProcesadorConsultarDestinatarioCTG(IRepositorio repositorio, IConversor conversor, ILogger log, LoginCMS serviceAfip,
                                 CTGServicePortType serviceAfipCTG, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCTG = serviceAfipCTG;
        }

        public override Resultado Ejecutar(ConsultarDestinatarioCTG comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////
           
            var resultado = new Resultado();

            var centro = Repositorio.Obtener<Centro>(comando.CentroId);
            var recorrido = Repositorio.Obtener<Recorrido>(g => g.InstanciaWorkflow == comando.WorkflowId);
            //var recorridoId = Repositorio.ObtenerProyeccion<Recorrido, int>(g => g.InstanciaWorkflow == comando.WorkflowId, x => x.Id);
            try
            {
                Log.Debug("ProcesadorConsultarDestinatarioCTG - Creo la autorizacion");
                // Obtengo la autorizacion
                var auth = accesoWsCtg.ObtenerAuthType(centro.Cuit.Replace("-", string.Empty), resultado);
                // Armo la consulta
                Log.Debug("ProcesadorConsultarDestinatarioCTG - armo consulta");
                var consultarCTGRequest = new consultarDetalleCTGRequestType
                    {
                        auth = auth,
                        ctg = Convert.ToInt64(comando.Dto.CTG)
                    };

                Log.Debug("ProcesadorConsultarDestinatarioCTG - Inicio la consulta");
                // Realizo la consulta
                var response = serviceAfipCTG.consultarDetalleCTG(new consultarDetalleCTGRequest { request = consultarCTGRequest });
                Log.Debug("ProcesadorConsultarDestinatarioCTG - Realizo la consulta ");
                var resEstado = response.response.consultarDetalleCTGDatos;

                if (response.response != null && response.response.arrayErrores != null && response.response.arrayErrores.Any())
                {
                    resultado.Errores.Add("CodigoDeBaja", response.response.arrayErrores.FirstOrDefault());
                    Log.Error("ProcesadorConsultarDestinatarioCTG -" + response.response.arrayErrores.FirstOrDefault());
                }
                else if (resEstado.estado == "Anulado" || resEstado.estado == "Rechazado")
                {
                    resultado.Errores.Add("2", "Consulta Detalle CTG - CTG Anulado o Rechazado");
                    Log.Error("ProcesadorConsultarDestinatarioCTG - CTG {0} Anulado o Rechazado ", resEstado.ctg);
                }
                else if (response.response != null)
                {
                    //Si no hay errores, registro la Consulta detalle del CTG
                    var respuesta = response.response.consultarDetalleCTGDatos;
                    var entidad = new DestinatarioCTG
                        {   
                            Recorrido = recorrido,
                            CanjeRemito = respuesta.canjeadorComoRemitenteComercial ?? "",
                            NumeroCCPP = respuesta.cartaPorte.ToString(CultureInfo.InvariantCulture).PadLeft(12,'0'),
                            Cosecha = respuesta.cosecha,
                            CTG = respuesta.ctg.ToString(CultureInfo.InvariantCulture),
                            CuitCanjeador = respuesta.cuitCanjeador == null ? null : respuesta.cuitCanjeador.Split(' ')[0],
                            CuitDestinatario = respuesta.cuitDestinatario == null ? null : respuesta.cuitDestinatario.Split(' ')[0],
                            CuitDestino = respuesta.cuitDestino == null ? null : respuesta.cuitDestino.Split(' ')[0],
                            Especie = respuesta.especie,
                            Establecimiento = respuesta.establecimiento.ToString(CultureInfo.InvariantCulture),
                            Estado = respuesta.estado,
                            FechaConf = respuesta.fechaHoraConfirmacionDefinitiva,
                            PesoNetoCarga = respuesta.pesoNetoCarga,
                            Solicitante = respuesta.solicitante == null ? null : respuesta.solicitante.Split(' ')[0],
                            Cupo = respuesta.turno == null ? null : (respuesta.turno.Length <= cLongitudMaximaCupo ? respuesta.turno : respuesta.turno.Substring(0, cLongitudMaximaCupo)),
                        };
                    Repositorio.Agregar(entidad);
                    Log.Debug("Consulta de CTG {0} procesada correctamente", comando.Dto.CTG);
                }
                else
                {
                    resultado.Errores.Add("respuesta","No se obtuvo respuesta desde AFIP");
                    Log.Debug("Consulta de CTG {0} sin respuesta", comando.Dto.CTG);
                }
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo consultar el ctg {0}", comando.Dto.CTG);
                resultado.Errores.Add("CodigoDeBaja", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo consultar el ctg {0}", comando.Dto.CTG);
                resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
            }
            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }
            return resultado;
        }
    }
}