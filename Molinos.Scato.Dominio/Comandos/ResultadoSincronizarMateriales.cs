using System.Collections.Generic;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ResultadoSincronizarMateriales : Resultado
    {
        public List<MaterialDto> Items { get; set; }
        public int Cantidad { get; set; }

        public ResultadoSincronizarMateriales()
        {
            Items = new List<MaterialDto>();
        }

        public void AgregarResultado(MaterialDto item)
        {
            Items.Add(item);
        }
    }
}