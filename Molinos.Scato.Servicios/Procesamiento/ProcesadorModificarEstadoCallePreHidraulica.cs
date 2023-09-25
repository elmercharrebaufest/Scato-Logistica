using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarEstadoCallePreHidraulica : ProcesadorModificar<ModificarEstadoCallePreHidraulica>
    {
        public ProcesadorModificarEstadoCallePreHidraulica(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarEstadoCallePreHidraulica comando)
        {
            var calle = Repositorio.Obtener<Calle>(comando.Id);
            calle.ActivoAutomatico = comando.ActivoAutomatico;
        }

        protected override void Validar(ModificarEstadoCallePreHidraulica comando, Resultado resultado)
        {
            if (Repositorio.Existe<AutomatismoGrano>(a => a.CallePreHidraulicaId == comando.Id && a.Activo == true))
            {
                resultado.Error("IdPreHidraulica", Textos.Automatismo_IdCalle);
            }
        }
    }
}