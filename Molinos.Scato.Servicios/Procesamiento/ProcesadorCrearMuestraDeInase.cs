using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearMuestraDeInase : ProcesadorCrear<CrearMuestraDeInase, MuestraDeInase>
    {
        public ProcesadorCrearMuestraDeInase(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override MuestraDeInase CrearEntidad(CrearMuestraDeInase comando)
        {
            var muestra = new MuestraDeInase()
            {
                Recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.Dto.WorkflowInstanceId),
                FechaMuestra = System.DateTime.Now,
                MuestraEnviada = false
            };
            return muestra;
        }

        protected override void Validar(CrearMuestraDeInase comando, Resultado resultado)
        {
        }
    }
}
