using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ActualizarMuestraEnvioACamara : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }
        public InArgument<DateTime?> FechaDescarga { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var workflowId = WorkflowId.Get<Guid>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            var fechaDescarga = FechaDescarga.Get<DateTime?>(context);

            var srvRepositorio = context.GetExtension<IServicioRepositorio>();
            var servicioComandos = context.GetExtension<IServicioComandos>();

            var logActividad = new LogActividadDto { Actividad = "Actualizar Muestra Envio A Camara", ActividadXaml = "ActualizarMuestraEnvioACamara", WorkflowInstanceId = workflowId };
            logActividad.Fecha = DateTime.Now;
            var resultado = new Resultado();
            try
            {
                resultado = servicioComandos.Ejecutar(new CrearLogActividad { Dto = logActividad });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.LogActividad_ErrorEnLaCarga);
            }

            var recorrido = srvRepositorio.ObtenerRecorridoPorGuid(workflowId);
            if (recorrido.Calado != null)
            {
                var muestraEnvioACamara = srvRepositorio.ObtenerUltimaMuestraEnvioACamaraPorCaladoId(recorrido.Calado.Id);

                if (muestraEnvioACamara != null)
                {
                    if (recorrido.PesoNeto.HasValue)
                    {
                        muestraEnvioACamara.PesoNeto = recorrido.PesoNeto.Value;
                    }
                    if (fechaDescarga.HasValue)
                    {
                        muestraEnvioACamara.FechaDescarga = fechaDescarga;
                    }
                    servicioComandos.Ejecutar(new ModificarEnvioACamara { Dto = muestraEnvioACamara });
                }
            }

            try
            {
                resultado = servicioComandos.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ActualizarMuestraEnvioACamara", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("1", Textos.FinDeActividad_ErrorEnLaCarga);
            }
        }
    }
}
