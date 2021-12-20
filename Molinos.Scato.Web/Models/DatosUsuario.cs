using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Web.Models
{
    public class DatosUsuario
    {
        public string NombreUsuario { set; get; }
        public int CentroId { set; get; }
        public string CentroDescripcion { set; get; }
        public string CentroCodigoSap { set; get; }
        public int BalanzaId { set; get; }
        public IEnumerable<string> Grupo { set; get; }
        public string GrupoUsuario { get { return Grupo != null ? String.Join(",",Grupo.Select( x => CentroId + "|" + x).ToArray()) : ""; } }
        public int PuestoDeTrabajoId { get; set; }
        public string NombrePc { get; set; }
        public bool RedireccionarAListaAutomatizada { get; set; }
        public bool RedireccionarABalanzaAutomatizada { get; set; }
    }
}