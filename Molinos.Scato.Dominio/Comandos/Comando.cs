using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    [KnownType("TiposDeComandos")]
    public abstract class Comando
    {
        [DataMember]
        public string Usuario { get; set; }
        /// <summary>
        /// Este metodo es para que cuando se exponga la clase Comando por WCF, se expongan tambien todas las subclases
        /// </summary>
        /// <returns>La lista de subclases de Comando que hay en el assembly</returns>
        public static Type[] TiposDeComandos()
        {
            var tipoComando = typeof (Comando);
            return tipoComando.Assembly.GetTypes().Where(tipoComando.IsAssignableFrom).ToArray();
        }

    }
}
