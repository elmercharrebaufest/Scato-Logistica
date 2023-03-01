using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class DarDeBajaCTGDGDefinitivo : Comando
    {
        public CartaPorteDto Dto { get; set; }
        public int CentroId { get; set; }
        public Guid WorkflowId { get; set; }
    }
}
