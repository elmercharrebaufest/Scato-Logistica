using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarComoUsadoPagosTasaMunicipal : ProcesadorModificar<ModificarComoUsadoPagosTasaMunicipal>
    {
        public ProcesadorModificarComoUsadoPagosTasaMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarComoUsadoPagosTasaMunicipal comando)
        {
            var tasaMunicipal = Repositorio.Obtener<PagosTasaMunicipal>(comando.PagoId);

                tasaMunicipal.IdInstance = comando.InstanceId;
                tasaMunicipal.Disponible = false;

            if(comando.DiferenciaPagoId > 0)
            {
                var diferenciaPago = Repositorio.Obtener<PagosTasaMunicipal>(comando.DiferenciaPagoId);
                diferenciaPago.IdInstance = comando.InstanceId;
                diferenciaPago.Disponible = false;
            }
        }

        protected override void Validar(ModificarComoUsadoPagosTasaMunicipal comando, Resultado resultado)
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
