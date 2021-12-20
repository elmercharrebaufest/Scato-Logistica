
namespace Molinos.Scato.Dominio.Dto
{
    public class EstadoMaterialDto
    {
        public int Id { get; set; }
        public int CamionesEnElDia { get; set; }
        public int CamionesEnPlanta { get; set; }
        public int TotalIngresosEnElDia { get; set; }
        public int Rechazados { get; set; }
        public int Peso { get; set; }
        public string Material { get; set; }

        public int PorcentajeEnElDia { get {return TotalIngresosEnElDia > 0 ? CamionesEnElDia * 100 / TotalIngresosEnElDia : 0; } }
        public int PorcentajeEnPlanta { get { return TotalIngresosEnElDia > 0 ? CamionesEnPlanta * 100 / TotalIngresosEnElDia : 0; } }

    }
}
