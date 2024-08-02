using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class AutorizarCpeDGDummy : Comando
    {
       
        public short TipoCPE { get; set; }
        public int NroOrden { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }
        public int CentroId { get; set; }
        public int MaterialId { get; set; }
        public int DestinoId { get; set; }
        public int DestinoPlanta { get; set; }
        public int DestinoDomicilioTipo { get; set; }
        public int DestinoDomicilioOrden { get; set; }
        public int TransportistaId { get; set; }
        public string[] Dominios { get; set; }
        public int KmRecorrer { get; set; }
        public string ChoferCuit { get; set; }
        public int PagadorFleteId { get; set; }
        public int? CorredorId { get; set; }
        public int? ComisionistaId { get; set; }
        public int? RemitenteId { get; set; }

        public int? IntermediarioFleteId { get; set; }
        public int? DestinatarioId { get; set; }
        public bool AplicaDestinatario { get; set; }
        public string Observaciones { get; set; }
    }
}