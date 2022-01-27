using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRegistroInactividad : ProcesadorModificar<ModificarRegistroInactividad>
    {
        public ProcesadorModificarRegistroInactividad(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarRegistroInactividad comando)
        {
            var registro = Repositorio.Obtener<RegistroInactividad>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, registro);
        }

        protected override void Validar(ModificarRegistroInactividad comando, Resultado resultado)
        {
            if (!Repositorio.Existe<RegistroInactividad>(e => e.Id == comando.Dto.Id))
            {
                resultado.Error("RegistroInactividadId", string.Format(Textos.Error_Generico));
            }
        }
    }
}
