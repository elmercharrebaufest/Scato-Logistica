using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearActividadPorDispositivo : Comando
    {
        public ActividadPorDispositivoDto Dto { get; set; }
    }
}
