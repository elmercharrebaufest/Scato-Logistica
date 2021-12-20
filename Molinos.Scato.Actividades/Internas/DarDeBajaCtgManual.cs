using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public sealed class DarDeBajaCtgManual : CodeActivity
    {
        public InArgument<CartaPorteDto> Orden { get; set; }
        public InArgument<string> CodigoDeBaja { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }
        public InArgument<Guid> WorkflowId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var orden = Orden.Get<CartaPorteDto>(context);
            var codigoDeBaja = CodigoDeBaja.Get<string>(context);
            var workflowId = WorkflowId.Get<Guid>(context);

            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var baja = new BajaCTGDto
                {
                    CartaPorteId = orden.Id,
                    CodigoDeBaja = orden.Cpe ? orden.NroCartaPorte : codigoDeBaja,
                    Fecha = DateTime.UtcNow,
                    WorkflowId = workflowId
                };

                var resultado = servicioComandos.Ejecutar(new CrearBajaCTG { Dto = baja });

                Resultado.Set(context, resultado);
            }
            catch (Exception e)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("", e.Message);
                Resultado.Set(context, resultado);
            }
        }
    }
}