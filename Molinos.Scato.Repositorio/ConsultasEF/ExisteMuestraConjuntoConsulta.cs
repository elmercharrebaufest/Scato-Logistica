using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;


namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ExisteMuestraConjuntoConsulta : IConsultaEscalar<bool>
    {
        private readonly int proveedorId;
        private readonly int muestraConj;

        public ExisteMuestraConjuntoConsulta(int proveedorId, int muestraConj)
        {
            this.proveedorId = proveedorId;
            this.muestraConj = muestraConj;
        }

        public bool Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;


            var resultado = (from item in contexto.Set<Recorrido>()
                             join r in contexto.Set<Remito>().DefaultIfEmpty() on item.Id equals r.Recorrido.Id into rJoined
                             from r in rJoined.DefaultIfEmpty()
                             where item.Calado.MuestraConjunto == muestraConj && proveedorId != 0 &&
                                    (item.Vehiculo.CartaPorte.TitularCartaPorte.Id != proveedorId || r.ProveedorOrigen.Id != proveedorId)
                             select item.Id);
            return resultado.Any();

        }
    }
}
