using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ValidarConsistenciaBalanzadas : Comando
    {
        public string Balanza { get; set; }
        public string CodigoDispositivo { get; set; }
        public int Hasta { get; set; }
        public int? Desde { get; set; }
    }
}
