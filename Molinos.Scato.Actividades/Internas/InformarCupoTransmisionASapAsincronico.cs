using System;
using System.Activities;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class InformarCupoTransmisionASapAsincronico:CodeActivity<Resultado>
    {
        [Required]
        public InArgument<Z_SDMF_Z2200NRequest> Request { get; set; }
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
                servicioSap.InformarCupo(instanceId, request.Z_SDMF_Z2200N);
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
