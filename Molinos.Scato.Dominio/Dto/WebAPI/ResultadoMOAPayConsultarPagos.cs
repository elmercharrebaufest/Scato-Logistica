using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto.WebAPI
{
    public class ResultadoMOAPayConsultarPagos
    {
        public bool Resultado { get; set; }

        public string Mensaje { get; set; }

        public int Registros { get; set; }

        public List<MOAPayConsultarPagosDatos> Datos { get; set; }
    }

    public class MOAPayConsultarPagosDatos
    {
        public int Id { get; set; }

        public string NumeroDocumento { get; set; }

        public string CuitInterviniente { get; set; }

        public string Dominio { get; set; }

        public string TipoVehiculo { get; set; }

        public string FechaPago { get; set; }

        public string FechaEmision { get; set; }

        public string Importe { get; set; }

        public string FechaAcceso { get; set; }
    }
}