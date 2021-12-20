using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class AlmacenarPrecintos : Comando
    {
        public PrecintoDto[] Precintos { get; set; }
    }
}
