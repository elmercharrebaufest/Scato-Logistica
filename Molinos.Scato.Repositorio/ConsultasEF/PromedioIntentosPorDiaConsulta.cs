using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class PromedioIntentosPorDiaConsulta : IConsulta<PromedioIntentosPorDiaDto>
    {
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly int? puestoDeTrabajoId;

        public PromedioIntentosPorDiaConsulta(FiltroDashboardCardlessDto filtro)
        {
            fechaDesde = filtro.FechaDesde;
            fechaHasta = filtro.FechaHasta;
            puestoDeTrabajoId = filtro.PuestoDeTrabajoId;
        }

        public List<PromedioIntentosPorDiaDto> Ejecutar(DbContext contexto)
        {
            const string sql = @"
                SELECT
                    CAST(l.FechaEvento AS DATE)           AS Fecha,
                    AVG(CAST(sub.IntentosPorEvento AS FLOAT)) AS PromedioIntentos
                FROM (
                    SELECT
                        l2.Id                             AS LogId,
                        l2.FechaEvento,
                        COUNT(d.Id)                       AS IntentosPorEvento
                    FROM LogIdentificacionVehicular l2
                    LEFT JOIN LogIdentificacionVehicularDetalle d ON d.LogIdentificacionVehicular_Id = l2.Id
                    WHERE l2.FechaEvento >= @FechaDesde
                      AND l2.FechaEvento <  DATEADD(DAY, 1, @FechaHasta)
                      AND (@PuestoDeTrabajoId IS NULL OR l2.PuestoDeTrabajo_Id = @PuestoDeTrabajoId)
                    GROUP BY l2.Id, l2.FechaEvento
                ) sub
                INNER JOIN LogIdentificacionVehicular l ON l.Id = sub.LogId
                GROUP BY CAST(l.FechaEvento AS DATE)
                ORDER BY Fecha;";

            return contexto.Database.SqlQuery<PromedioIntentosPorDiaDto>(
                sql,
                new SqlParameter("@FechaDesde", fechaDesde.Date),
                new SqlParameter("@FechaHasta", fechaHasta.Date),
                new SqlParameter("@PuestoDeTrabajoId", (object)puestoDeTrabajoId ?? DBNull.Value)
            ).ToList();
        }
    }
}
