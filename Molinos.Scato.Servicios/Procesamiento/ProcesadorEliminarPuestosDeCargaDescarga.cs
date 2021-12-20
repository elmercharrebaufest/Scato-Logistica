using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarPuestosDeCargaDescarga : ProcesadorEliminar<EliminarPuestosDeCargaDescarga, PuestosDeCargaDescarga>
    {
        public ProcesadorEliminarPuestosDeCargaDescarga(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarPuestosDeCargaDescarga comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarPuestosDeCargaDescarga comando, Resultado resultado)
        {
        }
    }
}
