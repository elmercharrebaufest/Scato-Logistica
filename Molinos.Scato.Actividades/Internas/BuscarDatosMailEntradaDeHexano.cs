using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Molinos.Scato.Actividades.Internas
{
    public class BuscarDatosMailEntradaDeHexano : CodeActivity
    {
        public OutArgument<string> Body { get; set; }
        public OutArgument<string> EnviarA { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
            var servComando = context.GetExtension<IServicioComandos>();

            try
            {
                var recorrido = servicioRepositorio.ObtenerRecorridoPorGuid(context.WorkflowInstanceId);
                var info = servicioRepositorio.ObtenerRemitoPorInstanceId(recorrido.InstanciaWorkflow);
                var chofer = recorrido.Chofer;
                var usuarioRechazo = servicioRepositorio.ObtenerUsuarioRechazoPorWorkflowInstance(context.WorkflowInstanceId);

                var usuarioAviso = servicioRepositorio.ObtenerUsuarioEntregaHexano();

                //genero body del mail
                StringBuilder tablaVehiculo = new StringBuilder();
                tablaVehiculo.Append("<TABLE>\n");
                tablaVehiculo.Append("<h3 style=\"text-align: center; background-color: #006344; margin-bottom: 0px\">DATOS DEL VEHICULO</h3>");
                tablaVehiculo.Append("<tr>\n");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">Patente</td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\"> { info.PatenteCamion } </td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">Tipo Comercial</td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\"> { recorrido.TipoComercial.Descripcion } </td>");
                tablaVehiculo.Append("</tr>\n");
                tablaVehiculo.Append("<tr>\n");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">Patente </ br> Acoplado</ td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\"> { info.PatenteAcoplado } </td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">Carta de Porte</td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\"> { info.Remito } </td>");
                tablaVehiculo.Append("</tr>\n");
                tablaVehiculo.Append("<tr>\n");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">Tarjeta RFD</td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\"> { recorrido.TarjetaDeAcceso } </td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">Fecha Ingreso</td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\"> { recorrido.FechaInicio.ToString("dd/MM/yyyy") } </td>");
                tablaVehiculo.Append("</tr>\n");
                tablaVehiculo.Append("<tr>\n");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">Transportista</td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\"> { recorrido.Transportista.RazonSocial } </td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">Titular CP</td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\"> { info.ProveedorOrigen.RazonSocial } </td>");
                tablaVehiculo.Append("</tr>\n");
                tablaVehiculo.Append("<tr>\n");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">Procedencia</td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\"> { info.Procedencia } </td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">Material</td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\"> { info.Material } </td>");
                tablaVehiculo.Append("</tr>\n");
                tablaVehiculo.Append("<tr>\n");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\">Almacen</td>");
                tablaVehiculo.Append($"<td style=\"background-color: #b9d870; text-align: center; font-weight: bold;\"> { recorrido.Almacen.Descripcion } </td>");
                tablaVehiculo.Append("</tr>\n");
                tablaVehiculo.Append("</TABLE>");

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
                
                Body.Set(context, string.Format(Textos.MailAvisoEntradaDeHexano, tablaVehiculo,
                                            tablaChofer));
                EnviarA.Set(context, string.Join(";",usuarioAviso));
            }
            catch(Exception e)
            {
                Body.Set(context, "Error al generar el mail");
                servComando.Ejecutar(new CrearControlRecorrido()
                {
                    Dto = new ControlRecorridoDto()
                    {
                        Actividad = "EnviarMailHexano",
                        Comentario = "Error:" + e.StackTrace,
                        WorkflowInstanceId = context.WorkflowInstanceId
                    }
                });
            }
        }
    }
}