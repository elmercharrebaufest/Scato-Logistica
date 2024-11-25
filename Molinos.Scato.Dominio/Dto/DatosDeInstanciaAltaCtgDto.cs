namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DatosDeInstanciaAltaCTGDto
    {
        public int Id { get; set; }
        public int WorkflowDefinicionId { get; set; }
        public string WorkflowCodigo { get; set; }
        public bool SolicitaConfirmarCTG { get; set; }
        public string CodigoTipoVariedad { get; set; }
    }
}