using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCarga : ProcesadorModificar<ModificarCarga>
    {
        public ProcesadorModificarCarga(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCarga comando)
        {
            var carga = Repositorio.Obtener<Carga>(e => comando.Dto.Id == e.Id && e.NumeroBalanza == comando.Dto.NumeroBalanza);
            Conversor.Convertir(comando.Dto, carga);
        }

        protected override void Validar(ModificarCarga comando, Resultado resultado)
        {
            if (Repositorio.Obtener<Carga>(e => e.Id == comando.Dto.Id && e.NumeroBalanza == comando.Dto.NumeroBalanza) == null)
            {
                resultado.Error("ModificarCarga", string.Format(Textos.Error_ActualizarGenerico));
            }
        }
    }
}
