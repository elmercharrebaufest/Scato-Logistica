using Molinos.Scato.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Molinos.Scato.Repositorio.ComandosEF
{

    /// <summary>
    /// Obtiene el primer Id consecutivo antes de un hueco entre el desde y el hasta.
    /// Ej [1, 2, 3, 5, 6] al faltar el 4, el ultimo valido es el 3
    /// En caso de no encontrar huecos, devuelve el numero id "hasta"
    /// </summary>
    public class ObtenerUltimoIdValidoBalanza : IComando<int>
    {
        private readonly int desde;
        private readonly int hasta;
        private readonly string numeroBalanza;

        public ObtenerUltimoIdValidoBalanza(int desde, int hasta, string numeroBalanza)
        {
            this.desde = desde;
            this.hasta = hasta;
            this.numeroBalanza = numeroBalanza;
        }

        public int Ejecutar(DbContext contexto)
        {
            return contexto.Database.SqlQuery<int>(@"
                SELECT MIN(Id) FROM (
	                SELECT Id, LEAD(Id, 1, Id) OVER(ORDER BY id) AS Next_Id FROM RegistroBalanzaPuerto
	                WHERE NumeroBalanza = @NumeroBalanza AND Id >= @Desde AND Id <= @Hasta
                ) AS A
                WHERE Next_Id - Id > 1 OR Next_Id = Id"
                , new SqlParameter("@Desde", desde)
                , new SqlParameter("@Hasta", hasta)
                , new SqlParameter("@NumeroBalanza", numeroBalanza)
            ).FirstOrDefault();
        }
    }
}
