using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarCorrespondeLlamadaCompliance : CodeActivity
    {
        [RequiredArgument]
        public InArgument<bool> ValidaCompliance { get; set; }
        public OutArgument<bool> CorrespondeLlamadaCompliance { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            if (ValidaCompliance.Get<bool>(context))
            {
                CorrespondeLlamadaCompliance.Set(context, true);
            }
            else
            {
                CorrespondeLlamadaCompliance.Set(context, false);
            }
        }
    }
}
