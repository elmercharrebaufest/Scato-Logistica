using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EtiquetaAuditoriaEnCartaDePorte : Comando
    {
        public ImpEtiquetaAuditoriaDto Dto { get; set; }
        public string RutaFotoCartaDePorte { get; set; }
    }
}
