using Molinos.Scato.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Molinos.Scato.Repositorio.ComandosEF
{
    /// <summary>
    /// Obtiene el ultimo fin sin CargaOpuesta_Id de la tabla Carga partiendo desde el id indicado
    /// </summary>
    public class ObtenerCargaFin : IComando<Carga>
    {
        private readonly int idOffset;
        private readonly string numeroBalanza;

        public ObtenerCargaFin(int idOffset, string numeroBalanza)
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
                AND R.Tipo = 'fin'
                AND CargaOpuesta_Id IS NULL
                ORDER BY C.Id DESC"
                , new SqlParameter("@IdOffset", idOffset)
                , new SqlParameter("@NumeroBalanza", numeroBalanza)
            ).FirstOrDefault();
        }
    }
}
