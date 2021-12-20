using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class EgresoSinFleteFasonesTransmisionASap : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<EgresoSinFleteFazonesRequest> Request { get; set; }
        public OutArgument<bool> FuncionaServicio { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioSap = context.GetExtension<ZSDWS_SCATO>();

            var request = Request.Get(context);

            var resultado = new Resultado();

            try
            {
                var respuesta = servicioSap.EgresoSinFleteFazones(request);
                FuncionaServicio.Set(context, !String.IsNullOrEmpty(respuesta.EgresoSinFleteFazonesResponse.Resultado.MBLNR));

                if (FuncionaServicio.Get(context))
                {
                    var servicioComandos = context.GetExtension<IServicioComandos>();
                    servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                    {
                        InstanceId = context.WorkflowInstanceId,
                        DocumentoInternoSap = respuesta.EgresoSinFleteFazonesResponse.Resultado.MBLNR,
                        NumeroDeDocumentoSap = respuesta.EgresoSinFleteFazonesResponse.Resultado.XBLNR,
                    });
                }
                else
                {
                    resultado.Errores.Add("WorkflowId", respuesta.EgresoSinFleteFazonesResponse.Resultado.TEXT);
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("WorkflowId", Textos.MovimientoStockSap_ErrorEnLaCarga + ": " + e.Message);
            }
            return resultado;
        }
    }
}
