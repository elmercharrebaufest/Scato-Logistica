using System;

namespace Molinos.Scato.Dominio.Dto
{
    /// <summary>
    /// Datos completos de un registro LogAvanceManualCamion para la pantalla de Resolución de Contingencia.
    /// </summary>
    public class LogAvanceManualCamionDetalleDto
    {
        public int      Id               { get; set; }
        public string   PatenteLeida     { get; set; }
        public string   PatenteIngresada { get; set; }
        public string   WorkflowNombre   { get; set; }
        public string   Transportista    { get; set; }
        public DateTime FechaEvento      { get; set; }
        public int?     PuestoDeTrabajoId { get; set; }
        public string   NombrePuesto     { get; set; }
        public string   NombreUsuario    { get; set; }
        public string   MotivoFallo      { get; set; }
        /// <summary>0=Pendiente, 1=Editando, 2=Atendido</summary>
        public int      Atendido         { get; set; }
        /// <summary>Número de tarjeta de acceso detectada originalmente.</summary>
        public string   Tarjeta          { get; set; }
        /// <summary>CodigoDispositivo del LogIdentificacionVehicular origen. Es el valor correcto para crear el log destino.</summary>
        public string   CodigoDispositivoOrigen { get; set; }
        public int?     LogIdentificacionVehicularOrigenId { get; set; }
    }
}
