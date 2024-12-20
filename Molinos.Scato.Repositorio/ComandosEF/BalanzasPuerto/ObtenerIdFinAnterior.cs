using Molinos.Scato.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Repositorio.ComandosEF.BalanzasPuerto
{
    public class ObtenerIdFinAnterior : IComando<int>
    {
        private readonly int idOffset;
        private readonly string numeroBalanza;

        public ObtenerIdFinAnterior(int idOffset, string numeroBalanza)
        {
            this.idOffset = idOffset;
            this.numeroBalanza = numeroBalanza;
        }

        public int Ejecutar(DbContext contexto)
        {
            return contexto.Database.SqlQuery<int>(@"
                SELECT TOP 1 C.Id FROM dbo.Carga C
                INNER JOIN dbo.RegistroBalanzaPuerto R
                    ON C.Id = R.Id AND C.NumeroBalanza = R.NumeroBalanza
                WHERE C.NumeroBalanza = @NumeroBalanza
                AND C.Id < @IdOffset
                AND R.Tipo = 'fin'
                ORDER BY C.Id DESC"
                , new SqlParameter("@IdOffset", idOffset)
                , new SqlParameter("@NumeroBalanza", numeroBalanza)
            ).FirstOrDefault();
        }
    }
}
