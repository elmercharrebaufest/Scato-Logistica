using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class InformeDeCupoTransmisionASapManual
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
                    Console.Write("Se va a procesar la Carta de porte: " + r.NroDoc + "\n");
                    var recorrido = srvRepositorio.ObtenerRecorridoPorNumeroDocumento(r.NroDoc).FirstOrDefault(x => x.Terminado && !x.Rechazado && x.Patente == r.Patente);

                    if (recorrido != null)
                    {
                        var request = ObtenerRequest(recorrido.Id, srvRepositorio, servicioComandos);
                        Console.Write("request:" + request != null);
                        Console.Write("Z_SDMF_Z2200NRequest:" + request.Z_SDMF_Z2200N != null);
                        var transmision = conversor.Convertir<Z_SDMF_Z2200N, Molinos.Scato.Dominio.Entidades.InformarCupoTransmisionASap>(request.Z_SDMF_Z2200N);

                        Console.Write("guardando en la base");
                        transmision.FuncionSap = FuncionSAP.InformarCupo;
                        transmision.InstanciaWorkflow = recorrido.InstanciaWorkflow;
                        transmision.Fecha = DateTime.Now;
                        transmision.MensajeError = "";
                        transmision.Estado = EstadoTransmisionASap.Error;

                        servicioComandos.Ejecutar(new ActualizarInformarCupoTransmisionASap { Dto = transmision });
                        Console.Write("Carta de porte: " + r.NroDoc + " procesada correctamente\n");
                    }
                    else
                    {
                        Console.Write("Carta de porte: " + r.NroDoc + " no existe, se encuentra rechazada, o el recorrido no se encuentra terminado\n");
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


        private static Z_SDMF_Z2200NRequest ObtenerRequest(int recorridoId, IServicioRepositorio srvRepositorio, IServicioComandos servicioComandos)
        {
            Console.Write("ObtenerRequest\n");
            var resultado = new Resultado();
            Z_SDMF_Z2200NRequest request = null;
            try
            {
                var recorrido = srvRepositorio.ObtenerRecorrido(recorridoId);
                var muestraEnvioACamara = srvRepositorio.ObtenerUltimaMuestraEnvioACamaraPorCaladoId(recorrido.Calado.Id);
                ConfigurationManager.AppSettings["SepararAlmacenSustentable"] = "false";
                var guid = recorrido.InstanciaWorkflow;

                var cartaPorte = srvRepositorio.ObtenerCartaPortePorInstanceId(guid);
                var cupo = cartaPorte.Cupo;
                var fechaIngreso = recorrido.FechaInicio;
                var centroId = recorrido.Centro.Id;
                var fechaPesoTara = recorrido.PesoTaraFecha;
                var fechaEgreso = recorrido.FechaEgreso;
                var camionRechazado = recorrido.Rechazado;
                var instanceId = recorrido.InstanciaWorkflow;


                Console.Write("Iniciando generación\n");
                var target = new InformarCupoGenerarRequest();
                var host = WorkflowInvokerTest.Create(target);
                host.Extensions.Add(srvRepositorio);
                host.Extensions.Add(servicioComandos
                    );
                host.Extensions.Add(() => new ScatoPersistenceParticipant());

                host.InArguments.CartaPorte = cartaPorte;
                host.InArguments.CodigoCupo = cupo;
                host.InArguments.FechaIngreso = fechaIngreso;
                host.InArguments.CentroId = centroId;
                host.InArguments.FechaPesadaTara = fechaPesoTara;
                host.InArguments.FechaEgreso = fechaEgreso;
                host.InArguments.CamionRechazado = camionRechazado;
                host.InArguments.InstanceId = instanceId;


                var retorno = host.TestActivity();
                request = (Z_SDMF_Z2200NRequest)retorno.First(f => f.Key == "Request").Value;
                var errores = (Resultado)retorno.First(f => f.Key == "Resultado").Value;
                foreach (var error in errores.Errores)
                {
                    Console.WriteLine(error.Key + " - " + error.Value);
                }
                Console.Write("Request Generada\n");
            }
            catch (Exception e)
            {

                Console.Write("ERRPR" + e.Message);
                Console.Write("ERRPR" + e.StackTrace);
                resultado.Errores.Add("", e.Message);
            }
            Console.Write("request" + request != null);
            return request;
        }


    }
}
