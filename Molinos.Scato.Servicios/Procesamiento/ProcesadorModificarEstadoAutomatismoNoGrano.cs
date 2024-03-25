using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarEstadoAutomatismoNoGrano : ProcesadorModificar<ModificarEstadoAutomatismoNoGrano>
    {
        public ProcesadorModificarEstadoAutomatismoNoGrano(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarEstadoAutomatismoNoGrano comando)
        {
            var automatismo = Repositorio.Obtener<AutomatismoNoGrano>(comando.Id);
            automatismo.Activo = comando.Estado;
        }

        protected override void Validar(ModificarEstadoAutomatismoNoGrano comando, Resultado resultado)
        {
            if (comando.Estado)
            {
                var automatismo = Repositorio.Obtener<AutomatismoNoGrano>(comando.Id);
                if (!automatismo.CallePlanta.ActivoAutomatico)
                    resultado.Error(string.Empty, Textos.AutomatismoNoGrano_CalleDesactivada);

                if (!automatismo.PuntoDeCarga.EstadoAutomatismo.GetValueOrDefault())
                    resultado.Error(string.Empty, Textos.AutomatismoNoGrano_PuntoDeCargaDesactivado);

                if (!automatismo.Almacen.EstadoAutomatismo.GetValueOrDefault())
                    resultado.Error(string.Empty, Textos.AutomatismoNoGrano_AlmacenDesactivado);
            }
        }
    }
}