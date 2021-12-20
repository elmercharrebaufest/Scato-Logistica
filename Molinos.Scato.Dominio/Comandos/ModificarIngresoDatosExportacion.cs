using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarIngresoDatosExportacion : Comando
    {
        public IngresoDeDatosDeExportacionDto Dto { get; set; }
    }
}
