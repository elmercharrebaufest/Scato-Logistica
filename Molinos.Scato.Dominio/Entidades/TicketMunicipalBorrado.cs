using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TicketMunicipalBorrado : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual int CentroId { get; set; }
        public virtual int MaterialId { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string NroTarjeta { get; set; }
        public virtual string Patente { get; set; }
        public virtual TipoDocumentoIngreso TipoDocumento { get; set; }
        public virtual string NroDocumento { get; set; }
        public virtual string ChoferNombre { get; set; }
        public virtual string ChoferCuil { get; set; }
        public virtual string Recibo { get; set; }
    }
}
