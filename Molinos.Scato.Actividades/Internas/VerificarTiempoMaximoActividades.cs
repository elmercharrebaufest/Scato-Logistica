using System;
using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarTiempoMaximoActividades : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> CodigoControl { get; set; }
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        public OutArgument<bool> Autorizado { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
            var codigoControl = CodigoControl.Get<string>(context);
            var instanceId = InstanceId.Get<Guid>(context);
            var controlDeTiempo = servicioRepositorio.ObtenerControlDeTiempoPorCodigoControlPorGuid(codigoControl, instanceId);

            if (controlDeTiempo != null)
            {
                var tiempo = servicioRepositorio.ObtenerTiempoEntreActividades(instanceId,
                                                                               controlDeTiempo.ActividadDesde,
                                                                               controlDeTiempo.ActividadHasta);

                var autorizado = tiempo.HasValue && tiempo.Value >= TimeSpan.Zero && TimeSpan.FromMinutes(controlDeTiempo.TiempoMaximo) >= tiempo.Value;
                Autorizado.Set(context, autorizado);
            }
            else
            {
                // Si no está definido el control, no se lo autoriza para que vaya a la pantalla
                Autorizado.Set(context, false);
            }
        }
    }
}
