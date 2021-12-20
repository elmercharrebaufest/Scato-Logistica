
namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DatosCaladoDto
    {
        public bool Rechazado { get; set; }
        public bool EnAnalisis { get; set; }
        public bool Recalado { get; set; }
        public string Calador { get; set; }
        public string Material { get; set; }
    }
}
