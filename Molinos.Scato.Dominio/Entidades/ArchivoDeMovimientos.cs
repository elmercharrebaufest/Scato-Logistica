using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ArchivoDeMovimientos : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string NombreUsuario { get; set; }
        public virtual TipoDeWorkflow TipoDeWorkflow { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual IList<MovimientoDeTerceros> Muestras { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string NumeroDeArchivo { get; set; }
        public virtual DateTime FechaDesde { get; set; }
        public virtual DateTime FechaHasta { get; set; }
        public virtual Material Material { get; set; }
    }
}

