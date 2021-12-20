using System.Activities.Persistence;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Molinos.Scato.Actividades.Behaviour
{
    /// <summary>
    /// Extiende el PersistenceParticipant base agregando nuevas propiedades para persistir.
    /// </summary>
    public class ScatoPersistenceParticipant : PersistenceParticipant
    {
        public const string PropiedadPatente = "Patente";
        public const string PropiedadMaterialId = "MaterialId";
        public const string PropiedadMaterial = "MaterialDesc";
        public const string PropiedadTransportista = "RazonSocial";
        public const string PropiedadTransportistaId = "TransportistaId";
        public const string PropiedadCuit = "Cuit";
        public const string PropiedadCalidad = "Calidad";
        public const string PropiedadTipoDocumentoDeIngreso = "TipoDocumentoDeIngreso";
        public const string PropiedadNumeroDocumentoDeIngreso = "NumeroDocumentoDeIngreso"; 
        public const string PropiedadWorkflow = "Workflow";
        public const string PropiedadDatosProximaActividad = "DatosProximaActividad";
        public const string PropiedadCentroId = "CentroId";
        public const string PropiedadCentroCodigoSap = "CentroCodigoSap";
        public const string PropiedadCentro = "Centro";
        public const string PropiedadMaterialCodigoSap = "MaterialCodigoSap";
        public const string PropiedadTipoVehiculo = "TipoVehiculo";
        public const string PropiedadTipoComercial = "TipoComercial";
        public const string PropiedadTipoComercialId = "TipoComercialId";
        public const string PropiedadNumeroDeTarjeta = "NumeroDeTarjeta";
        public const string PropiedadActividad = "Actividad";
        public const string PropiedadNameSpace = "{http://scato.molinos.com/PropiedadesCustom}";
        public const string PropiedadChoferDNI = "ChoferDNI";
        public const string PropiedadChoferNombre = "ChoferNombre";
        public const string PropiedadProcedencia = "Procedencia";

        private readonly XNamespace xNs = XNamespace.Get("http://scato.molinos.com/PropiedadesCustom");

        public string Patente { get; set; }
        public int? MaterialId { get; set; }
        public string Material { get; set; }
        public int? TransportistaId { get; set; }
        public string Transportista { get; set; }
        public string Cuit { get; set; }
        public string Calidad { get; set; }
        public int? TipoDocumentoDeIngreso { get; set; }
        public string NumeroDocumentoDeIngreso { get; set; }
        public string Workflow { get; set; }
        public string DatosProximaActividad { get; set; }
        public int? CentroId { get; set; }
        public string Centro { get; set; }
        public string CentroCodigoSap { get; set; }
        public string NumeroDeTarjeta { get; set; }
        public string MaterialCodigoSap { get; set; }
        public int? TipoVehiculo { get; set; }
        public int? TipoComercialId { get; set; }
        public string TipoComercial { get; set; }
        public string Actividad { get; set; }
        public string ChoferDNI { get; set; }
        public string ChoferNombre { get; set; }
        public string Procedencia { get; set; }

        protected override void CollectValues(out IDictionary<XName, object> readWriteValues,
                                              out IDictionary<XName, object> writeOnlyValues)

        {
            base.CollectValues(out readWriteValues, out writeOnlyValues);
            readWriteValues = new Dictionary<XName, object>
                {
                    {xNs.GetName(PropiedadPatente), Patente},
                    {xNs.GetName(PropiedadMaterialId), MaterialId},
                    {xNs.GetName(PropiedadMaterial), Material},
                    {xNs.GetName(PropiedadTransportista), Transportista},
                    {xNs.GetName(PropiedadTransportistaId), TransportistaId},
                    {xNs.GetName(PropiedadCuit), Cuit},
                    {xNs.GetName(PropiedadCalidad), Calidad},
                    {xNs.GetName(PropiedadTipoDocumentoDeIngreso), TipoDocumentoDeIngreso},
                    {xNs.GetName(PropiedadNumeroDocumentoDeIngreso), NumeroDocumentoDeIngreso},
                    {xNs.GetName(PropiedadWorkflow), Workflow},
                    {xNs.GetName(PropiedadDatosProximaActividad), DatosProximaActividad},
                    {xNs.GetName(PropiedadCentroId), CentroId},
                    {xNs.GetName(PropiedadCentroCodigoSap), CentroCodigoSap},
                    {xNs.GetName(PropiedadCentro), Centro},
                    {xNs.GetName(PropiedadNumeroDeTarjeta), NumeroDeTarjeta},
                    {xNs.GetName(PropiedadMaterialCodigoSap), MaterialCodigoSap},
                    {xNs.GetName(PropiedadTipoVehiculo), TipoVehiculo},
                    {xNs.GetName(PropiedadTipoComercialId), TipoComercialId},
                    {xNs.GetName(PropiedadTipoComercial), TipoComercial},
                    {xNs.GetName(PropiedadActividad), Actividad},
                    {xNs.GetName(PropiedadChoferDNI), ChoferDNI},
                    {xNs.GetName(PropiedadChoferNombre), ChoferNombre},
                    {xNs.GetName(PropiedadProcedencia), Procedencia},
                };
            writeOnlyValues = null;
        }

        protected override void PublishValues(IDictionary<XName, object> readWriteValues)
        {
            base.PublishValues(readWriteValues);
            Patente = readWriteValues[xNs.GetName(PropiedadPatente)] as string;
            MaterialId = readWriteValues[xNs.GetName(PropiedadMaterialId)] as int?;
            Material = readWriteValues[xNs.GetName(PropiedadMaterial)] as string;
            TransportistaId = readWriteValues[xNs.GetName(PropiedadTransportistaId)] as int?;
            Transportista = readWriteValues[xNs.GetName(PropiedadTransportista)] as string;
            Cuit = readWriteValues[xNs.GetName(PropiedadCuit)] as string;
            Calidad = readWriteValues[xNs.GetName(PropiedadCalidad)] as string;
            TipoDocumentoDeIngreso = readWriteValues[xNs.GetName(PropiedadTipoDocumentoDeIngreso)] as int?;
            NumeroDocumentoDeIngreso = readWriteValues[xNs.GetName(PropiedadNumeroDocumentoDeIngreso)] as string;
            Workflow = readWriteValues[xNs.GetName(PropiedadWorkflow)] as string;
            DatosProximaActividad = readWriteValues[xNs.GetName(PropiedadDatosProximaActividad)] as string;
            CentroId = readWriteValues[xNs.GetName(PropiedadCentroId)] as int?;
            CentroCodigoSap = readWriteValues[xNs.GetName(PropiedadCentroCodigoSap)] as string;
            NumeroDeTarjeta = readWriteValues[xNs.GetName(PropiedadNumeroDeTarjeta)] as string;
            Centro = readWriteValues[xNs.GetName(PropiedadCentro)] as string;
            MaterialCodigoSap = readWriteValues[xNs.GetName(PropiedadMaterialCodigoSap)] as string;
            TipoVehiculo = readWriteValues[xNs.GetName(PropiedadTipoVehiculo)] as int?;
            TipoComercial = readWriteValues[xNs.GetName(PropiedadTipoComercial)] as string;
            TipoComercialId = readWriteValues[xNs.GetName(PropiedadTipoComercialId)] as int?;
            Actividad = readWriteValues[xNs.GetName(PropiedadActividad)] as string;
            ChoferDNI = readWriteValues.ContainsKey(xNs.GetName(PropiedadChoferDNI)) ? readWriteValues[xNs.GetName(PropiedadChoferDNI)] as string : "";
            ChoferNombre = readWriteValues.ContainsKey(xNs.GetName(PropiedadChoferNombre)) ? readWriteValues[xNs.GetName(PropiedadChoferNombre)] as string : "";
            Procedencia = readWriteValues.ContainsKey(xNs.GetName(PropiedadProcedencia)) ? readWriteValues[xNs.GetName(PropiedadProcedencia)] as string : "";

        }
    }
}