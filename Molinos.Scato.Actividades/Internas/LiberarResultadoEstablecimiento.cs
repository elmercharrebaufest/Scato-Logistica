using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{

    public class LiberarResultadoEstablecimiento : CodeActivity<Resultado>
    {
        protected override Resultado Execute(CodeActivityContext context)
        {
           return new Resultado();
        }
    }
}
