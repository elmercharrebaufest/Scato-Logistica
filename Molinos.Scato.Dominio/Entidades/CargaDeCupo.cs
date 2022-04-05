using System.ComponentModel.DataAnnotations;
using System;
namespace Molinos.Scato.Dominio.Entidades
{
    public class CargaDeCupo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual string Numero { get; set; }

        public virtual string Cupo { get; set; }

        public virtual bool SinCupo { get; set; }

        public virtual Recorrido Recorrido { get; set; }

        public virtual string RespuestaSap { get; set; }

        public virtual PuestoDeTrabajo PuestoDeTrabajo { get; set; }

        public virtual Material Material { get; set; }

        public virtual DateTime Fecha { get; set; }

        public DateTime FechaSap { get; set; }

        public virtual Centro Centro { get; set; }

        public virtual bool EstuvoPendiente { get; set; }

        public virtual bool Especial { get; set; }

        public virtual string Camara { get; set; }

        public virtual string NumeroCartaPorte { get; set; }

        public virtual string FotoRutaDestino { get; set; }
        public virtual string FotoCamionRutaDestino { get; set; }       

        public virtual string CTG { get; set; }
        public virtual string CodEstab { get; set; }
        public virtual string RtteComercialCodigoSap { get; set; }
        public virtual string TitularCartaPorteCodigoSap { get; set; }
        public virtual string Patente { get; set; }
        public bool Reingresado { get; set; }
        public bool SacoTurnoConCircular { get; set; }
        public bool LlegoEnHorario { get; set; }
        public bool CPE { get; set; }
        public virtual string FotoRutaSustentable { get; set; }
    }
}
