using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class EliminarRecorrido : CodeActivity<Resultado>
    {
        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
                var id = servicioRepositorio.ObtenerRecorridoIdPorGuid(context.WorkflowInstanceId);
                resultado = servicioComandos.Ejecutar(new Dominio.Comandos.EliminarRecorrido {Id = id });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Recorrido_ErrorAlCrear);
            }
            return resultado;
        }
    }
}
