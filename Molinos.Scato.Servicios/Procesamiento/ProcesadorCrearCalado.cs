using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{

    public class ProcesadorCrearCalado : ProcesadorCrear<CrearCalado, Calado>
    {
        public ProcesadorCrearCalado(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Calado CrearEntidad(CrearCalado comando)
        {
            var calado = Conversor.Convertir<CaladoDto, Calado>(comando.Dto);
            calado.WorkflowInstanceId = comando.Dto.WorkflowInstanceId;
            calado.NumeroOrden = "";
            return calado;
        }

        protected override void Validar(CrearCalado comando, Resultado resultado)
        {
        }
    }
}