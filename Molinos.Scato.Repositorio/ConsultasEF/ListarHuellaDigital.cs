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
    public class ListarHuellaDigital : IConsulta<HuellaDigitalOrdenDto>
    {
        public ListarHuellaDigital()
        {
            
        }

        private List<HuellaDigitalOrdenDto> ListadoValoresHuellaDigital(DbContext contexto)
        {

            var listaHuellas = ObtenerVistaUnionHuellaDigitalRecorrido(contexto);

            return listaHuellas;
        }

        public virtual List<HuellaDigitalOrdenDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return ListadoValoresHuellaDigital(contexto);
            }
        }

        private List<HuellaDigitalOrdenDto> ObtenerVistaUnionHuellaDigitalRecorrido(DbContext contexto)
        {
            var sqlQuery = @"SELECT * FROM dbo.HuellaDigitalOrden";
            var sqlEjecucion = contexto.Database.SqlQuery<HuellaDigitalOrdenDto>(sqlQuery);

            var sqlResult = sqlEjecucion.OrderByDescending(o=> o.FechaHoraPesaje).ToList();
            return sqlResult;
        }
    }
}