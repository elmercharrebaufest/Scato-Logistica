using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ReconocimientoPorProveedorConsulta : IConsulta<ReconocimientoPorProveedorDto>
    {
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly int? puestoDeTrabajoId;

        public ReconocimientoPorProveedorConsulta(FiltroDashboardCardlessDto filtro)
        {
            fechaDesde = filtro.FechaDesde;
            fechaHasta = filtro.FechaHasta;
            puestoDeTrabajoId = filtro.PuestoDeTrabajoId;
        }

        public List<ReconocimientoPorProveedorDto> Ejecutar(DbContext contexto)
        {
            const string sql = @"
                SELECT
                    d.ProveedorALPR,
                    COUNT(*)                                                              AS TotalIntentos,
                    SUM(CASE WHEN d.Exitoso = 1 THEN 1 ELSE 0 END)                        AS IntentosExitosos,
                    CAST(
                        SUM(CASE WHEN d.Exitoso = 1 THEN 1.0 ELSE 0.0 END)
                        / NULLIF(COUNT(*), 0) * 100
                    AS DECIMAL(5,1))                                                       AS TasaReconocimiento
                FROM LogIdentificacionVehicularDetalle d
                INNER JOIN LogIdentificacionVehicular l ON d.LogIdentificacionVehicular_Id = l.Id
                WHERE l.FechaEvento >= @FechaDesde
                  AND l.FechaEvento <  DATEADD(DAY, 1, @FechaHasta)
                  AND (@PuestoDeTrabajoId IS NULL OR l.PuestoDeTrabajo_Id = @PuestoDeTrabajoId)
                GROUP BY d.ProveedorALPR
                ORDER BY TasaReconocimiento DESC;";

            return contexto.Database.SqlQuery<ReconocimientoPorProveedorDto>(
                sql,
                new SqlParameter("@FechaDesde", fechaDesde.Date),
                new SqlParameter("@FechaHasta", fechaHasta.Date),
                new SqlParameter("@PuestoDeTrabajoId", (object)puestoDeTrabajoId ?? DBNull.Value)
            ).ToList();
        }
    }
}
