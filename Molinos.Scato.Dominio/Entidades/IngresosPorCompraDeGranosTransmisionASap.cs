using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("IngresosPorCompraDeGranosTransmisionASap")]
    public class IngresosPorCompraDeGranosTransmisionASap : TransmisionASap
    {
        public virtual string PatenteAcoplado { get; set; }
        public virtual string AgenteCompra { get; set; }
        public virtual string Secuencia { get; set; }
        public virtual string Almacen { get; set; }
        public virtual string Apariencia { get; set; }
        public virtual string Balanza { get; set; }
        public virtual string Bruto { get; set; }
        public virtual string BrutoOrigen { get; set; }
        public virtual string Camara { get; set; }
        public virtual string Caratula { get; set; }
        public virtual string Cargador { get; set; }
        public virtual string CCPP { get; set; }
        public virtual string Analisis { get; set; }
        public virtual string Centro { get; set; }
        public virtual string Chofer { get; set; }
        public virtual string Contrato { get; set; }
        public virtual string Destino { get; set; }
        public virtual string EntradaSalida { get; set; }
        public virtual string EstadoSAP { get; set; }
        public virtual string FechaAlta { get; set; }
        public virtual string FechaBruto { get; set; }
        public virtual string FechaCalado { get; set; }
        public virtual string FechaEgreso { get; set; }
        public virtual string FechaIngreso { get; set; }
        public virtual string FechaNeto { get; set; }
        public virtual string FechaTara { get; set; }
        public virtual string Km { get; set; }
        public virtual string Material { get; set; }
        public virtual string FirmaPaga { get; set; }
        public virtual string MuestraConjunto { get; set; }
        public virtual string Neto { get; set; }
        public virtual string NetoOrigen { get; set; }
        public virtual string NroDocChofer { get; set; }
        public virtual string NumCarPor { get; set; }
        public virtual string Patente { get; set; }
        public virtual string Pesada { get; set; }
        public virtual string Prestador { get; set; }
        public virtual string Proveedor { get; set; }
        public virtual string Procedencia { get; set; }
        public virtual string ProvProc { get; set; }
        public virtual string Remitente { get; set; }
        public virtual string Tara { get; set; }
        public virtual string TaraOrigen { get; set; }
        public virtual string TipoDocChofer { get; set; }
        public virtual string TipoComercial { get; set; }
        public virtual string TipVehiculo { get; set; }
        public virtual string Transportista { get; set; }
        public virtual string Variedad { get; set; }
        public virtual string Corredor { get; set; }
        public virtual string NetoDescontado { get; set; }
        public virtual string CuentaOrden { get; set; }
        public virtual string HoraBruto { get; set; }
        public virtual string HoraCalado { get; set; }
        public virtual string HoraEgreso { get; set; }
        public virtual string HoraIngreso { get; set; }
        public virtual string HoraNeto { get; set; }
        public virtual string HoraTara { get; set; } 
        public virtual string Cuenta_Orden { get; set; }
        public virtual string CTG { get; set; }
        public virtual string FECHA_CTG { get; set; }
        [InverseProperty("IngresosPorCompraDeGranosTransmisionASap")]
        public virtual ICollection<RecepcionYRedespacho> RecepcionesYDespachosII { get; set; }

        public virtual string CuitDestinatarioCartaPorte { get; set; }
        public virtual string CosechaCartaPorte { get; set; }
        public virtual string EstablecimientoCartaPorte { get; set; } 

        public virtual string CanjeRemito { get; set; }
        public virtual string NumeroCCPP { get; set; }
        public virtual string Cosecha { get; set; } 
        public virtual string CTG_Destinatario { get; set; }
        public virtual string CuitCanjeador { get; set; }
        public virtual string CuitDestinatario { get; set; }
        public virtual string CuitDestino { get; set; }
        public virtual string Especie { get; set; }
        public virtual string Establecimiento { get; set; }
        public virtual string Estado_Destinatario { get; set; }
        public virtual string FechaConf { get; set; }
        public virtual string PesoNetoCarga { get; set; }
        public virtual string Solicitante { get; set; }
        public virtual string Cupo { get; set; }

        public virtual string InterFlete { get; set; }
        public virtual string Clasificacion { get; set; }

        public virtual string TrigoEspecial { get; set; }
        public virtual string CuitSolicitante { get; set; }

        public override Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}