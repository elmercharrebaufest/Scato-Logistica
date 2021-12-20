using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarAgenciaMaritimaPuerto : Comando
    {
        public AgenciaMaritimaPuertoDto Dto { get; set; }
    }
}