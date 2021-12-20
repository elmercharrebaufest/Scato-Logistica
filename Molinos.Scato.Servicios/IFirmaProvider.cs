using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Servicios
{
    [ServiceContract(Namespace = "http://scato.molinos.com.ar")]
    public interface IFirmaProvider
    {
        [OperationContract]
        FirmaDto ObtenerFirmaSinLogo();

        [OperationContract]
        Byte[] ObtenerLogo();

        [OperationContract]
        void RefrescarFirma();

        [OperationContract]
        Byte[] ObtenerFavicon();
    }
}
