using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarRecorrido : ProcesadorComando<EliminarRecorrido>
    {
        private readonly IServicioComandos comandos;

        public ProcesadorEliminarRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos comandos)
            : base(repositorio, conversor, log)
        {
            this.comandos = comandos;
        }

        public sealed override Resultado Ejecutar(EliminarRecorrido comando)
        {
            var resultado = new ResultadoEliminarRecorrido();
            Validar(comando, resultado);
            if (!resultado.HayErrores)
            {
                var recorrido = Repositorio.Obtener<Recorrido>(comando.Id);

                try
                {
                    Log.Debug($"LiberarStockSojaEpa para: {recorrido.InstanciaWorkflow}");

                    var resultadoStock = comandos.Ejecutar(new LiberarStockSojaEpa { InstanceId = recorrido.InstanciaWorkflow });
                    var mensaje = string.Empty;
                    if (resultado.HayErrores)
                    {
                        mensaje = resultadoStock.Errores.FirstOrDefault().Value;
                    }
                    Log.Debug($"Resultado LiberarStockSojaEpa: {mensaje}");

                }
                catch (Exception e)
                {
                    Log.Error(e,"LiberarStockSojaEpa");
                }

                var ultimaActividad = Repositorio.ObtenerMayor<ControlRecorrido, int>(x => x.WorkflowInstanceId == recorrido.InstanciaWorkflow, y => y.Id);
                var existeActividadHistorico =
                    Repositorio.Existe<LogActividadHistorico>(x => x.WorkflowInstanceId == recorrido.InstanciaWorkflow);
                var proximaActividadActual =
                    Repositorio.ObtenerMayor<LogActividad, int>(
                        x => x.WorkflowInstanceId == recorrido.InstanciaWorkflow, y => y.Id);

                Log.Debug("Crear DocumentoBorrado Motivo: {0}", comando.Motivo);
                Repositorio.Agregar(new DocumentoBorrado
                {
                    Fecha = DateTime.Now,
                    Motivo = comando.Motivo,
                    NombreUsuario = comando.NombreUsuario,
                    NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso,
                    Patente = recorrido.Patente,
                    TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso,
                    Centro = recorrido.Centro,
                    UltimaActividad = ultimaActividad != null ? ultimaActividad.Actividad : "",
                    EtapaDeBorrado = !existeActividadHistorico ? (proximaActividadActual != null ? proximaActividadActual.Actividad : "") : @Textos.RecorridoTerminado
                });
                var impReciboMunicipales = Repositorio.Listar<ImpReciboMunicipal>(x => x.WorkflowId == recorrido.InstanciaWorkflow && x.TipoImpresion == TipoImpresion.ReciboMunicipal);
                if (impReciboMunicipales != null && impReciboMunicipales.Any())
                {
                    foreach(var impReciboMunicipal in impReciboMunicipales)
                    {
                        Repositorio.Agregar(new TicketMunicipalBorrado
                        {
                            Fecha = impReciboMunicipal.FechaImpresion,
                            CentroId = recorrido.Centro.Id,
                            MaterialId = recorrido.Material.Id,
                            ChoferNombre = recorrido.Chofer.Nombre + " " + recorrido.Chofer.Apellido,
                            NroDocumento = recorrido.NumeroDocumentoIngreso,
                            NroTarjeta = recorrido.TarjetaDeAcceso,
                            Patente = recorrido.Patente,
                            Recibo = impReciboMunicipal.TicketNro,
                            TipoDocumento = recorrido.TipoDocumentoIngreso,
                            ChoferCuil = recorrido.Chofer.Cuil
                        });
                    }
                }

                var ajusteDeCalidad = Repositorio.Obtener<AjusteDeCalidad>(x => x.TipoDocumentoIngreso == recorrido.TipoDocumentoIngreso && x.NumeroDocumentoIngreso == recorrido.NumeroDocumentoIngreso);
                if (ajusteDeCalidad != null)
                {
                    Log.Debug("Borrar AjusteDeCalidad Id: {0}", ajusteDeCalidad.Id);
                    Repositorio.Remover(ajusteDeCalidad);
                }
                var calles = Repositorio.Listar<CallePorRecorrido>(x => x.Recorrido.Id == recorrido.Id);
                if (calles != null && calles.Any())
                {
                    Log.Debug("Borrar Calles asignadas Id: {0}", calles.Count);
                    Repositorio.RemoverTodos(calles);
                }

                if (recorrido.Vehiculo != null)
                {
                    var vehiculo = recorrido.Vehiculo;
                    if (!Repositorio.Existe<Recorrido>(r => r.Id != recorrido.Id && r.Vehiculo.CartaPorte.Id == recorrido.Vehiculo.CartaPorte.Id))
                    {
                        Log.Debug("Borrar CartaPorte Id: {0}", vehiculo.CartaPorte.Id);
                        Repositorio.Remover(vehiculo.CartaPorte);
                    }
                    recorrido.Vehiculo = null;
                    Repositorio.Remover(vehiculo);
                }
                Log.Debug("Borrar Recorrido Id: {0}", recorrido.Id);
                Repositorio.Remover(recorrido);

                try
                {                  
                    if (Repositorio.Existe<LogActividad>(x => x.WorkflowInstanceId == recorrido.InstanciaWorkflow && x.ActividadXaml == "AltaCTG"))
                    {
                        Log.Info("La actividad realizo alta de CTG");
                        resultado.Advertencias.Add("altaCTG", Textos.Advertencia_CTG);
                    }
                    if (Repositorio.Existe<LogActividad>(x => x.WorkflowInstanceId == recorrido.InstanciaWorkflow && x.ActividadXaml == "BajaCTG"))
                    {
                        Log.Info("La actividad realizo baja de CTG");
                        resultado.Advertencias.Add("bajaCTG", Textos.Advertencia_BajaCTG);
                    }
                    if (Repositorio.Existe<LogActividad>(x => x.WorkflowInstanceId == recorrido.InstanciaWorkflow && (x.Actividad.Contains("Sap"))))
                    {
                        Log.Info("La actividad paso por una transmisión a Sap");
                        resultado.Advertencias.Add("Sap", Textos.Error_PosibleInconsistencia);
                    }
                    if (recorrido.Material != null)
                    {
                        var material = Repositorio.Obtener<MaterialPorCentro>(x => x.Centro.Id == recorrido.Centro.Id && x.Material.Id == recorrido.Material.Id);

                        if (material.RequiereTecnologia & Repositorio.Existe<LogActividad>(x => x.WorkflowInstanceId == recorrido.InstanciaWorkflow && x.ActividadXaml == "ServicioMonsantoRegistrarCartadePorte"))
                        {
                            Log.Info("La actividad paso por ServicioMonsantoRegistrarCartadePorte");
                            resultado.Advertencias.Add("Monsanto", Textos.Advertencia_ServicioMonsantoRegistrarCartadePorte);
                        }
                    }
                    Repositorio.GuardarCambios();
                    Log.Debug("Fin de eliminacion del documento");
                }
                catch (EntidadReferenciadaException ex)
                {
                    Log.Error(ex, "Ocurrio un error de entidad referenciada al eliminar la entidad del tipo Recorrido - Id {0}", comando.Id);
                    resultado.Error("", Textos.Error_EliminarReferenciado);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Ocurrio al eliminar la entidad del tipo Recorrido - Id {0}", comando.Id);
                    resultado.Error("", Textos.Error_ActualizarGenerico);
                }
            }
            return resultado;
        }

        protected void Validar(EliminarRecorrido comando, Resultado resultado)
        {
            var recorrido = Repositorio.Obtener<Recorrido>(comando.Id);
            if (recorrido != null)
            {
                var calados = Repositorio.Listar<Calado,int>(x => x.Id, x => x.WorkflowInstanceId == recorrido.InstanciaWorkflow);
                if ( Repositorio.Existe<MicroMuestrasPorCasillero>(x => calados.Any(y => y == x.Muestra.Calado.Id)) && !resultado.HayErrores)
                {
                    resultado.Errores.Add("Eliminar Documento", Textos.Error_EliminarMicroMuestra);
                }
            }
        }
    }
}
