using System;
using System.Linq;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ArchivoINVFilaDto
    {
        public string Numero { get; set; }
        public string SesentaSiempre { get; set; }
        public string NumeroCiu { get; set; }
        public string FechaEgreso { get; set; }
        public string Campo5 { get; set; }
        public string NumeroINVBodega { get; set; }
        public string RazonSocialBodega { get; set; }
        public string CuitBodega { get; set; }
        public string IBBBodega { get; set; }
        public string Campo10 { get; set; }
        public string Campo11 { get; set; }
        public string NumeroINVVinedo { get; set; }
        public string RazonSocialVinedo { get; set; }
        public string CuitVinedo { get; set; }
        public string Campo15 { get; set; }
        public string Campo16 { get; set; }
        public string Campo17 { get; set; }
        public string Campo18 { get; set; }
        public string Campo19 { get; set; }
        public string Campo20 { get; set; }
        public string IIBBVinedo { get; set; }
        public string Campo22 { get; set; }
        public string Campo23 { get; set; }
        public string PesoBruto { get; set; }
        public string PesoTaraBodega { get; set; }
        public string PesoNeto { get; set; }
        public string Patente { get; set; }
        public string ModeloCamion { get; set; }
        public string NombreChofer { get; set; }
        public string NumeroInvVariedadMaterial { get; set; }
        public string TenorAzucarino { get; set; }
        public string EsPropiaPesTercerosT { get; set; }
        public string Observaciones { get; set; }
        public string Campo34 { get; set; }
        public string CiuCorrectoALRechazadoAN { get; set; }
        public string Campo36 { get; set; }
        public string Campo37 { get; set; }
        public string Campo38 { get; set; }
        public string Campo39 { get; set; }
        public string Campo40 { get; set; }
        public string AñoActual { get; set; }
        public string Campo42 { get; set; }
        public string Campo43 { get; set; }
        public string Campo44 { get; set; }
        public string Campo45 { get; set; }
        public string Campo46 { get; set; }
        public string TipoVehiculo { get; set; }
        public string TipoCosecha { get; set; }

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