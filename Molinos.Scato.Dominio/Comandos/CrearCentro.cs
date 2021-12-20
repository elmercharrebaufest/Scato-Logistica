using System;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;

namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]
    public class CrearCentro : Comando
    {
        public CentroDto Dto { get; set; }
        public String NombreUsuario { get; set; }
    }
}
