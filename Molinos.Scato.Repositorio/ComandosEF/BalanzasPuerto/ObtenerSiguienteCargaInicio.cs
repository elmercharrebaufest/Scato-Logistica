using Molinos.Scato.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Molinos.Scato.Repositorio.ComandosEF
{
    /// <summary>
    /// Obtiene el primer inicio de la tabla Carga que supere el id indicado
    /// </summary>
    public class ObtenerSiguienteCargaInicio : IComando<Carga>
    {
        private readonly int idOffset;
        private readonly string numeroBalanza;

        public ObtenerSiguienteCargaInicio(int idOffset, string numeroBalanza)
        {
            this.idOffset = idOffset;
            this.numeroBalanza = numeroBalanza;
        }

        public Carga Ejecutar(DbContext contexto)
        {
            return contexto.Set<Carga>().SqlQuery(@"
                SELECT TOP 1 * FROM dbo.Carga C
                INNER JOIN dbo.RegistroBalanzaPuerto R
                    ON C.Id = R.Id AND C.NumeroBalanza = R.NumeroBalanza
                WHERE C.NumeroBalanza = @NumeroBalanza
                AND C.Id > @IdOffset
                AND R.Tipo = 'inicio'
                ORDER BY C.Id ASC"
                , new SqlParameter("@IdOffset", idOffset)
                , new SqlParameter("@NumeroBalanza", numeroBalanza)
            ).FirstOrDefault();
        }
    }
}
