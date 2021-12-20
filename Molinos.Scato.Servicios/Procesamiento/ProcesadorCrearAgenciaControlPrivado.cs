using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAgenciaControlPrivado : ProcesadorCrear<CrearAgenciaControlPrivado, AgenciaControlPrivado>
    {
        public ProcesadorCrearAgenciaControlPrivado(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override AgenciaControlPrivado CrearEntidad(CrearAgenciaControlPrivado comando)
        {
            return Conversor.Convertir<AgenciaControlPrivadoDto, AgenciaControlPrivado>(comando.Dto);
        }

        protected override void Validar(CrearAgenciaControlPrivado comando, Resultado resultado)
        {
            
        }
    }
}
