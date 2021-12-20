using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.ServiciosSap;
using Recorrido = TransmisionASapManual.Entidades.Recorrido;

namespace TransmisionASapManual
{
    public class EgresosNoProductivosTransmisionASapManual
    {

        public static void Ejecutar(string path, IServicioRepositorio srvRepositorio, IServicioComandos servicioComandos)
        {
            var conversor = new ConversorAutoMapper();

            Console.Write("Se inicia proceso\n\n");

            if (File.Exists(path))
            {
                var reader = new StreamReader(File.OpenRead(path));
                var listaRecorridos = new List<Recorrido>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    listaRecorridos.Add(
                        new Recorrido
                        {
                            NroDoc = values[0],
                            Patente = values[1]
                        });
                }

                Console.Write("Se van a procesar " + listaRecorridos.Count + ".\n");
                Console.Write("Toque una tecla para comenzar el proceso...\n");
                Console.ReadKey();

                foreach (var r in listaRecorridos)
                {
                    Console.Write("Se va a procesar : " + r.NroDoc + "\n");
                    var recorrido = srvRepositorio.ObtenerRecorridoPorNumeroDocumento(r.NroDoc).FirstOrDefault(x => x.Terminado && !x.Rechazado && x.Patente == r.Patente);

                    if(recorrido != null)
                    {
                        var t = ObtenerEgresosMaterialNoProductivo(recorrido.Id, conversor,srvRepositorio,servicioComandos);

                        servicioComandos.Ejecutar(new ActualizarEgresosMaterialNoProductivoTransmisionASap() { Dto = t });
                        Console.Write(": " + r.NroDoc + " procesada correctamente\n");
                    }
                    else
                    {
                        Console.Write(": " + r.NroDoc + " no existe, se encuentra rechazada, o el recorrido no se encuentra terminado\n"); 
                    }
                }
                Console.Write("\n");
                Console.Write("Proceso finalizado\n");
                Console.ReadKey();
            }
            else
            {
                Console.Write("No se encuentra el archivo CPS.TXT con los numeros de carta de porte y patentes en el disco C:\n");
                Console.Write("Proceso finalizado\n");
                Console.ReadKey();
            }
        }


        private static EgresosNoProductivosTransmisionASap ObtenerEgresosMaterialNoProductivo(int recorridoId, ConversorAutoMapper conversor, IServicioRepositorio srvRepositorio, IServicioComandos servicioComandos)
        {
            srvRepositorio = new ChannelFactory<IServicioRepositorio>("ServicioRepositorio").CreateChannel();

            var target = new EgresosMaterialNoProductivoGenerarRequest();
            var host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio);
            host.Extensions.Add(() => new ScatoPersistenceParticipant());
            var recorrido = srvRepositorio.ObtenerRecorrido(recorridoId);
            var orden = srvRepositorio.ObtenerOrdenCargaInternaPorInstanceId(recorrido.InstanciaWorkflow);

            host.InArguments.CentroId = recorrido.Centro.Id;
            host.InArguments.TransportistaId = orden.TransportistaId;
            host.InArguments.ChoferId = orden.Chofer.Id;
            host.InArguments.Patente = orden.PatenteCamion;
            host.InArguments.PatenteAcoplado = orden.PatenteAcoplado;
            host.InArguments.MaterialId = orden.MaterialId;
            host.InArguments.InstanceId = recorrido.InstanciaWorkflow;
            host.InArguments.ClienteId = orden.DestinoId;
            host.InArguments.PesoNeto = recorrido.PesoBruto - recorrido.PesoTara;
            host.InArguments.Fecha = orden.FechaCreacion;

            var retorno = host.TestActivity();
            var request = retorno.First(f => f.Key == "Request").Value;
            var resultado = retorno.First(f => f.Key == "Resultado").Value;

            var transmision = conversor.Convertir<EgresosNoProductivos, EgresosNoProductivosTransmisionASap>(((EgresosNoProductivosRequest)request).EgresosNoProductivos);

            transmision.FuncionSap = FuncionSAP.EgresosMaterialNoProductivo;
            transmision.InstanciaWorkflow = recorrido.InstanciaWorkflow;
            transmision.Fecha = DateTime.Now;
            transmision.MensajeError = "";
            transmision.Estado = EstadoTransmisionASap.Error;

            return transmision;
        }

        private static readonly Regex Numeric = new Regex(@"^\d+$");

    }
}
