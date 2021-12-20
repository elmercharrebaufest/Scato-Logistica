using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarATAPuerto : Comando
    {
        public ATAPuertoDto Dto { get; set; }
    }
}