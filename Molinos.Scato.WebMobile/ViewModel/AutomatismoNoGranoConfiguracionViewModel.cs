using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.WebMobile.ViewModel
{
    public class AutomatismoNoGranoConfiguracionViewModel
    {
        public AutomatismoNoGranoConfiguracionViewModel()
        {
            PuntoDeCarga = new PuntoDeCargaDto();
            CallePlanta = new CalleDto();
        }
        public bool EstadoGeneralAutomatismoNoGrano { get; set; }
        public PuntoDeCargaDto PuntoDeCarga { get; set; }
        public CalleDto CallePlanta { get; set; }
        public ListaPaginada<CalleDto> ListaCallePlanta { get; set; }
        public ListaPaginada<PuntoDeCargaDto> ListaPuntoDeCarga { get; set; }
        public ListaPaginada<AlmacenDto> ListaAlmacen { get; set; }
    }
}