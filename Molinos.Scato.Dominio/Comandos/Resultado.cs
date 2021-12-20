using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    [KnownType("TiposDeResultados")]
    public class Resultado
    {
        private IDictionary<string, string> errores = new Dictionary<string, string>();

        [DataMember]
        public IDictionary<string, string> Errores 
        {
            get { return errores; }
            private set { errores = value; }
        }

        public bool HayErrores
        {
            get { return Errores.Count != 0; }
        }

        public void Error(string clave, string descripcion)
        {
            errores.Add(clave, descripcion);
        }

        /// <summary>
        /// Este metodo es para que cuando se exponga la clase Resultado por WCF, se expongan tambien todas las subclases
        /// </summary>
        /// <returns>La lista de subclases de Resultado que hay en el assembly</returns>
        public static Type[] TiposDeResultados()
        {
            var tipoResultado = typeof(Resultado);
            return tipoResultado.Assembly.GetTypes().Where(tipoResultado.IsAssignableFrom).ToArray();
        }

    }
}
