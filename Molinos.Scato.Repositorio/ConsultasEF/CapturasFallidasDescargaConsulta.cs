using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class CapturasFallidasDescargaConsulta : IConsulta<CapturaFallidaDto>
    {
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;

        public CapturasFallidasDescargaConsulta(FiltroCapturasFallidasDto filtro)
        {
            fechaDesde = filtro.FechaDesde;
            fechaHasta = filtro.FechaHasta;
        }

        public List<CapturaFallidaDto> Ejecutar(DbContext contexto)
        {
            const string sql = @"
                SELECT
                    d.Id            AS DetalleId,
                    d.RutaImagen,
                    d.CodigoCamara,
                    l.FechaEvento,
                    0               AS TotalRegistros
                FROM LogIdentificacionVehicularDetalle d
                INNER JOIN LogIdentificacionVehicular l ON d.LogIdentificacionVehicular_Id = l.Id
                WHERE d.Exitoso     = 0
                  AND d.RutaImagen IS NOT NULL
                  AND d.RutaImagen  <> ''
                  AND l.FechaEvento >= @FechaDesde
                  AND l.FechaEvento <  DATEADD(DAY, 1, @FechaHasta)
                ORDER BY l.FechaEvento DESC;";

            return contexto.Database.SqlQuery<CapturaFallidaDto>(
                sql,
                new SqlParameter("@FechaDesde", fechaDesde.Date),
                new SqlParameter("@FechaHasta", fechaHasta.Date)
            ).ToList();
        }
    }
}
