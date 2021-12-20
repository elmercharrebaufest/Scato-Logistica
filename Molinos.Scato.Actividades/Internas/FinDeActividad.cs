using System;
using System.Activities;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class FinDeActividad : CodeActivity<Resultado>
    {
        public InArgument<ControlRecorridoDto> ControlRecorridoDto { get; set; }

        public InArgument<Guid?> InstanceId { get; set; }

        public OutArgument<int> PuestoDeTrabajoId { get; set; }

        public OutArgument<string> NombreUsuario { get; set; } 

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioNotificarUsuario>();
            var servicioComandos = context.GetExtension<IServicioComandos>();
            var servicioRepositorio = context.GetExtension<IServicioRepositorio>();

            var controlRecorrido = ControlRecorridoDto.Get<ControlRecorridoDto>(context);
            var instanceId = InstanceId.Get<Guid?>(context);
            if (instanceId.HasValue)
            {
                controlRecorrido.WorkflowInstanceId = instanceId.Value;
            }

            var workflowInstanceId = instanceId ?? controlRecorrido.WorkflowInstanceId;
            var resultado = new Resultado();
            try
            {
                resultado = servicioComandos.Ejecutar(new Dominio.Comandos.FinDeActividad { InstanceId = workflowInstanceId, Actividad = controlRecorrido.ActividadXaml, PuestoDeTrabajoId = controlRecorrido.PuestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.FinDeActividad_ErrorEnLaCarga);
            }
            try
            {
                if (resultado.HayErrores)
                {
                    var centroId = servicioRepositorio.ObtenerCentroIdPorInstanceId(workflowInstanceId);

                    servicio.Notificar(new NotificacionDto
                    {
                        Mensaje = resultado.Errores.Last().Value,
                        Grupo = centroId + "|" + controlRecorrido.NombreUsuario,
                        TipoAlerta = TipoAlerta.Informacion
                    });
                }
                servicioComandos.Ejecutar(new CrearControlRecorrido { Dto = controlRecorrido});
                PuestoDeTrabajoId.Set(context, controlRecorrido.PuestoDeTrabajoId);
                NombreUsuario.Set(context, controlRecorrido.NombreUsuario);
            }
            catch (Exception)
            {
                resultado.Errores.Add("ErrorNotificar", Textos.FinDeActividad_ErrorEnLaCarga);
            }
            return resultado;
        }
    }
}
