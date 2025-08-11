using System;
using System.Linq;
using System.Runtime.Serialization;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    [KnownType("TiposDeDto")]
    public abstract class ComandoImpresion : Comando
    {
        [DataMember]
        public IDtoConCentroIdMaterialIdWorkflowId Dto { get; set; }

        [DataMember]
        public int CantidadCopias { get; set; }

        /// <summary>
        /// WCF llamará a este método para registrar todos los tipos concretos de IDtoConCentroIdMaterialIdWorkflowId como KnownType.
        /// </summary>
        /// <returns>Lista de tipos concretos que implementan IDtoConCentroIdMaterialIdWorkflowId</returns>
        public static Type[] TiposDeDto()
        {
            var tipoDto = typeof(IDtoConCentroIdMaterialIdWorkflowId);
            
            return tipoDto.Assembly.GetTypes()
                                   .Where(t => tipoDto.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                                   .ToArray();
        }
    }
}
