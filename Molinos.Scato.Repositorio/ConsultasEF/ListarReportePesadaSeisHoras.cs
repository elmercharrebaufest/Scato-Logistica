using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarReportePesadaSeisHoras : IConsultaPaginada<ReportePesadaSeisHorasDto>
    {
        private readonly DateTime inicio;
        private readonly DateTime fin;
        private Paginacion paginacion;
        private readonly int? exportador;
        private readonly int? material;

        public ListarReportePesadaSeisHoras(DateTime inicio, DateTime fin, Paginacion paginacion = null, int? exportador = null, int? material = null)
        {
            this.inicio = inicio;
            this.fin = fin;
            this.paginacion = paginacion;
            this.exportador = exportador;
            this.material = material;

        }

        public ListaPaginada<ReportePesadaSeisHorasDto> Ejecutar(DbContext contexto)
        {

            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var consulta = @"select CAST(r.Fecha as DATE) as Fecha, v.Nombre as Vapor, m.Descripcion as Material, e.Nombre as Exportador,
			                SUM (b.PesoBruto-b.pesotara) as Total,
			                SUM( CASE WHEN CAST(r.Fecha as time) <= '06:00:00.000' then b.PesoBruto-b.pesotara else 0 end) as RangoCeroASeis,
			                SUM( CASE WHEN CAST(r.Fecha as time) > '06:00:00.000' and CAST(r.Fecha as time) <= '12:00:00.000' then b.PesoBruto-b.pesotara else 0 end) as RangoSeisADoce,
			                SUM( CASE WHEN CAST(r.Fecha as time) > '12:00:00.000' and CAST(r.Fecha as time) <= '18:00:00.000' then b.PesoBruto-b.pesotara else 0 end) as RangoDoceADieciseis, 
			                SUM( CASE WHEN CAST(r.Fecha as time) > '18:00:00.000' and CAST(r.Fecha as time) <= '23:59:59.000' then b.PesoBruto-b.pesotara else 0 end) as RangoDieciseisAveinticuatro
			                from Balanzada b 
			                left join RegistroBalanzaPuerto r on b.Id = r.Id and b.CargaInicial_NumeroBalanza = r.NumeroBalanza
			                left join Carga c on b.CargaInicial_Id = c.Id and b.CargaInicial_NumeroBalanza = c.NumeroBalanza
			                left join Vapor v on v.Id = c.Vapor_Id
			                left join MaterialPuerto m on m.Id = c.Material_Id
			                left join Exportador e on e.Id = c.Exportador_Id
			                where (r.Fecha between @inicio and @fin) and (@exportadorId = 0 or e.Id = @exportadorId) and (@materialId = 0 or m.Id = @materialId) and e.nombre <> ''
			                group by CAST(r.Fecha as DATE), v.Nombre, m.Descripcion, e.Nombre
                            order by " + paginacion.OrdenarPor + " " + ((paginacion.DireccionOrden == DirOrden.Asc) ? "asc" : "desc");

            var resultados = contexto.Database.SqlQuery<ReportePesadaSeisHorasDto>(consulta,
                      new SqlParameter("@inicio", inicio),
                      new SqlParameter("@fin", fin),
                      new SqlParameter("@exportadorId", exportador ?? 0),
                      new SqlParameter("@materialId", material ?? 0)).ToList();
            var itemsTotales = resultados.Count();

            var resultado = resultados.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);
            return new ListaPaginada<ReportePesadaSeisHorasDto>(resultado.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
