using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class FletesDobleTramoTransmisionASap : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<FletesDobleTramoRequest> Request { get; set; }
        public OutArgument<bool> FuncionaServicio { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioSap = context.GetExtension<ZSDWS_SCATO>();
            var request = Request.Get(context);
            var resultado = new Resultado();
            try
            {
                var respuesta = servicioSap.FletesDobleTramo(request);
                if (respuesta.FletesDobleTramoResponse.Resultado.MSGNR.Trim() != "0")
                {
                    FuncionaServicio.Set(context, false);
                    resultado.Errores.Add("InstanciaWorkflow", respuesta.FletesDobleTramoResponse.Resultado.TEXT);
                }
                else
                {
                    FuncionaServicio.Set(context, true);
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("InstanciaWorkflow", Textos.MovimientoStockSap_ErrorEnLaCarga + ": " + e.Message);
            }
            return resultado;
        }
    }
}
