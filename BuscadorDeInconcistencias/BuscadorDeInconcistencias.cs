using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Description;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.AfipCTGWebService;
//using System.Linq;
//using System.ServiceModel;
//using System.ServiceModel.Description;
//using Molinos.Scato.Actividades.Interfaces;
//using Molinos.Scato.Actividades.Servicios;
//using Molinos.Scato.Dominio.Comandos;
//using Molinos.Scato.Dominio.Dto;
//using Molinos.Scato.Dominio.Helpers;
//using Molinos.Scato.Dominio.Recursos;
//using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ComplianceWebServiceV2;
using Molinos.Scato.Servicios.ServiciosSap;

//using Molinos.Scato.Servicios.ServiciosSap;

namespace BuscadorDeInconcistencias
{
    class BuscadorDeInconcistencias
    {
        //private static IServicioRepositorio srvRepositorio;
        //private static IServicioComandos servicioComandos;
        //private static IListaDeWorkflows servicioListaDeWorkflows;

        static void Main(string[] args)
        {
            //////////////////////////////////////////////////////////////////////////////
            // COT //
            //////////////////////////////////////////////////////////////////////////////
            ////var factory = new ChannelFactory<ZSDWS_SCATO>("ZSDWS_SCATO");
            ////var defaultCredentials = factory.Endpoint.Behaviors.Find<ClientCredentials>();
            ////factory.Endpoint.Behaviors.Remove(defaultCredentials);

            ////////step two -instantiate your credentials
            ////ClientCredentials loginCredentials = new ClientCredentials();
            ////loginCredentials.UserName.UserName = "GWEBSRV_SCA";
            ////loginCredentials.UserName.Password = "scatoqa2015";

            ////////step three -set that as new endpoint behavior on factory
            ////factory.Endpoint.Behaviors.Add(loginCredentials); //add required ones
            ////var srvSap = factory.CreateChannel();

            ////var respuesta =
            ////        srvSap.Z_SDMF_RFC_VENTA_TRIGO_MAIZ(new Z_SDMF_RFC_VENTA_TRIGO_MAIZRequest
            ////        {
            ////            Z_SDMF_RFC_VENTA_TRIGO_MAIZ = new Z_SDMF_RFC_VENTA_TRIGO_MAIZ
            ////            {
            ////                IM_CUIT = null,
            ////                IM_FECHA = "2020-02-06",
            ////                IM_MATERIAL = null,
            ////            }
            ////        });
            ////Console.Write(respuesta.ToXml());
            ////Console.Read();

            /////////////////////////////////////////////////////////////////////////////////
            //// AFIP 
            ////////////////////////////////////////////////////////////////////////////////////
            //var servicioAfip = new ChannelFactory<CTGServicePortType>("CTGServiceHttpSoap11Endpoint").CreateChannel();

            //var auth = new authType
            //{
            //    cuitRepresentado = 30715118773,
            //    sign = "aeqZhyh8v+nDVyAw5FR1OJAVuHBXBuw++zwHeU1iKjpVNGi2kdQqOQWZv+jROXVW7zC5DRzfyiZzCkUCdkRpB033eHuPXmyn+Xd2k3EckQOc1c/FPLu/U1f4E3zRRBzRYaOnKAvIabvjL7r1bYjlyhg/hCIE10tOn/RHxKdHa8M=",
            //    token = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9InllcyI/Pgo8c3NvIHZlcnNpb249IjIuMCI+CiAgICA8aWQgc3JjPSJDTj13c2FhLCBPPUFGSVAsIEM9QVIsIFNFUklBTE5VTUJFUj1DVUlUIDMzNjkzNDUwMjM5IiBkc3Q9ImNuPXdzY3RnLG89YWZpcCxjPWFyIiB1bmlxdWVfaWQ9IjkwNzYwMjc5NyIgZ2VuX3RpbWU9IjE1MjQ1NjkyNjgiIGV4cF90aW1lPSIxNTI0NjEyNTI4Ii8+CiAgICA8b3BlcmF0aW9uIHR5cGU9ImxvZ2luIiB2YWx1ZT0iZ3JhbnRlZCI+CiAgICAgICAgPGxvZ2luIGVudGl0eT0iMzM2OTM0NTAyMzkiIHNlcnZpY2U9IndzY3RnIiB1aWQ9IkM9YXIsIE89bW9saW5vcyBhZ3JvIHMuYS4sIFNFUklBTE5VTUJFUj1DVUlUIDMwNzE1MTE4NzczLCBDTj1tb2EiIGF1dGhtZXRob2Q9ImNtcyIgcmVnbWV0aG9kPSIyMiI+CiAgICAgICAgICAgIDxyZWxhdGlvbnM+CiAgICAgICAgICAgICAgICA8cmVsYXRpb24ga2V5PSIzMDcxNTExODc3MyIgcmVsdHlwZT0iNCIvPgogICAgICAgICAgICA8L3JlbGF0aW9ucz4KICAgICAgICA8L2xvZ2luPgogICAgPC9vcGVyYXRpb24+Cjwvc3NvPgo="
            //};

            //var consultarCTGRequest = new consultarDetalleCTGRequestType
            //{
            //    auth = auth,
            //    ctg = Convert.ToInt64("85492796")
            //};

            //var respuesta = servicioAfip.consultarDetalleCTG(new consultarDetalleCTGRequest { request = consultarCTGRequest });

            //Console.WriteLine(respuesta.ToXml());
            //Console.Read();
            //////////////////////////////////////////////////////////////////////////////
            //servicioCompliance //
            /// //////////////////////////////////////////////////////////////////////////////

            //Console.Write("Cuit transportista (ej: 33-71016453-9): ");
            //var cuit = Console.ReadLine();
            //Console.Write("DNI chofer(ej: 16807121): ");
            //var dni = Console.ReadLine();
            //Console.Write("Patente (ej: HRA187): ");
            //var patente1 = Console.ReadLine();
            //controlarDatosAgroacopiosRequest datosGranelesRequest = new controlarDatosAgroacopiosRequest()
            //{
            //    datos =
            //        new Datos() { cuit = "20-24440480-5", dni = "17279340", patente1 = "LTI578", planta = "5" }
            //};
            //Console.WriteLine("///////////////////////////Request///////////////////////////");
            //Console.WriteLine(datosGranelesRequest.ToXml());
            //Console.WriteLine("///////////////////////////////////////////////////////////////////");
            //var servicioCompliance = new ChannelFactory<DatosPort>("DatosPort").CreateChannel();

            //var respuesta = servicioCompliance.controlarDatosAgroacopios(datosGranelesRequest);
            //Console.WriteLine("///////////////////////////Response///////////////////////////");
            //Console.WriteLine(respuesta.ToXml());
            //Console.WriteLine("///////////////////////////////////////////////////////////////////");
            //Console.Read();
            //////////////////////////////////////////////////////////////////////////////
            //consultar existe pedido de traslado //
            //////////////////////////////////////////////////////////////////////////////
            //var factory = new ChannelFactory<ZSDWS_SCATO>("ZSDWS_SCATO");
            //var defaultCredentials = factory.Endpoint.Behaviors.Find<ClientCredentials>();
            //factory.Endpoint.Behaviors.Remove(defaultCredentials);

            ////////////step two -instantiate your credentials
            //ClientCredentials loginCredentials = new ClientCredentials();
            //loginCredentials.UserName.UserName = "GWEBSRV_SCA";
            //loginCredentials.UserName.Password = "SCA%2016";

            //////step three -set that as new endpoint behavior on factory
            //factory.Endpoint.Behaviors.Add(loginCredentials); //add required ones
            //var srvSap = factory.CreateChannel();
            //var request = new DatosClienteRequest(new DatosCliente
            //{
            //    CUIT = string.Empty,
            //    Fecha = string.Empty,
            //    IdSAP = "9811111114"
            //});
            //var datosSap = srvSap.DatosCliente(request
            //    );
            //Console.Write(request.ToXml());
            //Console.Write(datosSap.ToXml());
            //Console.Read();
            //Console.WriteLine("Centro codigo sap:");
            //var centro = Console.ReadLine();
            //Console.WriteLine("Patnete en mayusculas:");
            //var patente = Console.ReadLine();
            //var retorno = srvSap.VerifPedTrasladoRedespacho(new VerifPedTrasladoRedespachoRequest
            //{
            //    VerifPedTrasladoRedespacho = new VerifPedTrasladoRedespacho
            //    {
            //        CentroDestino = "1036",
            //        CentroEmisor = "1127",
            //        Material = "19908036",
            //        Transportista = "30624903311"
            //    }
            //});
            //Console.Write(retorno.ToXml());
            //Console.Read();
            //////////////////////////////////////////////////////////////////////////////
            // COT //
            //////////////////////////////////////////////////////////////////////////////
            ////////var factory = new ChannelFactory<ZSDWS_SCATO>("ZSDWS_SCATO");
            ////////var defaultCredentials = factory.Endpoint.Behaviors.Find<ClientCredentials>();
            ////////factory.Endpoint.Behaviors.Remove(defaultCredentials);

            ////////////step two -instantiate your credentials
            //ClientCredentials loginCredentials = new ClientCredentials();
            //loginCredentials.UserName.UserName = "GWEBSRV_SCA";
            //loginCredentials.UserName.Password = Encriptador.Decrypt("h88WYKNtezwGAqJMt205vw==");

            ////////////step three -set that as new endpoint behavior on factory
            ////////factory.Endpoint.Behaviors.Add(loginCredentials); //add required ones
            //var srvSap = factory.CreateChannel();

            ////////var respuesta =
            ////////        srvSap.ValidacionCOT(new ValidacionCOTRequest
            ////////        {
            ////////            ValidacionCOT =
            ////////                new ValidacionCOT
            ////////                {
            ////////                    PtoVtaRemito = "0424",
            ////////                    NroComprobante = "00018650",
            ////////                    DocInternoSAP = "4900701712",
            ////////                }
            ////////        });
            ////////Console.Write(respuesta.ToXml());
            ////////Console.Read();
            //////////////////////////////////////////////////////////////////////////////
            //consultar pedido //
            ////////////////////////////////////////////////////////////////////////////////
            //var factory = new ChannelFactory<ZSDWS_SCATO>("ZSDWS_SCATO");
            //var defaultCredentials = factory.Endpoint.Behaviors.Find<ClientCredentials>();
            //factory.Endpoint.Behaviors.Remove(defaultCredentials);

            //// step two - instantiate your credentials
            //ClientCredentials loginCredentials = new ClientCredentials();
            //loginCredentials.UserName.UserName = "GWEBSRV_SCA";
            //loginCredentials.UserName.Password =
            //Encriptador.Decrypt("ipiYlSviW3QB7BjrHn+7vw==");
            //Encriptador.Decrypt("RQsncbUlOC0T9iEPjcvyPg==");

            //// step three - set that as new endpoint behavior on factory
            //factory.Endpoint.Behaviors.Add(loginCredentials); //add required ones
            //var srvSap = factory.CreateChannel();
            //var resultado = srvSap.DatosProveedores(
            //    new DatosProveedoresRequest(new DatosProveedores
            //    {
            //        CUIT = "30711265658",
            //        Fecha = string.Empty,
            //        IdSAP = string.Empty,
            //        TipoProveedor = "PR"
            //    })); ;
            //Console.Write(resultado.ToXml());
            //resultado = srvSap.DatosProveedores(
            //   new DatosProveedoresRequest(new DatosProveedores
            //   {
            //       CUIT = "30506792165",
            //       Fecha = string.Empty,
            //       IdSAP = string.Empty,
            //       TipoProveedor = "AM"
            //   })); ;
            //Console.Write(resultado.ToXml());
            //resultado = srvSap.DatosProveedores(
            //   new DatosProveedoresRequest(new DatosProveedores
            //   {
            //       CUIT = "30506792165",
            //       Fecha = string.Empty,
            //       IdSAP = string.Empty,
            //       TipoProveedor = "VM"
            //   })); ;
            //Console.Write(resultado.ToXml());
            //resultado = srvSap.DatosProveedores(
            //   new DatosProveedoresRequest(new DatosProveedores
            //   {
            //       CUIT = "30506792165",
            //       Fecha = string.Empty,
            //       IdSAP = string.Empty,
            //       TipoProveedor = "CM"
            //   })); ;
            //Console.Write(resultado.ToXml());
            //Console.Read();



            //////////////////////////////////////////////////////////////////////////////
            //consultar pedido //
            //////////////////////////////////////////////////////////////////////////////
            //var factory = new ChannelFactory<ZSDWS_SCATO>("ZSDWS_SCATO");
            //var defaultCredentials = factory.Endpoint.Behaviors.Find<ClientCredentials>();
            //factory.Endpoint.Behaviors.Remove(defaultCredentials);

            //// step two - instantiate your credentials
            //ClientCredentials loginCredentials = new ClientCredentials();
            //loginCredentials.UserName.UserName = "WEBSRV_SCA";
            //loginCredentials.UserName.Password = "prueba";

            //// step three - set that as new endpoint behavior on factory
            //factory.Endpoint.Behaviors.Add(loginCredentials); //add required ones
            //var srvSap = factory.CreateChannel();
            //Console.WriteLine("orden de compra:");
            //var centro = Console.ReadLine();
            //var retorno = srvSap.ConsultaPedido(new ConsultaPedidoRequest(new ConsultaPedido{ Pedido = centro}));

            //Console.Write(retorno.ToXml());
            //Console.Read();
            //////////////////////////////////////////////////////////////////////////////
            //consultar orden de carga//
            //////////////////////////////////////////////////////////////////////////////
            var factory = new ChannelFactory<ZSDWS_SCATO>("ZSDWS_SCATO");
            var defaultCredentials = factory.Endpoint.Behaviors.Find<ClientCredentials>();
            factory.Endpoint.Behaviors.Remove(defaultCredentials);

            // step two - instantiate your credentials
            ClientCredentials loginCredentials = new ClientCredentials();
            loginCredentials.UserName.UserName = "GWEBSRV_SCA";
            loginCredentials.UserName.Password = "scatoqa2015"; //h88WYKNtezwGAqJMt205vw==

            // step three - set that as new endpoint behavior on factory
            factory.Endpoint.Behaviors.Add(loginCredentials); //add required ones
            var srvSap = factory.CreateChannel();
            Console.WriteLine("Centro codigo sap:");
            var centro = Console.ReadLine();
            Console.WriteLine("Patnete en mayusculas:");
            var patente = Console.ReadLine();
            var retorno = srvSap.ConsultaOrdenDeCarga(new ConsultaOrdenDeCargaRequest(new ConsultaOrdenDeCarga
            {
                Centro = "1029",
                Patente = "AAA111"
            }));

            Console.Write(retorno.ToXml());
            Console.Read();
            //////////////////////////////////////////////////////////////////////////////
            //consultar proveedor//
            //////////////////////////////////////////////////////////////////////////////
            //var factory = new ChannelFactory<ZSDWS_SCATO>("ZSDWS_SCATO");
            //var defaultCredentials = factory.Endpoint.Behaviors.Find<ClientCredentials>();
            //factory.Endpoint.Behaviors.Remove(defaultCredentials);

            //// step two - instantiate your credentials
            //ClientCredentials loginCredentials = new ClientCredentials();
            //loginCredentials.UserName.UserName = "GWEBSRV_SCA";
            //loginCredentials.UserName.Password = "SCA%2016";

            //// step three - set that as new endpoint behavior on factory
            //factory.Endpoint.Behaviors.Add(loginCredentials); //add required ones
            //var srvSap = factory.CreateChannel();
            //var request = new DatosProveedores
            //{
            //    CUIT = string.Empty,
            //    IdSAP = string.Empty,
            //    Fecha = string.Empty,
            //    TipoProveedor = "PR"
            //};
            //var retorno = srvSap.DatosProveedores(
            //    new DatosProveedoresRequest(request));
            //Console.Write(request.ToXml());
            //Console.Write(retorno.ToXml());
            //Console.Read();



            //////////////////////////////////////////////////////////////////////////////
            //imprimir etiqueta //
            /// //////////////////////////////////////////////////////////////////////////////
            //servicioComandos = new ChannelFactory<IServicioComandos>("ServicioComandos").CreateChannel();
            //servicioComandos.Ejecutar(new ImprimirEtiquetaIntacta
            //{
            //    CantidadCopias = 1,
            //    Dto = new ImpEtiquetaIntactaDto
            //    {
            //        Centro = "San lorenzo",
            //        Codigo = "test",
            //        FechaImpresion = DateTime.Now,
            //        Impresora = "\\\\vicfsp02\\ptandadm602",
            //        LaboratiorioCuit = "20-0000000-8",
            //        LaboratorioNombre = "test",
            //        Material = "test",
            //        NombreUsuario = "oliveraa",
            //        NumeroCartaPorte = "000000000000",
            //        Patente = "AAA000",
            //        TipoDeAnalisis = "test",
            //        WorkflowId = new Guid()
            //    }
            //});
            //////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////
            //buscador de inconsistencias //
            //////////////////////////////////////////////////////////////////////////////
            //srvRepositorio = new ChannelFactory<IServicioRepositorio>("ServicioRepositorio").CreateChannel();
            //servicioComandos = new ChannelFactory<IServicioComandos>("ServicioComandos").CreateChannel();
            //servicioListaDeWorkflows = new ChannelFactory<IListaDeWorkflows>("ListaDeWorkflows").CreateChannel();

            //while (true)
            //{
            //    Console.WriteLine("Ingrese la Patente: ");
            //    var workflow = servicioListaDeWorkflows.ObtenerWorkflowPorPatente(Console.ReadLine());

            //    var log = srvRepositorio.ObtenerUltimoLogActividad(workflow.Id, workflow.ProximaAccion);

            //    if (log == null)
            //    {
            //        Console.WriteLine("Inconcistencia: no existe el log actividad para el camion {0}, actividad {1}",
            //                          workflow.Patente, workflow.ProximaAccion);
            //        Console.WriteLine("Corregir problema? ");
            //        if (Console.ReadLine() == "OK")
            //        {
            //            var resultado = servicioComandos.Ejecutar(new CrearLogActividad
            //            {
            //                Dto = new LogActividadDto
            //                {
            //                    Actividad = Textos.ResourceManager.GetString("Act" + workflow.ProximaAccion),
            //                    ActividadXaml = workflow.ProximaAccion,
            //                    WorkflowInstanceId = workflow.Id
            //                }
            //            });
            //            if (resultado.HayErrores)
            //            {
            //                Console.WriteLine("Error: " + resultado.Errores.Values.First());
            //                Console.ReadLine();
            //                return;
            //            }
            //            Console.WriteLine("Ok");
            //        }
            //    }
            //    else if (workflow.ProximaAccion == "Calado")
            //    {
            //        Console.WriteLine("Verificamos que este bien cargado el calado");
            //        var calado = srvRepositorio.ObtenerCaladoPorGuid(workflow.Id);
            //        if (calado == null)
            //        {
            //            Console.WriteLine("Falta calado");
            //            Console.WriteLine("Corregir problema? ");
            //            if (Console.ReadLine() == "OK")
            //            {
            //                var resultado2 =
            //                    servicioComandos.Ejecutar(new ModificarRecorridoCalado
            //                        {
            //                            WorkflowInstanceId = workflow.Id
            //                        });
            //                if (resultado2.HayErrores)
            //                {
            //                    Console.WriteLine("Error: " + resultado2.Errores.Values.First());
            //                    Console.ReadLine();
            //                    return;
            //                }
            //                Console.WriteLine("Ok");
            //            }
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine("Ok");
            //    }




            //    Console.WriteLine("Fin");
            //}
        }
    }
}
