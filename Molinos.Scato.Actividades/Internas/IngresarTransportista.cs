using System;
using System.Activities;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresarTransportista : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<TransportistaDto> Transportista { get; set; }
        public InArgument<int> CartaPorteId { get; set; }
        public OutArgument<CartaPorteDto> CartaPorte { get; set; }
        public InArgument<Guid> WorkflowId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var transportista = Transportista.Get<TransportistaDto>(context);
            var cartaPorteId = CartaPorteId.Get<int>(context);
            var workflowId = WorkflowId.Get<Guid>(context);
            var resultado = new Resultado();
            try
            {
                resultado = servicioComandos.Ejecutar(new CrearTransportistaCartaPorte { Dto = transportista, CartaPorteId = cartaPorteId, WorkflowId = workflowId });
                var carta = repositorio.ObtenerCartaPorte(cartaPorteId);
                CartaPorte.Set(context,carta);
                context.GetExtension<ScatoPersistenceParticipant>().TransportistaId = carta.TransportistaId;
                context.GetExtension<ScatoPersistenceParticipant>().Transportista = carta.Transportista;
            }
            catch (Exception e)
            {
                resultado.Errores.Add("", Textos.Transportista_ErrorEnLaCarga + ": " + e.Message);
            }

            return resultado;
        }
    }
}
