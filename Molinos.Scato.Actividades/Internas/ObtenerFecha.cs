using System;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    /// <summary>
    /// Expone el Id de Instancia del workflow. Este Id es usado como correlation handle en los workflows, por ser de fácil acceso.
    /// </summary>
    public class ObtenerFecha : CodeActivity<DateTime>
    {
        protected override DateTime Execute(CodeActivityContext context)
        {
            return DateTime.Now;
        }
    }
}