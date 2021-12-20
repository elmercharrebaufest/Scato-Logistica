using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearEstadoPuerto : Comando
    {
        public EstadoPuertoDto Dto { get; set; }
    }

}