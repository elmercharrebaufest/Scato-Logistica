using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarVinedoPropio : Comando
    {
        public VinedoPropioDto Dto { get; set; }
        public string CodigoSapProveedor { get; set; }
    }
}
