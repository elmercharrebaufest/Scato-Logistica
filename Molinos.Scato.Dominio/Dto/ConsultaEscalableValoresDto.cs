using Molinos.Scato.Dominio.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Dominio.Dto
{
    public class ConsultaEscalableValoresDto
    {
        public string CategoriaEscalado { get; set; }

        public List<ConsultaEscalableDominioDto> Dominios { get; set; }

        public TipoVehiculo? MapeoCategoriaEscalado
        {
            get
            {
                if(Dominios == null || !Dominios.Any() || 
                    (Dominios.Any(x => x.Ejes == null) && 
                    (CategoriaEscalado == null || CategoriaEscalado == "A" || CategoriaEscalado == "B")))
                {
                    return null;
                }


                switch (CategoriaEscalado)
                {
                    case null:
                        return CamionBitren();
                    case "A":
                        return CamionBitren();
                    case "B":
                        return CamionBitren();
                    case "C":
                        return TipoVehiculo.CamiónC;
                    case "D":
                        return TipoVehiculo.CamiónD;
                    case "E":
                        return TipoVehiculo.CamiónE;
                    default:
                        return null;
                }
            }
        }

        private TipoVehiculo CamionBitren()
        {
            if (Dominios.Sum(x => x.Ejes ?? 0) >= 7)
            {
                return TipoVehiculo.Bitren;
            }
            return TipoVehiculo.Camión; 
        }
    }

}
