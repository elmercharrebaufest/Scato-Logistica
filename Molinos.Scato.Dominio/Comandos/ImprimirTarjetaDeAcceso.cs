
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirTarjetaDeAcceso : Comando
    {
        public ImpTarjetaDeAccesoDto Dto { get; set; }
        public string OrigenImpresion { get; set; }
    }
}
