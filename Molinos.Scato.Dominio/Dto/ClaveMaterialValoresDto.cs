using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ClaveMaterialValoresDto
    {
        public string Clave { get; set; }
        public List<ValorMaterialDto> Valores { get; set; }

        public ClaveMaterialValoresDto()
        {
            this.Valores = new List<ValorMaterialDto>();
        }
    }

    public sealed class ValorMaterialDto
    {
        public int Valor { get; set; }
        public int Toneladas { get; set; }
        public string Material { get; set; }
    }
}
