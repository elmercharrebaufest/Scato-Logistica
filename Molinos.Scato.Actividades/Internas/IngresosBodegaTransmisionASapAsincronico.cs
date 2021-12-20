using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresosBodegaTransmisionASapAsincronico : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<IngresosBodegaAsincronicoDto> Request { get; set; }
        public InArgument<Guid> InstanceId { get; set; }
        public OutArgument<bool> FuncionaServicio { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {

            var servicioSap = context.GetExtension<IServicioSapAsincronico>();
            var request = Request.Get(context);
            var instanceId = InstanceId.Get(context);
            var resultado = new Resultado();

            try
            {
                servicioSap.IngresosBodega(instanceId, request);
                FuncionaServicio.Set(context, true);
            }
            catch (Exception e)
            {
                resultado.Errores.Add("WorkflowId", Textos.MovimientoStockSap_ErrorEnLaCarga + ": " + e.Message);
            }
            return resultado;
        }
    }
}

