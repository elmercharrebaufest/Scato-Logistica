using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class LogExceptuadosTicketMunicipalDto
    {
        public Guid InstanceId { get; set; }
        [StringLength(100,ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Motivo { get; set; }
        public DateTime FechaExcepcion { get; set; }
        public string NombreUsuario { get; set; }
        public string MaterialDesc { get; set; }
        public int MaterialId { get; set; }
        public string NumeroDeDocumento { get; set; }
        public string Patente { get; set;}
        public int ChoferId { get; set; }
        public string ChoferNombre { get; set; }
        public DateTime FechaIngreso { get; set; }
        public bool PagaTicketMunicipal { get; set; }
    }
}
