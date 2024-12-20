using Molinos.Scato.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Molinos.Scato.Repositorio.ComandosEF
{
    /// <summary>
    /// Asigna un id de inicio de carga a las balanzadas entre el inicio y el fin. En caso de que el fin sea -1 lo asigna desde el inicio hasta la ultima.
    /// </summary>
    public class AsignarBalanzadas : IComando<List<Balanzada>>
    {
        private readonly string cargaInicialNumeroBalanza;
        private readonly int inicio;
        private readonly int fin;

        public AsignarBalanzadas(int inicio, string cargaInicialNumeroBalanza, int fin)
        {
            this.cargaInicialNumeroBalanza = cargaInicialNumeroBalanza;
            this.inicio = inicio;
            this.fin = fin;
        }

        public List<Balanzada> Ejecutar(DbContext contexto)
        {
            return contexto.Database.SqlQuery<Balanzada>(@"
                UPDATE B SET 
                    CargaInicial_Id = @inicio,
                    CargaInicial_NumeroBalanza = @cargaInicialNumeroBalanza
                    output inserted.*,R.*
                FROM BALANZADA AS B
                INNER JOIN RegistroBalanzaPuerto AS R ON R.id = B.id AND R.NumeroBalanza = B.NumeroBalanza
                WHERE R.id > @inicio 
                    AND (@fin = -1 or R.id < @fin)
                    AND R.numeroBalanza = @cargaInicialNumeroBalanza and [CargaInicial_NumeroBalanza] is null"
                , new SqlParameter("@inicio", inicio)
                , new SqlParameter("@cargaInicialNumeroBalanza", cargaInicialNumeroBalanza)
                , new SqlParameter("@fin", fin)
            ).ToList();
        }
    }
}
