using System;
using System.Activities;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class GuardarAutorizacion : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }

        [RequiredArgument]
        public InArgument<int> ChoferId { get; set; }
        public InArgument<int> CentroId { get; set; }

        [RequiredArgument]
        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }


        protected override Resultado Execute(CodeActivityContext context)
        {
            var choferId = ChoferId.Get<int>(context);
            var patente = Patente.Get<string>(context);
            var centroId = CentroId.Get<int>(context);

            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);
            var resultado = new Resultado();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                servicioComandos.Ejecutar(new GuardarAutorizacionComando
                {
                    Patente = patente,
                    ChoferId = choferId,
                    Mensaje = controlRecorrido.Comentario,
                    NombreUsuario = controlRecorrido.NombreUsuario,
                    Centro = centroId
                });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}
