namespace Molinos.Scato.Dominio.Dto.OperacionesAPI
{
    public class OrdenDeCargaComplementariaDto
    {
        public int? ClienteId { get; set; }
        public string ClienteDescripcion { get; set; }
        public int? TransportistaId { get; set; }
        public string TransportistaDescripcion { get; set; }
        public int TipoDeVehiculo { get; set; }
        public int? MaterialId { get; set; }
    }
}