using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class MonitorCPEsCacheadasConsulta : IConsultaPaginada<MonitorCPECacheadaListadoDto>
    {
        private MonitorCPECacheadaFiltroDto _filtro;
        private readonly Paginacion _paginacion;

        public MonitorCPEsCacheadasConsulta(MonitorCPECacheadaFiltroDto filtro, Paginacion paginacion)
        {
            _filtro = filtro;
            _paginacion = paginacion;
        }

        public ListaPaginada<MonitorCPECacheadaListadoDto> Ejecutar(DbContext contexto)
        {
            var estadosValidos = new List<string>() { "AC", "CO", "CF" };

            var centro = contexto.Set<Centro>().FirstOrDefault(q => q.Id == _filtro.CentroId);

            var materialList = contexto.Set<Material>().Where(q => q.EsGrano && !q.CodigoONCCA.Equals(null)).ToList();

          

            var query = contexto.Set<CartaPorteElectronica>().Where(q => q.PlantaDestino == centro.Planta && estadosValidos.Contains(q.Estado)).AsQueryable();

            if (_filtro.CTG.HasValue)
                query = query.Where(x => x.NroCTG == _filtro.CTG);

            if (_filtro.FechaCPEDesde.HasValue)
                query = query.Where(x => x.FechaCP >= _filtro.FechaCPEDesde);

            if (_filtro.FechaCPEHasta.HasValue)
                query = query.Where(x => x.FechaCP < _filtro.FechaCPEHasta);

            if (!string.IsNullOrEmpty(_filtro.Patente))
                query = query.Where(x => x.Dominio.StartsWith(_filtro.Patente));

            if (_filtro.MaterialId.HasValue)
            {
                var codigoONCA = materialList.FirstOrDefault(q => q.Id == _filtro.MaterialId.Value)?.CodigoONCCA;
                int codigoONCAInt = (codigoONCA != null) ? int.Parse(codigoONCA) : 0;

                query = query.Where(x => x.Material == codigoONCAInt);
            }

            var camionesPendientes = new List<int>();
            if (!_filtro.EsJobAutomatico) { 
                camionesPendientes = ObtenerCamionesCacheadosPendientes(contexto, centro.Planta);
            }

            if (_filtro.VerCamionesPorLlegar)
            {
                query = query.Where(x => camionesPendientes.Contains(x.Id));
            }

            var resultQuery = query.Select(x => new MonitorCPECacheadaListadoDto()
            {
                Id = x.Id,
                CTG = x.NroCTG,
                Patente = x.Dominio,
                CodigoONCA = x.Material,
                FechaCacheado = x.FechaCacheado,
                FechaCPE = x.FechaCP,
                TipoCartaPorte = x.TipoCartaPorte,
                TienePDF = (x.Pdf != null) ? true : false
            });

            var itemsTotales = resultQuery.Count();

            if (_paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<MonitorCPECacheadaListadoDto>(_paginacion.OrdenarPor);
                resultQuery = _paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultQuery.OrderBy(selectorOrden)
                                 : resultQuery.OrderByDescending(selectorOrden);
            }

            resultQuery = resultQuery.Skip((_paginacion.Pagina - 1) * _paginacion.ItemsPorPagina).Take(_paginacion.ItemsPorPagina);

            var result = resultQuery.ToList();

            LlenarValores(result, materialList);

            _filtro.CamionesPendientes = camionesPendientes.Count();
            _filtro.FechaEjecucionCacheoCPE = centro.FechaEjecucionCacheoCPE;
            _filtro.ErrorCacheoAfipCPE = centro.ErrorCacheoAfipCPE;
            return new ListaPaginada<MonitorCPECacheadaListadoDto>(result.ToList(), _paginacion.Pagina, _paginacion.ItemsPorPagina, itemsTotales);
        }

        private List<int> ObtenerCamionesCacheadosPendientes(DbContext contexto, int? planta)
        {
            var sqlQuery = @"SELECT CPE.Id FROM CartaPorteElectronica CPE
                        LEFT JOIN CartaPorte CP
                        ON CP.NroCartaPorte = CAST(CPE.NroCTG AS varchar)
                        WHERE CP.NroCartaPorte IS NULL AND CPE.PlantaDestino = @planta AND CPE.Estado IN ('AC', 'CO', 'CF')";

            var sqlEjecucion = contexto.Database.SqlQuery<int>(sqlQuery,
                new SqlParameter("@planta", planta));

            var sqlResult = sqlEjecucion.ToList();
            return sqlResult;
        }

        private void LlenarValores(List<MonitorCPECacheadaListadoDto> listadoCacheado
            , List<Material> materialList)
        {
            foreach (var item in listadoCacheado)
            {
                var patentes = item.Patente.Split(',');
                var codigoONCA = (item.CodigoONCA.HasValue) ? item.CodigoONCA.Value.ToString() : string.Empty;

                item.Patente = patentes.FirstOrDefault();
                item.MaterialDescripcion = materialList.FirstOrDefault(q => q.CodigoONCCA == codigoONCA)?.Descripcion;
            }
        }
    }
}