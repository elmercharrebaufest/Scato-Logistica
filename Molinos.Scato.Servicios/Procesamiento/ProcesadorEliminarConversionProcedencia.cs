using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarConversionProcedencia : ProcesadorEliminar<EliminarConversionProcedencia, ConversionProcedencia>
    {
        public ProcesadorEliminarConversionProcedencia(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarConversionProcedencia comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarConversionProcedencia comando, Resultado resultado)
        {
        }
    }
}