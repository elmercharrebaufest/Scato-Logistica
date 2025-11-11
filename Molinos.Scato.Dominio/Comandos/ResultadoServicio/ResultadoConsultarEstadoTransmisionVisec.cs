using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos.ResultadoServicio
{
    public class ResultadoConsultarEstadoTransmisionVisec : Resultado
    {
        public EstadoTransmisionAVisec? Estado { get; set; }
    }
}