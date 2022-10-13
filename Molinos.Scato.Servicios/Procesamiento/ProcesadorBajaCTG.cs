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
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorBajaCTG : ProcesadorComando<DarDeBajaCTG>
    {
        private readonly CTGServicePortType serviceAfipCtg;
        private readonly IAccesoWsCtg accesoWsCtg;
        public ProcesadorBajaCTG(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CTGServicePortType serviceAfipCtg, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCtg = serviceAfipCtg;
        }

        public override Resultado Ejecutar(DarDeBajaCTG comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////
           
            var resultado = new Resultado();

            try
            {
                var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                if (centro == null)
                {
                    throw new Exception(String.Format(Textos.Error_Requerido, Textos.Centro));
                }
                var transportista = Repositorio.Obtener<Transportista>(comando.Dto.TransportistaId ?? 0);
                if (transportista == null)
                {
                    throw new Exception(String.Format(Textos.Error_Requerido, Textos.Transportista));
                }

                if (comando.Vehiculo == null)
                {
                    throw new Exception(String.Format(Textos.Error_Requerido, Textos.Vehiculo));
                }

                Log.Debug("Creo la autorizacion");
                // Obtengo la autorizacion
                var cuitRepresentado = centro.Cuit != null ? centro.Cuit.Replace("-", string.Empty): string.Empty;
                var auth = accesoWsCtg.ObtenerAuthType(cuitRepresentado, resultado);
                // Armo la consulta
                Log.Debug("armo consulta");
                var confirmarArriboRequest = new confirmarArriboRequestType
                    {
                        auth = auth
                        ,
                        datosConfirmarArribo = new datosConfirmarArriboType
                            {
                                cantKilosCartaPorte = comando.Vehiculo.PesoNetoOrigen == null ? 0 : comando.Vehiculo.PesoNetoOrigen.Value,
                                cartaPorte = Convert.ToInt64(comando.Dto.NroCartaPorte),
                                ctg = Convert.ToInt64(comando.Dto.CTG),
                                cuitTransportista = Convert.ToInt64(transportista.Cuit.Replace("-", "")),
                                establecimiento = Convert.ToInt64(centro.CodigoEstablecimiento),
                                establecimientoSpecified = true,
                                cuitChofer = Convert.ToInt64(comando.Dto.Chofer.Cuil.Replace("-", ""))
                            }
                    };

                Log.Debug("Inicio la consulta");
                // Realizo la consulta
                var response = serviceAfipCtg.confirmarArribo(new confirmarArriboRequest { request = confirmarArriboRequest });
                Log.Debug("Realizo la consulta ");

                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsCtg"] == "1")
                    {

                        Repositorio.Agregar(new ControlRecorrido
                        {
                            Actividad = "ProcesadorBajaCTG",
                            Fecha = DateTime.Now,
                            Comentario = confirmarArriboRequest.ToXml(),
                            NombreUsuario = "",
                            WorkflowInstanceId = comando.WorkflowId,
                        });
                        Repositorio.GuardarCambios();
                    }
                }
                catch (Exception e)
                {
                    Log.Debug("Error al loguear request Afip CTG", e.Message);
                }

                if (response.response != null && response.response.arrayErrores.Any() && !response.response.arrayErrores.Any(x => x.Contains("ya está confirmado")))
                {
                    resultado.Errores.Add("CodigoDeBaja", response.response.arrayErrores.FirstOrDefault());
                    Log.Error("Error en la Baja: {0}", response.response.arrayErrores.FirstOrDefault());
                }
                else if (response.response != null)
                {
                    Log.Debug("Baja de ctg {0} procesada correctamente", comando.Dto.CTG);
                }
                else
                {
                    Log.Error("Baja de ctg {0} respuesta invalida", comando.Dto.CTG);
                }
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo hacer la baja de CTG del codigo {0}", comando.Dto.CTG);
                resultado.Errores.Add("CodigoDeBaja", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo hacer la baja de CTG del codigo {0}", comando.Dto.CTG);
                resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
            }
            var bajaCtg = Repositorio.ObtenerMasReciente<BajaCTG>(x => x.WorkflowId == comando.WorkflowId, x => x.Fecha);
            if (bajaCtg == null)
            {
                Repositorio.Agregar(
                  new BajaCTG
                  {
                      CartaPorte = Repositorio.Obtener<Dominio.Entidades.CartaPorte>(comando.Dto.Id),
                      CodigoDeBaja = (!resultado.HayErrores) ? "ProcesadorBajaCTG" : null,
                      Fecha = DateTime.Now,
                      WorkflowId = comando.WorkflowId
                  });
            }
            else
            {
                bajaCtg.CodigoDeBaja = (!resultado.HayErrores) ? "ProcesadorBajaCTG" : null;
                bajaCtg.Fecha = DateTime.Now;
            }

            Repositorio.GuardarCambios();

            return resultado;
        }
    }
}