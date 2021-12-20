using System;
using System.Activities;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class RemitoACCPP : CodeActivity
    {
        [RequiredArgument]
        public InArgument<RemitoDto> Remito { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaInicio { get; set; }
        [RequiredArgument]
        public InArgument<TipoVehiculo> TipoVehiculo { get; set; }

        public OutArgument<CartaPorteDto> CartaPorte { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();
            var remito = Remito.Get<RemitoDto>(context);
            var centroId = CentroId.Get<int>(context);
            var fechaInicio = FechaInicio.Get<DateTime>(context);
            var tipoVehiculo = TipoVehiculo.Get<TipoVehiculo>(context);

            var centro = srvRepositorio.ObtenerCentro(centroId);
            
            var cartaPorte = new CartaPorteDto();
            cartaPorte.MaterialId = remito.MaterialId;
            cartaPorte.Chofer = remito.Chofer;
            cartaPorte.NroCartaPorte = remito.Remito.Replace("R",string.Empty);
            cartaPorte.TipoComercialSentido = "E";
            cartaPorte.Aparceria = false;
            cartaPorte.TitularCartaPorteCodigoSap = remito.OrigenCodigoSap;
            cartaPorte.AcuerdoMarco = remito.AcuerdoMarco;
            cartaPorte.DestinoCodigoSap = centro.CodigoSAP;
            cartaPorte.FechaEmision = fechaInicio;
            cartaPorte.KmRecorrer = remito.KmRecorrer;
            cartaPorte.MaterialCodigoSap = remito.MaterialCodigoSap;
            cartaPorte.FleteAPagar = false;
            cartaPorte.TitularCartaPorteCodigoSap = remito.OrigenCodigoSap;
            cartaPorte.ProcedenciaCodigoSap = remito.ProcedenciaCodigoSap;
            cartaPorte.ProvinciaCodigoSap = remito.ProvinciaCodigoSap;
            cartaPorte.TipoComercialCodigoSap = remito.TipoComercialCodigoSap;
            cartaPorte.TipoVehiculo = tipoVehiculo;
            cartaPorte.TransportistaCUIT = remito.TransportistaCuit;
            cartaPorte.Cosecha = remito.Cosecha;
            cartaPorte.CodEstab = remito.CodEstab;

            CartaPorte.Set(context,cartaPorte);

        }
    }
}
