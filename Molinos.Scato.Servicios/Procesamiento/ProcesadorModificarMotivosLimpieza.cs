using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarMotivosLimpieza : ProcesadorModificar<ModificarMotivosLimpieza>
    {
        public ProcesadorModificarMotivosLimpieza(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarMotivosLimpieza comando)
        {
            var motivosLimpieza = Repositorio.Obtener<MotivosLimpieza>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, motivosLimpieza);
        }

        protected override void Validar(ModificarMotivosLimpieza comando, Resultado resultado)
        {

        }
    }
}