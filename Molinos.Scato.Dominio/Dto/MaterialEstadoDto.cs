namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MaterialEstadoDto
    {
        public int MaterialId { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionWebMobile { get; set; }
        public bool EsGrano { get; set; }
        public int? Orden { get; set; }
        public int CentroId { get; set; }

    }
}