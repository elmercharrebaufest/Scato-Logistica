using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Linq;

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
            var configuracion = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica && x.Nombre == Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos);
            if (!comando.EsEscalable && configuracion.Valor.Equals("True") && Repositorio.Existe<AutomatismoGrano>(x => x.Hidraulicas.Any(z=>z.Id==comando.Id) && x.Activo))
            {
                resultado.Error("Automatismo", Textos.Automatismo_HidraulicaUtilizadaEnAutomatismoActivo);
            }

        }
    }
}