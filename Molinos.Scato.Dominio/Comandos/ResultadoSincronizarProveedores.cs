using System.Collections.Generic;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ResultadoSincronizarProveedores : Resultado
    {
        public List<ProveedorDto> Items { get; set; }
        public int Cantidad { get; set; }

        public ResultadoSincronizarProveedores()
        {
            Items = new List<ProveedorDto>();
        }

        public void AgregarResultado(ProveedorDto item)
        {
            Items.Add(item);
        }
    }
}