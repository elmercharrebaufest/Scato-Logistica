using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearDestino : Comando
    {
        public DestinoDto Dto { get; set; }
    }
}
