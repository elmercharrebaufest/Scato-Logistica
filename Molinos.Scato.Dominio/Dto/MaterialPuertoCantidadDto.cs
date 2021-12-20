namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MaterialPuertoCantidadDto
    {
        public int MaterialId { get; set; }
        
        public int Cantidad { get; set; }

        public string DescripcionCorta { get; set; }
        public bool EsLiquido { get; set; }
        public string Color { get; set; }

        public override string ToString()
        {
            return $"{Cantidad} {DescripcionCorta}";
        }
    }
}
