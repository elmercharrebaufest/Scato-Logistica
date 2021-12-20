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
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.ServiciosSap;
using IngresosEgresosFazonesTransmisionASap = Molinos.Scato.Dominio.Entidades.IngresosEgresosFazonesTransmisionASap;
using Recorrido = TransmisionASapManual.Entidades.Recorrido;

namespace TransmisionASapManual
{
    public class IngresosEgresosFazonesTransmisionASapManual
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
                    var recorrido = srvRepositorio.ObtenerRecorridoPorNumeroDocumento(r.NroDoc).FirstOrDefault(x => !x.Rechazado && x.Patente == r.Patente);

                    if(recorrido != null)
                    {
                        var t = ObtenerIngresosEgresosFazones(recorrido.Id, conversor,srvRepositorio,servicioComandos);

                        servicioComandos.Ejecutar(new ActualizarIngresosEgresosFazonesTransmisionASap { Dto = t });
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


        private static IngresosEgresosFazonesTransmisionASap ObtenerIngresosEgresosFazones(int recorridoId, ConversorAutoMapper conversor, IServicioRepositorio srvRepositorio, IServicioComandos servicioComandos)
        {
            var target = new IngresosEgresosFazonesGenerarRequest();
            var host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio);
            host.Extensions.Add(() => new ScatoPersistenceParticipant());
            var recorrido = srvRepositorio.ObtenerRecorrido(recorridoId);
            
            host.InArguments.InstanceId = recorrido.InstanciaWorkflow;
            host.InArguments.PesoNeto = recorrido.PesoBruto - recorrido.PesoTara;
            host.InArguments.NumeroDocumento = recorrido.NumeroDocumentoIngresoLegal;
            host.InArguments.FechaIngreso = recorrido.FechaInicio;
            host.InArguments.Km = null;
            host.InArguments.LocalidadId = null;
            host.InArguments.CentroId = recorrido.Centro.Id;
            host.InArguments.ClienteCodigoSap = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? "" : recorrido.Centro.CodigoSAP;
            host.InArguments.MaterialId = recorrido.Material.Id;
            host.InArguments.Patente = recorrido.Patente;
            host.InArguments.ProvinciaId = null;
            host.InArguments.TipoMovimiento = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? "SAL" : "ENT";
            host.InArguments.TransportistaId = recorrido.Transportista.Id;
            host.InArguments.InstanceId = recorrido.InstanciaWorkflow;

            var retorno = host.TestActivity();
            var request = retorno.First(f => f.Key == "Request").Value;
            var resultado = retorno.First(f => f.Key == "Resultado").Value;

            var transmision = conversor.Convertir<IngresosEgresosFazones, IngresosEgresosFazonesTransmisionASap>(((IngresosEgresosFazonesRequest)request).IngresosEgresosFazones);

            transmision.FuncionSap = FuncionSAP.IngresosEgresosFazones;
            transmision.InstanciaWorkflow = recorrido.InstanciaWorkflow;
            transmision.Fecha = DateTime.Now;
            transmision.MensajeError = "";
            transmision.Estado = EstadoTransmisionASap.Error;

            return transmision;
        }

        private static readonly Regex Numeric = new Regex(@"^\d+$");

    }
}
