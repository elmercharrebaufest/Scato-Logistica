using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarReciboMunicipal : ProcesadorEliminar<EliminarReciboMunicipal, ReciboMunicipal>
    {
        public ProcesadorEliminarReciboMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarReciboMunicipal comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarReciboMunicipal comando, Resultado resultado)
        {
        }
    }
}
