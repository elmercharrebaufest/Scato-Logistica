using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresoDeCartaPorteRedespacho : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<CartaPorteDto> CartaPorte { get; set; }
        public InArgument<Pesada> Pesada { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();

            var cartaPorte = CartaPorte.Get<CartaPorteDto>(context);
            var pesada = Pesada.Get<Pesada>(context);

            var resultado = new Resultado();
            try
            {
                
                servicioComandos.Ejecutar(new Dominio.Comandos.CrearCartaPorte { Orden = cartaPorte, NombreWorkflow = "", InstanciaWorkflowId = context.WorkflowInstanceId});

            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Transportista_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}