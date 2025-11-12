using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using NPOI.Util;

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
            var excepcion = comando.Id > 0 ? Repositorio.Obtener<ExceptuadosTicketMunicipal>(comando.Id) : Repositorio.Obtener<ExceptuadosTicketMunicipal>(e => e.Patente == comando.PatenteActual && e.NumeroDocumentoIngreso == comando.NumeroDocumentoIngresoActual && e.WorkflowCodigo == comando.WorkflowModal);
            excepcion.Patente = comando.PatenteActual;
            excepcion.NumeroDocumentoIngreso = comando.NumeroDocumentoIngresoActual;
            excepcion.WorkflowCodigo = comando.WorkflowModal;
            excepcion.WorkflowDescripcion = comando.WorkflowDescripcionModal;
            excepcion.PermiteAcciones = comando.TieneRecorrido;
        }

        protected override void Validar(ModificarExcepcionPagoTasaMunicipal comando, Resultado resultado)
        {
            if (!Repositorio.Existe<ExceptuadosTicketMunicipal>(e => e.Id == comando.Id))
            {
                resultado.Error("Descripcion", "Excepcion no encontrada");
            }
        }
    }
}
