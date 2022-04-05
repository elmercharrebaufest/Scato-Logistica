using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarEficienciaCalado : IConsulta<EficienciaCaladoValoresDto>
    {
        private DateTime desde;
        private DateTime hasta;

        public ListarEficienciaCalado(DateTime desde, DateTime hasta)
        {
            this.desde = desde;
            this.hasta = hasta;
        }

        private List<EficienciaCaladoValoresDto> ListadoValoresCalado(DbContext contexto)
        {
            var calles = contexto.Set<Calle>().Where(c => c.TipoCalle == TipoCalle.Calado).Select(c => c.Nombre).ToList();
            var puestos = contexto.Set<PuestoDeTrabajo>().Where(p => calles.Any(c => p.NombrePuesto.StartsWith(c))).ToList();
            var idsPuestos = puestos.Select(p => p.Id).ToList();
            var ids = string.Join(",", idsPuestos);
           
            var listaCaladosPorCalle = ObtenerCamionesCaladoPorPuestoDeTrabajo(contexto, this.desde, this.hasta, ids);

            foreach (var caladoPorCalle in listaCaladosPorCalle)
            {
                var index = caladoPorCalle.Nombre.IndexOf("-");
                caladoPorCalle.Nombre = caladoPorCalle.Nombre.Substring(index + 1, caladoPorCalle.Nombre.Length - index - 1).Trim();
            }

            return listaCaladosPorCalle;
        }

        public virtual List<EficienciaCaladoValoresDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return ListadoValoresCalado(contexto);
            }
        }

        private List<EficienciaCaladoValoresDto> ObtenerCamionesCaladoPorPuestoDeTrabajo(DbContext contexto, DateTime desde, DateTime hasta, string ids)
        {
            var sqlQuery = @"SELECT pt.Id as 'Id', COUNT(llt.PuestoDeTrabajo_Id) as 'Cantidad', pt.NombrePuesto as 'Nombre' FROM LogLecturaDeTarjeta llt
                                INNER JOIN PuestoDeTrabajo pt
                                ON llt.PuestoDeTrabajo_Id = pt.Id
                                INNER JOIN (SELECT Patente, MAX(Fecha) as 'Fecha'
				                                FROM LogLecturaDeTarjeta 
				                                WHERE PuestoDeTrabajo_Id IN (" + ids + @") AND Fecha BETWEEN @Desde AND @Hasta
				                                GROUP BY Patente) AS X
                                ON llt.Patente = X.Patente AND llt.Fecha = X.Fecha
                                GROUP BY pt.NombrePuesto, pt.Id";
            var sqlEjecucion = contexto.Database.SqlQuery<EficienciaCaladoValoresDto>(sqlQuery,
                new SqlParameter("@Desde", desde),
                new SqlParameter("@Hasta", hasta));

            var sqlResult = sqlEjecucion.ToList();
            return sqlResult;
        }
    }
}