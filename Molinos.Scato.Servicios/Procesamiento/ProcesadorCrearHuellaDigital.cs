using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearHuellaDigital : ProcesadorCrear<CrearHuellaDigital, HuellaDigital>
    {
        public ProcesadorCrearHuellaDigital(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override HuellaDigital CrearEntidad(CrearHuellaDigital comando)
        {
            var huellaDigital = Conversor.Convertir<HuellaDigitalDto, HuellaDigital>(comando.Dto);
            return huellaDigital;
        }

        protected override void Validar(CrearHuellaDigital comando, Resultado resultado)
        {

        }
    }
}