using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarPuestosDeCargaDescarga : ProcesadorModificar<ModificarPuestosDeCargaDescarga>
    {
        public ProcesadorModificarPuestosDeCargaDescarga(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarPuestosDeCargaDescarga comando)
        {
            var hidraulica = Repositorio.Obtener<PuestosDeCargaDescarga>(comando.Dto.Id);
            //Conversor.Convertir(comando.Dto, hidraulica);
            if (hidraulica.PuestoDeTrabajo.Id != comando.Dto.PuestoDeTrabajoId)
            {
                hidraulica.PuestoDeTrabajo = Repositorio.Obtener<PuestoDeTrabajo>(comando.Dto.PuestoDeTrabajoId);
            }
            if (hidraulica.Nombre != comando.Dto.Nombre)
            {
                hidraulica.Nombre = comando.Dto.Nombre;
            }
            hidraulica.EsSojaSustentable = comando.Dto.EsSojaSustentable;
        }

        protected override void Validar(ModificarPuestosDeCargaDescarga comando, Resultado resultado)
        {
        }
    }
}
