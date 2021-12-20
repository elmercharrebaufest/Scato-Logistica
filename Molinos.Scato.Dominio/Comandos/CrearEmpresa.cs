using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearEmpresa : Comando
    {
        public EmpresaDto Dto { get; set; }
    }
}
