using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DatosInstanciaWorkflowDto
    {
        public Guid Id { get; set; }

        public string Material { get; set; }
        public int? MaterialId { get; set; }
        public string MaterialCodigoSap { get; set; }

        public string Transportista { get; set; }
        public int? TransportistaId { get; set; }

        public string Cuit { get; set; }
        public string Patente { get; set; }
        public bool Rechazado { get; set; }
        public bool Sustentable { get; set; }
        public bool? PagaTicketMunicipal { get; set; }
        public bool EsEspecial { get; set; }
        public string Workflow { get; set; }

        public string ChoferDNI { get; set; }
        public string ChoferNombre { get; set; }
        public string NumeroDeTarjeta { get; set; }

        public string Procedencia { get; set; }
        public string Entregador { get; set; }

        public bool CaracteristicasNoCorrenspodenEspecial { get; set; }

        public bool AnalisisObligatorio { get; set; }
        public int LocalidadId { get; set; }
        public int ProvinciaId { get; set; }
        public string Cosecha { get; set; }
        public int RecorridoId { get; set; }
        public bool VehiculoDemorado { get; set; }
        public bool LlegoEnHorario { get; set; }
        public string Proteina { get; set; }
        public string AlmacenDestino { get; set; }
        public int? DiferenciaPesoNeto { get; set; }
        public bool EsGrano { get; set; }
        public bool SojaEPA { get; set; }
        public bool SojaEUDR { get; set; }
        public bool SojaEPAyEUDR { get; set; }
    }
}