using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearAlmacenPorMaterial : Comando
    {
        public AlmacenPorMaterial Dto { get; set; }
    }
}
