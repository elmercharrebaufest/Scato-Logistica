using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearReasignacionDeTarjeta : Comando
    {
        public ReasignacionDeTarjetaDto Dto { get; set; }
    }
}
