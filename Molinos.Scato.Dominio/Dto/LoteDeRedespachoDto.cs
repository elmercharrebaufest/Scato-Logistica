
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LoteDeRedespachoDto
    {
        public int Id { get; set; }

        public string Almacen { get; set; }

        public string Centro { get; set; }

        public string Lote { get; set; }

        public string Material { get; set; }

        public string Stock { get; set; }
        
        public Guid InstanciaWorkflow { get; set; }
    }
}
