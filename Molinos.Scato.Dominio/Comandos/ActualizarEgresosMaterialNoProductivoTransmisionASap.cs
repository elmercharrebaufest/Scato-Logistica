using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarEgresosMaterialNoProductivoTransmisionASap : Comando
    {
        public EgresosNoProductivosTransmisionASap Dto { get; set; }
    }
}
