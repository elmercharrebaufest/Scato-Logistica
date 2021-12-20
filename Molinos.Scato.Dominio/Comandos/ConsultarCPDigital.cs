using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ConsultarCPDigital : Comando
    {
        public short TipoCPE { get; set; }
        public int NroOrden { get; set; }
        public int Sucursal { get; set; }
        public int CentroId { get; set; }
        public long NroCtg { get; set; }
        public long CuitSolicitante { get; set; }
        public int TipoVehiculo { get; set; }
        public bool ConsultaAfip { get; set; }
        public DateTime? FechaUltimaActualizacion { get; set; }
        public bool ConsultaMinima { get; set; }
        public bool ConsultaFerroviarioPorCtg { get; set; }
        public bool ConsultaImagenCpe { get; set; }
    }
}