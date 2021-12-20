using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ProveedorEsSustentable : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> ProveedorId { get; set; }
        public OutArgument<bool> EsSustentable { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
           try
           {
               var srvRepositorio = context.GetExtension<IServicioRepositorio>();
               EsSustentable.Set(context, srvRepositorio.EsProveedorSustentable(ProveedorId.Get<int>(context)));
           }
           catch 
           {
               
           }
        }
    }
}
