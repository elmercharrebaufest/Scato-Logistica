using Molinos.Scato.Dominio.Enums;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class CargaDeCupoResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> ValidationErrors { get; set; }
        public CargaDeCupoDataDto Data { get; set; }

        public CargaDeCupoResponseDto()
        {
            ValidationErrors = new Dictionary<string, string>();
            Data = new CargaDeCupoDataDto();
            Success = true;
        }
    }

    public class CargaDeCupoDataDto
    {
        public string FilaAsignada { get; set; }
        public int? Disponibilidad { get; set; }
        public string MensajeTasaMunicipal { get; set; }
        public TipoAlerta TipoAlertaTasaMunicipal { get; set; }
    }
}
