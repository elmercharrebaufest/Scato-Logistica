using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class VerificarProveedorExcluidoIntacta : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<bool> EsProveedorExcluido { get; set; }
        [RequiredArgument]
        public InArgument<int> ProveedorId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var proveedorId = ProveedorId.Get<int>(context);
            try
            {
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var esExcluido = srvRepositorio.EsProveedorExcluidoIntacta(proveedorId);
                EsProveedorExcluido.Set(context, esExcluido);
            }
            catch
            {
                EsProveedorExcluido.Set(context, false);
            }
            
        }
    }
}
