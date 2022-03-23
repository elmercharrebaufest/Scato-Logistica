using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class PuestoDeTrabajoDto
    {

        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_NombrePuesto")]
        [StringLength(35, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NombrePuesto { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_Lector")]
        public string Lector { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_LectorQr")]
        public string LectorQr { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_CartelLed")]
        public string CartelLed { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_Entrada")]
        public string Entrada { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_Salida")]
        public string CierreEntrada { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_EntradaSupervisor")]
        public string EntradaSupervisor { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_CierreSupervisor")]
        public string CierreSupervisor { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_SensorQuiebre")]
        public string SensorQuiebre { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ActividadPorDispositivo_CamaraSensor")]
        public string VideoCamara { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ActividadPorDispositivo_Directorio")]
        [StringLength(100, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string VideoCamaraDirectorio { get; set; }

        public List<VideoCamaraDto> VideoCamaras { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_NombrePc")]
        [StringLength(30, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string NombrePc { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_Automatico")]
        public bool Automatico { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_ValidarInactividadCalado")]
        public bool ValidarInactividadCalado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_PidePatente")]
        public bool PidePatente { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_ImprimeTarjetaDeAcceso")]
        public bool ImprimeTarjetaDeAcceso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_EncolaLecturas")]
        public bool EncolaLecturas { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_FotoAlMarcarTarjeta")]
        public bool FotoAlMarcarTarjeta { get; set; }

        public int CentroId { get; set; }

        public bool EstadoConexion { get; set; }

        public string MensajeConexion { get; set; }

        public string Lectura { get; set; }

        public string Patente { get; set; }

        public string PrimerLectura { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_Motivo")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Motivo { get; set; }

        public string VideoCamarasString
        { get { return VideoCamaras != null ? String.Join(", ", VideoCamaras.Select(y => y.Codigo)) : ""; } }

        public string VideoCamarasJson
        { get { return VideoCamaras != null ? VideoCamaras.ToJson() : "[]"; } }

        //public IList<TarjetaSupervisorDto> TarjetaSupervisorAsociadas { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_InvisibleEnListaDeTareas")]
        public bool InvisibleEnListaDeTareas { get; set; }

        public string PatenteLeida { get; set; }
        public bool OcrActivo { get; set; }
        public bool Automatizado { get; set; }
        public bool SinAfip { get; set; }
        public bool SinCupo { get; set; }
        public bool SinFotoCartaPorte { get; set; }
        public bool ImprimeCartaPorte { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_Balanza")]
        public int? BalanzaId { get; set; }

        public string BalanzaNombre { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_FullAutomatico")]
        public bool AutomatizadoFull { get; set; }

        public string NombreGarita
        {
            get
            {
                var numeroPuesto = string.IsNullOrEmpty(NombrePuesto) ? "" : string.Join("", NombrePuesto.ToCharArray().Where(Char.IsDigit));
                return (numeroPuesto != "") ? numeroPuesto : "1";
            }
        }

        public bool NoAsignaCalleEnGaritaEntrada { get; set; }
        public bool ReconocimientoExitoso { get; set; }
        public bool PausaAutoFull { get; set; }
        public string Firmware { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_Concentrador")]
        public string Concentrador { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_Intercomunicador")]
        public string IntercomunicadorCodigo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_ActivarRegistroInactividad")]
        public bool ActivarRegistroInactividad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "SemaforoVagones_Rojo")] 
        public string SemaforoRojoCodigo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "SemaforoVagones_Amarillo")]
        public string SemaforoAmarilloCodigo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "SemaforoVagones_Verde")]
        public string SemaforoVerdeCodigo { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_ConfigSensor")]
        public int? ConfigSensor_Id { get; set; }
        public ConfigSensoresDto ConfigSensores { get; set; }

    }
}