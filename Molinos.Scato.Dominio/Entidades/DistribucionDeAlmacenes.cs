using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class DistribucionDeAlmacenes : IIdentificable
    {
        [Key]
        public virtual int Id { get; private set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual string NombreDeUsuario { get; set; }
        public virtual DateTime Fecha { get; set; }

        [InverseProperty("DistribucionDeAlmacenes")]
        public ICollection<DistribucionDeAlmacen> DistribucionesDeAlmacenes { get; set; }
    }
}
