using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarCartaDePorteRegistradaServicioMonsanto : Comando
    {
        public Guid InstanceId { get; set; }
        public string TipoAnalisis { get; set; }
        public string LaboratorioRazonSocial { get; set; }
        public string LaboratorioCuit { get; set; }
    }
}
