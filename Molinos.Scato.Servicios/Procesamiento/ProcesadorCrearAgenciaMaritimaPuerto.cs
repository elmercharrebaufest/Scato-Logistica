using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAgenciaMaritimaPuerto : ProcesadorCrear<CrearAgenciaMaritimaPuerto, AgenciaMaritimaPuerto>
    {
        public ProcesadorCrearAgenciaMaritimaPuerto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override AgenciaMaritimaPuerto CrearEntidad(CrearAgenciaMaritimaPuerto comando)
        {
            return Conversor.Convertir<AgenciaMaritimaPuertoDto, AgenciaMaritimaPuerto>(comando.Dto);
        }

        protected override void Validar(CrearAgenciaMaritimaPuerto comando, Resultado resultado)
        {

        }
    }
}
