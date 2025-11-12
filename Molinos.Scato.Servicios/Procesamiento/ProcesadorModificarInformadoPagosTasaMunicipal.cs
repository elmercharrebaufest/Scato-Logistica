using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarInformadoPagosTasaMunicipal : ProcesadorModificar<ModificarInformadoPagosTasaMunicipal>
    {
        public ProcesadorModificarInformadoPagosTasaMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarInformadoPagosTasaMunicipal comando)
        {
            var recorrido = Repositorio.Obtener<Recorrido>(p => p.InstanciaWorkflow == comando.InstanceId);
            if (recorrido != null)
            {
                recorrido.PagoTasaMunicipalInformado = true;
            }
        }

        protected override void Validar(ModificarInformadoPagosTasaMunicipal comando, Resultado resultado)
        {
            if (comando.InstanceId == null || comando.InstanceId == Guid.Empty)
            {
                resultado.Errores.Add(nameof(comando.InstanceId), $"{comando.InstanceId} no púede ser null o empty.");
            }
        }
    }
}
