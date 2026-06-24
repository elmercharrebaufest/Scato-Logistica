using System;
using System.Activities;
using System.Configuration;
using System.Linq;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Ninject.Parameters;
using Ninject;
using static Molinos.Scato.Dominio.Constantes;
using System.ServiceModel;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Actividades
{
    public class IniciarWorkflowEgreso : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        [RequiredArgument]
        public InArgument<CartaPorteDto> Orden { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }

        public InArgument<int> PuestoDeTrabajoId { get; set; }
        public InArgument<string> CupoSalida { get; set; }
        public InArgument<int> KmARecorrerSalida { get; set; }
        public InArgument<decimal> TarifaDeSalida { get; set; }


        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();

            try
            { 
            
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var servicioFactory = context.GetExtension<IServicioActividadFactory<ICargarCartaPorteByPassService>>();
                var sericioRepositorio = context.GetExtension<IServicioRepositorio>();
                var logger = context.GetExtension<ILogger>();

                var instanceId = InstanceId.Get<Guid>(context);
                var centroId = CentroId.Get<int>(context);
                var puestoDeTrabajo = PuestoDeTrabajoId.Get<int>(context);
                var orden = Orden.Get<CartaPorteDto>(context);
                var cupoSalida = CupoSalida.Get<string>(context);
                var kmRecorrerSalida = KmARecorrerSalida.Get<int>(context);
                var tarifaSalida = TarifaDeSalida.Get<decimal>(context);

                var workflow = sericioRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass, Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.WorkFlowEgreso , centroId).Valor;
                
                var recorrido = sericioRepositorio.ObtenerRecorridoPorGuid(instanceId);
                if (recorrido == null)
                {
                    return resultado;
                }

                logger.Debug($"Iniciando Workflow Egreso para Recorrido {recorrido.Id} con Workflow {workflow} y WorkflowInstanceId {instanceId}");
                orden.Vehiculos.First().PesoBrutoOrigen = recorrido.PesoBruto;
                orden.Vehiculos.First().PesoNetoOrigen = recorrido.PesoNeto;
                orden.Vehiculos.First().PesoTaraOrigen = recorrido.PesoTara;

                logger.Debug($"Pesos para Recorrido {recorrido.Id} - PesoBruto: {recorrido.PesoBruto}, PesoNeto: {recorrido.PesoNeto}, PesoTara: {recorrido.PesoTara}");
                var adicionales = new AdicionalesCartaPorteByPassDto {
                  Calado = recorrido.Calado,
                  EstablecimientoId = recorrido.Establecimiento is null ? null : recorrido?.Establecimiento.Id,
                  AlmacenId = recorrido.Almacen is null ? null : recorrido?.Almacen.Id,
                  NombreUsuario = recorrido.Usuario,
                  RecorridoIdIngreso = recorrido.Id
                };
                

                orden.CartaPorteByPass = adicionales;

                var workflowDefinicionId = sericioRepositorio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);

                var controlRecorrido = GenerarControlRecorrido(recorrido.Usuario , puestoDeTrabajo);
                orden.Cupo = cupoSalida;
                orden.KmARecorrer = kmRecorrerSalida.ToString();
                orden.TarifaTonelada = tarifaSalida;
                servicioComandos.Ejecutar(new CrearLogActividad { Dto = new LogActividadDto { Actividad = "Invocando Nuevo Workflow", WorkflowInstanceId = instanceId } });
                var uri = ConfigurationManager.AppSettings["UrlBaseWorkflow"] + workflowDefinicionId + ".xamlx";
                var canal = new ChannelFactory<ICargarCartaPorteService>(new BasicHttpBinding("CommonBinding"), new EndpointAddress(uri)).CreateChannel();
                resultado = canal.CargarCartaPorte(orden, orden.Vehiculos.First(), centroId, workflow, workflowDefinicionId, recorrido.Usuario, controlRecorrido);
           
                if (resultado.HayErrores)
                {
                    servicioComandos.Ejecutar(new CrearControlRecorrido
                    {
                        Dto = new ControlRecorridoDto
                        {
                            Actividad = "InvocarWorkflowByPassEgreso",
                            Fecha = DateTime.Now,
                            Comentario = resultado.Errores.FirstOrDefault().Value,
                            NombreUsuario = recorrido.Usuario,
                            WorkflowInstanceId = instanceId,
                        }
                    });
                }
            }
            catch (Exception)
            {
                resultado.Errores.Add("", "Error al iniciar");
            }
            return resultado;
        }

        private ControlRecorridoDto GenerarControlRecorrido(string nombreUsuario , int puestoDeTrabajoId )
        {
            return new ControlRecorridoDto { Actividad = Textos.ActCargarCartaPorte, ActividadXaml = "CargarCartaPorteByPass", PuestoDeTrabajoId = puestoDeTrabajoId, NombreUsuario = nombreUsuario };
        }
    }
}
