using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Centro : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string CodigoSAP { get; set; }
        public virtual string CodigoSAPEspecial { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string MailBarreras { get; set; }
        public virtual int? CantEtiquetasMicromuestras { get; set; }
        public virtual int? CodigoDeChamico { get; set; }
        public virtual int? CodigoDeInsectos { get; set; }
        public virtual int? CodigoDeTenorAzucarino { get; set; }
        public virtual string Sociedad { get; set; }
        public virtual int? TiempoMaxEntreActividades { get; set; }
        public virtual bool CasillerosPorHumedad { get; set; }
        public virtual bool UsaConfirmacionDescarga { get; set; }
        public virtual bool ReingresaPatenteAlPesar { get; set; }
        public virtual bool EncolaBajaCtgAutomatico { get; set; }
        public virtual bool ReingresaPatenteEnCalado { get; set; }
        public virtual bool UsaBinPallet { get; set; }
        public virtual bool UsaNumeroMuestraTerceros { get; set; }
        public virtual bool ImprimeReciboMunicipal { get; set; }
        public virtual bool SolicitaConfirmarCTG { get; set; }
        public virtual bool LibroOnccaPorDescripcionCorta { get; set; }
        public virtual bool RequiereCupo { get; set; }
        public virtual bool ValidarCupo { get; set; }
        public virtual bool ModificaAlmacenEnPesada { get; set; }
        public virtual int? NumeroLoteInicial { get; set; }
        public virtual string Cuit { get; set; }
        public virtual Camara CamaraDefault { get; set; }

        public virtual Provincia Provincia { get; set; }
        public virtual Localidad Localidad { get; set; }
        public virtual string Direccion { get; set; }

        public virtual int? NumeroCAI { get; set; }
        public virtual DateTime? VigenciaDesde { get; set; }
        public virtual DateTime? VigenciaHasta { get; set; }
        public virtual string MailVencimientoCAI { get; set; }
        public virtual ICollection<Usuario> UsuariosAsociados { get; set; }

        public virtual bool EsVirtual { get; set; }
        public virtual string CodigoEstablecimiento { get; set; }
        public virtual string CodigoPostal { get; set; }
        public virtual string NumeroOrigenCamaraBsAs { get; set; }

        public virtual string RazonSocial { get; set; }
        public virtual string NumeroINV { get; set; }
        public virtual string IngresosBrutos { get; set; }

        public virtual bool AsignaBalanzaEnComando { get; set; }

        public virtual string CodigoDeAduana { get; set; }

        public virtual bool DescargaCartaPortePorCtg { get; set; }

        public virtual int? HorarioDesde { get; set; }
        public virtual int? HorarioHasta { get; set; }

        public virtual string CodigoEstacionMeteorologica { get; set; }

        public virtual bool ValidarLimiteDeCreditoVentaEnSAP { get; set; }
        public virtual bool ValidarLimiteMinimoDePeso { get; set; }
        public virtual int? LimiteMinimoDePeso { get; set; }
        public virtual bool TomarFotoEnMesa { get; set; }
        public virtual bool LeerCPDeFoto { get; set; }
        public virtual int? ToleranciaPatenteLeida { get; set; }
        public virtual bool ModificaPinchazosPorCalada { get; set; }
        public bool InformaCircular { get; set; }
        public bool NotificarCamioneroCircular { get; set; }
        public int? MinutosEsperaCircular { get; set; }
        public int? Sucursal { get; set; }
        public int? Planta { get; set; }
        public bool ContingenciaAfipCpe { get; set; }
        public int? MinutosInactividadCalado { get; set; }
        public int? LimiteCamionesCalado { get; set; }
        public int? MinutosEsperaPrecalado { get; set; }

        public virtual DateTime? FechaEjecucionCacheoCPE { get; set; }
        public virtual string ErrorCacheoAfipCPE { get; set; }
        public virtual string FotosPath { get; set; }
        public virtual bool AvanzaCpe { get; set; }
        public virtual Domicilio Domicilio { get; set; }
        public virtual bool ValidarAutomaticamenteCTGDG { get; set; }
        public int? PlantaDG { get; set; }
    }
}
