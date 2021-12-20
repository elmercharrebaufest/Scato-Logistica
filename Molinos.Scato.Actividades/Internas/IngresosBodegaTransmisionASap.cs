using System;
using System.Activities;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresosBodegaTransmisionASap : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<IngresosBodegaRequest> Request { get; set; }
        public InArgument<int> VehiculoId { get; set; }
        public OutArgument<bool> FuncionaServicio { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioSap = context.GetExtension<ZSDWS_SCATO>();
            var request = Request.Get(context);
            var vehiculoId = VehiculoId.Get<int>(context);

            var resultado = new Resultado();

            try
            {

                var respuesta = servicioSap.IngresosBodega(request);
                FuncionaServicio.Set(context, !String.IsNullOrEmpty(respuesta.IngresosBodegaResponse.EX_RESULTADO.MBLNR));
                if (FuncionaServicio.Get(context))
                {
                    var servicioComandos = context.GetExtension<IServicioComandos>();
                    servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                        {
                            InstanceId = context.WorkflowInstanceId,
                            DocumentoInternoSap = respuesta.IngresosBodegaResponse.EX_RESULTADO.MBLNR,
                            NumeroDeDocumentoSap = respuesta.IngresosBodegaResponse.EX_RESULTADO.XBLNR
                        });
                }
                else
                {
                    resultado.Errores.Add("WorkflowId", respuesta.IngresosBodegaResponse.EX_RESULTADO.TEXT);
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("WorkflowId", Textos.MovimientoStockSap_ErrorEnLaCarga + ": " + e.Message);
            }
            return resultado;
        }
    }
}
