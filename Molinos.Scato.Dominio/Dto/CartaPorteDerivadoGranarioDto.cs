using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class CartaPorteDerivadoGranarioDto
    {
        public int Id { get; set; }
        public string NroCTG { get; set; }
        public string Sucursal { get; set; }
        public string NroOrden { get; set; }
        public string RutaFotoCPEDG { get; set; }
        public int RecorridoId { get; set; }
    }
}
