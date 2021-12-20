using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class AsignarAutomaticamentePuestoComando : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        public OutArgument<bool> EstaAsignado { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var servicioComandos = context.GetExtension<IServicioComandos>();

            var instanceId = InstanceId.Get<Guid>(context);

            var asignacionDeRecorrido = repositorio.BuscarAsignacionDeRecorridoPorInstanceId(instanceId);
            var estaAsignado = false;
            if (asignacionDeRecorrido != null)
            {
                var materialPorCentro = repositorio.ObtenerMaterialPorCentroPorInstanceId(instanceId);
                var asignacionDto = new AsignacionDto
                    {
                        AlmacenId = asignacionDeRecorrido.AlmacenDestinoId,
                        BalanzaBrutoId = asignacionDeRecorrido.BalanzaBrutoId,
                        BalanzaTaraId = asignacionDeRecorrido.BalanzaTaraId,
                        CalleId = asignacionDeRecorrido.CalleId,
                        HidraulicasId = asignacionDeRecorrido.HidraulicasId,
                        InstanceIds = instanceId.ToString(),
                        MaterialId = materialPorCentro.MaterialId,
                    };
                var resultado =
                    servicioComandos.Ejecutar(new ActualizarPuestocomando
                        {
                            Dto = asignacionDto,
                            BalanzasObligatorias = false
                        }) as ResultadoPuestoComando;
                if (!resultado.HayErrores)
                {
                    estaAsignado = true;
                }
            }
            EstaAsignado.Set(context, estaAsignado);
        }
    }
}
