using Microsoft.ApplicationServer.StoreManagement.Control;
using Microsoft.ApplicationServer.StoreManagement.Query;
using Microsoft.ApplicationServer.StoreManagement.Sql.Control;
using Microsoft.ApplicationServer.StoreManagement.Sql.Query;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Behavior;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Threading;
using System.Web.Configuration;

namespace Molinos.Scato.Workflow
{
    /// <summary>
    /// Servicio que provee el listado de Workflows activos que están esperando por alguna acción del usuario.
    /// Provee la información estandard provista por WF e información custom agregada por <see cref="ScatoPersistenceParticipant"/>
    /// </summary>
    ///
    [AiErrorHandlerBehaviorAttribute]
    public class ListaDeWorkflows : IListaDeWorkflows, IDisposable
    {
        private readonly ILogger log;
        private readonly IServicioRepositorio servicioRepositorio;

        private InstanceQuery instanceQuery;
        private ManualResetEvent waiter;

        public ListaDeWorkflows(ILogger log, IServicioRepositorio servicioRepositorio)
        {
            this.log = log;
            this.servicioRepositorio = servicioRepositorio;
        }

        private void CreateInstanceQuery()
        {
            var factory = new SqlInstanceQueryProvider();
            string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServerWorkflowInstanceStoreConnectionString"].ConnectionString;
            factory.Initialize("storeA", new NameValueCollection { { "ConnectionString", connectionString } });
            instanceQuery = factory.CreateInstanceQuery();
        }

        private InstanceControl CreateInstanceControl()
        {
            var factoryControl = new SqlInstanceControlProvider();
            var connectionString = ConfigurationManager.ConnectionStrings["ApplicationServerWorkflowInstanceStoreConnectionString"].ConnectionString;
            factoryControl.Initialize("storeA", new NameValueCollection { { "ConnectionString", connectionString } });
            var instanceControl = factoryControl.CreateInstanceControl();
            return instanceControl;
        }

        public Resultado EliminarInstanciaWorkflow(Guid instanciaId)
        {
            var resultado = new Resultado();
            var instanceControl = CreateInstanceControl();
            var waiterControl = new ManualResetEvent(false);
            var command = new InstanceCommand { CommandType = CommandType.Delete, InstanceId = instanciaId };
            instanceControl.CommandSend.BeginSend(command, TimeSpan.FromSeconds(20),
                asyncResult =>
                {
                    var res = (Resultado)asyncResult.AsyncState;
                    try
                    {
                        instanceControl.CommandSend.EndSend(asyncResult);
                        if (!asyncResult.IsCompleted)
                        {
                            res.Error("", "Ocurrió un error al eliminar la instancia del workflow");
                        }
                    }
                    catch (Exception e)
                    {
                        res.Error("", "Ocurrió un error al eliminar la instancia del workflow: " + e.Message);
                    }
                    finally
                    {
                        waiterControl.Set();
                    }
                }, resultado);
            waiterControl.WaitOne();
            return resultado;
        }

        public Resultado ResumirInstanciaWorkflow(Guid instanciaId)
        {
            CreateInstanceQuery();
            var instanceQueryExecuteArgs = new InstanceQueryExecuteArgs
            {
                InstanceId = new[] { instanciaId }
            };
            waiter = new ManualResetEvent(false);
            var hostInfo = new Dictionary<string, string>();
            instanceQuery.BeginExecuteQuery(instanceQueryExecuteArgs, TimeSpan.FromSeconds(60), ObtenerHostInfo,
                                            hostInfo);
            waiter.WaitOne();
            waiter.Close();

            var resultado = new Resultado();
            var instanceControl = CreateInstanceControl();
            var waiterControl = new ManualResetEvent(false);

            var command = new InstanceCommand { CommandType = CommandType.Resume, InstanceId = instanciaId, ServiceIdentifier = hostInfo };
            instanceControl.CommandSend.BeginSend(command, TimeSpan.FromSeconds(20),
                asyncResult =>
                {
                    var res = (Resultado)asyncResult.AsyncState;
                    try
                    {
                        instanceControl.CommandSend.EndSend(asyncResult);
                        if (!asyncResult.IsCompleted)
                        {
                            res.Error("", "Ocurrió un error al resumir la instancia del workflow");
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, e.Message + (e.InnerException != null ? " " + e.InnerException.Message : ""));
                        res.Error("", "Ocurrió un error al resumir la instancia del workflow: " + e.Message);
                    }
                    finally
                    {
                        waiterControl.Set();
                    }
                }, resultado);
            waiterControl.WaitOne();
            return resultado;
        }

        /// <summary>
        /// Provee una lista paginada de Workflows activos que están esperando por alguna acción del usuario, con la posibilidad de filtrarlos por
        /// centro, workflow, proxima accion, patente, tipo documento de ingreso y numero documento de ingreso
        /// </summary>
        /// <param name="filtro">El objeto por el cual se filtran los resultados. Si se pasa null se devuelven todos los resultados sin filtrar por este campo</param>
        /// <param name="paginacion">indica el numero de items por pagina y la pagina buscada</param>
        /// <returns>Una lista de <see cref="InstanciaWorkflowDto"/></returns>
        public ListarWorkFlowsDto ListarWorkFlows(Paginacion paginacion, FiltroListaDeWorkflowsDto filtro)
        {
            var resultado = ObtenerWorkFlows();
            if (filtro.MostrarCamionesPendientes && filtro.MostrarCamionesPendientesNoGranos)
            {
                var pendientes = servicioRepositorio.ListarDatosDeWorkflowsPendientes(filtro.CentroId ?? 0, 0);
                if (filtro.MostrarCamionesPendientes)
                {
                    resultado = resultado.Union(pendientes.Where(x => x.ProximaAccion == PermisosScato.CamionesPendientesMesa.Text()));
                }
                if (filtro.MostrarCamionesPendientesNoGranos)
                {
                    resultado = resultado.Union(pendientes.Where(x => x.ProximaAccion == PermisosScato.CamionesPendientesNoGranos.ToString()));
                }
            }
            var resultadoWorkflows = FiltrarWorkFlows(resultado, filtro);
            resultadoWorkflows = servicioRepositorio.ConsultarEstadoWorkflow(resultadoWorkflows);

            var listarWorkflows = new ListarWorkFlowsDto
            {
                InstanciasWorkflowDto = ListarWorkFlows(resultadoWorkflows.InstanciasWorkflowDto, filtro, paginacion),
                ProximasAcciones = resultadoWorkflows.InstanciasWorkflowDto.Where(w => w.ProximaAccion != null).Select(s => s.ProximaAccion).Distinct().OrderBy(x => x).ToList(),
            };

            var datos = servicioRepositorio.ListarDatosDeWorkflows(listarWorkflows.InstanciasWorkflowDto.Select(x => x.Id).Distinct().ToList());

            foreach (var instanciaWorkflowDto in listarWorkflows.InstanciasWorkflowDto)
            {
                var dato = datos.FirstOrDefault(x => x.Id == instanciaWorkflowDto.Id);
                if (dato != null)
                {
                    instanciaWorkflowDto.RecorridoId = dato.RecorridoId;
                    instanciaWorkflowDto.Material = dato.Material;
                    instanciaWorkflowDto.MaterialId = dato.MaterialId;
                    instanciaWorkflowDto.MaterialCodigoSap = dato.MaterialCodigoSap;
                    instanciaWorkflowDto.Transportista = dato.Transportista;
                    instanciaWorkflowDto.TransportistaId = dato.TransportistaId;
                    instanciaWorkflowDto.Cuit = dato.Cuit;
                    instanciaWorkflowDto.Patente = dato.Patente;
                    instanciaWorkflowDto.Rechazado = dato.Rechazado;
                    instanciaWorkflowDto.Workflow = dato.Workflow;
                    instanciaWorkflowDto.ChoferDNI = dato.ChoferDNI;
                    instanciaWorkflowDto.ChoferNombre = dato.ChoferNombre;
                    instanciaWorkflowDto.NumeroDeTarjeta = dato.NumeroDeTarjeta;
                    instanciaWorkflowDto.EsSustentable = dato.Sustentable;
                    instanciaWorkflowDto.Entregador = dato.Entregador;
                    instanciaWorkflowDto.EsEspecial = dato.EsEspecial;
                    instanciaWorkflowDto.CaracteristicasNoCorrenspodenEspecial = dato.CaracteristicasNoCorrenspodenEspecial;
                    instanciaWorkflowDto.AnalisisObligatorio = dato.AnalisisObligatorio && instanciaWorkflowDto.ProximaAccion == "Calado";
                    instanciaWorkflowDto.VehiculoDemorado = dato.VehiculoDemorado;
                    instanciaWorkflowDto.LlegoEnHorario = dato.LlegoEnHorario;
                    instanciaWorkflowDto.Proteina = dato.Proteina;
                    instanciaWorkflowDto.AlmacenDestino = dato.AlmacenDestino;
                    instanciaWorkflowDto.DiferenciaPesoNeto = dato.DiferenciaPesoNeto.HasValue ? dato.DiferenciaPesoNeto.ToString() : "";
                    instanciaWorkflowDto.TipoVariedadDescripcion = dato.TipoVariedadDescripcion;
                    instanciaWorkflowDto.TipoVariedadColorTexto = dato.TipoVariedadColorTexto;
                }
                instanciaWorkflowDto.NumeroDocumentoDeIngreso = instanciaWorkflowDto.NumeroDocumentoDeIngreso is null ? instanciaWorkflowDto.CTG : instanciaWorkflowDto.NumeroDocumentoDeIngreso;
            }
            return listarWorkflows;
        }

        public IList<InstanciaWorkflowPuertoDto> ListarEmbarques(string filtroProximaAccion = null)
        {
            //var resultado = ObtenerTotalWorkflows().Where(x => x.TipoVehiculo == TipoVehiculo.Vapor && (string.IsNullOrEmpty(filtroProximaAccion) || x.ProximaAccion == filtroProximaAccion))

            var resultado = ObtenerWorkFlows().Where(x => x.TipoVehiculo == TipoVehiculo.Vapor && (string.IsNullOrEmpty(filtroProximaAccion) || x.ProximaAccion == filtroProximaAccion))
                    .Select(x => new InstanciaWorkflowPuertoDto
                    {
                        Id = x.Id,
                        FechaUltimaModificacion = x.FechaUltimaModificacion,
                        ProximaAccion = x.ProximaAccion
                    }).ToList();
            //
            var ids = resultado.Select(x => x.Id).Distinct().ToList();
            var datos = servicioRepositorio.ListarDatosDeWorkflowsPuerto(ids);
            //
            foreach (var instanciaWorkflowDto in resultado)
            {
                var embarque = datos.Embarques.FirstOrDefault(x => x.InstanciaWorkflow == instanciaWorkflowDto.Id);
                var lineup = datos.LineUps.FirstOrDefault(x => x.InstanciaWorkflow == instanciaWorkflowDto.Id);
                instanciaWorkflowDto.Embarque = embarque;
                instanciaWorkflowDto.LineUp = lineup;
                if (embarque != null && lineup != null)
                {
                    lineup.Ubicacion = embarque.Ubicacion;
                }
            }

            return resultado.FindAll(x => x.Embarque != null && x.Embarque.Ubicacion != 1).OrderBy(x => x.LineUp != null ? x.LineUp.Orden : int.MaxValue).ToList();
            //return resultado.OrderBy(x => x.LineUp != null ? x.LineUp.Orden : int.MaxValue).ToList();
        }

        public ListarWorkFlowsDto ListarTotalWorkFlows(Paginacion paginacion, FiltroListaDeWorkflowsDto filtro)
        {
            var lista = ObtenerTotalWorkflows();
            var resultadoWorkflows = FiltrarWorkFlowsPanel(lista, filtro);
            var listarWorkflows = new ListarWorkFlowsDto
            {
                InstanciasWorkflowDto = ListarWorkFlows(resultadoWorkflows.InstanciasWorkflowDto, paginacion)
            };

            return listarWorkflows;
        }

        private WorkFlowsFiltradosDto FiltrarWorkFlowsPanel(IEnumerable<InstanciaWorkflowDto> instanciasWorkflow, FiltroListaDeWorkflowsDto filtro)
        {
            var resultado = new WorkFlowsFiltradosDto();
            if (filtro != null)
            {
                Func<InstanciaWorkflowDto, bool> expresionFiltro =
                    x => x.CentroId == filtro.CentroId
                            && ((filtro.Workflow != null && x.Workflow != null && x.Workflow.Contains(filtro.Workflow)) || filtro.Workflow == null)
                            && ((filtro.Patente != null && x.Patente.ToLower().Contains(filtro.Patente.ToLower())) || filtro.Patente == null)
                            && ((filtro.TipoDocumentoDeIngreso != null && x.TipoDocumentoDeIngreso == filtro.TipoDocumentoDeIngreso) || filtro.TipoDocumentoDeIngreso == null)
                            && ((filtro.NumeroDocumentoDeIngreso != null && x.NumeroDocumentoDeIngreso.Contains(filtro.NumeroDocumentoDeIngreso)) || filtro.NumeroDocumentoDeIngreso == null)
                            && ((filtro.MaterialId.HasValue && x.MaterialId == filtro.MaterialId.Value) || filtro.MaterialId == null)
                            && ((filtro.TipoComercialId.HasValue && x.TipoComercialId == filtro.TipoComercialId) || filtro.TipoComercialId == null)
                            && ((filtro.Condicion.HasValue && (int)x.Condicion == (int)filtro.Condicion) || filtro.Condicion == null);
                instanciasWorkflow = instanciasWorkflow.Where(expresionFiltro);

                resultado.InstanciasWorkflowDto = instanciasWorkflow;
            }
            return resultado;
        }

        public InstanciaWorkflowDto ObtenerWorkflowPorPatente(string patente)
        {
            var resultado = ObtenerWorkFlows();
            return resultado.FirstOrDefault(f => f.Patente.ToLower() == patente.ToLower());
        }

        public InstanciaWorkflowDto ObtenerWorkflowPorGuid(Guid instancia)
        {
            return ObtenerWorkflowInstanciaActiva(instancia);
        }

        public ProximaAccionEjecutableDto ObtenerWorkflowProximaAccionEjecutable(Guid instanciaId, string nombreUsuario, int centroId)
        {
            ProximaAccionEjecutableDto proximaAccionEjecutableEjecutable = null;
            CreateInstanceQuery();
            var instancia = ObtenerWorkflowInstancia(instanciaId);

            if (instancia != null && instancia.Estado == InstanceStatus.Running && instancia.CentroId == centroId)
            {
                // Solo puedo ejecutar la instancia si esta Idle en un receive y el usuario tiene permisos para la actividad
                if (instancia.Condicion == InstanceCondition.Idle)
                {
                    proximaAccionEjecutableEjecutable = new ProximaAccionEjecutableDto
                    {
                        Actividad = instancia.ProximaAccion,
                        Ejecutar = servicioRepositorio.ActividadEsEjecutable(instancia.Workflow, instancia.ProximaAccion, nombreUsuario)
                    };
                }
            }
            else
            {
                // Instancia terminada, inexistente o en otro centro. No hay nada para ejecutar
                proximaAccionEjecutableEjecutable = new ProximaAccionEjecutableDto { Actividad = null, Ejecutar = false };
            }
            return proximaAccionEjecutableEjecutable;
        }

        public bool VerificarExistenciaDeWorkflow(string patente)
        {
            IEnumerable<InstanciaWorkflowDto> resultado = ObtenerWorkFlows();
            return resultado.Any(f => f.Patente.ToLower() == patente.ToLower());
        }

        public bool VerificarExistenciaDeWorkflowPorGuid(Guid instanciaId)
        {
            return ObtenerWorkflowInstanciaActiva(instanciaId) != null;
        }

        public List<string> ObtenerWorkflowProximasAcciones(string nombreUsuario, int centroId)
        {
            var resultado = ObtenerWorkFlows();
            var resultadoworkflows = FiltrarWorkFlows(resultado, new FiltroListaDeWorkflowsDto { NombreUsuario = nombreUsuario, CentroId = centroId });
            return resultadoworkflows.InstanciasWorkflowDto.Where(w => w.ProximaAccion != null).Select(s => s.ProximaAccion).Distinct().OrderBy(x => x).ToList();
        }

        public ProximaAccionDto ObtenerWorkflowProximaAccion(Guid instanciaId)
        {
            CreateInstanceQuery();
            var instanceQueryExecuteArgs = new InstanceQueryExecuteArgs
            {
                InstanceId = new[] { instanciaId }
            };
            var resultado = new ProximaAccionDto
            {
                Mensaje = Textos.Error_WorkfowProximaActividad
            };
            using (var wait = new ManualResetEvent(false))
            {
                instanceQuery.BeginExecuteQuery(instanceQueryExecuteArgs, TimeSpan.FromSeconds(60),
                    result =>
                    {
                        try
                        {
                            var info = instanceQuery.EndExecuteQuery(result).FirstOrDefault();
                            if (info != null)
                            {
                                if (info.InstanceStatus == InstanceStatus.Running)
                                {
                                    if (info.InstanceCondition == InstanceCondition.Idle)
                                    {
                                        resultado.ProximaAccion = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadActividad);
                                        resultado.Mensaje = null;
                                    }
                                    else
                                    {
                                        resultado.Mensaje = Textos.Error_WorkflowNoIdle;
                                        log.Debug("Workflow {0} aun corriendo", info.InstanceId);
                                    }
                                }
                                else
                                {
                                    resultado.Mensaje = Textos.Error_WorkflowSuspendido;
                                    log.Error("Error encontrado al intentar ejecutar el wf {0}, error {1}: {2}", info.InstanceId, info.ExceptionName, info.ExceptionMessage);
                                }
                            }
                        }
                        finally { wait.Set(); }
                    }, null);
                wait.WaitOne();
            }

            return resultado;
        }

        private InstanciaWorkflowDto ObtenerWorkflowInstancia(Guid instanciaId)
        {
            CreateInstanceQuery();
            var instanceQueryExecuteArgs = new InstanceQueryExecuteArgs
            {
                InstanceId = new[] { instanciaId }
            };
            waiter = new ManualResetEvent(false);
            var resultadoPrueba = new List<InstanciaWorkflowDto>();
            instanceQuery.BeginExecuteQuery(instanceQueryExecuteArgs, TimeSpan.FromSeconds(60), ExecuteQueryCallback,
                                            resultadoPrueba);
            waiter.WaitOne();
            waiter.Close();
            var instancia = resultadoPrueba.FirstOrDefault();
            return instancia;
        }

        private InstanciaWorkflowDto ObtenerWorkflowInstanciaActiva(Guid instanciaId)
        {
            CreateInstanceQuery();
            var instanceQueryExecuteArgs = new InstanceQueryExecuteArgs
            {
                InstanceStatus = InstanceStatus.Running,
                InstanceCondition = InstanceCondition.Idle,
                InstanceId = new[] { instanciaId }
            };
            waiter = new ManualResetEvent(false);
            var resultadoPrueba = new List<InstanciaWorkflowDto>();
            instanceQuery.BeginExecuteQuery(instanceQueryExecuteArgs, TimeSpan.FromSeconds(60), ExecuteQueryCallback,
                                            resultadoPrueba);
            waiter.WaitOne();
            waiter.Close();
            var instancia = resultadoPrueba.FirstOrDefault();
            return instancia;
        }

        private IEnumerable<InstanciaWorkflowDto> ObtenerWorkFlows()
        {
            var instanceQueryExecuteArgsRunning = new InstanceQueryExecuteArgs()
            {
                InstanceStatus = InstanceStatus.Running,
                InstanceCondition = InstanceCondition.Idle
            };

            var instanceQueryExecuteArgsException = new InstanceQueryExecuteArgs()
            {
                InstanceStatus = InstanceStatus.Suspended,
                InstanceCondition = InstanceCondition.Exception,
            };

            var instanceQueryExecuteArgsSuspended = new InstanceQueryExecuteArgs()
            {
                InstanceStatus = InstanceStatus.Suspended,
                InstanceCondition = InstanceCondition.UserSuspension,
            };

            var resultadoFinal = new List<InstanciaWorkflowDto>();
            var resultadoRunning = new List<InstanciaWorkflowDto>();
            var resultadoException = new List<InstanciaWorkflowDto>();
            var resultadoSuspended = new List<InstanciaWorkflowDto>();

            // PARA RUNNING
            CreateInstanceQuery();
            var instanceQueryExecuteArgs = new InstanceQueryExecuteArgs { InstanceStatus = InstanceStatus.Running, InstanceCondition = InstanceCondition.Idle };
            waiter = new ManualResetEvent(false);
            var resultadoPrueba = new List<InstanciaWorkflowDto>();
            instanceQuery.BeginExecuteQuery(instanceQueryExecuteArgs, TimeSpan.FromSeconds(60), ExecuteQueryCallback, resultadoPrueba);
            waiter.WaitOne();
            waiter.Close();
            return resultadoPrueba;
        }

        public IEnumerable<InstanciaWorkflowDto> ObtenerTotalWorkflows()
        {
            var instanceQueryExecuteArgsRunning = new InstanceQueryExecuteArgs()
            {
                InstanceStatus = InstanceStatus.Running,
                InstanceCondition = InstanceCondition.Idle
            };

            var instanceQueryExecuteArgsException = new InstanceQueryExecuteArgs()
            {
                InstanceStatus = InstanceStatus.Suspended,
                InstanceCondition = InstanceCondition.Exception,
            };

            var instanceQueryExecuteArgsSuspended = new InstanceQueryExecuteArgs()
            {
                InstanceStatus = InstanceStatus.Suspended,
                InstanceCondition = InstanceCondition.UserSuspension,
            };

            var resultadoFinal = new List<InstanciaWorkflowDto>();
            var resultadoRunning = new List<InstanciaWorkflowDto>();
            var resultadoException = new List<InstanciaWorkflowDto>();
            var resultadoSuspended = new List<InstanciaWorkflowDto>();

            // PARA RUNNING
            CreateInstanceQuery();
            waiter = new ManualResetEvent(false);
            instanceQuery.BeginExecuteQuery(instanceQueryExecuteArgsRunning, TimeSpan.FromSeconds(60), ExecuteQueryCallback,
                                            resultadoRunning);
            waiter.WaitOne();
            waiter.Close();
            resultadoFinal.AddRange(resultadoRunning);

            // PARA EXCEPTION
            CreateInstanceQuery();
            waiter = new ManualResetEvent(false);
            instanceQuery.BeginExecuteQuery(instanceQueryExecuteArgsException, TimeSpan.FromSeconds(60), ExecuteQueryCallback,
                                     resultadoException);
            waiter.WaitOne();
            waiter.Close();
            resultadoFinal.AddRange(resultadoException);

            // PARA SUSPENDED
            CreateInstanceQuery();
            waiter = new ManualResetEvent(false);
            instanceQuery.BeginExecuteQuery(instanceQueryExecuteArgsSuspended, TimeSpan.FromSeconds(60), ExecuteQueryCallback,
                                     resultadoSuspended);
            waiter.WaitOne();
            waiter.Close();
            resultadoFinal.AddRange(resultadoSuspended);

            return resultadoFinal;
        }

        public IEnumerable<InstanciaWorkflowDto> ObtenerWorkflowFiltro(int status, int condition)
        {
            CreateInstanceQuery();
            var instanceQueryExecuteArgs = new InstanceQueryExecuteArgs()
            {
                InstanceStatus = (InstanceStatus)status,
                InstanceCondition = (InstanceCondition)condition
            };
            waiter = new ManualResetEvent(false);
            var resultadoPrueba = new List<InstanciaWorkflowDto>();
            instanceQuery.BeginExecuteQuery(instanceQueryExecuteArgs, TimeSpan.FromSeconds(60), ExecuteQueryCallback,
                                            resultadoPrueba);
            waiter.WaitOne();
            waiter.Close();

            return resultadoPrueba;
        }

        public InstanciaWorkflowDto ObtenerWorkflow(Guid instance)
        {
            CreateInstanceQuery();
            var instanceQueryExecuteArgs = new InstanceQueryExecuteArgs
            {
                InstanceId = new[] { instance }
            };
            waiter = new ManualResetEvent(false);
            var resultadoPrueba = new List<InstanciaWorkflowDto>();
            instanceQuery.BeginExecuteQuery(instanceQueryExecuteArgs, TimeSpan.FromSeconds(60), ExecuteQueryCallback,
                                            resultadoPrueba);
            waiter.WaitOne();
            waiter.Close();
            var instancia = resultadoPrueba.FirstOrDefault();
            return instancia;
        }

        public List<GraficoDePlantaDto> ListarGraficoDePlanta(int centroId)
        {
            var graficos = servicioRepositorio.ListarGraficosDePlanta(centroId);
            var tiempo = servicioRepositorio.ObtenerTiempoMaximoCentro(centroId);

            var resultado = ObtenerWorkFlows().Where(x => x.CentroId == centroId).GroupBy(x => x.ProximaAccion).Select(group => new GraficoDePlantaDto
            {
                NombreActividad = group.Key,
                CantidadCamionesNoDemorados = group.Count(x => !tiempo.HasValue || (x.FechaUltimaModificacion.HasValue && ((DateTime)x.FechaUltimaModificacion).AddSeconds((int)tiempo.Value) > DateTime.Now)),
                CantidadCamionesDemorados = group.Count(x => !x.FechaUltimaModificacion.HasValue || tiempo.HasValue && ((DateTime)x.FechaUltimaModificacion).AddSeconds((int)tiempo.Value) <= DateTime.Now)
            }).ToList();

            foreach (var actividad in resultado)
            {
                var grafico = graficos.FirstOrDefault(x => x.NombreActividad == actividad.NombreActividad);
                if (grafico != null)
                {
                    actividad.Color = grafico.Color;
                    actividad.Rango = grafico.Rango;
                    actividad.Sector = grafico.Sector;
                }
                else
                {
                    actividad.Color = "#000";
                    actividad.Rango = 10;
                }
                actividad.NombreActividadDesc = Textos.ResourceManager.GetString("Act" + actividad.NombreActividad);
            }

            return resultado;
        }

        public List<InstanciaWorkflowDto> ListarWorflows() => ObtenerWorkFlows().ToList();

        private ListaPaginada<InstanciaWorkflowDto> ListarWorkFlows(IEnumerable<InstanciaWorkflowDto> resultado, Paginacion paginacion)
        {
            var instanciaWorkflowDtos = resultado as InstanciaWorkflowDto[] ?? resultado.ToArray();
            int itemsTotales = instanciaWorkflowDtos.Count();
            IEnumerable<InstanciaWorkflowDto> items = instanciaWorkflowDtos;
            if (paginacion.OrdenarPor != null)
            {
                Func<InstanciaWorkflowDto, object> selectorOrden = Expresiones.PropiedadValueType<InstanciaWorkflowDto>(paginacion.OrdenarPor).Compile();
                items = paginacion.DireccionOrden == DirOrden.Asc
                                        ? items.OrderBy(selectorOrden)
                                        : items.OrderByDescending(selectorOrden);
            }

            resultado = items.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina).ToList();

            return new ListaPaginada<InstanciaWorkflowDto>(resultado.ToList(), paginacion.Pagina,
                                                           paginacion.ItemsPorPagina, itemsTotales);
        }

        private WorkFlowsFiltradosDto FiltrarWorkFlows(IEnumerable<InstanciaWorkflowDto> instanciasWorkflow, FiltroListaDeWorkflowsDto filtro)
        {
            var resultado = new WorkFlowsFiltradosDto();
            if (filtro != null)
            {
                var permisos = servicioRepositorio.ListarPermisosDeActividadPorUsuario(filtro.NombreUsuario);
                Func<InstanciaWorkflowDto, bool> expresionFiltro =
                    x => x.CentroId == filtro.CentroId
                        && permisos.Any(y => y == x.ProximaAccion)
                        && (!filtro.SoloDemorados || (filtro.SoloDemorados && x.FechaUltimaModificacion != null && filtro.TiempoMaxEntreActividades != null && ((DateTime)x.FechaUltimaModificacion).AddSeconds((int)filtro.TiempoMaxEntreActividades) < DateTime.Now))
                            && ((filtro.Workflow != null && x.Workflow != null && x.Workflow.Contains(filtro.Workflow)) || filtro.Workflow == null)
                            && ((filtro.ProximaAccion != null && ((filtro.ProximaAccion == "Pendiente" && x.ProximaAccion == "Pendiente") || (filtro.ProximaAccion != "Pendiente" && x.ProximaAccion == filtro.ProximaAccion))) || filtro.ProximaAccion == null)
                            && ((filtro.Patente != null && x.Patente != null && x.Patente.ToLower().Contains(filtro.Patente.ToLower())) || filtro.Patente == null)
                            && ((filtro.TipoDocumentoDeIngreso != null && x.TipoDocumentoDeIngreso == filtro.TipoDocumentoDeIngreso) || filtro.TipoDocumentoDeIngreso == null)
                            && ((filtro.NumeroDocumentoDeIngreso != null && x.NumeroDocumentoDeIngreso != null && x.NumeroDocumentoDeIngreso.Contains(filtro.NumeroDocumentoDeIngreso)) || filtro.NumeroDocumentoDeIngreso == null)
                            && ((filtro.MaterialId.HasValue && x.MaterialId == filtro.MaterialId.Value) || filtro.MaterialId == null)
                            && ((filtro.Calidad != null && x.Calidad.ToLower().Contains(filtro.Calidad.ToLower())) || filtro.Calidad == null)
                            && ((filtro.TipoComercialId.HasValue && x.TipoComercialId == filtro.TipoComercialId) || filtro.TipoComercialId == null)
                            && (!filtro.ExcluirRechazados || (filtro.ExcluirRechazados && x.EsRechazado != "True"));
                instanciasWorkflow = instanciasWorkflow.Where(expresionFiltro);

                if (filtro.TipoMaterial != TipoMaterial.Todos)
                {
                    instanciasWorkflow = (filtro.TipoMaterial == TipoMaterial.Granos)
                        ? instanciasWorkflow.Where(x => x.EsGrano == "True" && !string.IsNullOrEmpty(x.EsGrano))
                        : instanciasWorkflow.Where(x => x.EsGrano == "False" && !string.IsNullOrEmpty(x.EsGrano));
                }

                if (filtro.TieneEntregador != FiltroEntregador.Todos)
                {
                    instanciasWorkflow = (filtro.TieneEntregador == FiltroEntregador.SinEntregador)
                    ? instanciasWorkflow.Where(x => x.Entregador == "SIN ENTREGA" || string.IsNullOrEmpty(x.Entregador))
                    : instanciasWorkflow.Where(x => x.Entregador != "SIN ENTREGA" && !string.IsNullOrEmpty(x.Entregador));
                }

                resultado.InstanciasWorkflowDto = instanciasWorkflow;
            }
            return resultado;
        }

        private ListaPaginada<InstanciaWorkflowDto> ListarWorkFlows(IEnumerable<InstanciaWorkflowDto> resultado, FiltroListaDeWorkflowsDto filtro, Paginacion paginacion)
        {
            var instanciaWorkflowDtos = resultado as InstanciaWorkflowDto[] ?? resultado.ToArray();
            int itemsTotales = instanciaWorkflowDtos.Count();
            IEnumerable<InstanciaWorkflowDto> items = instanciaWorkflowDtos;

            if (paginacion.OrdenarPor != null)
            {
                Func<InstanciaWorkflowDto, object> selectorOrden = Expresiones.PropiedadValueType<InstanciaWorkflowDto>(paginacion.OrdenarPor).Compile();
                items = paginacion.DireccionOrden == DirOrden.Asc
                                        ? resultado.OrderByDescending(x => x.Reingreso == true ? 1 : x.LlegoEnHorario == true ? 2 : -1).ThenBy(selectorOrden)
                                        : resultado.OrderByDescending(x => x.Reingreso == true ? 1 : x.LlegoEnHorario == true ? 2 : -1).ThenByDescending(selectorOrden);
            }

            resultado = items.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina).ToList();
            foreach (var instanciaWorkflowDto in resultado)
            {
                instanciaWorkflowDto.EstaDemorado = EstaDemorado(filtro.TiempoMaxEntreActividades,
                                                                 instanciaWorkflowDto.FechaUltimaModificacion);
            }
            return new ListaPaginada<InstanciaWorkflowDto>(resultado.ToList(), paginacion.Pagina,
                                                           paginacion.ItemsPorPagina, itemsTotales);
        }

        private void ExecuteQueryCallback(IAsyncResult result)
        {
            var resultadoPrueba = (List<InstanciaWorkflowDto>)result.AsyncState;
            try
            {
                foreach (var info in instanceQuery.EndExecuteQuery(result))
                {
                    var codigoWorkflow = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadWorkflow);
                    var tipoVehiculoValor = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadTipoVehiculo);
                    var tipoDocumentoDeIngresoValor = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadTipoDocumentoDeIngreso);



                    TipoVehiculo? tipoVehiculo = null;
                    if (!string.IsNullOrEmpty(tipoVehiculoValor))
                        tipoVehiculo = (TipoVehiculo)Enum.Parse(typeof(TipoVehiculo), tipoVehiculoValor);

                    TipoDocumentoIngreso? tipoDocumentoDeIngreso = null;
                    if (!string.IsNullOrEmpty(tipoDocumentoDeIngresoValor))
                        tipoDocumentoDeIngreso = (TipoDocumentoIngreso)Enum.Parse(typeof(TipoDocumentoIngreso), tipoDocumentoDeIngresoValor, true);

   

                    resultadoPrueba.Add(new InstanciaWorkflowDto
                    {
                        TipoVehiculo = tipoVehiculo,
                        TipoDocumentoDeIngreso = tipoDocumentoDeIngreso,
                        NumeroDocumentoDeIngreso = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadNumeroDocumentoDeIngreso),
                        Patente = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadPatente),
                        ProximaAccion = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadActividad),
                        Workflow = codigoWorkflow,
                        MaterialCodigoSap = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadMaterialCodigoSap),
                        Material = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadMaterial),
                        Transportista = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadTransportista),
                        Cuit = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadCuit),
                        CentroCodigoSap = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadCentroCodigoSap),
                        NumeroDeTarjeta = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadNumeroDeTarjeta),
                        CentroId = Convert.ToInt32(ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadCentroId, true)),
                        Centro = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadCentro),
                        DatosProximaActividad = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadDatosProximaActividad),
                        TipoComercialId = Convert.ToInt32(ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadTipoComercialId, true)),
                        MaterialId = Convert.ToInt32(ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadMaterialId, true)),
                        Calidad = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadCalidad),
                        TransportistaId = Convert.ToInt32(ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadTransportistaId, true)),
                        TipoComercial = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadTipoComercial),
                        Codigo = codigoWorkflow,
                        ChoferDNI = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadChoferDNI),
                        ChoferNombre = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadChoferNombre),
                        Procedencia = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadProcedencia),
                        EsGrano = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadEsGrano),
                        Entregador = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadEntregador),
                        EsRechazado = ObtenerValor(info, ScatoPersistenceParticipant.PropiedadNameSpace + ScatoPersistenceParticipant.PropiedadEsRechazado),

                        Id = info.InstanceId,
                        Estado = info.InstanceStatus,
                        FechaCreacion = info.CreationTime,
                        FechaUltimaModificacion = info.LastUpdateTime,
                        Condicion = info.InstanceCondition
                    });
                }
            }
            finally
            {
                waiter.Set();
            }
        }

        private void ObtenerHostInfo(IAsyncResult result)
        {
            var resultadoPrueba = (IDictionary<string, string>)result.AsyncState;
            try
            {
                var dictionaryHostInfo =
                    instanceQuery.EndExecuteQuery(result).FirstOrDefault()?.HostInfo.HostMetadata;
                foreach (var info in dictionaryHostInfo)
                {
                    resultadoPrueba.Add(info);
                    if (info.Key == "RelativeServicePath")
                    {
                        resultadoPrueba.Add("VirtualPath", info.Value);
                    }
                }
            }
            finally
            {
                waiter.Set();
            }
        }

        public List<ServicioDto> ListarEstadoDeServicios()
        {
            var clientSection = (WebConfigurationManager.GetSection("system.serviceModel/client") as ClientSection);
            var model = new List<ServicioDto>();
            foreach (ChannelEndpointElement cee in clientSection.Endpoints.Cast<ChannelEndpointElement>().Where(cee => !cee.Address.AbsoluteUri.StartsWith("net.msmq")))
            {
                model.Add(new ServicioDto
                {
                    Nombre = cee.Name,
                    Url = cee.Address.AbsoluteUri
                });
            }
            return model;
        }

        private static string ObtenerValor(InstanceInfo info, string propiedad, bool esInt = false)
        {
            var valor = info.PrimitiveProperties.FirstOrDefault(x => x.PropertyName == propiedad);
            if (valor != null)
            {
                return String.IsNullOrEmpty(valor.PropertyValue) ? esInt ? "0" : "" : valor.PropertyValue;
            }
            return esInt ? "0" : "";
        }

        private static bool EstaDemorado(int? tiempoMaxEntreActividades, DateTime? fechaUltimaModificacion)
        {
            return tiempoMaxEntreActividades != null && fechaUltimaModificacion != null &&
                   ((DateTime)fechaUltimaModificacion).AddSeconds((int)tiempoMaxEntreActividades) < DateTime.Now;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && waiter != null)
            {
                waiter.Dispose();
                waiter = null;
            }
        }

        public InstanciaWorkflowDto ObtenerWorkflowPendientePorNumeroTarjetaAcceso(string numeroTarjeta, int? centroId)
        {
            var pendientes = servicioRepositorio.ListarDatosDeWorkflowsPendientes(centroId ?? 0, 0);
            return !string.IsNullOrEmpty(numeroTarjeta) ? pendientes.FirstOrDefault(f => f.NumeroDeTarjeta == numeroTarjeta) : null;
        }
    }
}
