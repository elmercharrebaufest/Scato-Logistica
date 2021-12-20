using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class CrearLineUp : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InOutArgument<LineUpDto> LineUp { get; set; }
       
        protected override Resultado Execute(CodeActivityContext context)
        {
            var lineUp = LineUp.Get<LineUpDto>(context);
            var resultado = new ResultadoCrearWorkflow();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var resultadoCrear = servicioComandos.Ejecutar(new Dominio.Comandos.CrearLineUp
                {
                    LineUp = lineUp,
                    InstanciaWorkflowId = context.WorkflowInstanceId
                }) as ResultadoCrear;
                resultado.Id = resultadoCrear.Id;

                if (resultadoCrear.HayErrores)
                {
                    resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
                }
            }
            catch (Exception)
            {
                resultado.Errores.Add("1", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}
