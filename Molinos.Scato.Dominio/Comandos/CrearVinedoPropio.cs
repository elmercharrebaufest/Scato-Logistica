using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearVinedoPropio : Comando
    {
        public VinedoPropioDto Dto { get; set; }
        public string CodigoSapProveedor { get; set; }
    }
}
