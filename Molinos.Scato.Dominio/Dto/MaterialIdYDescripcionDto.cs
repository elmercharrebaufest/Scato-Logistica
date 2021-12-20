namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MaterialIdYDescripcionDto
    {
        public int MaterialId { get; set; }

        public string Descripcion { get; set; }

        public string DescripcionWebMobile { get; set; }

        public int CentroId { get; set; }
    }
}