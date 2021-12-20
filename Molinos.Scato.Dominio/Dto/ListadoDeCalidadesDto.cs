using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ListadoDeCalidadesDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Calidades_TipoDocumentoDeIngreso")]
        public string TipoDocumentoDeIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaDeDescarga")]
        public string FechaNeto { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Patente")]
        public string Patente { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PatenteAcoplado")]
        public string PatenteAcoplado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "NumeroDocumentoIngreso")]
        public string NumeroDocumentoDeIngreso { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Almacen")]
        public string AlmacenOrigenSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "DescripcionAlmacen")]
        public string AlmacenOrigen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Producto")]
        public string MaterialSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "DescripcionProducto")]
        public string Material { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material_Contrato")]
        public string Contrato { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "NetoPlanta")]
        public string NetoPlanta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "NetoOrigen")]
        public string NetoOrigen { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        public string CentroSap { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "DescripcionCentro")]
        public string Centro { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Transporte")]
        public string Transportista { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Procedencia")]
        public string Procedencia { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TitularCP")]
        public string TitularCP { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Intermediario")]
        public string Intermediario { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "RemitenteComercial")]
        public string RemitenteComercial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Destinatario")]
        public string Destinatario { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "TipoComercial")]
        public string TipoComercial { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Camara")]
        public string Camara { get; set; }

        public Dictionary<string, decimal?> Calidades { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Variedad")]
        public string Variedad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "NetoDescontado")]
        public string NetoDescontado { get; set; }

        public static void CargarValores(ListadoDeCalidadesDto actual, ListadoDeCalidadesDto otro)
        {
            var t = typeof(ListadoDeCalidadesDto);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(otro, null);
                var valueActual = prop.GetValue(actual, null);

                if (!String.IsNullOrEmpty((string)value) && String.IsNullOrEmpty((string)valueActual))
                {
                    prop.SetValue(actual, value, null);
                }
            }
        }
    }
}