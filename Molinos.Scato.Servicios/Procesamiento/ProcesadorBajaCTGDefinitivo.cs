using System;
using System.Configuration;
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
    public class ProcesadorBajaCTGDefinitivo : ProcesadorComando<DarDeBajaCTGDefinitivo>
    {
        private readonly CTGServicePortType serviceAfipCTG;
        private readonly IAccesoWsCtg accesoWsCtg;
        public ProcesadorBajaCTGDefinitivo(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CTGServicePortType serviceAfipCTG, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCTG = serviceAfipCTG;
        }

        public override Resultado Ejecutar(DarDeBajaCTGDefinitivo comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////
           
            var resultado = new Resultado();

            var centro = Repositorio.Obtener<Centro>(comando.CentroId);
            var recorrido = Repositorio.Obtener<Recorrido>(g => g.InstanciaWorkflow == comando.WorkflowId);
            if(!recorrido.PesoBruto.HasValue || !recorrido.PesoTara.HasValue)
            {
                Log.Error("ProcesadorBajaCTGDefinitivo - PESO NO ENCONTRADO");
                resultado.Errores.Add("CodigoDeBaja", "No se puede ejecutar la baja definitiva de un camión sin peso");
                return resultado;
            }
            try
            {
                Log.Debug("ProcesadorBajaCTGDefinitivo - Creo la autorizacion");
                // Obtengo la autorizacion
                var auth = accesoWsCtg.ObtenerAuthType(centro.Cuit.Replace("-", string.Empty), resultado);
                // Armo la consulta
                Log.Debug("ProcesadorBajaCTGDefinitivo - armo consulta");
                var confirmarArriboRequest = new confirmarDefinitivoRequestType
                    {
                        auth = auth
                        ,
                        datosConfirmarDefinitivo = new datosConfirmarDefinitivoType
                            {
                                cartaPorte = Convert.ToInt64(comando.Dto.NroCartaPorte),
                                ctg = Convert.ToInt64(comando.Dto.CTG),
                                pesoNeto =  Convert.ToInt64((recorrido.PesoBruto ?? 0) - (recorrido.PesoTara ?? 0)),
                                especie = recorrido.Material.CodigoEspecie.HasValue ? recorrido.Material.CodigoEspecie.Value : 0,
                                codigoCosecha = recorrido.Vehiculo.CartaPorte.Cosecha.Replace("-", String.Empty),
                                especieSpecified = true,
                                pesoNetoSpecified = true
                            }
                    };

                Log.Debug("ProcesadorBajaCTGDefinitivo - Inicio la consulta");
                // Realizo la consulta
                var response = serviceAfipCTG.confirmarDefinitivo(new confirmarDefinitivoRequest { request = confirmarArriboRequest });
                Log.Debug("ProcesadorBajaCTGDefinitivo - Realizo la consulta ");

                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsCtg"] == "1")
                    {

                        Repositorio.Agregar(new ControlRecorrido
                        {
                            Actividad = "ProcesadorBajaCTGDefinitivo",
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
                
                if (response.response != null && response.response.arrayErrores != null && response.response.arrayErrores.Any())
                {
                    resultado.Errores.Add("CodigoDeBaja", response.response.arrayErrores.FirstOrDefault());
                    Log.Error("ProcesadorBajaCTGDefinitivo -" + response.response.arrayErrores.FirstOrDefault());
                }
                else
                {
                    //Si no hay errores, registro la baja del CTG
                    var bajaCtg = Repositorio.ObtenerMasReciente<BajaCTG>(x => x.WorkflowId == comando.WorkflowId, x => x.Fecha);
                    if (bajaCtg != null)
                    {
                        bajaCtg.CodigoDeBajaDefinitivo = response.response != null ? response.response.detalle : response.ToString();
                    }

                    Log.Debug("Baja de CTG {0} procesada correctamente", comando.Dto.CTG);
                }
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo dar de baja definitiva el ctg {0}", comando.Dto.CTG);
                resultado.Errores.Add("CodigoDeBaja", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e,"No se pudo dar de baja definitiva el ctg {0}", comando.Dto.CTG);
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