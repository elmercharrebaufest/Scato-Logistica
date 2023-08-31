using System;
using System.Activities;
using System.Linq;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class PuestoComandoPuertoRechazo : CodeActivity
    {
        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }

        public OutArgument<string> Observacion { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);

            Observacion.Set(context, controlRecorrido.Mensaje + "\n" + " " + controlRecorrido.Comentario);
        }
    }
}
