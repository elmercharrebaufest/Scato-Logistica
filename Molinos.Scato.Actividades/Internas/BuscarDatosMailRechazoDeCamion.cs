using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Globalization;
using System.Text;

namespace Molinos.Scato.Actividades.Internas
{
    public class BuscarDatosMailRechazoDeCamion : CodeActivity
    {
        public OutArgument<string> Body { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            var servicioRepositorio = context.GetExtension<IServicioRepositorio>();

            try
            {
                var recorrido = servicioRepositorio.ObtenerRecorridoPorGuid(context.WorkflowInstanceId);
                var info = servicioRepositorio.ObtenerInformacionCartaPorte(recorrido.Id);
                var chofer = recorrido.Chofer;
                var usuarioRechazo = servicioRepositorio.ObtenerUsuarioRechazoPorWorkflowInstance(context.WorkflowInstanceId);
                var controlRecorridoRechazo = servicioRepositorio.ObtenerControlRecorrido(context.WorkflowInstanceId, Textos.Actividad_VerificacionCamionRechazado);
                var motivoRechazo = controlRecorridoRechazo?.Mensaje ?? string.Empty;
                var observaciones = controlRecorridoRechazo?.Comentario ?? string.Empty;

                //genero tabla de caracteristicas
                StringBuilder tablaCalidades = new StringBuilder();
                if (recorrido.Calado != null)
                {
                    var calidades = servicioRepositorio.ListarAnalisisYCaladoPorCaracteristica(context.WorkflowInstanceId);

                    
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
                    tablaCalidades.Append("</TABLE>");
                }

                //genero tabla de chofer

                StringBuilder tablaChofer = new StringBuilder();
                tablaChofer.Append("<TABLE>\n");
                tablaChofer.Append("<h3 style=\"text-align: center; background-color: #006344; margin-bottom: 0px\">DATOS DEL CHOFER</h3>");
                tablaChofer.Append("<tr>\n");
                tablaChofer.Append("<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">Apellido</td>");
                tablaChofer.Append("<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">Nombre</td>");
                tablaChofer.Append("<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">Numero de Documento</td>");
                tablaChofer.Append("<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">CUIL</td>");
                tablaChofer.Append("</tr>\n");
                tablaChofer.Append("<TR>\n");
                tablaChofer.Append("<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">" + chofer.Apellido + "</td>");
                tablaChofer.Append("<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">" + chofer.Nombre + "</td>");
                tablaChofer.Append("<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">" + chofer.NumeroDeDocumento + "</td>");
                tablaChofer.Append("<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">" + chofer.Cuil + "</td>");
                tablaChofer.Append("</TR>\n");
                tablaChofer.Append("</TABLE>");

                tablaChofer.Append("<TABLE>\n");
                tablaChofer.Append("<h3 style=\"text-align: center; background-color: #FFA500; margin-bottom: 0px\">Datos Rechazo</h3>");
                tablaChofer.Append("<tr>\n");
                tablaChofer.Append("<td style=\"background-color: #FFA500; text-align: center; font-weight: bold;\">Nombre</td>");
                tablaChofer.Append("<td style=\"background-color: #FFA500; text-align: center; font-weight: bold;\">Usuario</td>");
                tablaChofer.Append("</tr>\n");
                tablaChofer.Append("<TR>\n");
                tablaChofer.Append("<td style=\"background-color: #FFA500; text-align: center; font-weight: bold;\">" + usuarioRechazo[0] + "</td>");
                tablaChofer.Append("<td style=\"background-color: #FFA500; text-align: center; font-weight: bold;\">" + usuarioRechazo[1] + "</td>");
                tablaChofer.Append("</TR>\n");
                tablaChofer.Append("</TABLE>");

                //genero body del mail

                Body.Set(context, string.Format(Textos.RechazadosMailMesaEntrada,
                                            recorrido.Patente,
                                            recorrido.TipoComercial.Descripcion,
                                            recorrido.Vehiculo != null ? recorrido.Vehiculo.PatenteAcoplado : string.Empty,
                                            recorrido.NumeroDocumentoIngreso,
                                            recorrido.TarjetaDeAcceso,
                                            !string.IsNullOrEmpty(info.CTG) ? info.CTG : Textos.No,
                                            string.Format(CultureInfo.CurrentCulture, "{0:d/M/yyyy}", recorrido.FechaInicio),
                                            info.TitularCartaPorte,
                                            recorrido.Transportista.RazonSocial,
                                            info.RtteComercial,
                                            recorrido.PesoNetoOrigen,
                                            !string.IsNullOrEmpty(info.AgenteCompras) ? info.AgenteCompras : Textos.No,
                                            recorrido.PesoNeto,
                                            info.Entregador,
                                            recorrido.PesoDiferencia,
                                            recorrido.Calado != null ? recorrido.Calado.CicloDeCalado : 0,
                                            info.Cupo,
                                            recorrido.Calado != null ? recorrido.Calado.Comentario : "",
                                            info.Procedencia,
                                            recorrido.Material.Descripcion,
                                            tablaCalidades.ToString(),
                                            tablaChofer.ToString(),
                                            motivoRechazo,
                                            observaciones));
            }
            catch(Exception e)
            {
                Body.Set(context, "Error al generar el mail");
            }            
        }
    }
}
