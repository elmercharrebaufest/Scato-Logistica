using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class MovimientoStockTransmisionSap : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<MovAjuste> Request { get; set; }

        public OutArgument<bool> FuncionaServicio { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioSap = context.GetExtension<ZSDWS_SCATO>();

            var request = Request.Get(context);
            var requestObj = new MovAjusteRequest(request);
            
            var resultado = new Resultado();

            try
            {
                var respuesta = servicioSap.MovAjuste(requestObj);
                FuncionaServicio.Set(context, respuesta.MovAjusteResponse.Resultado.MSGNR == "000");
                if (!FuncionaServicio.Get(context))
                {
                    resultado.Errores.Add("WorkflowId", respuesta.MovAjusteResponse.Resultado.TEXT);
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("WorkflowId", Textos.MovimientoStockSap_ErrorEnLaCarga + ": "+ e.Message);
            }
            return resultado;
        }
    }
}
