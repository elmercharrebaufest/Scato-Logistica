using System;
using System.Activities;
using System.Configuration;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{
    public class ReintentarEntregaSap : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        public OutArgument<bool> CotCorrecto { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioSap = context.GetExtension<ZSDWS_SCATO>();
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();
            var instanceId = InstanceId.Get<Guid>(context);

            var resultado = new Resultado();
            try
            {
                var recorridoDto = srvRepositorio.ObtenerRecorridoValoresSapPorGuid(instanceId);
                var numeroDocumento = recorridoDto.NumeroDeDocumentoSap;
                var request = new Z_SDMF_RFC_MSJ_ENTREGARequest
                {
                    Z_SDMF_RFC_MSJ_ENTREGA = new Z_SDMF_RFC_MSJ_ENTREGA
                    {
                        IM_XBLNR = numeroDocumento
                    }
                };

                if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                {

                    var srv = context.GetExtension<IServicioComandos>();
                    srv.Ejecutar(new CrearControlRecorrido
                    {
                        Dto = new ControlRecorridoDto
                        {
                            Actividad = "Reintentar Entrega Sap",
                            Fecha = DateTime.Now,
                            Comentario = request.Z_SDMF_RFC_MSJ_ENTREGA.ToXml(),
                            NombreUsuario = "",
                            WorkflowInstanceId = context.WorkflowInstanceId,
                        }
                    });
                }

                var respuesta = servicioSap.Z_SDMF_RFC_MSJ_ENTREGA(request);

                if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                {

                    var srv = context.GetExtension<IServicioComandos>();
                    srv.Ejecutar(new CrearControlRecorrido
                    {
                        Dto = new ControlRecorridoDto
                        {
                            Actividad = "Respuesta de Reintentar Entrega Sap",
                            Fecha = DateTime.Now,
                            Comentario = respuesta.Z_SDMF_RFC_MSJ_ENTREGAResponse.ToXml(),
                            NombreUsuario = "",
                            WorkflowInstanceId = context.WorkflowInstanceId,
                        }
                    });
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
