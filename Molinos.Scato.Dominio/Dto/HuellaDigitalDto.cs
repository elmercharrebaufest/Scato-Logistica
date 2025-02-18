using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Recursos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class HuellaDigitalDto
    {
        public  int Id { get; set; }
        public  bool Estado { get; set; }
        [Required(ErrorMessage = "La valor Patente es obligatorio")]
        [RegularExpression(@"^[A-Z]{3}\d{3}$|^[A-Z]{2}\d{3}[A-Z]{2}$",
        ErrorMessage = "La patente debe tener el formato ABC123 o AB123CD.")]

        public string Patente { get; set; }

        [RegularExpression(@"^[A-Z]{3}\d{3}$|^[A-Z]{2}\d{3}[A-Z]{2}$",
        ErrorMessage = "La patente debe tener el formato ABC123 o AB123CD.")]
        public string Acoplado { get; set; }

        [Required(ErrorMessage = "El valor Chofer es obligatorio")]
        public string Chofer { get; set; }

        
        public int IdChofer { get; set; }

        [Required(ErrorMessage = "Ea valor Transportista es obligatorio")]
        public string Transportista { get; set; }

        
        public int IdTransportista { get; set; }

        [Required(ErrorMessage = "La valor Peso Tara es obligatorio")]
        public int PesoTara { get; set; }

        
        public string Balanza { get; set; }

        [Required(ErrorMessage = "La valor Balanza es obligatorio")]
        public int IdBalanza { get; set; }

        [Required(ErrorMessage = "La valor Lugar Pesaje es obligatorio")]
        public int IdCentro { get; set; }
        public string  LugarPesaje { get; set; }
        
        
        [Required(ErrorMessage = "La valor Fecha Hora Pesaje es obligatorio")]
        public string FechaHoraPesaje { get; set; }
        public string Usuario { get; set; }

        [Required(ErrorMessage = "La valor Observaciones es obligatorio")]
        [StringLength(250, MinimumLength = 10, ErrorMessage = "El campo debe tener entre 10 y 250 caracteres.")]
        public string Observaciones { get; set; }
      
        public bool EsEdicion { get; set; }

        public  void DarFormatoChofer(ChoferDto chofer)
        {
            if(chofer != null)
            Chofer  =  $"{chofer.Cuil} - {chofer.Nombre + " " + chofer.Apellido}";
        }

        public void DarFormatoTransportista(TransportistaDto transportista)
        {
            if(transportista != null)
            Transportista = $"{transportista.Cuit} - {transportista.RazonSocial}";
        }

        public string Centro { get; set; }
        public string EstadoRegistro { get; set; }

    }
}

      
      

       
 


