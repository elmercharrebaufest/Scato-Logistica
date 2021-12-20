using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class LlegadaADestinoEnRedespachosTransmisionASap : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Mov305Request> Request { get; set; }
        public OutArgument<bool> FuncionaServicio { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioSap = context.GetExtension<ZSDWS_SCATO>();

            var request = Request.Get(context);

            var resultado = new Resultado();

            try
            {
                var respuesta = servicioSap.Mov305(request);
                FuncionaServicio.Set(context, !String.IsNullOrEmpty(respuesta.Mov305Response.Resultado.MBLNR));
                if (!FuncionaServicio.Get(context))
                {
                    resultado.Errores.Add("WorkflowId", respuesta.Mov305Response.Resultado.TEXT);
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
