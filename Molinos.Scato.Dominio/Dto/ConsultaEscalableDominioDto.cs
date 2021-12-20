using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public class ConsultaEscalableDominioDto
    {
        public string Dominio { get; set; }

        public ConsultaEscalableRtoDto Rto { get; set; }
        
        public int? Ejes { get
            {
                return int.TryParse(Rto != null ? Rto.CantEjes : string.Empty, out int resultado) ? resultado : (int?)null;
            }
        }
    }

}
