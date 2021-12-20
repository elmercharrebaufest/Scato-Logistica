using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearActividadConCargaAutomatica : Comando
    {
        public ActividadConCargaAutomaticaDto Dto { get; set; }
    }
}
