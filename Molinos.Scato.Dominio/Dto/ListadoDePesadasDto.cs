using System;
using System.Linq;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ListadoDePesadasDto
    {
        private string remitoDeProveedores;
        private string documentoSap;
        public string CentroOrigen { get; set; }
        public string CentroDestino { get; set; }
        public string TipoDocumentoDeIngreso { get; set; }
        public string NumeroDocumentoDeIngreso { get; set; }
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
        public string TipoComercial { get; set; }
        public string Material { get; set; }
        public string FechaIngreso { get; set; }
        public string FechaEgreso { get; set; }
        public string FechaNeto { get; set; }
        public string BrutoPlanta { get; set; }
        public string TaraPlanta { get; set; }
        public string NetoPlanta { get; set; }
        public string BrutoOrigen { get; set; }
        public string TaraOrigen { get; set; }
        public string NetoOrigen { get; set; }
        public string Procedencia { get; set; }
        public string Corredor { get; set; }
        public string CuitCorredor { get; set; }
        public string TitularCP { get; set; }
        public string CuitTitularCP { get; set; }
        public string Intermediario { get; set; }
        public string CuitIntermediario { get; set; }
        public string RemitenteComercial { get; set; }
        public string CuitRemitenteComercial { get; set; }
        public string Destinatario { get; set; }
        public string CuitDestinatario { get; set; }
        public string Cliente { get; set; }
        public string CuitCliente { get; set; }
        public string Transportista { get; set; }
        public string CuitTransportista { get; set; }
        public string Chofer { get; set; }
        public string CuilChofer { get; set; }
        public string Entregador { get; set; }
        public string Humedad { get; set; }
        public string AlmacenDestino { get; set; }
        public string AlmacenOrigen { get; set; }
        public string Camara { get; set; }
        public string NumeroDeLote { get; set; }
        public string Casillero { get; set; }
        public string RemitoSAP { get; set; }
        public string TipoDeVehiculo { get; set; }
        public string Observaciones { get; set; }
        public string NumeroOrdenDeCargaSAP { get; set; }
        public string Variedad { get; set; }
        public string KmARecorrer { get; set; }
        public string TarifaPorTonelada { get; set; }
        public string CpOrigen { get; set; }

        public string RemitoDeProveedores
        {
            get { return remitoDeProveedores; }
            set
            {
                if (!String.IsNullOrEmpty(value) && !String.IsNullOrEmpty(remitoDeProveedores))
                {
                    remitoDeProveedores += "/" + value;
                }
                else
                {
                    remitoDeProveedores = value;
                }
            }
        }
        public string DocumentoSap
        {
            get { return documentoSap; }
            set
            {
                if (!String.IsNullOrEmpty(value) && !String.IsNullOrEmpty(documentoSap))
                {
                    documentoSap += "/" + value;
                }
                else
                {
                    documentoSap = value;
                }
            }
        }
        public string CTG { get; set; }
        public string ValorDevueltoPorAfipArriboCTG { get; set; }
        public string ValorDevueltoPorAfipDefinitivoCTG { get; set; }
        public string BalanzaBruto { get; set; }
        public string BalanzaTara { get; set; }
        public string ObservacionesControlDeTiempo { get; set; }
        public string NumeroDeCot { get; set; }
        public string Rechazado { get; set; }
        public string NombreEstablecimiento { get; set; }
        public string CodigoEstablecimiento { get; set; }
        public string Cosecha { get; set; }
        public string Usuario { get; set; }
        public string ModalidadPesadaBruto { get; set; }
        public string ModalidadPesadaTara { get; set; }
        public string CPEDG { get; set; }
        public string CTGDG { get; set; }
        public string TipoVariedad { get; set; }

        public static void CargarValores(ListadoDePesadasDto actual, ListadoDePesadasDto otro)
        {
            var t = typeof(ListadoDePesadasDto);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(otro, null);
                var valueActual = prop.GetValue(actual, null);

                if (!String.IsNullOrEmpty((string) value) && String.IsNullOrEmpty((string) valueActual))
                {
                    prop.SetValue(actual, value, null);
                }
            }
        }
    }
}