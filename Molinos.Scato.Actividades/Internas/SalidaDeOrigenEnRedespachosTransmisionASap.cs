using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class SalidaDeOrigenEnRedespachosTransmisionASap : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Mov975Request> Request { get; set; }
        public OutArgument<bool> FuncionaServicio { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioSap = context.GetExtension<ZSDWS_SCATO>();
            var request = Request.Get(context);

            var resultado = new Resultado();

            try
            {
                var respuesta = servicioSap.Mov975(request);
                FuncionaServicio.Set(context, !String.IsNullOrEmpty(respuesta.Mov975Response.Resultado.MBLNR));
                if (FuncionaServicio.Get(context))
                {
                    var servicioComandos = context.GetExtension<IServicioComandos>();
                    servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                    {
                        InstanceId = context.WorkflowInstanceId,
                        DocumentoInternoSap = respuesta.Mov975Response.Resultado.MBLNR,
                        NumeroDeDocumentoSap = respuesta.Mov975Response.Resultado.XBLNR,
                    });
                }
                else
                {
                    resultado.Errores.Add("WorkflowId", respuesta.Mov975Response.Resultado.TEXT);
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
