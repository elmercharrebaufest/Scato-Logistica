using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Firmware;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Molinos.Scato.Web.Firmware
{
    public class FirmwareFactory : IFirmwareFactory
    {
        private readonly IKernel kernel;

        public FirmwareFactory(IKernel kernel, ILogger log)
        {
            this.kernel = kernel;
        }

        public TFirmware Firmware<TFirmware>(string firmware) where TFirmware : class, IFirmware
        {
            var claseDriver = ClaseDriver(firmware);

            return kernel.Get(claseDriver) as TFirmware;
        }

        private Type ClaseDriver(string firmware)
        {
            return Type.GetType(firmware);
        }

        public Dictionary<string, string> FirmwareDisponibles()
        {
            var dic = new Dictionary<string, string>();

            var firmwareTypes = Assembly.GetAssembly(typeof(FirmwareBase)).GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IFirmware)));

            firmwareTypes.ToList().ForEach(x => dic.Add(x.Name, NombreClase(x)));

            return dic;
        }

        private string NombreClase(Type type)
        {
            return new Regex(", Version=.*").Replace(type.AssemblyQualifiedName, "");
        }
    }
}
