using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConfiguracionMailDto
    {
        public int Id { get; set; }

       public string TemplateMail { get; set; }

        public string Direcciones { get; set; }


    }
}