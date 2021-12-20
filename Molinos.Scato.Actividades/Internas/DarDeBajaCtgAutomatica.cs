using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public sealed class DarDeBajaCtgAutomatica : CodeActivity
    {
        public InOutArgument<int> Intentos { get; set; }
        public InArgument<CartaPorteDto> Orden { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }
        public InArgument<Guid> WorkflowId { get; set; }

        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<VehiculoDto> Vehiculo { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var intentos = Intentos.Get<int>(context);
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var orden = Orden.Get<CartaPorteDto>(context);
                var centroId = CentroId.Get<int>(context);
                var vehiculo = Vehiculo.Get<VehiculoDto>(context);
                var workflowId = WorkflowId.Get<Guid>(context);

                var resultado = orden.Cpe ?
                    servicioComandos.Ejecutar(new ConfirmarArribo { Dto = orden, Vehiculo = vehiculo, CentroId = centroId, WorkflowId = workflowId }) :
                    servicioComandos.Ejecutar(new DarDeBajaCTG { Dto = orden, Vehiculo = vehiculo, CentroId = centroId, WorkflowId = workflowId});

                Resultado.Set(context, resultado);
            }
            catch (Exception e)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("", e.Message);
                Resultado.Set(context, resultado);
            }
            Intentos.Set(context, ++intentos);
        }
    }
}