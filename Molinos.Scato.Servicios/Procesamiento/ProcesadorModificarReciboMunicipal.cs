using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarReciboMunicipal : ProcesadorModificar<ModificarReciboMunicipal>
    {
        public ProcesadorModificarReciboMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarReciboMunicipal comando)
        {
            var reciboMunicipal = Repositorio.Obtener<ReciboMunicipal>(comando.Dto.Id);
            reciboMunicipal.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            Conversor.Convertir(comando.Dto, reciboMunicipal);
        }

        protected override void Validar(ModificarReciboMunicipal comando, Resultado resultado)
        {
        }
    }
}
