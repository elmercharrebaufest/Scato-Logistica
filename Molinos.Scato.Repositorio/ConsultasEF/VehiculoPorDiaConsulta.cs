using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class VehiculoPorDiaConsulta : IConsulta<VehiculoPorDiaDto>
    {
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly int? puestoDeTrabajoId;

        public VehiculoPorDiaConsulta(FiltroDashboardCardlessDto filtro)
        {
            fechaDesde = filtro.FechaDesde;
            fechaHasta = filtro.FechaHasta;
            puestoDeTrabajoId = filtro.PuestoDeTrabajoId;
        }

        public List<VehiculoPorDiaDto> Ejecutar(DbContext contexto)
        {
            const string sql = @"
                SELECT
                    CAST(l.FechaEvento AS DATE)                                                   AS Fecha,
                    DATENAME(WEEKDAY, l.FechaEvento)                                              AS NombreDia,
                    SUM(CASE WHEN l.VehiculoPresente = 1 THEN 1 ELSE 0 END)                       AS Presente,
                    SUM(CASE WHEN l.VehiculoPresente = 0 THEN 1 ELSE 0 END)                       AS NoPresente
                FROM LogIdentificacionVehicular l
                WHERE l.FechaEvento >= @FechaDesde
                  AND l.FechaEvento <  DATEADD(DAY, 1, @FechaHasta)
                  AND (@PuestoDeTrabajoId IS NULL OR l.PuestoDeTrabajo_Id = @PuestoDeTrabajoId)
                GROUP BY CAST(l.FechaEvento AS DATE), DATENAME(WEEKDAY, l.FechaEvento)
                ORDER BY Fecha;";

            return contexto.Database.SqlQuery<VehiculoPorDiaDto>(
                sql,
                new SqlParameter("@FechaDesde", fechaDesde.Date),
                new SqlParameter("@FechaHasta", fechaHasta.Date),
                new SqlParameter("@PuestoDeTrabajoId", (object)puestoDeTrabajoId ?? DBNull.Value)
            ).ToList();
        }
    }
}
