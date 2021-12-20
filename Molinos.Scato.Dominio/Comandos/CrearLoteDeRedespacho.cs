using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearLoteDeRedespacho : Comando
    {
        public LoteDeRedespachoDto Dto { get; set; }
    }
}
