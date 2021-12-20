using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarEstadoConexion : Comando
    {
        public EstadoConexionDto Dto { get; set; }
    }
}
