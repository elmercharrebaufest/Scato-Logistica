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
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.ServiciosSap;
using AjusteDeDiferenciasEnRedespachosTransmisionASap = Molinos.Scato.Dominio.Entidades.AjusteDeDiferenciasEnRedespachosTransmisionASap;
using Recorrido = TransmisionASapManual.Entidades.Recorrido;

namespace TransmisionASapManual
{
    public class AjusteDeDiferenciasEnRedespachosTransmisionASapManual
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
                    var recorrido = srvRepositorio.ObtenerRecorridoPorNumeroDocumento(r.NroDoc).LastOrDefault(x => x.Terminado && !x.Rechazado && x.Patente == r.Patente);

                    if(recorrido != null)
                    {
                        var t = ObtenerTransaccion(recorrido, conversor, srvRepositorio);

                        servicioComandos.Ejecutar(new ActualizarAjusteDeDiferenciasEnRedespachosTransmisionASap { Dto = t });
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


        private static AjusteDeDiferenciasEnRedespachosTransmisionASap ObtenerTransaccion(RecorridoDto recorrido, ConversorAutoMapper conversor, IServicioRepositorio servicio)
        {
            var target = new MovimientoStockSapGenerarRequest();
            var host = WorkflowInvokerTest.Create(target);

            var orden = servicio.ObtenerCartaPorte(recorrido.Vehiculo.Id);

            host.Extensions.Add(servicio);
            host.Extensions.Add(() => new ScatoPersistenceParticipant());

            host.InArguments.InstanceId = recorrido.InstanciaWorkflow;
            host.InArguments.CentroId = recorrido.Centro.Id;
            host.InArguments.MaterialId = recorrido.Material.Id;
            host.InArguments.Patente = recorrido.Patente;
            host.InArguments.Cantidad = recorrido.PesoBruto - recorrido.PesoTara;
            host.InArguments.CentroCosteId = orden.DestinoId;
            host.InArguments.ClaseExp = "CA";
            host.InArguments.FechaContab = recorrido.FechaEgreso ?? recorrido.FechaInicio;
            host.InArguments.FechaDoc = recorrido.FechaEgreso ?? recorrido.FechaInicio;
            host.InArguments.NroDocumento = recorrido.NumeroDocumentoIngreso;

            var retorno = host.TestActivity();
            var request = retorno.First(f => f.Key == "Request").Value;
            
            var transmision = conversor.Convertir<MovAjuste, AjusteDeDiferenciasEnRedespachosTransmisionASap>(((MovAjuste)request));

            transmision.FuncionSap = FuncionSAP.AjusteDeDiferencias;
            transmision.InstanciaWorkflow = recorrido.InstanciaWorkflow;
            transmision.Fecha = DateTime.Now;
            transmision.MensajeError = "";
            transmision.Estado = EstadoTransmisionASap.Error;

            return transmision;
        }

        private static readonly Regex Numeric = new Regex(@"^\d+$");

    }
}
