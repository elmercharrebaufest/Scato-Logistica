using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos.ResultadoServicio
{
    public class ResultadoOrdenFas : Resultado
    {
        public OrdenCargaFasDto Dto { get; set; }
        public ControlRecorridoDto ControlRecorrido { get; set; }
    }
}
