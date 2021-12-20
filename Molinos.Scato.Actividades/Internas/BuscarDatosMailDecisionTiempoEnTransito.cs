using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Configuration;
using System.Globalization;
using System.Text;

namespace Molinos.Scato.Actividades.Internas
{
    public class BuscarDatosMailDecisionTiempoEnTransito : CodeActivity
    {
        public InArgument<bool> Autorizado { get; set; }
        public InArgument<string> Usuario { get; set; }
        public OutArgument<string> Body { get; set; }
        public OutArgument<string> Asunto { get; set; }
        public OutArgument<string> Destino { get; set; }
        protected override void Execute(CodeActivityContext context)
        {

            var autorizado = Autorizado.Get<bool>(context);
            var usuario = Usuario.Get<string>(context);

            var servicioRepositorio = context.GetExtension<IServicioRepositorio>();

            try
            {
                var recorrido = servicioRepositorio.ObtenerDatosDeInstanciaPorGuid(context.WorkflowInstanceId);
                var motivoRechazo = servicioRepositorio.ObtenerEstadoRecorrido(context.WorkflowInstanceId) ?? 
                    servicioRepositorio.ObtenerMotivoAutorizarRecorrido(context.WorkflowInstanceId) ?? new Dominio.Dto.UltimoEstadoDto();

                var model = servicioRepositorio.ObtenerAutorizacionTiempoEnTransito(context.WorkflowInstanceId, recorrido.DatosProximaActividad, string.Empty);
                var destinatarios = servicioRepositorio.ObtenerUsuariosAutorizarTiempoEnTransito();
                destinatarios.AddRange(servicioRepositorio.ObtenerUsuariosAutorizarTiempoEnTransitoConfirmado());
                if (!autorizado)
                {
                    destinatarios.AddRange(servicioRepositorio.ObtenerUsuariosAutorizarTiempoEnTransitoRechazado());
                }
                var decisionTexto = autorizado ? Textos.Aprobado : Textos.Rechazado;
                var asunto = $"{string.Format(Textos.ExcesoDeTiempo_Estado, decisionTexto, usuario, DateTime.Now.ToString("HH:mm"), model.Patente, motivoRechazo.Descripcion)}";
                var tabla = $@"
                    Estimados,<br/>
                    {model.DocumentoIngreso} N°: <b>{model.NroDocumentoIngreso}</b><br/>
                    Patente: <b>{model.Patente}</b><br/>
                    Estado: <b>{decisionTexto}</b><br/>
                    <span style='background-color:yellow'>Material: <b>{model.Material}</b></span><br/>
                    <b>{asunto}</b><br/><br/>

                    
                    Actividad 1: {model.Actividad1} - {model.FechaActividad1}<br/>
                    Actividad 2: {model.Actividad2} - {model.FechaActividad2}<br/>
                    <span style='background-color:yellow'>Tiempo en tránsito: {model.TiempoEnTransito}</span><br/>
                    <span style='background-color:yellow'>Tiempo permitido: {model.TiempoAceptado}</span><br/>
                    Diferencia: {model.DiferenciaTiempo}<br/>
                    
                    <br/>
                    <span style='background-color:yellow'>Comentario: {motivoRechazo.Descripcion}</span><br/>
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


                tablaCalidades.Append("<TABLE>\n");
                tablaCalidades.Append("<TR>\n");
                tablaCalidades.Append("<td style=\"background-color: #fffa75; text-align: center; font-weight: bold;\">Estado:</td>");
                tablaCalidades.Append("<td style=\"background-color: #fffa75; text-align: center; font-weight: bold;\">" + decisionTexto + "</td>");
                tablaCalidades.Append("</TR>\n");
                tablaCalidades.Append("<TR>\n");
                tablaCalidades.Append("<td style=\"background-color: #fffa75; text-align: center; font-weight: bold;\">Usuario:</td>");
                tablaCalidades.Append("<td style=\"background-color: #fffa75; text-align: center; font-weight: bold;\">" + usuario + "</td>");
                tablaCalidades.Append("</TR>\n");
                tablaCalidades.Append("<TR>\n");
                tablaCalidades.Append("<td style=\"background-color: #fffa75; text-align: center; font-weight: bold;\">Horario:</td>");
                tablaCalidades.Append("<td style=\"background-color: #fffa75; text-align: center; font-weight: bold;\">" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "</td>");
                tablaCalidades.Append("</TR>\n");
                tablaCalidades.Append("</TABLE><br/><br/>");

                //genero body del mail
                Asunto.Set(context, model.Patente + ": " + asunto);
                Body.Set(context, tabla + tablaCalidades);
                Destino.Set(context, string.Join(";", destinatarios));
            }
            catch (Exception e)
            {
                Body.Set(context, "Error al generar el mail");
            }
        }
    }
}
