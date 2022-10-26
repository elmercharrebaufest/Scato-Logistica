using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class SetearPuestoEnTransito : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<ControlRecorridoDto> ControlRecorridoDto { get; set; }

        public OutArgument<int> PuestoDeTrabajoEnTransitoId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            var controlRecorrido = ControlRecorridoDto.Get<ControlRecorridoDto>(context);
            if (controlRecorrido != null)
            {
                PuestoDeTrabajoEnTransitoId.Set(context, controlRecorrido.PuestoDeTrabajoId);
            }
            return resultado;
        }
    }
}
