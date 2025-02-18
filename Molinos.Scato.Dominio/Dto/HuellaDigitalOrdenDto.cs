using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Recursos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class HuellaDigitalOrdenDto
    {
        public int Id { get; set; }
        public int Estado { get; set; }
        public string Patente { get; set; }
        public string Acoplado { get; set; }
        public string Chofer { get; set; }
        public string Transportista { get; set; }
        public int? PesoTara { get; set; }
        public string Balanza { get; set; }
        public string LugarPesaje { get; set; }
        public DateTime? FechaHoraPesaje { get; set; }
        public string Usuario { get; set; }
        public string Observaciones { get; set; }
        public int Tipo { get; set; }
        public long Orden { get; set; }

        public int IdChofer { get; set; }
        public int IdTransportista { get; set; }
        public int IdBalanza { get; set; }
        public int IdCentro { get; set; }

        public string DescripcionEstado
        {
            get
            {
                switch (Estado)
                {
                    case 0:
                        return "Inactivo";
                    case 1:
                        return "Activo";
                    default:
                        return "";
                }
            }
        }

        public string DescripcionTipo
        {
            get
            {
                switch (Tipo)
                {
                    case 1: return TipoHuellaDigital.Recorrido.ToString();
                    case 2: return TipoHuellaDigital.HuellaManual.ToString();
                    default: return "";
                }
            }
        }
    }
}