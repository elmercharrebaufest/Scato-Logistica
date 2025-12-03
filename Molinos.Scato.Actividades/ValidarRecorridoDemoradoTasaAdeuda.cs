using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades
{
    public class ValidarRecorridoDemoradoTasaAdeuda : CodeActivity<Resultado>
    {
        public OutArgument<bool> DemoradoPorTasadaAdeudada { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
            var servicioComandos = context.GetExtension<IServicioComandos>();

            var estaDemoradoPorTasaAdeuda = servicioRepositorio.EstaDemoradoPorTasaAdeudada(context.WorkflowInstanceId);
            DemoradoPorTasadaAdeudada.Set(context, estaDemoradoPorTasaAdeuda);

            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = nameof(ValidarRecorridoDemoradoTasaAdeuda),
                Fecha = DateTime.Now,
                Comentario = estaDemoradoPorTasaAdeuda ? "Demorado por Tasa Adeudada" : "No está demorado por Tasa Adeudada",
                NombreUsuario = "",
                WorkflowInstanceId = context.WorkflowInstanceId,
            };
            servicioComandos.Ejecutar(new CrearControlRecorrido
            {
                Dto = controlRecorrido
            });

            return new Resultado();
        }
    }
}