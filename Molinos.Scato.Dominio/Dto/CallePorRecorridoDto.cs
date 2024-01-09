using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CallePorRecorridoDto
    {
        public int Id { get; set; }
        public string Patente { get; set; }
        public int MaterialId { get; set; }
        public string MaterialDesc { get; set; }
        public int CalleId { get; set; }
        public int Calidad { get; set; }
        public DateTime FechaIngeso { get; set; }
        public bool UltimoDeLaFila { get; set; }
        public bool Rechazado { get; set; }
        public bool AsignadoEnPuestoComando { get; set; }
        public TipoCalle TipoCalle { get; set; }
        public bool Escalable { get; set; }
        public string ColorFondo { get; set; }
        public string ColorTexto { get; set; }
        public int? CalleRecorridoId { get; set; }
        public bool EsSojaEPA { get; set; }
        public bool EsSojaIMPO { get; set; }
        public bool EsDemorado { get; set; }
        public int? RecorridoId { get; set; }
        public DateTime? FechaEgreso { get; set; }
    }

    public class CallePorRecorridoListadoCamionesDto
    {
        public int Id { get; set; }
        public TipoCalidad? Calidad { get; set; }
        public int? RecorridoMaterialId { get; set; }
        public int? CargaCupoMaterialId { get; set; }
        public string RecorridoMaterialDescripcion { get; set; }
        public string CargaCupoMaterialDescripcion { get; set; }
        public string RecorridoPatente { get; set; }
        public string CargaDeCupoPatente { get; set; }
        public string CargaDeCupoRecorridoPatente { get; set; }
        public int CalleId { get; set; }
        public DateTime FechaIngreso { get; set; }
        public bool UltimoDeLaFila { get; set; }
        public bool? Rechazado { get; set; }
        public bool AsignadoEnPuestoComando { get; set; }
        public TipoCalle TipoCalle { get; set; }
        public TipoVehiculo? TipoVehiculo { get; set; }
        public string ColorFondo { get; set; }
        public string ColorTexto { get; set; }
        public bool EsSojaEPA { get; set; }
        public bool EsSojaIMPO { get; set; }
        public bool? EPA { get; set; }
        public string MaterialColorFondo { get; set; }
        public string MaterialColorTexto { get; set; }
        public string CargaCupoColorFondo { get; set; }
        public string CargaCupoColorTexto { get; set; }
        public string RecorridoCodigoSAP { get; set; }
        public string CargaDeCupoCodigoSAP { get; set; }
        public bool EsDemorado { get; set; }
    }
}