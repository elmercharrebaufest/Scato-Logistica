using System;
using System.Activities;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class ZE7550TransmisionASap : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Z_SDMF_RFC_ZE7550Request> Request { get; set; }
        public OutArgument<bool> FuncionaServicio { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioSap = context.GetExtension<ZSDWS_SCATO>();
            var request = Request.Get(context);
            var resultado = new Resultado();
            try
            {
                var respuesta = servicioSap.Z_SDMF_RFC_ZE7550(request);
                if (respuesta.Z_SDMF_RFC_ZE7550Response.EX_RESULTADO.Any(a => a.TIPO == "E"))
                {
                    FuncionaServicio.Set(context, false);
                    resultado.Errores.Add("InstanciaWorkflow", respuesta.Z_SDMF_RFC_ZE7550Response.EX_RESULTADO.First(a => a.TIPO == "E").TEXTO);
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
