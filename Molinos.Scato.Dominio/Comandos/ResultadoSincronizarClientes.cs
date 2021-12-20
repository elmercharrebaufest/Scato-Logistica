using System.Collections.Generic;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ResultadoSincronizarClientes : Resultado
    {
        public List<ClienteDto> Items { get; set; }
        public int Cantidad { get; set; }

        public ResultadoSincronizarClientes()
        {
            Items = new List<ClienteDto>();
        }

        public void AgregarResultado(ClienteDto item)
        {
            Items.Add(item);
        }
    }
}