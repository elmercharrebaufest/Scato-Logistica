using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

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
            var workflowsTipos = contexto.Set<Workflow>().Where(x => x.Centro.Id == filtro.CentroId).Select(x => x.Codigo);

            var resultados = from x in contexto.Set<Recorrido>()
                             join caracteristicasAnalizadas in contexto.Set<CaracteristicasAnalizadas>().DefaultIfEmpty() on x.Id equals caracteristicasAnalizadas.Recorrido.Id into caracteristicasAnalizadasJoined
                             from caracteristicasAnalizadas in caracteristicasAnalizadasJoined.DefaultIfEmpty()
                             where (x.Centro.Id == filtro.CentroId
                    && ((filtro.Workflow != null && x.Workflow != null && x.Workflow.Codigo == filtro.Workflow) || filtro.Workflow == null)
                    && (workflowsTipos.Any(y => y == x.Workflow.Codigo))
                    && (filtro.TipoEstado == TipoEstado.Todos || (filtro.TipoEstado == TipoEstado.Si && x.Rechazado) || (filtro.TipoEstado == TipoEstado.No && !x.Rechazado))
                    && (filtro.TipoDeSoja == TipoDeSoja.Todos || (filtro.TipoDeSoja == TipoDeSoja.Si && x.Establecimiento != null) || (filtro.TipoDeSoja == TipoDeSoja.No && x.Establecimiento == null))
                    && ((filtro.ProximaAccion != null && contexto.Set<LogActividad>().Where( y => y.WorkflowInstanceId == x.InstanciaWorkflow).OrderByDescending(y => y.Id).FirstOrDefault().ActividadXaml == filtro.ProximaAccion) || filtro.ProximaAccion == null)
                    && ((filtro.Patente != null && x.Patente.ToLower().Contains(filtro.Patente.ToLower())) || filtro.Patente == null)
                    && ((filtro.TipoDocumentoDeIngreso != null && x.TipoDocumentoIngreso == filtro.TipoDocumentoDeIngreso) || filtro.TipoDocumentoDeIngreso == null)
                    && ((filtro.NumeroDocumentoDeIngreso != null && x.NumeroDocumentoIngreso.Contains(filtro.NumeroDocumentoDeIngreso)) || filtro.NumeroDocumentoDeIngreso == null)
                    && ((filtro.MaterialId.HasValue && x.Material.Id == filtro.MaterialId.Value) || filtro.MaterialId == null || filtro.MaterialId == 0)
                    && ((filtro.Calidad != null && x.Calado.CalidadMaterial.Descripcion.ToLower() == filtro.Calidad.ToLower()) || filtro.Calidad == null)
                    && ((filtro.TipoComercialId.HasValue && x.TipoComercial.Id == filtro.TipoComercialId) || filtro.TipoComercialId == null || filtro.TipoComercialId == 0)
                    && ((filtro.SoloNoAsignados && x.Almacen == null) || !filtro.SoloNoAsignados)
                    && ((filtro.SoloSinDescuentos && caracteristicasAnalizadas != null && caracteristicasAnalizadas.TieneDescuentos) || !filtro.SoloSinDescuentos)
                    && (filtro.TipoDeProteina == TipoDeProteina.Todos || filtro.TipoDeProteina == TipoDeProteina.Baja || (filtro.TipoDeProteina == TipoDeProteina.Alta && caracteristicasAnalizadas != null && caracteristicasAnalizadas.EsProteinaAlta))
                    && (filtro.TipoDeProteina == TipoDeProteina.Todos || filtro.TipoDeProteina == TipoDeProteina.Alta || (filtro.TipoDeProteina == TipoDeProteina.Baja && caracteristicasAnalizadas != null && caracteristicasAnalizadas.EsProteinaBaja))  
                    && ((filtro.TipoVehiculo != null && (filtro.TipoVehiculo == TipoVehiculo.Camiones && x.TipoVehiculo!= TipoVehiculo.Bitren && x.TipoVehiculo != TipoVehiculo.Tren && x.TipoVehiculo != TipoVehiculo.Vapor) ||
                    x.TipoVehiculo == filtro.TipoVehiculo) || filtro.TipoVehiculo  == null)
                    && (filtro.TipoMaterial == TipoMaterial.Todos || (filtro.TipoMaterial == TipoMaterial.Granos && x.Material.EsGrano) || (filtro.TipoMaterial == TipoMaterial.NoGranos && !x.Material.EsGrano))
                    && (filtro.CalleId == null ||  x.CallePorRecorridos.Any(y=>y.FechaEgreso == null && filtro.CalleId == y.Calle.Id))
                    && !x.Terminado)
                    || (filtro.NumeroDeTarjeta != null && x.TarjetaDeAcceso == filtro.NumeroDeTarjeta)
                                 select x;

            if (paginacion.OrdenarPor != null)
            {
                if (paginacion.OrdenarPor == "Calle")
                {
                    Expression<Func<Recorrido, string>> selectorOrden = x => x.CallePorRecorridos.FirstOrDefault(y => y.FechaEgreso == null).Calle.Nombre;
                    resultados = paginacion.DireccionOrden == DirOrden.Asc
                                     ? resultados.OrderBy(selectorOrden)
                                     : resultados.OrderByDescending(selectorOrden);
                }
                else
                {
                    var selectorOrden = Expresiones.Propiedad<Recorrido>(paginacion.OrdenarPor);
                    resultados = paginacion.DireccionOrden == DirOrden.Asc
                                     ? resultados.OrderBy(selectorOrden)
                                     : resultados.OrderByDescending(selectorOrden);
                }
            }
            var itemsTotales = resultados.Count();

            resultados = resultados.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            var pagina = resultados
                .Include(x => x.CaracteristicasAnalizadasList)
                .Include(x => x.Calado.CalidadMaterial)
                .Include(x => x.Transportista)
                .Include(x => x.Material)
                .Include(x => x.Workflow)
                .Include(x => x.Establecimiento)
                .Include(x => x.TipoComercial)
                .Include(x => x.Chofer)
                .Include(x => x.Centro)
                .Include(x => x.CallePorRecorridos)
                .ToList().Select(x => new InstanciaWorkflowDto
            {
                Id = x.InstanciaWorkflow,
                Material = x.Material.Descripcion,
                MaterialId = x.Material.Id,
                MaterialCodigoSap = x.Material.CodigoSAP,
                Transportista = x.Transportista != null ? x.Transportista.RazonSocial : "",
                TransportistaId = x.Transportista != null ? x.Transportista.Id : 0,
                Cuit = x.Chofer.Cuil,
                Calidad = x.Calado != null && x.Calado.CalidadMaterial != null ? x.Calado.CalidadMaterial.Descripcion : "",
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
                Humedad = x.CaracteristicasAnalizadas != null && x.CaracteristicasAnalizadas.Humedad.HasValue ? x.CaracteristicasAnalizadas.Humedad.ToString() : "",
                Workflow = x.Workflow.Descripcion,
                Codigo = x.Workflow.Codigo,
                EsSustentable = x.Establecimiento != null,
                TieneDescuentos = x.CaracteristicasAnalizadas != null && x.CaracteristicasAnalizadas.TieneDescuentos,
                FueAsignado = x.Almacen != null,
                Rechazado = x.Rechazado,
                PagaTicketMunicipal = x.PagaTicketMunicipal != null && x.PagaTicketMunicipal.Value,
                EsHumedad = x.CaracteristicasAnalizadas != null && x.CaracteristicasAnalizadas.EsHumedad,
                EsGranosVerdes = x.CaracteristicasAnalizadas != null && x.CaracteristicasAnalizadas.EsGranosVerdes,
                EsGranosDañados = x.CaracteristicasAnalizadas != null && x.CaracteristicasAnalizadas.EsGranosDañados,
                EsCuerposExtranos = x.CaracteristicasAnalizadas != null && x.CaracteristicasAnalizadas.EsCuerposExtranos,
                EsProteinaBaja = x.CaracteristicasAnalizadas != null && x.CaracteristicasAnalizadas.EsProteinaBaja,
                EsProteinaAlta = x.CaracteristicasAnalizadas != null && x.CaracteristicasAnalizadas.EsProteinaAlta,
                TieneInsectosVivos = x.CaracteristicasAnalizadas != null && x.CaracteristicasAnalizadas.TieneInsectosVivos,
                TipoVehiculo = x.TipoVehiculo,
                Calle = x.CallePorRecorridos.Where(y => y.FechaEgreso == null).FirstOrDefault()?.Calle?.Nombre
                });



            return new ListaPaginada<InstanciaWorkflowDto>(pagina.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
