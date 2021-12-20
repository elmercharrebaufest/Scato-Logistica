using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearEstiba : ProcesadorCrear<CrearEstiba, Estiba>
    {
        public ProcesadorCrearEstiba(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Estiba CrearEntidad(CrearEstiba comando)
        {
            return Conversor.Convertir<EstibaDto, Estiba>(comando.Dto);
        }

        protected override void Validar(CrearEstiba comando, Resultado resultado)
        {
            
        }
    }
}
