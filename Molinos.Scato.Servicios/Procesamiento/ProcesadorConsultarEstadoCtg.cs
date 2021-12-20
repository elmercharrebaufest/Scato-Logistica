using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCTGWebService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarEstadoCtg : ProcesadorComando<ConsultarEstadoCtg>
    {
        const int cLongitudMaximaCupo = 16; //Se establece el limite maximo de caracteres para el campo cupo de la consulta de CTG.
        private CTGServicePortType serviceAfipCTG;
        private IAccesoWsCtg accesoWsCtg;
        private IKernel kernel;

        public ProcesadorConsultarEstadoCtg(IRepositorio repositorio, IConversor conversor, ILogger log,
                                 CTGServicePortType serviceAfipCTG, IAccesoWsCtg accesoWsCtg, IKernel kernel)
            : base(repositorio, conversor, log)
        {
            this.accesoWsCtg = accesoWsCtg;
            this.kernel = kernel;
            this.serviceAfipCTG = serviceAfipCTG;
        }

        public override Resultado Ejecutar(ConsultarEstadoCtg comando)
        {
            /////////////
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);
            //////////////

            var resultado = new ResultadoDetalleCTG();

            var centro = Repositorio.Obtener<Centro>(comando.CentroId);

            try
            {
                Log.Debug("ProcesadorConsultarDetalleCTG - Creo la autorizacion");
                // Obtengo la autorizacion
                var auth = accesoWsCtg.ObtenerAuthType(centro.Cuit.Replace("-", string.Empty), resultado);
                // Armo la consulta
                Log.Debug("ProcesadorConsultarDetalleCTG - armo consulta");
                var consultarCTGRequest = new consultarDetalleCTGRequestType
                {
                    auth = auth,
                    ctg = Convert.ToInt64(comando.Ctg)
                };

                Log.Debug("ProcesadorConsultarDetalleCTG - Inicio la consulta");
                // Realizo la consulta
                serviceAfipCTG.consultarDetalleCTG(new consultarDetalleCTGRequest { request = consultarCTGRequest });
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo consultar el ctg {0}", comando.Ctg);
                resultado.Errores.Add("2", "Error, el servicio de AFIP nos responde: " + e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo consultar el ctg {0}", comando.Ctg);
                resultado.Errores.Add("2", Textos.Error_Generico);
            }
            return resultado;
        }

    }
}
