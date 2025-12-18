using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorLiberarPagoTasaMunicipal : ProcesadorModificar<LiberarPagoTasaMunicipal>
    {
        public ProcesadorLiberarPagoTasaMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(LiberarPagoTasaMunicipal comando)
        {
            var tasaMunicipal = Repositorio.Obtener<PagosTasaMunicipal>(comando.PagoId);

                tasaMunicipal.IdInstance = null;
                tasaMunicipal.Disponible = true;
        }

        protected override void Validar(LiberarPagoTasaMunicipal comando, Resultado resultado)
        {
            if(!Repositorio.Existe<PagosTasaMunicipal>(c=> c.Id == comando.PagoId))
            {
                resultado.Errores.Add(nameof(PagosTasaMunicipal) , "No se encontró el pago de la tasa municipal.");
            }

            if (comando.InstanceId == null)
            {
                resultado.Errores.Add(nameof(comando.InstanceId), $"{comando.InstanceId} no púede ser null.");
            }

        }
    }
}
