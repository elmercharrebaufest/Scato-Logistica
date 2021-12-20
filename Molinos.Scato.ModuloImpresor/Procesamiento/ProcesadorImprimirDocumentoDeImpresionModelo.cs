using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirDocumentoDeImpresionModelo : ProcesadorComando<ImprimirDocumentoDeImpresionModelo>
    {
        public ProcesadorImprimirDocumentoDeImpresionModelo(ILogger log)
            : base(log)
        {
        }

        public override Resultado Ejecutar(ImprimirDocumentoDeImpresionModelo comando)
        {

            if (comando.Formato != null)
            {
                try
                {
                    Log.Debug("Iniciando impresión de DocumentoDeImpresionModelo en la impresora: " + comando.Impresora);

                    var dto = new ImpImpresionGenericaDto
                        {
                            AcuerdoMarco = "AcuerdoMarco",
                            Codigo = "Codigo",
                            CuitEntregador = "CuitEntregador",
                            CuitRtteComercial = "CuitRtteComercial",
                            Entregador = "Entregador",
                            CodigoAnexo = "CodigoAnexo",
                            Caratula = "Caratula",
                            Chofer = "Chofer",
                            CodigoEstablecimiento = "CodigoEstablecimiento",
                            Corredor = "Corredor",
                            Cosecha = "Cosecha",
                            CuitChofer = "CuitChofer",
                            CuitCorredor = "CuitCorredor",
                            CuitDestinatario = "CuitDestinatario",
                            CuitIntermediario = "CuitIntermediario",
                            CuitTitularCP = "CuitTitularCP",
                            Destinatario = "Destinatario",
                            FechaCP = DateTime.Now,
                            FechaEmision = DateTime.Now,
                            FechaVencimiento = DateTime.Now,
                            Intermediario = "Intermediario",
                            KmARecorrer = "KmARecorrer",
                            Material = "Material",
                            NumeroDeDocumentoDeIngreso = "NumeroDeDocumentoDeIngreso",
                            ObservacionesONCCA = "ObservacionesONCCA",
                            Patente = "Patente",
                            PatenteAcoplado = "PatenteAcoplado",
                            PesoBruto = "PesoBruto",
                            PesoBrutoOrigen = "PesoBrutoOrigen",
                            PesoNeto = "PesoNeto",
                            PesoNetoOrigen = "PesoNetoOrigen",
                            PesoTara = "PesoTara",
                            PesoTaraOrigen = "PesoTaraOrigen",
                            Procedencia = "Procedencia",
                            RtteComercial = "RtteComercial",
                            SaldosSTOCK = "SaldosSTOCK",
                            TarifaReferencia = "TarifaReferencia",
                            TarifaTonelada = "TarifaTonelada",
                            TipoDeComprobanteONCCA = "TipoDeComprobanteONCCA",
                            TipoDocumentoIngreso = "TipoDocumentoIngreso",
                            TitularCP = "TitularCP",
                            Variedad = "Variedad",
                            CTG = "CTG",
                            CuitDestino = "CuitDestino",
                            CuitTransportista = "CuitTransportista",
                            Destino = "Centro Destino",
                            Transportista = "Transportista",
                            LocalidadCentroOrigen = "LocalidadCentroOrigen",
                            ProvinciaCentroOrigen = "ProvinciaCentroOrigen",
                            CodigoPostalCentroOrigen = "CodigoPostalCentroOrigen",
                            DireccionCentroOrigen = "DireccionCentroOrigen",
                            LocalidadCentroDestino = "LocalidadCentroDestino",
                            ProvinciaCentroDestino = "ProvinciaCentroDestino",
                            CodigoPostalCentroDestino = "CodigoPostalCentroDestino",
                            DireccionCentroDestino = "DireccionCentroDestino",
                            LocalidadClienteDestino = "LocalidadClienteDestino",
                            ProvinciaClienteDestino = "ProvinciaClienteDestino",
                            DireccionClienteDestino = "DireccionClienteDestino",
                            Prestador = "Prestador",
                            CuitPrestador = "CuitPrestador",
                            DomicilioBocaDestino = "DomicilioBocaDestino",
                            ProvinciaBocaDestino = "ProvinciaBocaDestino",
                            LocalidadBocaDestino = "LocalidadBocaDestino",
                            BocaDestino = "BocaDestino",
                            FleteAPagar = "x",
                            FletePagado = "x",
                            CuitBodega = "CuitBodega",
                            CuitViñatero = "CuitViñatero",
                            EsAcoplado = "x",
                            EsBines = "x",
                            EsCamion = "x",
                            EsMoliendaEnVinedos = "x",
                            EsTractor = "x",
                            EsUvaPropia = "x",
                            EsUvaTerceros = "x",
                            IIBBBodega = "IIBBBodega",
                            IIBBViñatero = "IIBBViñatero",
                            INVBodega = "INVBodega",
                            INVVariedad = "INVVariedad",
                            INVVinatero = "INVVinatero",
                            MarcaVehiculo = "MarcaVehiculo",
                            MaterialCodigoONCCA = "MaterialCodigoONCCA",
                            MaterialDescCorta = "MaterialDescCorta",
                            ModeloVehiculo = "ModeloVehiculo",
                            NumeroCiu = "NumeroCiu",
                            Observaciones = "Observaciones",
                            PesoNetoEgreso = "PesoNetoEgreso",
                            PesoNetoIngreso = "PesoNetoIngreso",
                            PesoNetoSinHumedad = "PesoNetoSinHumedad",
                            RazonSocialVinatero = "RazonSocialVinatero",
                            FechaPesoNetoBodega = DateTime.Now,
                            RazonSocialBodega = "RazonSocialBodega",
                            TenorAzucarino = "TenorAzucarino",
                            CorredorVendedor = "CorredorVendedor",
                            CuitCorredorVendedor = "CUITCorredorVendedor",
                            IntermediarioFlete = "IntermediarioDelFlete",
                            CuitIntermediarioDelFlete = "CUITIntermediarioDelFlete",
                            MercadoATermino = "MercadoATermino",
                            CuitMercadoATermino = "CUITMercadoATermino",                            
                            FechaImpresion = DateTime.Now,
                            Impresora = comando.Impresora,
                            TipoDeWorkflow = TipoDeWorkflow.Ingreso
                        };

                    var impresora = new ImpresionGenerica();

                    impresora.Imprimir(dto, comando.Impresora, comando.Formato);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error al imprimir en la impresora: " + comando.Impresora);
                }
            }
            else
            {
                Log.Error("Error al imprimir en la impresora: codigo de impresion no encontrado");
            }
            return new Resultado();

        }
    }
}
