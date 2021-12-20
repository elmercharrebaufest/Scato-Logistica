using Molinos.Scato.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Molinos.Scato.Repositorio.ComandosEF
{
    public class AsignarBalanzadas : IComando<List<Balanzada>>
    {
        private readonly int cargaInicialId;
        private readonly string cargaInicialNumeroBalanza;
        private readonly int inicio;
        private readonly int fin;

        public AsignarBalanzadas(int cargaInicialId, string cargaInicialNumeroBalanza, int inicio, int fin)
        {
            this.cargaInicialId = cargaInicialId;
            this.cargaInicialNumeroBalanza = cargaInicialNumeroBalanza;
            this.inicio = inicio;
            this.fin = fin;
        }

        public List<Balanzada> Ejecutar(DbContext contexto)
        {
            return contexto.Database.SqlQuery<Balanzada>(@"
                    UPDATE
                        B
                    SET CargaInicial_Id = @cargaInicialId,
                        CargaInicial_NumeroBalanza = @cargaInicialNumeroBalanza
                        output inserted.*,R.*
                    FROM
                        BALANZADA AS B
                        INNER JOIN RegistroBalanzaPuerto AS R
                            ON R.id = B.id AND R.NumeroBalanza = B.NumeroBalanza
                    WHERE @inicio < R.id and (@fin = -1 or R.id < @fin) and R.numeroBalanza = @cargaInicialNumeroBalanza and [CargaInicial_NumeroBalanza] is null
                ", new SqlParameter("@cargaInicialId", cargaInicialId)
                 , new SqlParameter("@cargaInicialNumeroBalanza", cargaInicialNumeroBalanza)
                 , new SqlParameter("@inicio", inicio)
                 , new SqlParameter("@fin", fin)
            ).ToList();
        }
    }
}
