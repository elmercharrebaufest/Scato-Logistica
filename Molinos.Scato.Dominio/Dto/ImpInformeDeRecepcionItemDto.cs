
namespace Molinos.Scato.Dominio.Dto
{
    public class ImpInformeDeRecepcionItemDto
    {
        public int Id { get; set; }
        public ImpInformeDeRecepcionDto ImpInformeDeRecepcion { get; set; }
        public int ItemNro { get; set; }
        public string MaterialCodigo { get; set; }
        public string MaterialDescripcion { get; set; }
        public string UniMed { get; set; }
        public string CantidadDescargada { get; set; }
        public string Remito { get; set; }
    }
}
