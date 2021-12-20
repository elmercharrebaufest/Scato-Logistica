using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class AutorizacionChoferDto
    {

        public int Id { get; set; }

        public DateTime Fecha { get; set; }
        public string Comentario { get; set; }

        public int InhabilitacionChoferId { get; set; }

        public string NombreUsuarioResponsable { get; set; }

        public List<AutorizacionChoferDto> AutorizacionChofer { get; set; } 

    }
}
