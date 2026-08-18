using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class CapturasFallidasConsulta : IConsulta<CapturaFallidaDto>
    {
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly int? puestoDeTrabajoId;
        private readonly int pagina;
        private const int PageSize = 15;

        public CapturasFallidasConsulta(FiltroCapturasFallidasDto filtro, int pagina)
        {
            fechaDesde = filtro.FechaDesde;
            fechaHasta = filtro.FechaHasta;
            puestoDeTrabajoId = filtro.PuestoDeTrabajoId;
            this.pagina = pagina < 1 ? 1 : pagina;
        }

        public List<CapturaFallidaDto> Ejecutar(DbContext contexto)
        {
            const string sql = @"
                SELECT
                    d.Id            AS DetalleId,
                    d.RutaImagen,
                    d.CodigoCamara,
                    l.FechaEvento,
                    COUNT(*) OVER () AS TotalRegistros
                FROM LogIdentificacionVehicularDetalle d
                INNER JOIN LogIdentificacionVehicular l ON d.LogIdentificacionVehicular_Id = l.Id
                WHERE d.Exitoso     = 0
                  AND d.RutaImagen IS NOT NULL
                  AND d.RutaImagen  <> ''
                  AND l.FechaEvento >= @FechaDesde
                  AND l.FechaEvento <  DATEADD(DAY, 1, @FechaHasta)
                  AND (@PuestoId IS NULL OR l.PuestoDeTrabajo_Id = @PuestoId)
                ORDER BY l.FechaEvento DESC
                OFFSET (@Pagina - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;";

            return contexto.Database.SqlQuery<CapturaFallidaDto>(
                sql,
                new SqlParameter("@FechaDesde", fechaDesde.Date),
                new SqlParameter("@FechaHasta", fechaHasta.Date),
                new SqlParameter("@PuestoId", (object)puestoDeTrabajoId ?? DBNull.Value),
                new SqlParameter("@Pagina", pagina),
                new SqlParameter("@PageSize", PageSize)
            ).ToList();
        }
    }
}
