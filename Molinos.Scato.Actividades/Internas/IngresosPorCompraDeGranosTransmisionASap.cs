using System;
using System.Activities;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresosPorCompraDeGranosTransmisionASap : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Fill_Z1000Request> Request { get; set; }
        public OutArgument<bool> FuncionaServicio { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioSap = context.GetExtension<ZSDWS_SCATO>();
            var request = Request.Get(context);
            var resultado = new Resultado();
            try
            {
                var respuesta = servicioSap.Fill_Z1000(request);
                if (respuesta.Fill_Z1000Response.Resultado.Any(a => a.MSGNR != "000"))
                {
                    FuncionaServicio.Set(context, false);
                    resultado.Errores.Add("InstanciaWorkflow", respuesta.Fill_Z1000Response.Resultado.First(a => a.MSGNR != "000").TEXT);
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
