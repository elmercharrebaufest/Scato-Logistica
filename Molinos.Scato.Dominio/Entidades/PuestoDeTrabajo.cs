using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class PuestoDeTrabajo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string NombrePuesto { get; set; }
        public virtual string Lector { get; set; }
        public virtual string Entrada { get; set; }
        public virtual string CierreEntrada { get; set; }
        public virtual string EntradaSupervisor { get; set; }
        public virtual string CierreSupervisor { get; set; }
        
        public virtual string LectorQr { get; set; }
        public virtual string CartelLed { get; set; }
        public virtual Balanza Balanza { get; set; }

        public virtual string SensorQuiebre { get; set; }
        //Videocamara de sensor de quiebre
        public virtual string VideoCamara { get; set; }
        public virtual string VideoCamaraDirectorio { get; set; }
        public virtual string NombrePc { get; set; }
        public virtual bool Automatico { get; set; }
        public virtual bool PidePatente { get; set; }
        public virtual bool ImprimeTarjetaDeAcceso { get; set; }
        public virtual bool EncolaLecturas { get; set; }
        public virtual bool FotoAlMarcarTarjeta { get; set; }
        public virtual bool InvisibleEnListaDeTareas { get; set; }
        public bool Automatizado { get; set; }
        //Solo se usan en garita de entrada
        public virtual bool SinAfip { get; set; }
        public virtual bool SinCupo { get; set; }

        public virtual bool SinFotoCartaPorte { get; set; }
        public virtual bool ImprimeCartaPorte { get; set; }

        public virtual Centro Centro { get; set; }
        [InverseProperty("PuestoDeTrabajo")]
        public virtual ICollection<VideoCamara> VideoCamaras { get; set; }
        [InverseProperty("PuestoDeTrabajo")]
        public virtual ICollection<LecturaDeTarjeta> Lecturas { get; set; }
        [InverseProperty("PuestoDeTrabajo")]
        public virtual ICollection<EstadoConexion> Estados { get; set; }
        [InverseProperty("PuestosDeTrabajoAsociados")]
        public virtual IList<TarjetaSupervisor> TarjetasSupervisorAsociadas { get; set; }
        [InverseProperty("PuestosDeTrabajoAsociados")]
        public virtual IList<AnalisisObligatorio> AnalisisObligatoriosAsociados { get; set; }
        public virtual bool AutomatizadoFull { get; set; }
        public virtual bool PausaAutoFull { get; set; }
        
        public virtual bool NoAsignaCalleEnGaritaEntrada { get; set; }
        public virtual string Firmware { get; set; }

        public virtual string Concentrador { get; set; }
        public virtual string IntercomunicadorCodigo { get; set; }
        public IEnumerable<string> EntradasSupervisor()
        {
            return string.IsNullOrEmpty(EntradaSupervisor) ? new string[0] : EntradaSupervisor.Split(',');
        }

        public IEnumerable<string> Entradas()
        {
            return string.IsNullOrEmpty(Entrada) ? new string[0] : Entrada.Split(',');
        }

        public IEnumerable<string> CierresEntrada()
        {
            return string.IsNullOrEmpty(CierreEntrada) ? new string[0] : CierreEntrada.Split(',');
        }
        public IEnumerable<string> CierresSupervisor()
        {
            return string.IsNullOrEmpty(CierreSupervisor) ? new string[0] : CierreSupervisor.Split(',');
        }
        public virtual int? OrdenBalanza { get; set; }
        public virtual bool ActivarRegistroInactividad { get; set; }

        public virtual string SemaforoRojo { get; set; }
        public virtual string SemaforoAmarillo { get; set; }
        public virtual string SemaforoVerde { get; set; }
        public virtual ConfigSensores ConfigSensores { get; set; }
        public virtual VisualizacionBarrera VisualizacionBarrera { get; set; }
        public virtual bool RequiereComentarioGestionarBarrera { get; set; }
        public virtual string GrupoBarreraCodigo { get; set; }
        public virtual string SensorVehicular { get; set; }
        public virtual string CodigoConfigIdentificacionVehicular { get; set; }
    }
}
