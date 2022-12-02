using System;
using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ProveedorEsSustentable : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> ProveedorId { get; set; }
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        public OutArgument<bool> EsSustentable { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
           try
           {
                var instanceId = InstanceId.Get<Guid>(context);
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var cargaDeCupo = srvRepositorio.ObtenerCargaDeCupoPorGuid(instanceId);
                if (cargaDeCupo != null && cargaDeCupo.IngresoAvanceCPEAutomatico)
                {
                    EsSustentable.Set(context, false);
                } else
                {
                    EsSustentable.Set(context, srvRepositorio.EsProveedorSustentable(ProveedorId.Get<int>(context)));
                }
           }
           catch 
           {
               
           }
        }
    }
}
