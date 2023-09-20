namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ProveedorInfoDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Cuil { get; set; }

        public string CodigoSap { get; set; }

        public string RazonSocial { get; set; }
    }
}
 