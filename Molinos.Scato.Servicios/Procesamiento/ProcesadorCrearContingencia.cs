using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearContingencia : ProcesadorCrear<CrearContingencia, Contingencia>
    {
        public ProcesadorCrearContingencia(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Contingencia CrearEntidad(CrearContingencia comando)
        {
            var contingencia = Conversor.Convertir<ContingenciaDto, Contingencia>(comando.Dto);
            return contingencia;
        }

        protected override void Validar(CrearContingencia comando, Resultado resultado)
        {
            
        }
    }
}
