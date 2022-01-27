using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearRegistroInactividad : ProcesadorCrear<CrearRegistroInactividad, RegistroInactividad>
    {
        public ProcesadorCrearRegistroInactividad(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override RegistroInactividad CrearEntidad(CrearRegistroInactividad comando)
        {
            var registro = Conversor.Convertir<RegistroInactividadDto, RegistroInactividad>(comando.Dto);
            return registro;
        }

        protected override void Validar(CrearRegistroInactividad comando, Resultado resultado)
        {
        }
    }
}
