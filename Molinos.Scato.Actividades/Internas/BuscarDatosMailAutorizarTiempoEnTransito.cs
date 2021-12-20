using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Configuration;
using System.Globalization;
using System.Text;

namespace Molinos.Scato.Actividades.Internas
{
    public class BuscarDatosMailAutorizarTiempoEnTransito : CodeActivity
    {
        public OutArgument<string> Body { get; set; }
        public OutArgument<string> Asunto { get; set; }
        public OutArgument<string> Destino { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var servicioRepositorio = context.GetExtension<IServicioRepositorio>();

            try
            {
                var recorrido = servicioRepositorio.ObtenerDatosDeInstanciaPorGuid(context.WorkflowInstanceId);

                var model = servicioRepositorio.ObtenerAutorizacionTiempoEnTransito(context.WorkflowInstanceId, recorrido.DatosProximaActividad, string.Empty);
                var destinatarios = servicioRepositorio.ObtenerUsuariosAutorizarTiempoEnTransito();
                var asunto = $"{string.Format(Textos.ExcesoDeTiempo_Detalle, model.TiempoAceptado, model.TiempoEnTransito)}";
                var tabla = $@"
                    Estimados,<br/>
                    {model.DocumentoIngreso} N°: <b>{model.NroDocumentoIngreso}</b><br/>
                    Patente: <b>{model.Patente}</b><br/>
                    <span style='background-color:yellow'>Material: <b>{model.Material}</b></span><br/>
                    <b>{asunto}</b><br/><br/>

                    
                    Actividad 1: {model.Actividad1} - {model.FechaActividad1}<br/>
                    Actividad 2: {model.Actividad2} - {model.FechaActividad2}<br/>
                    <span style='background-color:yellow'>Tiempo en tránsito: {model.TiempoEnTransito}</span><br/>
                    <span style='background-color:yellow'>Tiempo permitido: {model.TiempoAceptado}</span><br/>
                    Diferencia: {model.DiferenciaTiempo}<br/>
                    
                    <br/>
                ";

                var calidades = servicioRepositorio.ListarAnalisisYCaladoPorCaracteristica(context.WorkflowInstanceId);

                StringBuilder tablaCalidades = new StringBuilder();

                tablaCalidades.Append("<TABLE>\n");
                tablaCalidades.Append("<h3 style=\"text-align: center; background-color: #f9db22; margin-bottom: 0px\">CARACTERISTICAS DE CALIDAD</h3>");
                tablaCalidades.Append("<tr>");
                tablaCalidades.Append("<td style=\"background-color: #f9db22; text-align: center; font-weight: bold;\"></td>");
                tablaCalidades.Append("<td style=\"background-color: #f9db22; text-align: center; font-weight: bold;\">Unidad de Medida</td>");
                tablaCalidades.Append("<td style=\"background-color: #f9db22; text-align: center; font-weight: bold;\">Valor Calado</td>");
                tablaCalidades.Append("<td style=\"background-color: #f9db22; text-align: center; font-weight: bold;\">Valor Analisis</td>");
                tablaCalidades.Append("<td style=\"background-color: #f9db22; text-align: center; font-weight: bold;\">% descuento</td>");
                tablaCalidades.Append("<td style=\"background-color: #f9db22; text-align: center; font-weight: bold;\">Kg. Descuento</td>");
                tablaCalidades.Append("<td style=\"background-color: #f9db22; text-align: center; font-weight: bold;\">Rango</td>");
                tablaCalidades.Append("</tr>\n");

                foreach (var item in calidades)
                {
                    tablaCalidades.Append("<TR>\n");
                    tablaCalidades.Append("<td style=\"background-color: #f7f14a; text-align: center; font-weight: bold;\">" + item.Caracteristica + "</td>");
                    tablaCalidades.Append("<td style=\"background-color: #f7f14a; text-align: center; font-weight: bold;\">" + item.Unidad + "</td>");
                    tablaCalidades.Append("<td style=\"background-color: #f7f14a; text-align: center; font-weight: bold;\">" + item.ValorCalado + "</td>");
                    tablaCalidades.Append("<td style=\"background-color: #f7f14a; text-align: center; font-weight: bold;\">" + item.ValorAnalisis + "</td>");
                    tablaCalidades.Append("<td style=\"background-color: #f7f14a; text-align: center; font-weight: bold;\">" + item.DescuentoEnPorcentaje + "</td>");
                    tablaCalidades.Append("<td style=\"background-color: #f7f14a; text-align: center; font-weight: bold;\">" + item.DescuentoEnKg + "</td>");
                    tablaCalidades.Append("<td style=\"background-color: #f7f14a; text-align: center; font-weight: bold;\">" + item.Rango + "</td>");
                    tablaCalidades.Append("</TR>\n");
                }
                tablaCalidades.Append("</TABLE><br/><br/>");

                var botones =   $@"<table width='100%' cellspacing='0' cellpadding='0'>
                                          <tr>
                                              <td>
                                                  <table cellspacing='0' cellpadding='0'>
                                                      <tr>
                                                          <td style='border-radius: 10px;' bgcolor='#10ba57'>
                                                              <a href='{ConfigurationManager.AppSettings["WebMobile"]}/TiempoEnTransito/Aceptar/{model.WorkflowInstanceId}' target='_blank' style='padding: 16px 24px; border: 1px solid #10ba57;border-radius: 2px;font-family: Helvetica, Arial, sans-serif;font-size: 14px; color: #ffffff;text-decoration: none;font-weight:bold;display: inline-block;'>
                                                                  Aceptar             
                                                              </a>
                                                          </td><td style='width: 80px' td></td>
                                                          <td style='border-radius: 10px;' bgcolor='#ED2939'>
                                                              <a href='{ConfigurationManager.AppSettings["WebMobile"]}/TiempoEnTransito/Rechazar/{model.WorkflowInstanceId}' target='_blank' style='padding: 16px 24px; border: 1px solid #ED2939;border-radius: 2px;font-family: Helvetica, Arial, sans-serif;font-size: 14px; color: #ffffff;text-decoration: none;font-weight:bold;display: inline-block;'>
                                                                  Rechazar             
                                                              </a>
                                                          </td>
                                                      </tr>
                                                  </table>";

                //genero body del mail
                Asunto.Set(context, model.Patente + ": " + asunto);
                Body.Set(context, tabla+ tablaCalidades + botones);
                Destino.Set(context, string.Join(";", destinatarios));
            }
            catch(Exception e)
            {
                Body.Set(context, "Error al generar el mail");
            }            
        }
    }
}
