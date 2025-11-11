using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class VisecTransmisionBaseDto
    {
        public int Id { get; set; }
        public string CUITEmpresa { get; set; } = Constantes.ValoresPorDefecto.CuitMOA.ToString();
        public DateTime FechaHoraMovimiento { get; set; }
        public DateTime FechaCPE { get; set; }
        public string NumeroCPE { get; set; }
        public string NumeroCTG { get; set; }
        public string CUITTitular { get; set; }
        public int? NumeroRUCAOrigen { get; set; }
        public string CUITDestinatario { get; set; }
        public string CUITDestino { get; set; }
        public int NumeroRUCADestino { get; set; }
        public int Producto { get; set; }
        public string Campania { get; set; }
        public int PesoNetoCargaKg { get; set; }
        public List<VisecTransmisionMovimientoDto> VisecTransmisionMovimientos { get; set; } = new List<VisecTransmisionMovimientoDto>();
        public EstadoTransmisionAVisec Estado { get; set; }
    }
}