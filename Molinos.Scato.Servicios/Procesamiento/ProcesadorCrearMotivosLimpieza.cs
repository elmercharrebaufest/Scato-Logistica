using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearMotivosLimpieza : ProcesadorCrear<CrearMotivosLimpieza, MotivosLimpieza>
    {
        public ProcesadorCrearMotivosLimpieza(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override MotivosLimpieza CrearEntidad(CrearMotivosLimpieza comando)
        {
            return Conversor.Convertir<MotivosLimpiezaDto, MotivosLimpieza>(comando.Dto);
        }

        protected override void Validar(CrearMotivosLimpieza comando, Resultado resultado)
        {

        }
    }
}