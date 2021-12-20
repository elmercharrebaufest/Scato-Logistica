using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresosEgresosFazonesTransmisionASap : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<IngresosEgresosFazonesRequest> Request { get; set; }
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
                var respuesta = servicioSap.IngresosEgresosFazones(request);
                FuncionaServicio.Set(context, !String.IsNullOrEmpty(respuesta.IngresosEgresosFazonesResponse.Resultado.MBLNR));
                if (FuncionaServicio.Get(context))
                {
                    var servicioComandos = context.GetExtension<IServicioComandos>();
                    servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                        {
                            InstanceId = context.WorkflowInstanceId,
                            DocumentoInternoSap = respuesta.IngresosEgresosFazonesResponse.Resultado.MBLNR,
                            NumeroDeDocumentoSap = respuesta.IngresosEgresosFazonesResponse.Resultado.XBLNR
                        });
                }
                else
                {
                    resultado.Errores.Add("WorkflowId", respuesta.IngresosEgresosFazonesResponse.Resultado.TEXT);
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
