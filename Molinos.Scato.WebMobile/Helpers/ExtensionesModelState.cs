using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;

namespace Molinos.Scato.WebMobile.Helpers
{
    public static class ExtensionesModelState
    {
        public static void AgregarErrores(this ModelStateDictionary estado, Resultado resultado)
        {
            foreach (var error in resultado.Errores)
            {
                estado.AddModelError(error.Key, error.Value);
            }
        }
    }
}