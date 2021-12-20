using System;
using System.ServiceModel;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios
{
    [ServiceContract(Namespace = "http://scato.molinos.com.ar")]
    public interface IServicioSapAsincronico
    {
        [OperationContract(IsOneWay = true)]
        void IngresoPorCompraDeGranos(Guid idInstancia, Fill_Z1000 registro);
        [OperationContract(IsOneWay = true)]
        void SalidaDeOrigenEnRedespachos(Guid idInstancia, Mov975 mov975);
        [OperationContract(IsOneWay = true)]
        void LlegadaADestinoEnRedespachos(Guid idInstancia, Mov305 mov305);
        [OperationContract(IsOneWay = true)]
        void EgresosMaterialNoProductivo(Guid idInstancia, EgresosNoProductivos egresosNoProductivos);
        [OperationContract(IsOneWay = true)]
        void AjusteDeDiferenciasDePesoEnRedespachos(Guid idInstancia, MovAjuste movAjuste);
        [OperationContract(IsOneWay = true)]
        void IngresosEgresosFazones(Guid idInstancia, IngresosEgresosFazones ingresosEgresosFazones);
        [OperationContract(IsOneWay = true)]
        void PesaNeto(Guid idInstancia, PesaNeto pesaNeto);
        [OperationContract(IsOneWay = true)]
        void FletesDobleTramo(Guid idInstancia, FletesDobleTramo mov);
        [OperationContract(IsOneWay = true)]
        void EgresoSinFleteFazones(Guid idInstancia, EgresoSinFleteFazones mov291);

        [OperationContract(IsOneWay = true)]
        void RegistrarCartaDePorteTransporteAutomotor(Guid idInstancia, CartaPorteTransporteAutomotorRegistro cartaPorteRegistro);
        [OperationContract(IsOneWay = true)]
        void RegistrarCartaDePorteVagonFerroviario(Guid idInstancia, CartaPorteVagonFerroviarioRegistro cartaPorteRegistro);

        [OperationContract(IsOneWay = true)]
        void RegistrarMuestreoYPesajeTransporteAutomotor(Guid idInstancia, MuestreoPesajeTransporteAutomotor muestreoRegistro);
        [OperationContract(IsOneWay = true)]
        void RegistrarMuestreoYPesajeVagonFerroviario(Guid idInstancia, MuestreoPesajeVagonFerroviario muestreoRegistro);

        [OperationContract(IsOneWay = true)]
        void InformarCupo(Guid idInstancia, Z_SDMF_Z2200N informarCupo);

        [OperationContract(IsOneWay = true)]
        void IngresosBodega(Guid idInstancia, IngresosBodegaAsincronicoDto ingresosBodega);

        [OperationContract(IsOneWay = true)]
        void ZE7550(Guid idInstancia, Z_SDMF_RFC_ZE7550 registro);

    }
}
