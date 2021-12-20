using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Internas
{
    public class PuestoComando : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }
        public OutArgument<string> NombreUsuario { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);

            NombreUsuario.Set(context, controlRecorrido.NombreUsuario);
            Resultado.Set(context, new Resultado());
        }
    }
}
