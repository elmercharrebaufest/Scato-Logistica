using System;
using System.Activities;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;

namespace Molinos.Scato.Actividades.Internas
{
    public class RegistrarMuestreoYPesajeTransmisionAMonsanto : CodeActivity<Resultado>
    {
        public InOutArgument<int> Intentos { get; set; }
        [RequiredArgument]
        public InArgument<registerSampleAndWeightRequest> Request { get; set; }
        public OutArgument<bool> FuncionaServicio { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var request = Request.Get(context);
            var resultado = new Resultado();

            try
            {
                var servicio = context.GetExtension<WaybillManagementPODv2>();
                var respuesta = servicio.registrarMuestreoYPesaje(new registrarMuestreoYPesaje(request));
                FuncionaServicio.Set(context, !String.IsNullOrEmpty(respuesta.@return) && respuesta.@return.ToUpper() == "OK");
            }
            catch (System.ServiceModel.FaultException<webServiceExceptionV2FaultDetailsBean> e)
            {
                resultado.Errores.Add("WorkflowId", Textos.Monsanto_ErrorEnLaCarga + ": " + (e.Detail.errores != null && e.Detail.errores.Any() ? e.Detail.errores.First().descripcion + "(" + e.Detail.errores.First().codigo + ")" : e.Message));
            }
            catch (Exception e)
            {
                resultado.Errores.Add("WorkflowId2", Textos.Monsanto_ErrorEnLaCarga + ": " + e.Message);
            }
            var intentos = Intentos.Get<int>(context);
            Intentos.Set(context, ++intentos);
            return resultado;
        }
    }
}
