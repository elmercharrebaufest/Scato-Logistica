using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarExcepcionPagoTasaMunicipal : ProcesadorEliminar<EliminarExcepcionPagoTasaMunicipal, ExceptuadosTicketMunicipal>
    {
        public ProcesadorEliminarExcepcionPagoTasaMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarExcepcionPagoTasaMunicipal comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarExcepcionPagoTasaMunicipal comando, Resultado resultado)
        {

        }
    }
}
