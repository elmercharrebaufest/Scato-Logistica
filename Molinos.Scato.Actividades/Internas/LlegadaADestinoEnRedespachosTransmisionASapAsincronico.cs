using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class LlegadaADestinoEnRedespachosTransmisionASapAsincronico : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Mov305Request> Request { get; set; }
        [RequiredArgument]
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
                servicioSap.LlegadaADestinoEnRedespachos(instanceId, request.Mov305);
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
