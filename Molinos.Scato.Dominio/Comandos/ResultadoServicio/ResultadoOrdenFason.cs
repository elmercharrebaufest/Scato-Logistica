using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos.ResultadoServicio
{
    public class ResultadoOrdenFason : Resultado
    {
        public OrdenCargaInternaFasonDto Dto { get; set; }
        public ControlRecorridoDto ControlRecorrido { get; set; }
    }
}
