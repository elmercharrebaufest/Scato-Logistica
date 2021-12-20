using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarIngresosBodegaTransmisionASap : Comando
    {
        public IngresosBodegaTransmisionASap Dto { get; set; }
        public int? TipoBinId { get; set; }
    }
}
