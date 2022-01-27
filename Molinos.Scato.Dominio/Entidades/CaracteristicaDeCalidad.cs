using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CaracteristicaDeCalidad : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual CaracteristicaDeCalidadMaestro CaracteristicaDeCalidadMaestro { get; set; }
        public virtual MaterialPorCentro MaterialPorCentro { get; set; }
        public virtual string DescripcionCorta { get; set; }
        public virtual string UnidadDeMedida { get; set; }
        public virtual FormulaDescuento DescuentoEnPorcentaje { get; set; }
        public virtual decimal CaladoMinimo { get; set; }
        public virtual decimal CaladoMaximo { get; set; }
        public virtual string CodigoSAP { get; set; }
        public virtual TipoAnalisis Analisis { get; set; }
        public virtual string Ensayo { get; set; }
        public virtual bool CargaEnCalado { get; set; }
        public virtual bool Obligatorio { get; set; }
        public virtual bool NoObservableEnCalado { get; set; }
        public virtual bool InternoPorObservados { get; set; }
        public virtual bool InspeccionDeCamionesVacios { get; set; }
        public virtual bool EsModificable { get; set; }
        public virtual bool EsHumedad { get; set; }
        public virtual EnvioACamara SituacionEnvioACamara { get; set; }
        public virtual decimal? SiSuperaValorCamara { get; set; }

        [InverseProperty("CaracteristicaDeCalidad")]
        public virtual ICollection<Descuento> Descuentos { get; set; }
        public virtual ICollection<ConfiguracionDeTabla> ConfiguracionesDeTabla { get; set; }
        public virtual bool EsTenorAzucarino { get; set; }
        public virtual bool EsEstadoSanitario { get; set; }
        public virtual bool EsCalidadUva { get; set; }
        public virtual bool EsCuerposExtranos { get; set; }
        public virtual bool EsGranosVerdes { get; set; }
        public virtual bool EsGranosDañados { get; set; }
        public virtual bool EsMermaVolatil { get; set; }
        public virtual int? PrioridadEnCalado { get; set; }
        public virtual bool NoAceptarSiSeDefineUnValor { get; set; }
        public virtual decimal? ToleranciaSinAnalisis { get; set; }
        public virtual bool EsProteina { get; set; }

        public virtual bool EsInsectosVivos { get; set; }
        public virtual decimal? ValorProteina { get; set; }
        public virtual decimal? ValorProteinaMedia { get; set; }

        public virtual decimal? ValorEspecialMinimo { get; set; }
        public virtual decimal? ValorEspecialMaximo { get; set; }
        public virtual bool EsAutomatizable { get; set; }
        public virtual TipoDispositivo Dispositivo { get; set; }
        public virtual string NombreNirs { get; set; }

        public virtual bool IntervaloDeAnalisis { get; set; }
        public virtual bool EnviaASap { get; set; }
        public virtual decimal? ToleranciaSinMensaje { get; set; }
    }
}
