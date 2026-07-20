using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class CamionesPorDiaConsulta : IConsulta<CamionPorDiaDto>
    {
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly int? puestoDeTrabajoId;

        public CamionesPorDiaConsulta(FiltroDashboardCardlessDto filtro)
        {
            fechaDesde = filtro.FechaDesde;
            fechaHasta = filtro.FechaHasta;
            puestoDeTrabajoId = filtro.PuestoDeTrabajoId;
        }

        public List<CamionPorDiaDto> Ejecutar(DbContext contexto)
        {
            const string sql = @"
                SELECT
                    CAST(l.FechaEvento AS DATE)                    AS Fecha,
                    COUNT(*)                                        AS CantidadDiaria,
                    SUM(COUNT(*)) OVER (
                        ORDER BY CAST(l.FechaEvento AS DATE)
                    )                                               AS Acumulado
                FROM LogIdentificacionVehicular l
                WHERE l.FechaEvento >= @FechaDesde
                  AND l.FechaEvento <  DATEADD(DAY, 1, @FechaHasta)
                  AND (@PuestoDeTrabajoId IS NULL OR l.PuestoDeTrabajo_Id = @PuestoDeTrabajoId)
                GROUP BY CAST(l.FechaEvento AS DATE)
                ORDER BY Fecha;";

            return contexto.Database.SqlQuery<CamionPorDiaDto>(
                sql,
                new SqlParameter("@FechaDesde", fechaDesde.Date),
                new SqlParameter("@FechaHasta", fechaHasta.Date),
                new SqlParameter("@PuestoDeTrabajoId", (object)puestoDeTrabajoId ?? DBNull.Value)
            ).ToList();
        }
    }
}
