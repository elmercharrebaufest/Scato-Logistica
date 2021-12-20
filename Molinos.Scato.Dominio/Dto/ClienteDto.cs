namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ClienteDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Cuit { get; set; }
        public string CodigoSap { get; set; }
        public bool Activo { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public bool Bloqueado { get; set; }
    }
}
 