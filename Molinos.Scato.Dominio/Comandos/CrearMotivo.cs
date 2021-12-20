using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearMotivo : Comando
    {
        public MotivoDto Dto { get; set; }
    }
}
