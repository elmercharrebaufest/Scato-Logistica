using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarReimpresion : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }
        [RequiredArgument]
        public InArgument<int?> ImpresionId { get; set; }
        public InOutArgument<Resultado> Resultado { get; set; }
        public OutArgument<bool> ReimprimeDocumento { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = Resultado.Get<Resultado>(context);
            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);
            var impresionId = ImpresionId.Get<int?>(context) ?? 0;
            ReimprimeDocumento.Set(context, controlRecorrido.Decision);
            if (controlRecorrido.Decision)
            {
                try
                {
                    var srvComando = context.GetExtension<IServicioComandos>();
                    srvComando.Ejecutar(new EliminarDocumento { Id = impresionId });
                }
                catch (Exception e)
                {
                    resultado.Errores.Add("EliminarImpresion",e.Message);
                }
            }
            Resultado.Set(context, resultado);
            return resultado;
        }
    }
}
