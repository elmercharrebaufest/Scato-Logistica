using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    /// <summary>
    /// Registro de auditoría de cada avance manual de camión ejecutado desde el Panel de Soporte de Sistemas,
    /// y también de vehículos detectados por identificación vehicular sin recorrido activo (pendientes de atención).
    /// </summary>
    public class LogAvanceManualCamion : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        /// <summary>Nombre de usuario del operador de soporte que ejecutó el avance. Null cuando el registro fue generado automáticamente por identificación vehicular.</summary>
        public virtual string NombreUsuario { get; set; }

        /// <summary>Timestamp del evento.</summary>
        public virtual DateTime FechaEvento { get; set; }

        /// <summary>Puesto de trabajo donde se detectó o avanzó el vehículo.</summary>
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }

        /// <summary>Descripción del error; null si no hubo fallo.</summary>
        public virtual string MotivoFallo { get; set; }

        /// <summary>Patente detectada por el lector en el momento del evento.</summary>
        public virtual string PatenteLeida { get; set; }

        /// <summary>Patente ingresada manualmente por el operador durante la resolución de la contingencia. Null hasta que se resuelva.</summary>
        public virtual string PatenteIngresada { get; set; }

        /// <summary>Número de tarjeta de acceso detectada. Poblado cuando el registro proviene de identificación vehicular.</summary>
        public virtual string Tarjeta { get; set; }

        /// <summary>Estado de atención: 0=Pendiente, 1=Editando, 2=Atendido, 4=Liberado manualmente.</summary>
        public virtual int Atendido { get; set; }

        /// <summary>Motivo ingresado por el operador al liberar manualmente la barrera (Atendido=4).</summary>
        public virtual string MotivoLiberar { get; set; }

        /// <summary>Fecha en que el operador marcó el registro como atendido.</summary>
        public virtual DateTime? FechaAtencion { get; set; }

        /// <summary>Registro de LogIdentificacionVehicular que originó este caso (la lectura de patente que disparó la alerta).</summary>
        [Column("LogIdentificacionVehicularId_origen")]
        public virtual int? LogIdentificacionVehicularOrigenId { get; set; }

        [ForeignKey("LogIdentificacionVehicularOrigenId")]
        public virtual LogIdentificacionVehicular LogIdentificacionVehicularOrigen { get; set; }

        /// <summary>Registro de LogIdentificacionVehicular con la patente corregida por el operador (null hasta que se resuelva el caso).</summary>
        [Column("LogIdentificacionVehicularId_Destino")]
        public virtual int? LogIdentificacionVehicularDestinoId { get; set; }

        [ForeignKey("LogIdentificacionVehicularDestinoId")]
        public virtual LogIdentificacionVehicular LogIdentificacionVehicularDestino { get; set; }
    }
}