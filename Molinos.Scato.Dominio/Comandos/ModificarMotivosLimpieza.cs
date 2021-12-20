using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarMotivosLimpieza : Comando
    {
        public MotivosLimpiezaDto Dto { get; set; }
    }
}