using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ReconocimientoPorDiaSemanaConsulta : IConsulta<ReconocimientoPorDiaSemanaDto>
    {
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly int? puestoDeTrabajoId;

        public ReconocimientoPorDiaSemanaConsulta(FiltroDashboardCardlessDto filtro)
        {
            fechaDesde = filtro.FechaDesde;
            fechaHasta = filtro.FechaHasta;
            puestoDeTrabajoId = filtro.PuestoDeTrabajoId;
        }

        public List<ReconocimientoPorDiaSemanaDto> Ejecutar(DbContext contexto)
        {
            const string sql = @"
                SELECT
                    CAST(l.FechaEvento AS DATE) AS Fecha,

                    SUM(
                        CASE
                            WHEN l.Recorrido_Id IS NOT NULL
                             AND l.PuestoDeTrabajo_Id IS NOT NULL
                            THEN 1
                            ELSE 0
                        END
                    ) AS Reconocidos,

                    SUM(
                        CASE
                            WHEN l.Recorrido_Id IS NULL
                              OR l.PuestoDeTrabajo_Id IS NULL
                            THEN 1
                            ELSE 0
                        END
                    ) AS NoReconocidos

                FROM LogIdentificacionVehicular l
                WHERE l.FechaEvento >= @FechaDesde
                  AND l.FechaEvento < DATEADD(DAY, 1, @FechaHasta)
                  AND (
                        @PuestoDeTrabajoId IS NULL
                        OR l.PuestoDeTrabajo_Id = @PuestoDeTrabajoId
                      )
                GROUP BY CAST(l.FechaEvento AS DATE)
                ORDER BY Fecha;";

            return contexto.Database.SqlQuery<ReconocimientoPorDiaSemanaDto>(
                sql,
                new SqlParameter("@FechaDesde", fechaDesde.Date),
                new SqlParameter("@FechaHasta", fechaHasta.Date),
                new SqlParameter(
                    "@PuestoDeTrabajoId",
                    (object)puestoDeTrabajoId ?? DBNull.Value
                )
            ).ToList();
        }
    }
}