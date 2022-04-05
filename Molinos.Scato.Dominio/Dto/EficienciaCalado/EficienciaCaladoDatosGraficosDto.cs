using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class EficienciaCaladoDatosGraficosDto
    {
        public EficienciaCaladoDatosGraficosDto()
        {
            Porcentajes = new List<decimal>();
            Calles = new List<string>();
        }
        public List<decimal> Porcentajes { get; set; }
        public List<string> Calles { get; set; }
    }
}