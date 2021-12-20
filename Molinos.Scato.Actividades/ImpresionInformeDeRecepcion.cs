using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Actividades.Helpers;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionInformeDeRecepcion : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        public InArgument<int> PuestoDeTrabajoId { get; set; } 

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresión Informe De Descarga",
                    ActividadXaml = "ImpresionInformeDeRecepcion",
                    WorkflowInstanceId = workflowId,
                    Fecha = DateTime.Now
                };
            try
            {
                resultado = servicio.Ejecutar(new CrearLogActividad { Dto = logActividad });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.LogActividad_ErrorEnLaCarga);
            }

            try
            {
                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null)
                {
                    resultado.Errores.Add("1", Textos.Impresion_DocumentoDeImpresionPorCentroCodigoError);
                }
                else
                {
                    var impresora = repositorio.ObtenerImpresora(documento.ImpresoraId);
                    var romaneos = repositorio.ObtenerRomaneosPorGuid(workflowId);
                    var listaRomaneos = new List<ImpInformeDeRecepcionDto>();

                    foreach (var item in romaneos)
                    {
                        var romaneoItem = repositorio.ObtenerItemRomaneosPorRomaneoId(item.Numero);

                        if (romaneoItem.Any())
                        {
                            List<ImpInformeDeRecepcionItemDto> listaRomaneoItem =
                                romaneoItem.Select(t => new ImpInformeDeRecepcionItemDto()
                                    {
                                        MaterialCodigo = repositorio.ObtenerMaterial(t.MaterialId).CodigoSAP,
                                        MaterialDescripcion = t.Material,
                                        ItemNro = t.ItemNro,
                                        UniMed = repositorio.ObtenerMaterial(t.MaterialId).UnidadDeMedidad,
                                        CantidadDescargada = t.PesoNeto.ToString(),
                                        Remito = t.RemitoNro
                                    }).ToList();
                            var dto = new ImpInformeDeRecepcionDto
                                {
                                    Impresora = impresora != null ? impresora.Direccion : "",
                                    Fecha = DateTime.Now,
                                    Codigo = codigo,
                                    ImpInformeDeRecepcionItems = listaRomaneoItem,
                                    Proveedor =
                                        repositorio.ObtenerProveedor(item.ProveedorId).CodigoSap + " - " +
                                        item.ProveedorDescripcion,
                                    NumeroPedido = item.NroPedido,
                                    NumeroInforme =
                                        repositorio.ObtenerNumeroInformeGenerado()
                                                   .ToString(CultureInfo.InvariantCulture)
                                                   .PadLeft(8, '0'),
                                    Estado = item.Estado.DisplayEnum(),
                                };
                            listaRomaneos.Add(dto);
                        }
                    }
                    resultado = servicio.Ejecutar(new ImprimirInformeDeRecepcion() { Dto = listaRomaneos, Impresora = impresora.Direccion });
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionInformeDeRecepcion", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
