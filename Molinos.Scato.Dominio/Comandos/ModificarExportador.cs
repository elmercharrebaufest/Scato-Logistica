using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarExportador : Comando
    {
        public ExportadorDto Dto { get; set; }
    }
}
