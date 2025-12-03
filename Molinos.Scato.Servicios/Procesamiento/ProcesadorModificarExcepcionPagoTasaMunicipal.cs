using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarExcepcionPagoTasaMunicipal : ProcesadorModificar<ModificarExcepcionPagoTasaMunicipal>
    {
        public ProcesadorModificarExcepcionPagoTasaMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarExcepcionPagoTasaMunicipal comando)
        {
            var excepcion = Repositorio.Obtener<ExceptuadosTicketMunicipal>(comando.Id);

            if (!string.IsNullOrWhiteSpace(comando.Patente) && excepcion.Patente != comando.Patente)
                excepcion.Patente = comando.Patente;

            if (comando.WorkflowInstanceId.HasValue &&
                comando.WorkflowInstanceId.Value != Guid.Empty &&
                comando.WorkflowInstanceId != excepcion.WorkflowInstanceId)
            {
                excepcion.WorkflowInstanceId = comando.WorkflowInstanceId.Value;
            }
        }

        protected override void Validar(ModificarExcepcionPagoTasaMunicipal comando, Resultado resultado)
        {
            if (!Repositorio.Existe<ExceptuadosTicketMunicipal>(e => e.Id == comando.Id))
                resultado.Error("ExcepcionNoEncontrada", "Excepción no encontrada");
            else if(Repositorio.Existe<ExceptuadosTicketMunicipal>(e => e.Id == comando.Id && e.WorkflowInstanceId.HasValue))
                resultado.Error("ExcepcionAsociadaARecorrido", "No se puede modificar una excepción con asociada a un recorrido");
            else if (Repositorio.Existe<ExceptuadosTicketMunicipal>(e => e.Patente == comando.Patente && !e.WorkflowInstanceId.HasValue && e.Id != comando.Id))
                resultado.Error("Patente", "Ya existe una excepción para la patente indicada");
        }
    }
}
