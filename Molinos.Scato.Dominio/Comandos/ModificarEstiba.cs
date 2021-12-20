using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarEstiba : Comando
    {
        public EstibaDto Dto { get; set; }
    }
}