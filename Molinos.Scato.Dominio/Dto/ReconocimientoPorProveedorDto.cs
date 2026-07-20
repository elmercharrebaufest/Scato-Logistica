namespace Molinos.Scato.Dominio.Dto
{
    public class ReconocimientoPorProveedorDto
    {
        public string ProveedorALPR { get; set; }
        public int TotalIntentos { get; set; }
        public int IntentosExitosos { get; set; }
        public decimal TasaReconocimiento { get; set; }
    }
}
