using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class LimpiarLecturasDeTarjeta : CodeActivity<Resultado>
    {
        public InArgument<ControlRecorridoDto> ControlRecorridoDto { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();
            var controlRecorrido = ControlRecorridoDto.Get<ControlRecorridoDto>(context);
            
            var resultado = new Resultado();
            try
            {
                servicioComandos.Ejecutar(new EliminarLecturaDeTarjeta { Id = controlRecorrido.PuestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.FinDeActividad_ErrorEnLaCarga);
            }
            
            return resultado;
        }
    }
}
