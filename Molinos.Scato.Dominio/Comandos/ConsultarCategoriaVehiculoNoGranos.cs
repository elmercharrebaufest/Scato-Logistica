using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ConsultarCategoriaVehiculoNoGranos : Comando
    {
        public int CentroId { get; set; }
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
    }
}
