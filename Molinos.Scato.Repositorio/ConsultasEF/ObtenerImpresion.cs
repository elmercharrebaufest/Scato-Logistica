using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerImpresion : IConsultaEscalar<Impresion>
    {
        private readonly int id;

        public ObtenerImpresion(int id)
        {
            this.id = id;
        }


        public virtual Impresion Ejecutar(DbContext contexto)
        {

            var impresion = contexto.Database.SqlQuery<ImpresionDto>(
                 "Select * from Impresion where Impresion.Id = @id"
                 , new SqlParameter("@id", id)).First();

            var tabla = "Imp" + impresion.TipoImpresion.ToString();
            var type = Type.GetType(typeof(Impresion).AssemblyQualifiedName.Replace("Impresion", tabla));
            var tabla2 = tabla;
            if (tabla == "ImpImpresionGenerica")
            {
                tabla2 = "ImpImpresion";
            }

            return typeof(ObtenerImpresion).GetMethod("Obtener")
                .MakeGenericMethod(type).Invoke(null, new object[] { contexto, id, tabla2 }) as Impresion;
        }

        public static Impresion Obtener<T>(DbContext contexto, int id, string tabla) where T : class
        {
            return contexto.Set<T>().SqlQuery(
                             $"SELECT * FROM Impresion R INNER JOIN { tabla} imp on imp.id = R.id where R.id = @idd "
                             , new SqlParameter("@idd", id)).First() as Impresion;
        }
    }
}
