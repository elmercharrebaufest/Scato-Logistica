using System;
using System.Management;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorInstalarImpresora : ProcesadorComando<InstalarImpresora>
    {
        public ProcesadorInstalarImpresora(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(InstalarImpresora comando)
        {
            var resultado = new Resultado();
            var resultadoInstalar = AddPrinter(comando.Dto.Direccion);
            if (!String.IsNullOrEmpty(resultadoInstalar))
            {
                resultado.Errores.Add("Direccion", resultadoInstalar);
            }
            return resultado;
        }

        private string AddPrinter(string sPrinterName)
        {
            try
            {
                var scope = new ManagementScope(ManagementPath.DefaultPath);
                //oManagementScope.Connect();

                //var scope = new ManagementScope(@"\\localhost\root\cimv2");
                scope.Connect();
                Log.Debug("Impresora {0}, scope conectado", sPrinterName);
                var oPrinterClass = new ManagementClass(scope, new ManagementPath("Win32_Printer"), null);

                var oInputParameters = oPrinterClass.GetMethodParameters("AddPrinterConnection");

                oInputParameters.SetPropertyValue("Name", sPrinterName);

                Log.Debug("Impresora {0}, se va a invocar AddPrinterConnection", sPrinterName);
                using (var result = oPrinterClass.InvokeMethod("AddPrinterConnection", oInputParameters, null))
                {
                    var errorCode = (uint)result.Properties["returnValue"].Value;
                    Log.Debug("Impresora {0}, resultado de AddPrinterConnection: {1}", sPrinterName, errorCode);
                    switch (errorCode)
                    {
                        case 0:
                            return String.Empty;
                        case 5:
                            return Textos.Error_AccesoDenegado;
                        case 123:
                            return Textos.Error_RutaIncorrecta;
                        case 1801:
                            return Textos.Error_NombreDeImpresoraIncorrecto;
                        case 1930:
                            return Textos.Error_DriversIncompatibles;
                        case 3019:
                            return Textos.Error_DriversNecesitanSerInstalados;
                        default:
                            return Textos.Error_InstalarImpresora;

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "No se pudo instalar la impresora {0}", sPrinterName);
                return Textos.Error_InstalarImpresora;
            }
        }
    }
}