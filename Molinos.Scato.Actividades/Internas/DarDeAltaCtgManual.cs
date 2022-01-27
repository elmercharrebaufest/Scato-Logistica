using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public sealed class DarDeAltaCtgManual : CodeActivity
    {
        public InOutArgument<CartaPorteDto> Orden { get; set; }
        public InArgument<string> CodigoCTG { get; set; }
        public InArgument<decimal> TarifaReferencia { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }
        public InArgument<Guid> WorkflowId { get; set; }
        public InArgument<string> Sucursal { get; set; }
        public InArgument<string> NroOrden { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var repositorio = context.GetExtension<IServicioRepositorio>();
                
                var codigoCTG = CodigoCTG.Get<string>(context);
                var tarifaReferencia = TarifaReferencia.Get<decimal>(context);
                var workflowId = WorkflowId.Get<Guid>(context);
                var orden = repositorio.ObtenerCartaPortePorInstanceId(workflowId);
                var sucursal = Sucursal.Get<string>(context);
                var nroOrden = NroOrden.Get<string>(context);
                var recorridoId = repositorio.ObtenerRecorridoIdPorGuid(workflowId);

                var resultado = servicioComandos.Ejecutar(new CrearAltaCTG { Dto = new AltaCTGDto {
                        CartaPorteId = orden.Id,
                        CodigoCTG = codigoCTG.Trim(),
                        TarifaReferencia = tarifaReferencia,
                        Fecha = DateTime.UtcNow,
                        WorkflowId = workflowId,
                        Sucursal = sucursal.Trim(),
                        NroOrden = nroOrden.Trim(),
                        Cpe = orden.Cpe,
                        RecorridoId = recorridoId
                    } });
                if (!resultado.HayErrores)
                {
                    orden = repositorio.ObtenerCartaPortePorInstanceId(workflowId);//necesario para actualizar campos luego de dar de alta el ctg/cpe
                    Orden.Set(context, orden);
                }
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