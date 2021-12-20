using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class KgRecibidosPorFincaViñedoAño : IConsultaEscalar<decimal>
    {
        private readonly int vinedoId;
        private readonly int variedadId;
        private readonly string cosecha;
        public KgRecibidosPorFincaViñedoAño(int vinedoId, int variedadId, string cosecha)
        {
            this.vinedoId = vinedoId;
            this.variedadId = variedadId;
            this.cosecha = cosecha;
        }

        public decimal Ejecutar(DbContext contexto)
        {
            var resultado = contexto.Set<RemitoBodegaUva>().Where(x => x.Vinedo.Id == vinedoId && x.Material.Variedad.Id == variedadId && x.Cosecha == cosecha && !x.Recorrido.Rechazado);
            return resultado.Any() ? resultado.Sum(x => x.Recorrido.PesoBruto.HasValue && x.Recorrido.PesoTaraBodega.HasValue ? (long)(x.Recorrido.PesoBruto.Value - x.Recorrido.PesoTaraBodega.Value) : 0) : 0;
        }
    }
}
