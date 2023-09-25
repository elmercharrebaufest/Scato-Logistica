using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarEstadoEscalableHidraulica : ProcesadorModificar<ModificarEstadoEscalableHidraulica>
    {
        public ProcesadorModificarEstadoEscalableHidraulica(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarEstadoEscalableHidraulica comando)
        {
            var hidraulica = Repositorio.Obtener<PuestosDeCargaDescarga>(comando.Id);
            hidraulica.EsEscalable = comando.EsEscalable;
        }

        protected override void Validar(ModificarEstadoEscalableHidraulica comando, Resultado resultado)
        {
        }
    }
}