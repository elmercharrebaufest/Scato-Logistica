using System;
using System.Activities;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class LeerPesoMaximo : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        [RequiredArgument]
        public InArgument<TipoDeWorkflow> TipoPesoMaximo { get; set; }

        public OutArgument<int> PesoMaximo { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
                var peso = servicioRepositorio.LeerPesoMaximo(InstanceId.Get<Guid>(context),TipoPesoMaximo.Get<TipoDeWorkflow>(context));

                PesoMaximo.Set(context, peso);
            }
            catch
            {
                
            }
        }
    }
}
