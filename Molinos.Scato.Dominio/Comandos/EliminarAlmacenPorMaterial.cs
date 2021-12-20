using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EliminarAlmacenPorMaterial : Comando
    {
        public AlmacenPorMaterial Dto { get; set; }
    }
}
