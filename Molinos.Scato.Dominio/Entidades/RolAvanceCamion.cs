using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    /// <summary>
    /// Mapeo de rol (grupo AD) a puesto de trabajo para el Panel de Avance Manual.
    /// Un rol puede tener acceso a uno o más puestos; un puesto puede ser atendido por uno o más roles.
    /// </summary>
    [Table("RolAvanceCamion")]
    public class RolAvanceCamion : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        /// <summary>Código del rol/grupo de Active Directory (por ejemplo, "SCATO_OPERADOR_BALANZA").</summary>
        [Required]
        [StringLength(200)]
        public virtual string CodigoRol { get; set; }

        /// <summary>Puesto de trabajo al que tiene acceso este rol.</summary>
        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }
    }
}
