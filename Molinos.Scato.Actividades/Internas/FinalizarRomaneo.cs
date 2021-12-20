using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class FinalizarRomaneo : CodeActivity<Resultado>
    {
        public OutArgument<decimal> PesoNetoRomaneo { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var recorrido = new Resultado();
            
            try
            {
                var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
                var servicioComandos = context.GetExtension<IServicioComandos>();

                var pesoNetoRomaneo = servicioRepositorio.ObtenerPesoNetoRomaneo(context.WorkflowInstanceId);
                servicioComandos.Ejecutar(new Dominio.Comandos.FinalizarRomaneo { WorkflowId = context.WorkflowInstanceId });

                PesoNetoRomaneo.Set(context, pesoNetoRomaneo);
            }
            catch (Exception e)
            {
                recorrido.Errores.Add("",e.Message);
            }
            return recorrido;
        }
    }
}
