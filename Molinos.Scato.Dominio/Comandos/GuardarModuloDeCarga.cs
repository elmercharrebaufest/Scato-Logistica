using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class GuardarModuloDeCarga : Comando
    {
        public ModuloDeCargaDto Dto { get; set; }
    }
}