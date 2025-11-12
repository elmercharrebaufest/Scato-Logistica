using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Web.Seguridad;

namespace Molinos.Scato.Web.Helpers
{
    public static class ExtensionesWebGrid
    {
        public static IHtmlString Grilla<TEntidad>(
            this HtmlHelper helper,
            ListaPaginada<TEntidad> items,
            Func<WebGrid, WebGridColumn[]> columnas)
        {
            var grid = new WebGrid(rowsPerPage: items.ItemsPorPagina,
                 sortDirectionFieldName: "dirOrden",
                 pageFieldName: "pagina",
                 sortFieldName: "ordenarPor");
            grid.Bind((IEnumerable<dynamic>)items.Items, autoSortAndPage: false, rowCount: items.ItemsTotales);

            return grid.GetHtml(
                tableStyle: "table table-striped table-bordered",
                columns: columnas(grid),
                htmlAttributes: new { id = "grid" });
        }

        public static WebGridColumn Columna(this WebGrid grid, string columnName, string header,
                                           Func<dynamic, object> format = null, string style = null, bool canSort = true)
        {
            if (canSort)
            {
                header += grid.SortColumn == columnName ? grid.SortDirection == SortDirection.Ascending ? "  ▲" : "  ▼" : " ▲▼";
            }
            return grid.Column(columnName, header, format, style, canSort);
        }

        public static WebGridColumn ColumnaEliminarModificar(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {
            return grid.Column("EliminarModificar", "", f =>
                    html.Raw(
                    "<span>" +
                    html.BotonLink(Textos.Modificar, "Modificar", controller, new { f.id }, style + " ajax-editar-link", "icon-edit", true).ToHtmlString() +
                    html.BotonLink(Textos.Eliminar, "Eliminar", controller, new { f.id }, style + " ajax-borrar-link", "icon-trash", true).ToHtmlString() +
                    "</span>"
                    )
                    , "editar-borrar-columna", false);
            
        }

        public static WebGridColumn ColumnaEliminarModificarCalles(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {

            return grid.Column("EliminarModificar", "", f =>

                html.Raw((f.EsNoEditableGrilla) ?
                    "<span class=\"label label-important\" data-toggle=\"tooltip\" title=\"" + Textos.No_EditableLargo + "\" >" + Textos.No_Editable + "</span>" :
                    "<span>" +
                    html.BotonLink(Textos.Modificar, "Modificar", controller, new { f.id }, style + " ajax-editar-link", "icon-edit", true).ToHtmlString() +
                    html.BotonLink(Textos.Eliminar, "Eliminar", controller, new { f.id }, style + " ajax-borrar-link", "icon-trash", true).ToHtmlString() +
                    "</span>"
                )
                , "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaModificarListar(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {

            return grid.Column("ModificarListar", "", f =>
                html.Raw(
                "<span>" +
                html.BotonLink(Textos.Modificar, "Modificar", controller, new { f.id }, style + " ajax-editar-link", "icon-edit", true).ToHtmlString() +
                  html.BotonLink(Textos.Listar, "Listar", controller, new { f.id }, style + " ajax-ver-link", "icon-list", true).ToHtmlString() +              
                "</span>"
                )
                , "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaEliminarCopiarModificar(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {

            return grid.Column("EliminarModificar", "", f =>
                html.Raw(
                "<span>" +
                html.BotonLink(Textos.Modificar, "Modificar", controller, new { f.id }, style + " ajax-editar-link", "icon-edit", true).ToHtmlString() +
                html.BotonLink(Textos.Eliminar, "Eliminar", controller, new { f.id }, style + " ajax-borrar-link", "icon-trash", true).ToHtmlString() +
                html.BotonLink(Textos.Copiar, "Crear", controller, new { f.id }, style + " ajax-editar-link", "icon-list-alt", true).ToHtmlString() +
                "</span>"
                )
                , "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaEliminarImprimir(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {
            return grid.Column("EliminarImprimir", "", f =>
                html.Raw(
                "<span>" +
                html.BotonLink(Textos.Previsualizar, "Previsualizar", controller, new { f.id, f.ctg, f.ctgdg }, style + " ajax-previsualizar-link", "icon-search", true).ToHtmlString() +
                html.BotonLink(Textos.Imprimir, "Imprimir", controller, new { f.id, f.ctg, f.ctgdg }, style + " ajax-imprimir-link", "icon-print", true).ToHtmlString() +
                html.BotonLink(Textos.Eliminar, "Eliminar", controller, new { f.id, f.ctg, f.ctgdg }, style + " ajax-borrar-link", "icon-trash", true).ToHtmlString() +
                "</span>"
                )
                , "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaEliminarImprimirModificar(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {

            return grid.Column("EliminarImprimir", "", f =>
                html.Raw(
                "<span>" +
                html.BotonLink(Textos.Imprimir, "Imprimir", controller, new { f.id }, style + " ajax-imprimir-link", "icon-print", true).ToHtmlString() +
                html.BotonLink(Textos.Modificar, "Modificar", controller, new { f.id }, style + " ajax-editar-link", "icon-edit", true).ToHtmlString() +
                html.BotonLink(Textos.Eliminar, "Eliminar", controller, new { f.id }, style + " ajax-borrar-link", "icon-trash", true).ToHtmlString() +
                "</span>"
                )
                , "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaVerDocumentoOrigen(this WebGrid grid, HtmlHelper html, string style = "")
        {
            return grid.Column("verDocumentoOrigen", "", f => html.Raw(html.BotonLink("", "Seleccionar", "ModificarDocumentoDeIngreso", new { Id = (int)f.RecorridoId }, style, "icon-plus", true, true).ToHtmlString()), "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaVerFotos(this WebGrid grid, HtmlHelper html, string style = "")
        {
            var pendiente = PermisosScato.CamionesPendientesMesa.DisplayText();
            return grid.Column("verFotos", "", f => html.Raw(html.BotonLink("", ((string)f.ProximaAccion) == pendiente ? "IndexPorCargaDeCupo" : "Index", "Foto", new { Id = ((string)f.ProximaAccion) == pendiente ? f.RecorridoId : f.id }, style + " ajax-popup-link", "icon-picture", true).ToHtmlString()), "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaVerFotoDeQuiebre(this WebGrid grid, HtmlHelper html, string style = "")
        {
            return grid.Column("verFotos", "", f => html.Raw(html.BotonLink("", "ObtenerPorQuiebre", "Foto", new { f.id }, style + " ajax-popup-link", "icon-picture", true).ToHtmlString()), "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaEjecutar(this WebGrid grid, HtmlHelper html, string style = "")
        {
            var pendiente = PermisosScato.CamionesPendientesMesa.DisplayText();
            return 
                grid.Column(
                    "ejecutar", 
                    "", 
                    f => html.Raw(
                            html.BotonLink(
                                Textos.Ejecutar, 
                                ((string)f.ProximaAccion).Contains(pendiente) ? "EjecutarPendiente" : "Ejecutar", 
                                "ListaDeCamiones", 
                                new 
                                { 
                                    Id = ((string)f.ProximaAccion).Contains(pendiente)? f.RecorridoId : f.id, proximaAccion = (string)f.ProximaAccion, 
                                    fletemoa = (string)f.FleteMOA, 
                                    tipoOrdenCargaNoGranos = f.TipoOrdenCargaNoGranos 
                                }, 
                                style + (f.Reingreso ? " rowReingresado" : "") + (f.Rechazado ? " rowRechazado " : "") + (f.LlegoEnHorario ? " rowPrioridadCircular " : ""), 
                                "icon-play", 
                                true)
                            .ToHtmlString()), 
                    "editar-borrar-columna", 
                    false);
        }

        public static WebGridColumn ColumnaModificar(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {
            return grid.Column("editar", "", f => html.Raw(html.BotonLink(Textos.Modificar, "Modificar", controller, new { f.id }, style + " ajax-editar-link", "icon-edit", true).ToHtmlString()),
                                                        "editar-borrar-columna", false);
        }


        public static WebGridColumn ColumnaModificar(this WebGrid grid, HtmlHelper html, string controller, Func<dynamic, bool> condition, string style = "")
        {
            return grid.Column("editar", "", f =>
            {
                // Verificar si la condición es nula o si se cumple
                if (condition(f))
                {
                    return html.Raw(html.BotonLink(Textos.Modificar, "Modificar", controller, new { f.id }, style + " ajax-editar-link", "icon-edit", true).ToHtmlString());
                }
                return null; // No renderizar nada si la condición no se cumple
            },
            "editar-borrar-columna", false);
        }


        //public static WebGridColumn ColumnaModificar(this WebGrid grid, HtmlHelper html, string controller, Func<dynamic, object> format = null, string style = "")
        //{
        //    Func<dynamic, object> formatdefault = (f => true ? html.Raw(html.BotonLink(Textos.Modificar, "Modificar", controller, new { f.id }, (style + " ajax-editar-link"), "icon-edit", true).ToHtmlString()) : null);

        //    return grid.Column("editar", "", formatdefault,
        //                                                "editar-borrar-columna", false);
        //}

        public static WebGridColumn ColumnaModificarCarga(this WebGrid grid, HtmlHelper html, string action, string controller, string style = "")
        {
            return grid.Column("editar", "", f => html.Raw(html.BotonLink(Textos.Modificar, action, controller, new { f.id, f.numeroBalanza }, style, "icon-edit", true).ToHtmlString()),
                                                        "editar-borrar-columna", false);
        }
        public static WebGridColumn ColumnaModificarCarga(this WebGrid grid, HtmlHelper html, string action, string controller, string style = "", bool activo = true)
        {
            return grid.Column("editar", "", f => html.Raw((!f.EnviadoASap) ? html.BotonLink(Textos.Modificar, action, controller, new { f.id, f.numeroBalanza }, style, "icon-edit", true).ToHtmlString() : null),
                                                        "editar-borrar-columna", false);
        }
        public static WebGridColumn ColumnaModificarCargaInicioFin(this WebGrid grid, HtmlHelper html, string action, string controller, string style = "")
        {
            return grid.Column("editar", "", f => html.Raw(html.BotonLink(Textos.Modificar, action, controller, new { f.id, f.numeroBalanza, f.idFin }, style, "icon-edit", true).ToHtmlString()),
                                                        "editar-borrar-columna", false);
        }
        public static WebGridColumn ColumnaAsignarTicketMunicipal(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {
            return grid.Column("editar", "Imprime", f => html.Raw(
                                            ExtensionesHtml.CheckBoxLink(html, "Imprime", f.PagaTicketMunicipal, "Modificar", controller, new { f.Id, PagaTicketMunicipal = !f.PagaTicketMunicipal }, style + " ajax-editar-link " + (f.Modificado ? "modificado" : "")).ToHtmlString()
                                        ),
                                                        "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaEliminarWorkflow(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {
            return grid.Column("Eliminar", "", f => html.Raw(html.BotonLink(Textos.Eliminar, "EliminarWorkflow", controller, new { f.id }, style + " ajax-borrar-link", "icon-trash", true).ToHtmlString()),
                                                        "editar-borrar-columna", false);
        }

        public static WebGridColumn VerLogsWorkflow(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {
            return grid.Column("VerLogs", "",
                               f =>
                               html.Raw(
                                   html.BotonLink(Textos.VerLogs, "ListarSeguimientoWorkflow", controller, new { f.id },
                                                  style + " ajax-ver-link", "icon-list", true).ToHtmlString()),
                               "editar-borrar-columna", false);
        }

        public static WebGridColumn VerColaImpresion(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {
            return grid.Column("VerCola", "",
                               f =>
                               html.Raw(
                                   html.BotonLink(Textos.Impresora_VerCola, "VerCola", controller, new { f.id },
                                                  style + " ajax-ver-link", "icon-list", true).ToHtmlString()),
                               "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaResumirWorkflow(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {
            return grid.Column("editar", "", f => html.Raw(ExtensionesHtml.BotonLinkDesactivable(html, Textos.Resumir, "ResumirWorkflow", controller, new { f.id }, style, "icon-plus-sign", true, !f.EstaSuspendido).ToHtmlString()),
                                                        "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaSeleccionar(this WebGrid grid, HtmlHelper html, string controller, string style = "", bool useAjax = true)
        {
            return grid.Column("Seleccionar", "Ver", f => html.Raw(html.BotonLink(Textos.Seleccionar, "Seleccionar", controller, new { f.id }, style + ((useAjax) ? " ajax-editar-link" : string.Empty), "icon-play", true).ToHtmlString()),
                                                        "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaSeleccionarDocumento(this WebGrid grid, HtmlHelper html, string controller, string style = "", bool useAjax = true, bool filtrarPorTarjeta = false, bool soloLectura = false)
        {
            return grid.Column("Seleccionar", "Ver", f => html.Raw(

                ((soloLectura || (!soloLectura && f.EsModificable)) ?
                html.BotonLink(Textos.Seleccionar, "Seleccionar", controller, new { f.id, filtrarPorTarjeta, soloLectura }, style + ((useAjax) ? " ajax-editar-link" : string.Empty), "icon-play", true).ToHtmlString()
                : "<span class=\"label label-important\" data-toggle=\"tooltip\" title=\"" + Textos.Error_NoModificacion + "\" >" + Textos.Error_NoModificable + "</span>"
                )
                ),
                                                        "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaConsultaVehiculo(this WebGrid grid, HtmlHelper html, string controller)
        {
            var pendiente = PermisosScato.CamionesPendientesMesa.DisplayText();
            return grid.Column("Patente", "Patente", f => ((string)f.ProximaAccion).Contains(pendiente) ? html.Raw((String)f.Patente) : html.Raw(html.BotonLink((String)f.Patente, "Seleccionar", controller, new { Id = (int)f.RecorridoId, actualizarCookie = true }, "ajax-popup-link2", "", false, false).ToHtmlString()),
                                                        "", true);
        }

        public static WebGridColumn ColumnaEliminarDocumento(this WebGrid grid, HtmlHelper html, string controller, string style = "", bool pideMotivo = false)
        {
            return grid.Column("Eliminar", "", f => html.Raw(
                f.EsEliminable && f.Terminado && PermisosHelper.Is(PermisosScato.BorrarDocumentosTerminados) ?
                html.BotonLink(Textos.Eliminar, "EliminarTerminado", controller, new { f.id }, style + (pideMotivo ? "ajax-borrar-con-motivo-link" : " ajax-borrar-link"), "icon-trash", true).ToHtmlString()
                : f.EsEliminable && !f.Terminado && PermisosHelper.Is(PermisosScato.BorrarDocumentosNoTerminados) ?
                html.BotonLink(Textos.Eliminar, "EliminarNoTerminado", controller, new { f.id }, style + (pideMotivo ? "ajax-borrar-con-motivo-link" : " ajax-borrar-link"), "icon-trash", true).ToHtmlString()
                : !f.EsEliminable ? "<span class=\"label label-important\" data-toggle=\"tooltip\" title=\"" + Textos.Error_NoEliminar + "\" >" + Textos.Error_NoEliminable + "</span>"
                : "<span class=\"label label-important\" data-toggle=\"tooltip\" title=\"" + Textos.Permiso_Insuficiente + "\" >" + Textos.Error_NoEliminable + "</span>"
                ),
                                                        "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaEliminar(this WebGrid grid, HtmlHelper html, string controller, string style = "", bool pideMotivo = false)
        {
            return grid.Column("Eliminar", "", f => html.Raw(html.BotonLink(Textos.Eliminar, "Eliminar", controller, new { f.id }, style + (pideMotivo ? "ajax-borrar-con-motivo-link" : " ajax-borrar-link"), "icon-trash", true).ToHtmlString()),
                                                        "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaRadioBoton(this WebGrid grid, HtmlHelper html, string label, string name, bool required = false, string errorMessage = null)
        {
            return grid.Column("Seleccionar", "", f => html.Raw(html.RadioBoton(label, name, ((int)f.CentroId).ToString(CultureInfo.InvariantCulture), required, errorMessage).ToHtmlString()), null, false);
        }

        public static WebGridColumn ColumnaCheckBox(this WebGrid grid, HtmlHelper html, string header, string name, bool isChecked = false)
        {
            return grid.Column("Seleccionar", header, f => html.Raw(html.CheckBox(name, isChecked, new { value = f.Id, @class = "columna-checkbox" }).ToHtmlString()), null, false);
        }

        public static WebGridColumn ColumnaCheckBoxConMaterial(this WebGrid grid, HtmlHelper html, string header, string name, bool isChecked = false)
        {
            return grid.Column("Seleccionar", header, f =>
            html.Raw(html.CheckBox(name, isChecked, new
            {
                f.Id,
                data_materialId = f.MaterialId,
                data_EsSustentable = f.EsSustentable,
                data_SojaEPA = f.SojaEPA,
                data_SojaIMPO = f.SojaIMPO,
                data_SojaEUDR = f.SojaEUDR,
                data_SojaEPAyEUDR = f.SojaEPAyEUDR,
                @class = "columna-checkbox " +
            (f.Rechazado ? "estado-rechazado " : "") +
            (f.FueAsignado ? "estado-asignado " : f.TieneDescuentos ? "estado-descuento " : "") +
                (f.EsGranosVerdes ? "estado-grano-verde " : "") +
                (f.EsCuerposExtranos ? "estado-cuerpo-extraño " : "") +
                (f.EsGranosDañados ? "estado-dañado " : "") +
                (f.EsHumedad ? "estado-humedo" : "")
            }).ToHtmlString()), null, false);
        }

        public static WebGridColumn ColumnaCerear(this WebGrid grid, HtmlHelper html, string texto)
        {

            return grid.Column("Cerear", "Cerear",
                               f => html.Raw(html.BotonId(texto, (int)f.Id, "btn botonCerear").ToHtmlString()),
                               "cereo-columna", false);
        }

        public static WebGridColumn ColumnaColor(this WebGrid grid, string columnName, string header,
                                           HtmlHelper html, string style = null, bool canSort = true)
        {

            header += grid.SortColumn == columnName ? grid.SortDirection == SortDirection.Ascending ? "  ▲" : "  ▼" : " ▲▼";
            return grid.Column(columnName, header, f => html.Raw(html.IconoColor((string)f.Color)), style, canSort);
        }

        public static WebGridColumn ColumnaVerFotoCartaPorte(this WebGrid grid, HtmlHelper html, string action, string action2, string controller, string style = "")
        {
            return grid.Column("verFotos"
                                , "[FotoHeader]"
                                , f => f.tieneFotoIngreso ?
                                    html.Raw(
                                        html.BotonLink(Textos.Previsualizar, action, controller, new { id = (int)f.id, f.numeroDocumentoIngreso }, style + " ajax-popup-link", "icon-camera", true).ToHtmlString() +
                                        html.BotonLink(Textos.Previsualizar, action2, controller, new { id = (int)f.id, f.numeroDocumentoIngreso }, style + " ajax-previsualizar-link", "icon-download-alt", true).ToHtmlString())
                                    : null
                                , "editar-borrar-columna"
                                , false
                                );
        }

        public static WebGridColumn ColumnaPrevisualizar(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {
            return grid.Column("PrevisualizarImprimir", "", f =>
                html.Raw(
                "<span>" +
                html.BotonLink(Textos.Previsualizar, "Previsualizar", controller, new { f.id }, style + " ajax-previsualizar-link", "icon-search", true).ToHtmlString() +
                "</span>"
                )
                , "editar-borrar-columna", false);
        }

        public static WebGridColumn ColumnaDescargarPDFCartaPorte(this WebGrid grid, HtmlHelper html, string action, string action2, string controller, string style = "")
        {
            return grid.Column("descargarCPE"
                                , "PDF"
                                , f => f.tipoWorkFlowEgreso && f.tipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte ?
                                    html.Raw(
                                        html.BotonLink(Textos.Descargar_PDF_CPE, action, controller, new { id = f.numeroDocumentoIngreso, f.numeroDocumentoIngreso }, style + " ajax-popup-link", "icon-download-alt", true).ToHtmlString())
                                    : null
                                , "editar-pdf-columna"
                                , false
                                );
        }

        public static WebGridColumn ColumnaModificarClienteProvisorio(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {
            return  grid.Column("editar", "", f => f.EsClienteProvisorio ? 
                                                html.Raw(html.BotonLink(Textos.Modificar, "Modificar", controller, new { f.id }, style + " ajax-editar-link", "icon-edit", true).ToHtmlString()) 
                                                :  
                                                html.Raw("<span disabled></span>"), "editar-borrar-columna", false);
                                   
        }

        public static WebGridColumn ColumnaEliminarModificarHuellaDigital(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {
            return grid.Column("EliminarModificar", "", f =>

                    html.Raw((f.Tipo == (int)TipoHuellaDigital.Recorrido) ?
                    "<span class=\"label label-important\" data-toggle=\"tooltip\" title=\"" + Textos.No_EditableLargo + "\" >" + Textos.No_Editable + "</span>":
                    "<span>" +
                    html.BotonLink(Textos.Modificar, "Modificar", controller, new { f.id }, style + " ajax-editar-link", "icon-edit", true).ToHtmlString() +
                    
                    "</span>"
                    )
                    , "editar-borrar-columna", false);

        }

        public static WebGridColumn ColumnaModificarExcepcionPagoTasaMunicipal(this WebGrid grid, HtmlHelper html, string controller, string style = "")
        {
            string modalModificar = $"{"#modal-editar"}";
            string modalBorrar = $"{"#modal-borrar"}";
            return grid.Column("Modificar", "", f =>

                    html.Raw((f.PermiteAcciones) ?
                    "<span class=\"label label-important\" data-toggle=\"tooltip\" title=\"" + Textos.No_EditableLargo + "\" >" + Textos.No_Editable + "</span>" :
                    "<span>" +
                    html.BotonModalLink(Textos.Modificar, "Modificar", controller, new { f.id}, style + "abrirModal", "icon-edit", modalModificar, $"data-id = \"{f.id}\" data-patente = \"{f.patente}\" data-numeroDocumento = \"{f.numeroDocumentoIngreso}\" data-workflowCodigo = \"{f.workflowCodigo}\" data-workflowDescripcion = \"{f.workflowDescripcion}\"", true).ToHtmlString() +
                    html.BotonModalLink(Textos.Modificar, "Borrar", controller, new { f.id }, style + "abrirModalBorrar", "icon-trash", modalBorrar, $"data-id = \"{f.id}\"", true).ToHtmlString() +

                    "</span>"
                    )
                    , "editar-borrar-columna", false);

        }

    }
}
