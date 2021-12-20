using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarActividadConCargaAutomatica : Comando
    {
        public ActividadConCargaAutomaticaDto Dto { get; set; }
    }
}
