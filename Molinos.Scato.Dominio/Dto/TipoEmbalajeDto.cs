namespace Molinos.Scato.Dominio.Dto
{
    public sealed class TipoEmbalajeDto
    {
        public  int Id { get;  set; }
        public  string Codigo { get; set; }
        public  string Descripcion { get; set; }
        public  bool Activo { get; set; }
    }
}