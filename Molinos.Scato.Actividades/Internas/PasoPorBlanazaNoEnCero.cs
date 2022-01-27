using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class PasoPorBlanazaNoEnCero : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<int> PuestoId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var puestoId = PuestoId.Get<int>(context);
            
            var resultado = new Resultado();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
                var balanza = servicioRepositorio.ObtenerBalanzaPorPuestoDeTrabajoAutomatico(puestoId);
                resultado = servicioComandos.Ejecutar(new ModificarBalanzaEstaEnCero { BalanzaId = balanza.Id, EstaEnCero = false });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}
