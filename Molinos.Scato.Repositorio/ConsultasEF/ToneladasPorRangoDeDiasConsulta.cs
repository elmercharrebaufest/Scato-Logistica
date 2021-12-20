using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ToneladasPorRangoDeDiasConsulta : IConsultaEscalar<GraficoToneladasRangoDeDiasDto>
    {
        private readonly DateTime fechasDesde;
        private readonly DateTime fechaHasta;
        private readonly int centroId;
        private readonly int materialId;

        private GraficoToneladasRangoDeDiasDto modelo;

        public ToneladasPorRangoDeDiasConsulta(GraficoToneladasRangoDeDiasDto model, int centroId)
        {
            modelo = model;
            fechasDesde = model.FechaDesde;
            fechaHasta = model.FechaHasta;
            this.centroId = centroId;
            materialId = model.MaterialId;
        }

        public GraficoToneladasRangoDeDiasDto Ejecutar(DbContext contexto)
        {
            var consulta = @"declare @start datetime = cast(@fechaDesde as datetime)
                            declare @end   datetime = cast(@fechaHasta as datetime)


                            select CONVERT(varchar, cast(r.PesoTaraFecha as date), 103) as Fecha, cast(r.PesoTaraFecha as date) as Orden, pcd.Codigo, sum(r.PesoTara)/1000 as Toneladas from Recorrido r 
                            inner join ControlRecorrido cr on r.InstanciaWorkflow = cr.WorkflowInstanceId
                            left join PuestosDeCargaDescarga pcd on cr.PuestoDeTrabajo_Id = pcd.PuestoDeTrabajo_Id
                            where (cast(r.PesoTaraFecha as date) between @start and @end) and  
		                            cr.ActividadXaml = 'ConfirmacionDeCargaDescarga' and
		                            R.pesotarafecha is not null and 
		                            cr.PuestoDeTrabajo_Id is not null AND 
		                            R.Rechazado = 0 AND 
		                            R.centro_id = @centroId AND
                                    (R.Material_Id = @materialId OR @materialId = 0 )
                            group by cast(r.PesoTaraFecha as date), pcd.Codigo

                                        ";

            var query = contexto.Database.SqlQuery<ToneladasPorDiaDto>(consulta,
                new SqlParameter("@fechaDesde", fechasDesde.Date),
                new SqlParameter("@fechaHasta", fechaHasta.Date),
                new SqlParameter("@materialId", materialId),
                new SqlParameter("@centroId", centroId));
            modelo.Toneladas = query.ToList();

            var days = Enumerable.Range(0, 1 + fechaHasta.Date.Subtract(fechasDesde.Date).Days).ToList();
            foreach (var hidraulica in modelo.Toneladas.GroupBy(x => x.Codigo).Select(x => x.Key))
            {
                var diasFiltrados = days
                .Select(offset => new ToneladasPorDiaDto
                {
                    Codigo = hidraulica,
                    Fecha = fechasDesde.Date.AddDays(offset).ToString("dd/MM/yyyy"),
                    Orden = fechasDesde.Date.AddDays(offset),
                    Toneladas = 0,
                }).Where(x => !modelo.Toneladas.Any(y => y.Fecha == x.Fecha && y.Codigo == hidraulica));
                modelo.Toneladas.AddRange(diasFiltrados);
            }
            modelo.Toneladas = modelo.Toneladas.OrderBy(x => x.Orden).ToList();
            return modelo;
        }
    }
}

//CONSULTA ORIGINAl
//var consulta = @"declare @start datetime = cast(@fechaDesde as datetime)
//                            declare @end   datetime = cast(@fechaHasta as datetime)


//                            select cast(r.PesoTaraFecha as date) as fechaPeso, sum(r.PesoTara) as sumaPeso into #pesoPorFecha from Recorrido r 
//                            inner join ControlRecorrido cr on r.InstanciaWorkflow = cr.WorkflowInstanceId
//                            where (cast(r.PesoTaraFecha as date) between @start and @end) and  
//		                            cr.ActividadXaml = 'ConfirmacionDeCargaDescarga' and
//		                            R.pesotarafecha is not null and 
//		                            cr.PuestoDeTrabajo_Id is not null AND 
//		                            R.Rechazado = 0 AND 
//		                            R.centro_id = @centroId AND
//                                    R.Material_Id = @materialId
//                            group by cast(r.PesoTaraFecha as date)


//                            ;with amonth(day) as
//                            (
//                                select @start as day
//                                    union all
//                                select day + 1
//                                    from amonth
//                                    where day < @end
//                            )
//                             select CONVERT(varchar, amonth.day, 103) as Fecha, ISNULL(sum(#pesoPorFecha.sumaPeso),0) as Toneladas
//                                from amonth 
//                                left join #pesoPorFecha on #pesoPorFecha.fechaPeso = amonth.day
//                            group by amonth.day
//                                        ";