using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Linq;

namespace Molinos.Scato.Actividades
{
    public class ActualizarAsignacionNoGranoEnRecorrido : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();

            var comandos = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();

            try
            {

                var recorrido = repositorio.ObtenerRecorridoPorGuid(context.WorkflowInstanceId);

                var asignacionNoGranoEnRecorrido = repositorio.ObtenerAsignacionNoGranoEnRecorridoPorRecorridoId(recorrido.Id);
                asignacionNoGranoEnRecorrido.AplicaConteo = true;


                var asignacionNoGrano = new Dominio.Comandos.ActualizarAsignacionNoGranoEnRecorrido()
                {
                    Dto = asignacionNoGranoEnRecorrido,
                };

              resultado =  comandos.Ejecutar(asignacionNoGrano);
            }
            catch (System.Exception ex)
            {

                resultado.Error(string.Empty, "Ocurrió un error. " + ex.Message);
            }

            if (resultado.HayErrores)
            {
                comandos.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "ActualizarAsignacionNoGranoEnRecorrido",
                        Fecha = DateTime.Now,
                        Comentario = resultado.Errores.Values.First(),
                        NombreUsuario = "",
                        WorkflowInstanceId = context.WorkflowInstanceId,
                    }
                });
            }

            

        }
    }
}