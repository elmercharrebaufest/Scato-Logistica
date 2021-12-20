using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoPesoNetoTransile : Comando
    {
        public Guid InstanceId { get; set; }
        public int PesoAgregado { get; set; }
    }
}
