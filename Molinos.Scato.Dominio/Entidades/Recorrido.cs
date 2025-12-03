using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Recorrido
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual Workflow Workflow { get; set; }
        public virtual Guid InstanciaWorkflow { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual Chofer Chofer { get; set; }
        public virtual Almacen Almacen { get; set; }
        public virtual Transportista Transportista { get; set; }
        public virtual TipoComercial TipoComercial { get; set; }
        public virtual TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        public virtual string NumeroDocumentoIngreso { get; set; }
        public virtual string NumeroDocumentoIngresoLegal { get; set; }
        public virtual string Patente { get; set; }
        public virtual int? PesoBruto { get; set; }
        public virtual int? PesoTara { get; set; }
        public virtual int? PesoBrutoOrigen { get; set; }
        public virtual int? PesoTaraOrigen { get; set; }
        public virtual int? PesoTaraBodega { get; set; }
        public virtual int? PesoNetoBodegaEnLitros { get; set; }
        public virtual Balanza BalanzaTara { get; set; }
        public virtual Balanza BalanzaBruto { get; set; }
        public virtual DateTime? PesoBrutoFecha { get; set; }
        public virtual DateTime? PesoTaraFecha { get; set; }
        public virtual Modalidad? PesoTaraModalidad { get; set; }
        public virtual Modalidad? PesoBrutoModalidad { get; set; }
        public virtual string PesoBrutoUsuario { get; set; }
        public virtual string PesoTaraUsuario { get; set; }
        public virtual Material Material { get; set; }
        public virtual string DatosProximaActividad { get; set; }
        public virtual Calado Calado { get; set; }
        public virtual AnalisisDeCalidad AnalisisDeCalidad { get; set; }
        public virtual DateTime FechaInicio { get; set; }
        public virtual DateTime? FechaEgreso { get; set; }
        public virtual string Usuario { get; set; }
        public virtual bool Rechazado { get; set; }
        public virtual Vehiculo Vehiculo { get; set; }
        public virtual WorkflowDefinicion WorkflowDefinicion { get; set; }
        public virtual string DocumentoInternoSap { get; set; }
        public virtual string NumeroDeDocumentoSap { get; set; }
        public virtual string NumeroCiu { get; set; }
        public virtual int? PesoNetoTransile { get; set; }
        public virtual bool Terminado { get; set; }
        public virtual bool ControlBalanza { get; set; }
        public virtual string TarjetaDeAcceso { get; set; }
        public virtual bool EnvioMuestraAuditoria { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }
        public virtual bool? PagaTicketMunicipal { get; set; }
        public virtual TipoVehiculo TipoVehiculo { get; set; }

        public virtual Calle Calle { get; set; }

        public virtual ICollection<PuestosDeCargaDescarga> PuestosDeCargaDescargas { get; set; }
        public virtual TipoDocumentoIngreso? TipoDocumentoIngresoRelacionado { get; set; }
        public virtual string NumeroDocumentoIngresoRelacionado { get; set; }
        public virtual string NumeroCot { get; set; }
        public virtual decimal DescuentoEnKgOncca { get; set; }
        public virtual bool CorrespondeCaladoEnPlanta { get; set; }
        public virtual CaladoEnPlanta CaladoEnPlanta { get; set; }

        //EF no permite hacer inverse property en una relacion one to one
        [InverseProperty("Recorrido")]
        public virtual ICollection<CaracteristicasAnalizadas> CaracteristicasAnalizadasList { get; set; }

        public CaracteristicasAnalizadas CaracteristicasAnalizadas
        {
            get
            {
                return CaracteristicasAnalizadasList != null ? CaracteristicasAnalizadasList.FirstOrDefault() : null;
            }
        }

        [InverseProperty("Recorrido")]
        public virtual ICollection<CallePorRecorrido> CallePorRecorridos { get; set; }

        public virtual bool EnvioMuestraAuditoriaCamara { get; set; }
        public virtual bool VehiculoDemorado { get; set; }
        public virtual bool EstablecimientoDemorado { get; set; }
        public virtual string MotivoDemora { get; set; }
        public bool SacoTurnoConCircular { get; set; }
        public bool LlegoEnHorario { get; set; }
        public bool PasoPorContingenciaPesoExc { get; set; }
        public virtual PuntoDeCarga PuntoDeCarga { get; set; }

        [Column("TipoVariedad_Id")]
        public virtual int? TipoVariedadId { get; set; }
        public virtual TipoVariedad TipoVariedad { get; set; }
        public virtual bool IngresoContingenciaPagoMunicipal { get; set; }
        public virtual bool PagoTasaMunicipalInformado { get; set; }

        public virtual RecorridoTasaMunicipal RecorridoTasaMunicipal { get; set; }
    }
}