
namespace Molinos.Scato.Dominio.Dto
{
    public sealed class VerificarKilosDeclaradosPorFincaDto
    {
        public bool ExcedeKilosARecibir { get; set; }
        public bool AvisoDeCorte { get; set; }

        public decimal KilosARecibir { get; set; }
        public string Variedad { get; set; }
        public string Vinedo { get; set; }

        public bool Error { get; set; }
        public string MensajeError { get; set; }
    }
}
