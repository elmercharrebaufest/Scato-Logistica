using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class BalanzaNoEnCero : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<int> BalanzaId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var balanzaId = BalanzaId.Get<int>(context);
            
            var resultado = new Resultado();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                resultado = servicioComandos.Ejecutar(new ModificarBalanzaEstaEnCero { BalanzaId = balanzaId, EstaEnCero = false });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}
