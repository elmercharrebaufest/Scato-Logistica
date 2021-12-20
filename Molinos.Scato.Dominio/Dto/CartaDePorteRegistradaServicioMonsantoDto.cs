namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CartaDePorteRegistradaServicioMonsantoDto
    {
        public int Id { get; set; }
        public int RecorridoId { get; set; }
        public string TipoAnalisis { get; set; }
        public string LaboratorioRazonSocial { get; set; }
        public string LaboratorioCuit { get; set; }
    }
}