using System.ComponentModel.DataAnnotations;
using enums = Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Helpers;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CategoriaVehiculoDto
    {
        public int Id { get; set; }
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
        public string PatenteAcoplado2 { get; set; }
        public int TipoVehiculo { get; set; }
        public string TipoVehiculoDescripcion {
            get { return ((enums.TipoVehiculo)TipoVehiculo).DisplayEnum(); }
        }
    }
}
