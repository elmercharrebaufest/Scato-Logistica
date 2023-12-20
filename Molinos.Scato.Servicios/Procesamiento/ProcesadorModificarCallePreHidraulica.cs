using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCallePreHidraulica : ProcesadorModificar<ModificarCallePreHidraulica>
    {
        public ProcesadorModificarCallePreHidraulica(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCallePreHidraulica comando)
        {
            var calle = Repositorio.Obtener<Calle>(comando.Dto.Id);
            calle.Nombre = comando.Dto.Descripcion;
            calle.CantidadDeCamiones = comando.Dto.Camiones;
            calle.AutomatismoTipoLlamadoId = comando.Dto.AutomatismoTipoLlamadoId;
        }

        protected override void Validar(ModificarCallePreHidraulica comando, Resultado resultado)
        {
            var calle = Repositorio.Obtener<Calle>(comando.Dto.Id);
            bool existeCallePBPI = Repositorio.Existe<CallePreBalanzaPlayaInterna>(c => c.CallePlayaInternaId == calle.Id);
            if (comando.Dto.AutomatismoTipoLlamadoId != calle.AutomatismoTipoLlamadoId && existeCallePBPI != false)
            {
                resultado.Error("AutomatismoTipoLlamado", Dominio.Recursos.Textos.TipoLlamadoNoEditable);
            }
        }
    }
}