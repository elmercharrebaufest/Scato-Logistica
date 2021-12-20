using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearExcepcionEnvioCamara : Comando
    {
        public ExcepcionEnvioCamaraDto Dto { get; set; }
    }
}
