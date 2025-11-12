using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearCargaDeCupoNoGrano : Comando
    {
        public CargaDeCupoDto Dto { get; set; }
        public int? OrdenOperacionesId { get; set; } 
    }

    public class ResultadoCrearCargaDeCupo : ResultadoCrear
    {
        public bool FastPassValido { get; set; }
        public int FastPassWorkflowDefinicionId { get; set; }
        public OrdenCargaInternaDto OrdenCargaInterna { get; set; }
        public OrdenCargaInternaFasonDto OrdenCargaInternaFason { get; set; }
        public OrdenCargaFasDto OrdenCargaFasDto { get; set; }
        public ControlRecorridoDto ControlRecorrido { get; set; }
        public string MensajeTasaMunicipal { get; set; }
        public TipoAlerta tipoAlerta { get; set; }
        public int? IdPagoMunicipal { get; set; }
        public bool ErroresOExcepcionesConsultaTasaMunicipal { get; set; }
        public int? IdExcepcionPagoMunicipal { get; set; }
    }
}
