using System;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    public class AnularCPEDGDummy : Comando
    {
        public int CentroId { get; set; }
        public int NroOrden { get; set; }
        public int Sucursal { get; set; }
        public short TipoCPE { get; set; }
    }
}
