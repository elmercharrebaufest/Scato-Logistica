using System;
using System.Activities;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class PesaNetoTransmisionASap : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<PesaNetoRequest> Request { get; set; }
        public OutArgument<bool> FuncionaServicio { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioSap = context.GetExtension<ZSDWS_SCATO>();
            var request = Request.Get(context);
            var resultado = new Resultado();
            try
            {
                var respuesta = servicioSap.PesaNeto(request);
                FuncionaServicio.Set(context, respuesta.PesaNetoResponse.Mensajes.All(a => a.MSGNR == "000"));
                if (FuncionaServicio.Get(context))
                {
                    var servicioComandos = context.GetExtension<IServicioComandos>();
                    servicioComandos.Ejecutar(new ModificarVehiculoDocumentoInterno
                    {
                        InstanceId = context.WorkflowInstanceId,
                        DocumentoInternoSap = respuesta.PesaNetoResponse.Mensajes.First().MBLNR,
                        NumeroDeDocumentoSap = respuesta.PesaNetoResponse.Mensajes.First().XBLNR
                    });
                }
                else
                {
                    resultado.Errores.Add("WorkflowId", respuesta.PesaNetoResponse.Mensajes.Any() ? respuesta.PesaNetoResponse.Mensajes.First().TEXT : "Respuesta Vacía");
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
