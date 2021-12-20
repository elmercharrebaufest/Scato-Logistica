using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;

namespace Molinos.Scato.Actividades.Internas
{
    public class RegistrarMuestreoYPesajeTransmisionAMonsantoAsincronico : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<registerSampleAndWeightRequest> Request { get; set; }
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        [RequiredArgument]
        public InArgument<TipoVehiculo> TipoVehiculo { get; set; }

        public OutArgument<bool> FuncionaServicio { get; set; }
        protected override Resultado Execute(CodeActivityContext context)
        {

            var servicioSap = context.GetExtension<IServicioSapAsincronico>();
            var tipoVehiculo = TipoVehiculo.Get<TipoVehiculo>(context);
            var request = Request.Get(context);
            var instanceId = InstanceId.Get(context);
            var resultado = new Resultado();

            try
            {
                if (tipoVehiculo != Dominio.Enums.TipoVehiculo.Tren)
                {
                    servicioSap.RegistrarMuestreoYPesajeTransporteAutomotor(instanceId, (MuestreoPesajeTransporteAutomotor)request.Item);
                }
                else
                {
                    servicioSap.RegistrarMuestreoYPesajeVagonFerroviario(instanceId, (MuestreoPesajeVagonFerroviario)request.Item);
                }
                
                FuncionaServicio.Set(context, true);
            }
            catch (Exception e)
            {
                resultado.Errores.Add("WorkflowId", Textos.Monsanto_ErrorEnLaCarga + ": " + e.Message);
            }
            return resultado;
        }
    }
}

