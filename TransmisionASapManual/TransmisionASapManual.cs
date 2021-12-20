using System;
using System.IO;
using System.ServiceModel;
using Molinos.Scato.Servicios;

namespace TransmisionASapManual
{
    public class TransmisionASapManual
    {
        private static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.Write(@"Debe ingresar por parametro el path del archivo input o arrastrarlo sobre el ejecutable de la consola al ingresar.
El archivo debe tener el formato:
<numero de documento(tabla 'recorrido', 'campo NumeroDocumentoIngreso'>,<patente>");
                Console.Read();
                return;
            }
            IServicioRepositorio srvRepositorio;
            IServicioComandos servicioComandos;
            try
            {
                srvRepositorio = new ChannelFactory<IServicioRepositorio>("ServicioRepositorio").CreateChannel();
                servicioComandos = new ChannelFactory<IServicioComandos>("ServicioComandos").CreateChannel();
                srvRepositorio.ListarTiposDocumentoIdentidad();
            }
            catch (Exception)
            {
                Console.Write(@"Falló la conexión a la capa de serivicios de Scato");
                Console.Read();
                return;
            }
            string path = args[0];
            if (!File.Exists(path))
            {
                Console.Write(@"El archivo ingresado no existe o no puede ser accedido");
                Console.Read();
                return;
            }

            Console.Write(@"Elija el tipo de transaccion:
                            1 - Z1000
                            2 -Ingresos Egresos Fazones
                            3 - Mov305
                            4 - Ajuste de Diferencias
                            5 - Egresos No Productivos
                            6 - MOV975
                            7 - Informar Cupo
                            ");
            var numero = Console.ReadLine();
            switch (numero)
            {
                case "1":
                    IngresosPorCompraDeGranosTransmisionASapManual.Ejecutar(path, srvRepositorio, servicioComandos);
                    break;
                case "2":
                    IngresosEgresosFazonesTransmisionASapManual.Ejecutar(path, srvRepositorio, servicioComandos);
                    break;
                case "3":
                    LlegadaADestinoEnRedespachosTransmisionASapManual.Ejecutar(path, srvRepositorio, servicioComandos);
                    break;
                case "4":
                    AjusteDeDiferenciasEnRedespachosTransmisionASapManual.Ejecutar(path, srvRepositorio, servicioComandos);
                    break;
                case "5":
                    EgresosNoProductivosTransmisionASapManual.Ejecutar(path, srvRepositorio, servicioComandos);
                    break;
                case "6":
                    SalidaDeOrigenEnRedespachosTransmisionASapManual.Ejecutar(path, srvRepositorio, servicioComandos);
                    break;
                case "7":
                    InformeDeCupoTransmisionASapManual.Ejecutar(path, srvRepositorio, servicioComandos);
                    break;
            }
            Console.ReadKey();
        }


    }
}
