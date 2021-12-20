
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ListadoCamionesDto
    {
        public string NroCartaDePorte { get; set; }
        public DateTime FechaDeCarga { get; set; }
        public string LocalidadDeOrigen { get; set; }
        public string ProvinciaDeOrigen { get; set; }
        public string LocalidadDeDestino { get; set; }
        public string ProvinciaDeDestino { get; set; }
        public string CuitTransportista { get; set; }
        public string NombreTransportista { get; set; }
        public string CuilDelChofer { get; set; }
        public string NombreChofer { get; set; }
    }
}