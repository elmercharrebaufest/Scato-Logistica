namespace Molinos.Scato.Dominio.Dto
{
    public class CargaComercialHistoricoDto
    {
        public int Id { get; set; }
        public ExportadorDto Exportador { get; set; }
        public MaterialPuertoDto MaterialPuerto { get; set; }
        public int Cantidad { get; set; }
    }
}
