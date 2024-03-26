using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarEstadoCallePlantaNoGrano : ProcesadorModificar<ModificarEstadoCallePlantaNoGrano>
    {
        public ProcesadorModificarEstadoCallePlantaNoGrano(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarEstadoCallePlantaNoGrano comando)
        {
            var calle = Repositorio.Obtener<Calle>(comando.Id);
            calle.ActivoAutomatico = comando.ActivoAutomatico;
        }

        protected override void Validar(ModificarEstadoCallePlantaNoGrano comando, Resultado resultado)
        {
            var configuracion = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.TableroComandoPuerto && x.Nombre == Constantes.ConfiguracionGeneral.LlamadoAutomatico.NoGranos);
            if (!comando.ActivoAutomatico && configuracion.Valor.Equals("True") && Repositorio.Existe<AutomatismoNoGrano>(a => a.CallePlanta.Id == comando.Id && a.Activo))
            {
                resultado.Error("PreBalanza", Textos.Automatismo_CalleUtilizadaEnAutomatismoActivo);
            }
        }
    }
}