namespace Molinos.Scato.Dominio.Dto.OperacionesAPI
{
    public class OrdenDeCargaDto
    {
        public int Id { get; set; }
        public string EstadoDescripcion { get; set; }
        public string FechaCreacion { get; set; }
        public string FechaRetiro { get; set; }
        public int Cantidad { get; set; }
        public string PatenteChasis { get; set; }
        public string PatenteAcoplado { get; set; }
        public string NombreChofer { get; set; }
        public string ApellidoChofer { get; set; }
        public string CUILChofer { get; set; }
        public string RazonSocialTransporte { get; set; }
        public string CUITTransporte { get; set; }
        public string Observacion { get; set; }
        public string RazonSocialCorredor { get; set; }
        public string CUITCorredor { get; set; }
        public string Contrato { get; set; }
        public string Pedido { get; set; }
        public string CUITCliente { get; set; }
        public string TipoOrden { get; set; }
        public string DescripcionProducto { get; set; }
        public string Cliente { get; set; }
        public int LocalidadId { get; set; }
        public string LocalidadDescripcion { get; set; }
        public string CodigoProducto { get; set; }
        public string KmARecorrer { get; set; }
        public bool FleteMOA { get; set; }
        public bool Reventa { get; set; }
        public string PlantaCodigo { get; set; }
        public string DomicilioTipo { get; set; }
        public int DomicilioOrden { get; set; }
        public string DomicilioDescr { get; set; }
        public bool Escalable { get; set; }
        public string CUITDestinatario { get; set; }
        public string RazonSocialDestinatario { get; set; }
        public string CUITDestino { get; set; }
        public string RazonSocialDestino { get; set; }
        public string CUITIntermediarioFlete { get; set; }
        public string RazonSocialIntermediarioFlete { get; set; }
        public string RemitenteComercial { get; set; }
        public long RemitenteComercialId { get; set; }
        public string PagadorFlete { get; set; }
        public string MaterialId { get; set; } 
        public string DestinoMercaderia { get; set; }
    }
}