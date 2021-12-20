using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class KgEPAUtilizados : IConsultaEscalar<decimal>
    {
        private readonly string codigoEstablecimiento;
        private readonly string cosecha;
        public KgEPAUtilizados(string codigoEstablecimiento, string cosecha)
        {
            this.codigoEstablecimiento = codigoEstablecimiento;
            this.cosecha = cosecha;
        }

        public decimal Ejecutar(DbContext contexto)
        {
            //var resultado = contexto.Set<Recorrido>().Where(x => x.Establecimiento.CodigoDeEstablecimiento == codigoEstablecimiento &&  !x.Rechazado).Join();
            //return resultado.Any() ? resultado.Sum(x => x.Recorrido.PesoBruto.HasValue && x.Recorrido.PesoTaraBodega.HasValue ? (long)(x.Recorrido.PesoBruto.Value - x.Recorrido.PesoTaraBodega.Value) : 0) : 0;
            return 0;
        }
    }
}
