using System.Collections.Generic;
using System.ServiceModel;

namespace Molinos.Scato.Servicios
{
    [ServiceContract(Namespace = "http://scato.molinos.com.ar")]
    public interface IFirmwareFactory
    {
        TFirmware Firmware<TFirmware>(string firmware) where TFirmware : class, IFirmware;
        Dictionary<string, string> FirmwareDisponibles();
    }
}
