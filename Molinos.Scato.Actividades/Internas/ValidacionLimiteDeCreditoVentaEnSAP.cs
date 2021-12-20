using System;
using System.Activities;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Actividades.Internas
{

    public sealed class ValidacionLimiteDeCreditoVentaEnSAP : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<string> Cuit { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        public OutArgument<bool> CreditoOk { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {

            var resultado = new Resultado();

            try
            {
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var validarLimiteDeCreditoVentaEnSAP = srvRepositorio.ValidarLimiteDeCreditoVentaEnSAP(CentroId.Get(context));
                var servicioComandos = context.GetExtension<IServicioComandos>();

                if (validarLimiteDeCreditoVentaEnSAP)
                {

                    var respuesta = servicioComandos.Ejecutar(
                        new ValidarStock
                        {
                            InstanceId= context.WorkflowInstanceId,
                        });
                    
                    try
                    {
                        if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                        {
                            var srv = context.GetExtension<IServicioComandos>();
                            srv.Ejecutar(new CrearControlRecorrido
                            {
                                Dto = new ControlRecorridoDto
                                {
                                    Actividad = "ValidacionLimiteDeCreditoVentaEnSAP",
                                    Fecha = DateTime.Now,
                                    Comentario = respuesta.HayErrores ? respuesta.Errores.First().Value : "ok",
                                    NombreUsuario = "",
                                    WorkflowInstanceId = context.WorkflowInstanceId,
                                }
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        resultado.Errores.Add("ControlRecorrido", e.Message);
                    }
                    CreditoOk.Set(context, !respuesta.HayErrores);
                    if (!CreditoOk.Get(context))
                    {
                        resultado.Errores.Add("WorkflowId", Textos.SinCreditoVentaSAP);
                    }


                }
                else
                {
                    CreditoOk.Set(context, true);
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
