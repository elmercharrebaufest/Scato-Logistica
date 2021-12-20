using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarDetalleDeMovimientoIngreso : IConsulta<DSIngresoDto>
    {
        private readonly int centroId;
        private readonly DateTime fecha;
        //private DSIngresoDto modelo;

        public ListarDetalleDeMovimientoIngreso(int centroId, DateTime fecha)
        {
            this.centroId = centroId;
            this.fecha = fecha;
        }

        public List<DSIngresoDto> Ejecutar(DbContext contexto)
        {

            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var consulta = @"select COALESCE(IngresoxD, m.Descripcion) as IngresoxD,  
                                COALESCE(CantidadxD, 0) as CantidadxD,
                                COALESCE(TonsxD, 0) as TonsxD,
                                COALESCE(CantidadxM, 0) as CantidadxM,
                                COALESCE(TonsxM, 0) as TonsxM,
                                COALESCE(CantidadxA, 0) as CantidadxA,
                                COALESCE(TonsxA, 0) as TonsxA
                                from MaterialReporteDeMovimientos m
                                left join (
                                SELECT CASE 
			                                when (case when r.TipoVehiculo =1 then 1 else 0 end) = 1 then 'Operativos de Vagones'
			                                when m.id = 4 and (case when Establecimiento_Id is null or TipoVehiculo = 1 then 1 else 0 end) = 0 then 'Soja Sustentable'
			                                when m.id = 4 and (case when Establecimiento_Id is null or TipoVehiculo = 1 then 1 else 0 end) = 1 then 'Soja'			
			                                else m.Descripcion end
			                                as 'IngresoxD',
                                 sum(case when r.FechaEgreso >= dateadd(day, datediff(day, 0, @fecha), 0) + '00:00'  and r.FechaEgreso <= dateadd(day, datediff(day, 0, @fecha), 0) + '23:59:59' then 1 else 0 end  ) as 'CantidadxD',
                                cast(((sum(case when r.FechaEgreso >= dateadd(day, datediff(day, 0, @fecha), 0) + '00:00'  and r.FechaEgreso <= dateadd(day, datediff(day, 0, @fecha), 0) + '23:59:59' then cast(r.PesoBruto- PesoTara as numeric(18,2)) else 0 end) )/1000) as numeric(18,2)) AS 'TonsxD',
                                 sum(case when r.FechaEgreso >= dateadd(month,datediff(month,0,@fecha),0) and r.FechaEgreso <= @fecha + '23:59:59' then 1 else 0 end  ) as 'CantidadxM',
                                cast(((sum(case when r.FechaEgreso >= dateadd(month,datediff(month,0,@fecha),0) and r.FechaEgreso <= @fecha + '23:59:59' then cast(r.PesoBruto- PesoTara as numeric(18,2)) else 0 end) )/1000)  as numeric(18,2))  AS 'TonsxM',
                                count(*) as 'CantidadxA',cast( (sum(cast(r.PesoBruto- PesoTara as numeric(18,2)))/1000) as numeric(18,2)) as 'TonsxA'
                                FROM [Recorrido] as r WITH(NOLOCK)
                                join Workflow as w WITH(NOLOCK) on r.Workflow_Id = w.Id 
                                join Material as m WITH(NOLOCK) on m.Id = r.Material_Id and (r.TipoVehiculo <> 1 or (r.TipoVehiculo = 1 and r.Material_Id = 4))

                                where w.TipoDeWorkflow = 0 and r.Centro_Id = @CentroId
                                and r.FechaEgreso is not null and terminado = 1 and Rechazado = 0
                                and ((MONTH(@fecha) > 3 and r.FechaEgreso > DATEFROMPARTS(YEAR(@fecha),4,1)) OR
                                (MONTH(@fecha) <= 3 and r.FechaEgreso >= DATEFROMPARTS(YEAR(@fecha)-1,4,1))
                                and r.FechaEgreso <= @fecha+'23:59:59')

                                group by m.Descripcion,m.Id, case when Establecimiento_Id is null or TipoVehiculo = 1 then 1 else 0 end, case when r.TipoVehiculo =1 then 1 else 0 end
                                ) as Datos on datos.IngresoxD = m.Material
                                where ingreso = 1
                                order by orden";
            var resultados = contexto.Database.SqlQuery<DSIngresoDto>(consulta,
                new SqlParameter("@fecha", fecha),
                new SqlParameter("@CentroId", centroId));

            return new List<DSIngresoDto>(resultados.ToList());

            //var actual = query.ToDictionary(r => r.Clave, r => r.Valor);

            //var salida = new int[31];
            //int contador = 0;

            //for (int i = salida.Length - 1; i >= 0; i--)
            //{
            //    if (actual.ContainsKey(i))
            //    {
            //        salida[contador] = actual[i];
            //    }
            //    contador++;
            //}
            //modelo. = salida;
            //return modelo;
        }
    }
}
