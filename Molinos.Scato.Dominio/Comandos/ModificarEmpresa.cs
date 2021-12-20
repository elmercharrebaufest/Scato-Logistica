using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarEmpresa: Comando
    {
        public EmpresaDto Dto { get; set; }
    }
}
