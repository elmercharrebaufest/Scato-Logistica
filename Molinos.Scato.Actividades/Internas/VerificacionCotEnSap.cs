using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificacionCotEnSap : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        public OutArgument<bool> CotCorrecto { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioSap = context.GetExtension<ZSDWS_SCATO>();
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();
            var log = context.GetExtension<ILogger>();
            var instanceId = InstanceId.Get<Guid>(context);

            var resultado = new Resultado();
            try
            {
                var recorridoDto = srvRepositorio.ObtenerRecorridoValoresSapPorGuid(instanceId);
                var numeroDocumento = recorridoDto.NumeroDeDocumentoSap;

                var documentos = numeroDocumento != null ? numeroDocumento.Split(new[] { '-', 'R' }) : null;
                var validacion = new ValidacionCOT
                {
                    PtoVtaRemito = documentos != null && documentos.Length > 0 ? documentos[0] : string.Empty,
                    NroComprobante = documentos != null && documentos.Length > 1 ? documentos[1] : string.Empty,
                    DocInternoSAP = recorridoDto.DocumentoInternoSap ?? string.Empty,
                };

                var validacionRequest = new ValidacionCOTRequest
                {
                    ValidacionCOT = validacion
                };

                log.Debug($"ValidacionCOTRequest {instanceId}:\n {validacionRequest.ToXml()}");
                var respuesta = servicioSap.ValidacionCOT(validacionRequest);
                log.Debug($"ValidacionCOTResponse {instanceId}:\n {respuesta.ToXml()}");

                CotCorrecto.Set(context, respuesta.ValidacionCOTResponse.COTAprobado == "X");
                if (!CotCorrecto.Get(context))
                {
                    resultado.Errores.Add("WorkflowId", respuesta.ValidacionCOTResponse.Error);
                }
                //CotCorrecto.Set(context, true); //Solo para testeo
            }
            catch (Exception e)
            {
                resultado.Errores.Add("WorkflowId", Textos.MovimientoStockSap_ErrorEnLaCarga + ": " + e.Message);
            }
            return resultado;
        }
    }
}