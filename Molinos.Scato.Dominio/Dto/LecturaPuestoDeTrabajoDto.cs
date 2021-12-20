using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LecturaPuestoDeTrabajoDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Workflow_NumeroDeTarjeta")]
        public string NumeroDeTarjeta { get; set; }

        public string PrimerNumeroDeTarjeta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo")]
        public int PuestoDeTrabajoId { get; set; }
        
        public bool PuestoDeTrabajoPidePantente { get; set; }
        public bool PuestoDeTrabajoImprimeTarjetaDeAcceso { get; set; }
        
        public bool EsTarjetaSupervisor { get; set; }
        public IEnumerable<string> DispositivosSupervisor { get; set; }

        public int CentroId { get; set; }

        public bool TarjetaValida { get; set; }
        public string MensajeError { get; set; }

        public IEnumerable<string> Entrada { get; set; }
        public IEnumerable<VideoCamaraDto> VideoCamaras { get; set; }

        public LecturaPuestoDeTrabajoDto()
        {
            VideoCamaras = new List<VideoCamaraDto>();
        }
        public IEnumerable<string> Salida { get; set; }

        public string Patente { get; set; }
        public string PatenteLeida { get; set; }

        public bool OcrActivo { get; set; }
        public bool ReconocimientoExitoso { get; set; }
        public bool Automatizado { get; set; }
        public string CodigoDispositivo { get; set; }
        public string Firmware { get; set; }
    }
}
