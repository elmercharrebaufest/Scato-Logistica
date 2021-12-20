using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public sealed class DemorarBajaCtg : CodeActivity
    {
       
        [RequiredArgument]
        public OutArgument<bool> DemorarBaja { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DemorarBaja.Set(context, true);
        }
    }
}