using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearLote : Comando
    {
        public LoteDto Dto { get; set; }
    }
}
