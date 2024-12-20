using Molinos.Scato.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Molinos.Scato.Repositorio.ComandosEF
{
    /// <summary>
    /// Obtiene el primer inicio de la tabla Carga por debajo del id indicado
    /// </summary>
    public class ObtenerCargaInicio : IComando<Carga>
    {
        private readonly int idOffset;
        private readonly string numeroBalanza;
        private readonly bool sinFin;

        public ObtenerCargaInicio(int idOffset, string numeroBalanza, bool sinFin = false)
        {
            this.idOffset = idOffset;
            this.numeroBalanza = numeroBalanza;
            this.sinFin = sinFin;
        }

        public Carga Ejecutar(DbContext contexto)
        {
            return contexto.Set<Carga>().SqlQuery(@"
                SELECT TOP 1 * FROM dbo.Carga C
                INNER JOIN dbo.RegistroBalanzaPuerto R
                    ON C.Id = R.Id AND C.NumeroBalanza = R.NumeroBalanza
                WHERE C.NumeroBalanza = @NumeroBalanza
                AND C.Id < @IdOffset
                AND R.Tipo = 'inicio'
                AND (@SinFin = 0 OR (@SinFin = 1 AND C.CargaOpuesta_Id IS NULL))
                ORDER BY C.Id DESC"
                , new SqlParameter("@IdOffset", idOffset)
                , new SqlParameter("@NumeroBalanza", numeroBalanza)
                , new SqlParameter("@SinFin", sinFin)
            ).FirstOrDefault();
        }
    }
}
