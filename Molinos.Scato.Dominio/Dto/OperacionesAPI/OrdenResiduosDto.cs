namespace Molinos.Scato.Dominio.Dto.OperacionesAPI
{
    public class OrdenResiduosDto
    {
        public int Id { get; set; }
        public int AlmacenId { get; set; }
        public string Cliente { get; set; }
        public int CodigoProducto { get; set; }
        public string CUILChofer { get; set; }
        public string CUITCliente { get; set; }
        public string CUITTransporte { get; set; }
        public string DescripcionProducto { get; set; }
        public string DomicilioDescr { get; set; }
        public int DomicilioOrden { get; set; }
        public string DomicilioTipo { get; set; }
        public string FechaCreacion { get; set; }
        public int LocalidadId { get; set; }
        public string LocalidadDescripcion { get; set; }
        public string NombreChofer { get; set; }
        public string ApellidoChofer { get; set; }
        public string Observacion { get; set; }
        public string PatenteAcoplado { get; set; }
        public string PatenteChasis { get; set; }
        public string PlantaCodigo { get; set; }
        public string RazonSocialTransporte { get; set; }
        public string TipoOrden { get; set; }
        public string KmARecorrer { get; set; }
        public string PagadorFlete  { get; set; }

    }
}