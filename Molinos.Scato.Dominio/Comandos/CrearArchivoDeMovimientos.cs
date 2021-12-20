using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearArchivoDeMovimientos : Comando
    {
        public FiltroArchivoDeMovimientosDto Dto { get; set; }
    }
}
