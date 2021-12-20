using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MuestraDeHumedad : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual Guid WorkflowInstanceId { get; set; }

        public virtual Humedimetro Humedimetro { get; set; }

        public virtual string Modalidad { get; set; }

        public virtual string NumeroOrden { get; set; }

        public virtual string NumeroDocumentoIngreso { get; set; }

        public virtual int CicloDeCalado { get; set; }

        public virtual int NroDeToma { get; set; }

        public virtual decimal ValorLeido { get; set; }

        public virtual decimal ValorFinal { get; set; }

        public virtual DateTime Fecha { get; set; }

        public virtual string Usuario { get; set; }

        public virtual Centro Centro { get; set; }
        public MotivoHumedadManual MotivoHumedadManual { get; set; }

        public virtual string Dispositivo { get; set; }
    }
}
