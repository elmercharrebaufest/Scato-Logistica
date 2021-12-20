using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ConfiguracionMail : IIdentificable
    {
        [Key]
        public int Id { get; set; }
        //public virtual ModuloDeCarga ModuloDeCarga { get; set; }
        public string TemplateMail { get; set; }
        public string Direcciones { get; set; }
        
    }
}
