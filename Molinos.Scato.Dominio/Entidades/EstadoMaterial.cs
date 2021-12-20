using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class EstadoMaterial
    {
        [Key]
        public int Id { get; set; }
        public int CamionesEnElDia { get; set; }
        public int CamionesEnPlanta { get; set; }
        public int TotalIngresosEnElDia { get; set; }
        public int Rechazados { get; set; }
        public int Peso { get; set; }
        public string Material { get; set; }
        public int CentroId { get; set; }
        public bool EsGrano { get; set; }
        public bool EsIngreso { get; set; }
    }
}
