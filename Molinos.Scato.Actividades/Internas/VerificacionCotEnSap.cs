using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

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
            var instanceId = InstanceId.Get<Guid>(context);
            
            var resultado = new Resultado();
            try
            {
                var recorridoDto = srvRepositorio.ObtenerRecorridoValoresSapPorGuid(instanceId);
                var numeroDocumento = recorridoDto.NumeroDeDocumentoSap;

                var respuesta =
                    servicioSap.ValidacionCOT(new ValidacionCOTRequest
                    {
                        ValidacionCOT =
                            new ValidacionCOT
                            {
                                PtoVtaRemito = numeroDocumento != null ? numeroDocumento.Substring(0, 4) : "",
                                NroComprobante = numeroDocumento != null ? numeroDocumento.Substring(5, 8) : "",
                                DocInternoSAP = recorridoDto.DocumentoInternoSap,
                            }
                    });
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
