using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class EficienciaHidraulicasConsulta : IConsultaEscalar<GraficoEficienciaHidraulicasDto>
    {
        private readonly int centroId;
        private readonly string codigoHidraulicaVagon;
        private readonly int materialId;
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;

        private GraficoEficienciaHidraulicasDto modelo;

        public EficienciaHidraulicasConsulta(GraficoEficienciaHidraulicasDto model, int centroId, string codigoHidraulicaVagon)
        {
            modelo = model;
            fechaDesde = model.FechaHoraDesde;
            fechaHasta = model.FechaHoraHasta;
            this.centroId = centroId;
            this.codigoHidraulicaVagon = codigoHidraulicaVagon;
            materialId = model.MaterialId;
        }

        public GraficoEficienciaHidraulicasDto Ejecutar(DbContext contexto)
        {
            var consulta = @"select count(lah.PuestoDeTrabajo_Id) as Valor, ISNULL(sum(lah.PesoTara), 0 ) as Toneladas, p.Codigo as Clave, M.Descripcion as Material from PuestosDeCargaDescarga p WITH (NOLOCK)
				left join (
				select R.Material_Id as Material, max(cr.PuestoDeTrabajo_Id) as PuestoDeTrabajo_Id, SUM (r.PesoTara) / 1000 as PesoTara 
				from Recorrido R WITH (NOLOCK)
						   inner join ControlRecorrido cr WITH (NOLOCK) on R.instanciaworkflow = cr.WorkflowInstanceId
						   where R.pesotarafecha is not null and cr.ActividadXaml = 'ConfirmacionDeCargaDescarga' and r.TipoVehiculo <> 1
						   and @fechaDesde <= R.pesotarafecha and @fechaHasta >= R.pesotarafecha and cr.PuestoDeTrabajo_Id is not null
								AND R.Rechazado = 0 AND R.centro_id = @centroId and ( @materialId = 0 OR R.Material_Id = @materialId )
								group by R.instanciaworkflow, R.Material_Id
							) as lah
						on lah.PuestoDeTrabajo_Id = p.PuestoDeTrabajo_Id and p.Codigo <> @hidraulicaVagon
				left join Material m ON m.Id = lah.Material
				where p.Codigo <> @hidraulicaVagon
				group by p.Codigo, M.Descripcion
                union
                select count(lah.PuestoDeTrabajo_Id) as Valor, ISNULL(sum(lah.PesoTara), 0 ) as Toneladas, p.Codigo as Clave, M.Descripcion as Material from PuestosDeCargaDescarga p WITH (NOLOCK)
				left join (
				select R.Material_Id as Material, max(cr.PuestoDeTrabajo_Id) as PuestoDeTrabajo_Id, SUM (r.PesoTara) / 1000 as PesoTara 
				from Recorrido R WITH (NOLOCK)
						   inner join ControlRecorrido cr WITH (NOLOCK) on R.instanciaworkflow = cr.WorkflowInstanceId
						   where R.pesotarafecha is not null and cr.ActividadXaml = 'PesadaTara' and r.TipoVehiculo = 1
						   and @fechaDesde <= R.pesotarafecha and @fechaHasta >= R.pesotarafecha and cr.PuestoDeTrabajo_Id is not null
								AND R.Rechazado = 0 AND R.centro_id = @centroId and ( @materialId = 0 OR R.Material_Id = @materialId )
								group by R.instanciaworkflow, R.Material_Id
							) as lah
						on p.Codigo = @hidraulicaVagon
				left join Material m ON m.Id = lah.Material
				where p.Codigo = @hidraulicaVagon
				group by p.Codigo, M.Descripcion
                order by p.Codigo";

            var query = contexto.Database.SqlQuery<ClaveStringValorMaterialDto>(consulta,
                new SqlParameter("@materialId", materialId),
                new SqlParameter("@centroId", centroId),
                new SqlParameter("@hidraulicaVagon", codigoHidraulicaVagon),
                new SqlParameter("@fechaDesde", fechaDesde),
                new SqlParameter("@fechaHasta", fechaHasta));

            modelo.CamionesPorHidraulicaMaterial = query.ToList();
            return modelo;
        }
    }
}
