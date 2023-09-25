using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarWorkFlowsConsulta : IConsultaPaginada<InstanciaWorkflowDto>
    {
        private readonly FiltroListaDeWorkflowsDto filtro;
        private readonly Paginacion paginacion;

        public ListarWorkFlowsConsulta(FiltroListaDeWorkflowsDto filtro, Paginacion paginacion)
        {
            this.filtro = filtro;
            this.paginacion = paginacion;
        }

        public ListaPaginada<InstanciaWorkflowDto> Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var query = contexto.Set<Recorrido>().Where(q => q.Centro.Id == filtro.CentroId);

            if (string.IsNullOrWhiteSpace(filtro.NumeroDeTarjeta))
            {
                var workflowsTipos = contexto.Set<Workflow>().Where(x => x.Centro.Id == filtro.CentroId).Select(x => x.Codigo).ToList();

                query = query.Where(q => workflowsTipos.Contains(q.Workflow.Codigo));

                query = query.Where(q => q.Terminado != true);

                if (!string.IsNullOrWhiteSpace(filtro.Workflow))
                    query = query.Where(q => q.Workflow.Codigo == filtro.Workflow);

                if (filtro.TipoEstado == TipoEstado.Si)
                    query = query.Where(q => q.Rechazado == true);
                else if (filtro.TipoEstado == TipoEstado.No)
                    query = query.Where(q => q.Rechazado != true);

                if (filtro.TipoDeSoja == TipoDeSoja.Si)
                    query = query.Where(q => q.Establecimiento != null);
                else if (filtro.TipoDeSoja == TipoDeSoja.No)
                    query = query.Where(q => q.Establecimiento == null);

                if (filtro.TipoDeSoja == TipoDeSoja.Si)
                    query = query.Where(q => q.Establecimiento != null);
                else if (filtro.TipoDeSoja == TipoDeSoja.No)
                    query = query.Where(q => q.Establecimiento == null);

                if (!string.IsNullOrWhiteSpace(filtro.ProximaAccion))
                    query = query.Where(q => contexto.Set<LogActividad>().Where(y => y.WorkflowInstanceId == q.InstanciaWorkflow).OrderByDescending(y => y.Id).FirstOrDefault().ActividadXaml == filtro.ProximaAccion);

                if (!string.IsNullOrWhiteSpace(filtro.Patente))
                    query = query.Where(q => q.Patente.Contains(filtro.Patente));

                if (filtro.TipoDocumentoDeIngreso.HasValue)
                    query = query.Where(q => q.TipoDocumentoIngreso == filtro.TipoDocumentoDeIngreso);

                if (!string.IsNullOrWhiteSpace(filtro.NumeroDocumentoDeIngreso))
                    query = query.Where(q => q.NumeroDocumentoIngreso.Contains(filtro.NumeroDocumentoDeIngreso));

                if (filtro.MaterialId.HasValue && filtro.MaterialId != 0)
                    query = query.Where(q => q.Material.Id == filtro.MaterialId);

                if (!string.IsNullOrWhiteSpace(filtro.Calidad))
                    query = query.Where(q => q.Calado.CalidadMaterial.Descripcion.ToLower() == filtro.Calidad.ToLower());

                if (filtro.TipoComercialId.HasValue && filtro.TipoComercialId != 0)
                    query = query.Where(q => q.TipoComercial.Id == filtro.TipoComercialId);

                if (filtro.SoloNoAsignados)
                    query = query.Where(q => q.Almacen == null);

                if (filtro.SoloSinDescuentos)
                    query = query.Where(q => q.CaracteristicasAnalizadasList.FirstOrDefault().TieneDescuentos);

                if (filtro.TipoDeProteina == TipoDeProteina.Alta)
                    query = query.Where(q => q.CaracteristicasAnalizadasList.FirstOrDefault().EsProteinaAlta);
                else if (filtro.TipoDeProteina == TipoDeProteina.Media)
                    query = query.Where(q => q.CaracteristicasAnalizadasList.FirstOrDefault().EsProteinaMedia);
                else if (filtro.TipoDeProteina == TipoDeProteina.Baja)
                    query = query.Where(q => q.CaracteristicasAnalizadasList.FirstOrDefault().EsProteinaBaja);

                if (filtro.TipoVehiculo == TipoVehiculo.Camiones)
                    query = query.Where(q => q.TipoVehiculo == TipoVehiculo.Camión || q.TipoVehiculo == TipoVehiculo.CamiónC || q.TipoVehiculo == TipoVehiculo.CamiónD || q.TipoVehiculo == TipoVehiculo.CamiónE);
                else if (filtro.TipoVehiculo.HasValue)
                    query = query.Where(q => q.TipoVehiculo == filtro.TipoVehiculo);

                if (filtro.TipoMaterial == TipoMaterial.Granos)
                    query = query.Where(q => q.Material.EsGrano == true);
                else if (filtro.TipoMaterial == TipoMaterial.NoGranos)
                    query = query.Where(q => q.Material.EsGrano != true);

                if (filtro.CalleId.HasValue)
                    query = query.Where(q => q.CallePorRecorridos.Any(y => y.FechaEgreso == null && y.Calle.Id == filtro.CalleId));
            }
            else
            {
                query = query.Where(q => q.TarjetaDeAcceso == filtro.NumeroDeTarjeta);
            }

            if (!string.IsNullOrWhiteSpace(paginacion.OrdenarPor))
            {
                if (paginacion.OrdenarPor == "Calle")
                {
                    Expression<Func<Recorrido, string>> selectorOrden = x => x.CallePorRecorridos.FirstOrDefault(y => y.FechaEgreso == null).Calle.Nombre;
                    query = query = paginacion.DireccionOrden == DirOrden.Asc
                                     ? query.OrderBy(selectorOrden)
                                     : query.OrderByDescending(selectorOrden);
                }
                else
                {
                    var selectorOrden = Expresiones.Propiedad<Recorrido>(paginacion.OrdenarPor);
                    query = query = paginacion.DireccionOrden == DirOrden.Asc
                                     ? query.OrderBy(selectorOrden)
                                     : query.OrderByDescending(selectorOrden);
                }
            }

            var itemsTotales = query.Count();
            query = query.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            var datos = query
                .Select(x => new InstanciaWorkflowDto
                {
                    RecorridoId = x.Id,
                    Id = x.InstanciaWorkflow,
                    Material = x.Material.Descripcion,
                    MaterialId = x.Material.Id,
                    MaterialCodigoSap = x.Material.CodigoSAP,
                    Transportista = x.Transportista != null ? x.Transportista.RazonSocial : string.Empty,
                    TransportistaId = x.Transportista != null ? x.Transportista.Id : 0,
                    Cuit = x.Chofer.Cuil,
                    Calidad = x.Calado.CalidadMaterial != null ? x.Calado.CalidadMaterial.Descripcion : string.Empty,
                    TipoDocumentoDeIngreso = x.TipoDocumentoIngreso,
                    NumeroDocumentoDeIngreso = x.NumeroDocumentoIngreso,
                    CentroId = x.Centro.Id,
                    CaladoId = x.Calado != null ? x.Calado.Id : 0,
                    Patente = x.Patente,
                    FechaCreacion = x.FechaInicio,
                    FechaCalado = x.Calado != null && x.Calado.FechaCreacion.HasValue ? x.Calado.FechaCreacion.Value : DateTime.MinValue,
                    Centro = x.Centro.Descripcion,
                    CentroCodigoSap = x.Centro.CodigoSAP,
                    NumeroDeTarjeta = x.TarjetaDeAcceso,
                    TipoComercial = x.TipoComercial.Descripcion,
                    TipoComercialId = x.TipoComercial.Id,
                    Workflow = x.Workflow.Descripcion,
                    Codigo = x.Workflow.Codigo,
                    EsSustentable = x.Establecimiento != null,
                    FueAsignado = x.Almacen != null,
                    Rechazado = x.Rechazado,
                    TipoVehiculo = x.TipoVehiculo,
                    PagaTicketMunicipal = x.PagaTicketMunicipal != null && x.PagaTicketMunicipal.Value,
                    Calle = x.CallePorRecorridos.Where(o => o.FechaEgreso == null).Select(y => y.Calle.Nombre).FirstOrDefault(),
                    SojaEPA = x.Establecimiento != null && x.Establecimiento.EPA,
                }).ToList();

            LlenarDatosCaracteristicasAnalizadas(datos, contexto);

            var result = new ListaPaginada<InstanciaWorkflowDto>(datos, paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
            return result;
        }

        private void LlenarDatosCaracteristicasAnalizadas(List<InstanciaWorkflowDto> datos, DbContext contexto)
        {
            foreach (var dato in datos)
            {
                var caracteristicaAnalizada = contexto.Set<CaracteristicasAnalizadas>().FirstOrDefault(q => q.Recorrido.Id == dato.RecorridoId);
                dato.TieneDescuentos = (caracteristicaAnalizada != null) ? caracteristicaAnalizada.TieneDescuentos : false;
                dato.Humedad = (caracteristicaAnalizada != null && caracteristicaAnalizada.Humedad.HasValue) ? caracteristicaAnalizada.Humedad.ToString() : string.Empty;
                dato.EsHumedad = (caracteristicaAnalizada != null) ? caracteristicaAnalizada.EsHumedad : false;
                dato.EsGranosVerdes = (caracteristicaAnalizada != null) ? caracteristicaAnalizada.EsGranosVerdes : false;
                dato.EsGranosDañados = (caracteristicaAnalizada != null) ? caracteristicaAnalizada.EsGranosDañados : false;
                dato.EsCuerposExtranos = (caracteristicaAnalizada != null) ? caracteristicaAnalizada.EsCuerposExtranos : false;
                dato.EsProteinaBaja = (caracteristicaAnalizada != null) ? caracteristicaAnalizada.EsProteinaBaja : false;
                dato.EsProteinaMedia = (caracteristicaAnalizada != null) ? caracteristicaAnalizada.EsProteinaMedia : false;
                dato.EsProteinaAlta = (caracteristicaAnalizada != null) ? caracteristicaAnalizada.EsProteinaAlta : false;
                dato.TieneInsectosVivos = (caracteristicaAnalizada != null) ? caracteristicaAnalizada.TieneInsectosVivos : false;
            }
        }
    }
}