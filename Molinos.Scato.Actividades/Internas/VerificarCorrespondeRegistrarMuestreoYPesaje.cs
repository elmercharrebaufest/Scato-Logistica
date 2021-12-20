using System;
using System.Activities;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarCorrespondeRegistrarMuestreoYPesaje : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<bool> CorrespondeLlamada { get; set; }
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        [RequiredArgument]
        public InArgument<TipoVehiculo> TipoVehiculo { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var instanceId = InstanceId.Get<Guid>(context);
            var tipoVehiculo = TipoVehiculo.Get<TipoVehiculo>(context);
            try
            {
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var existe = srvRepositorio.RequiereTecnologia(instanceId);
                if (existe && tipoVehiculo == Dominio.Enums.TipoVehiculo.Tren)
                {
                    existe = srvRepositorio.CorrespondeRegistrarMuestreoYPesajeTren(instanceId);
                }
                CorrespondeLlamada.Set(context, existe);
            }
            catch
            {
                CorrespondeLlamada.Set(context, false);
            }
            
        }
    }
}
