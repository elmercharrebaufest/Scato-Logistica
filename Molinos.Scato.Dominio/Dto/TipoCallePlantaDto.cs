using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class TipoCallePlantaDto
    {
        public TipoCallePlantaDto()
        {
            Calles = new List<CallePlantaDto>();
        }

        public TipoCalle TipoCalle { get; set; }
        public List<CallePlantaDto> Calles { get; set; }
    }

    public class CallePlantaDto
    {
        public CallePlantaDto()
        {
            Camiones = new List<CamionPlantaDto>();
        }

        public int CalleId { get; set; }
        public string CalleDesc { get; set; }
        public string ColorFondo { get; set; }
        public string ColorTexto { get; set; }
        public int LimiteDeCamiones { get; set; }
        public bool Bloqueada { get; set; }
        public bool EsPrimero { get; set; }
        public bool EsUltimo { get; set; }
        public List<CamionPlantaDto> Camiones { get; set; }
    }

    public class CamionPlantaDto
    {
        public int CamionId { get; set; }
        public string Patente { get; set; }
        public bool Escalable { get; set; }
        public bool UltimoDeLaFila { get; set; }
        public string ColorFondo { get; set; }
        public string ColorTexto { get; set; }
        public bool? Rechazado { get; set; }
        public int Calidad { get; set; }
        public int CalleId { get; set; }
        public DateTime FechaIngreso { get; set; }
        public int MaterialId { get; set; }
        public bool EsSojaEPA { get; set; }
        public bool EsSojaIMPO { get; set; }
    }
}