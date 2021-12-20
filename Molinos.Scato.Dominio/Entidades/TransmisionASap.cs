using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TransmisionASap : ITipeable
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        public virtual Guid InstanciaWorkflow { get; set; }

        [Required]
        public virtual EstadoTransmisionASap Estado { get; set; }

        [Required]
        public virtual DateTime Fecha { get; set; }

        [Required]
        public virtual FuncionSAP FuncionSap { get; set; }

        public virtual string MensajeError { get; set; }

        public virtual Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}