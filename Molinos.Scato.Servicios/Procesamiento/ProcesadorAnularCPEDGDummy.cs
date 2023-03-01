using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.ServiceModel;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorAnularCPEDGDummy : ProcesadorComando<AnularCPEDGDummy>
    {
        private readonly CpePortType serviceAfipCpe;
        private readonly IAccesoWsCtg accesoWsCtg;

        public ProcesadorAnularCPEDGDummy(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CpePortType serviceAfipCpe, IAccesoWsCtg accesoWsCtg)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.serviceAfipCpe = serviceAfipCpe;
        }

        public override Resultado Ejecutar(AnularCPEDGDummy comando)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);

            var resultado = new Resultado();

            try
            {
                ValidarDatosNecesarios(resultado, comando);
                if (resultado.HayErrores)
                {
                    return resultado;
                }

                var centro = Repositorio.Obtener<Centro>(x => x.Id == comando.CentroId);

                Log.Debug("Creo la anulacion");

                // Obtengo la autorizacion
                var cuitRepresentado = centro.Cuit != null ? centro.Cuit.Replace("-", string.Empty) : string.Empty;
                var auth = accesoWsCtg.ObtenerAuth(cuitRepresentado, resultado);

                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                if (comando.TipoCPE == Constantes.TipoCP.CPCamion)
                {
                    resultado = AnularCPEAutomotorDG(comando, auth);
                }
                else
                {
                    resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo hacer la Anulacion del NroOrden {0} ", comando.NroOrden);
                resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
            }
            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }
            return resultado;
        }

        private Resultado AnularCPEAutomotorDG(AnularCPEDGDummy comando, Auth auth)
        {
            var resultado = new Resultado();
            var response = new anularCPEResponse();
            var request = "";

            try
            {
                var anularCpeRequest = new anularCPERequest
                {
                    auth = auth,
                    solicitud = new AnularCPESolicitud
                    {
                        cartaPorte = new AfipCPDigitalService.CartaPorte
                        {
                            tipoCPE = comando.TipoCPE,
                            nroOrden = comando.NroOrden,
                            sucursal = comando.Sucursal
                        }
                    }
                };
                request = anularCpeRequest.ToXml();
                Log.Debug($"Request Anular : {request}");

                // Realizo la consulta
                response = serviceAfipCpe.anularCPE(anularCpeRequest);
                Log.Debug("Realizo la consulta ");

                if (response != null && response?.respuesta?.errores?.Length > 0)
                {
                    resultado.Errores.Add(response?.respuesta?.errores?.FirstOrDefault()?.codigo, response?.respuesta?.errores?.FirstOrDefault()?.descripcion);
                    Log.Error("Error en la Anulacion: {0}", response?.respuesta?.errores?.FirstOrDefault()?.descripcion);
                }
                else if (response != null && !string.IsNullOrEmpty(response?.respuesta?.cabecera?.nroCTG.ToString()))
                {
                    var responseAFIP = response?.respuesta;
                    Repositorio.Agregar(
                        new LogAfipCpe
                        {
                            Servicio = "ProcesadorAnularCPEDGDummy",
                            Consulta = request,
                            Respuesta = responseAFIP is null ? string.Empty : responseAFIP.ToXml(),
                            Fecha = DateTime.Now
                        });
                    Log.Debug("La Anulacion {0} procesada correctamente", comando.NroOrden);
                }
                else
                {
                    Log.Error("La Anulacion {0} respuesta invalida", comando.NroOrden);
                    throw new Exception("Ocurrio un error al anular la  carta de porte electronica.");
                }
            }
            catch (FaultException e)
            {
                Log.Error(e, "Ocurrió un error con AFIP al intentar anularCPE para el NroOrden {0} ", comando.NroOrden);
                resultado.Errores.Add("CodigoDeBaja", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar anularCPE para el NroOrden {0} ", comando.NroOrden);
                resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
            }

            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }

            return resultado;
        }

        private void ValidarDatosNecesarios(Resultado resultado, AnularCPEDGDummy comando)
        {
            var centro = Repositorio.Existe<Centro>(x => x.Id == comando.CentroId);

            if (!centro)
            {
                resultado.Errores.Add("CodigoDeBaja", String.Format("No existe un {0} para esta alta.", Textos.Centro));
            }
        }
    }
}