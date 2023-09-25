using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EliminarAutomatismoNoGranoAlmacen : Comando
    {
        public AutomatismoNoGranoAlmacenDto Dto { get; set; }
    }
}
