using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Linq;
using System.Threading;

namespace Molinos.Scato.Actividades.Internas
{
    public class PesadaACeroAutomatico : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }
        public OutArgument<ControlRecorridoDto> ControlRecorrido { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            var servComando = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var puestoId = PuestoDeTrabajoId.Get<int>(context);
            var recorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActBalanzaACero,
                ActividadXaml = "BalanzaACero",
                WorkflowInstanceId = context.WorkflowInstanceId,
                PuestoDeTrabajoId = puestoId,
                NombreUsuario = "Automatico",
            };
            ControlRecorrido.Set(context, recorrido);
            try
            {
                var datosRecorrido = repositorio.ObtenerDatosRecorridoActivoPorWorkflow(context.WorkflowInstanceId);
                recorrido.Patente = datosRecorrido.Patente;
                recorrido.CartaDePorte = datosRecorrido.CartaDePorte;
                recorrido.Entregador = datosRecorrido.Entregador;
                recorrido.Material = datosRecorrido.Material;
                recorrido.TipoVehiculo = datosRecorrido.TipoVehiculo;
                recorrido.Tarjeta = datosRecorrido.TarjetaDeAcceso;
                recorrido.Calle = datosRecorrido.Calle;
                recorrido.TipoDeWorkflow = datosRecorrido.TipoDeWorkflow;
                recorrido.RecorridoId = datosRecorrido.Id;
                recorrido.PasoPorContingenciaPesosExc = datosRecorrido.PasoPorContingenciaPesosExc;

                var resultadoCero = VolverACero(repositorio, servComando, recorrido, context);
                ControlRecorrido.Set(context, recorrido);
                Resultado.Set(context, resultadoCero);
                
            }
            catch (Exception e)
            {
                recorrido.Automatizado = false;
                ControlRecorrido.Set(context, recorrido);
            }
            
        }
        private void EnviarNotificacion(CodeActivityContext context, ControlRecorridoDto recorrido, ResultadoPesaje pesaje, string error, bool nohayError, TipoAlerta tipo = TipoAlerta.Automatica, bool noRedirecciona = false)
        {
            var servicio = context.GetExtension<IServicioNotificarUsuario>();
            try
            {
                var enCero = pesaje != null && !pesaje.HayErrores && pesaje.Peso == 0;
                var notificacion = new NotificacionPesadaAutomaticaDto
                {
                    Id = recorrido.PuestoDeTrabajoId,
                    Error = error,
                    CartaPorte = enCero ? "": recorrido.CartaDePorte,
                    Diferencia = "0",
                    DifPeso = "0",
                    DifNeto="0",
                    Peso = pesaje != null && !pesaje.HayErrores ? pesaje.Peso.ToString() : "",
                    Entregador = enCero ? "" : recorrido.Entregador? "Si":"No",
                    Material = enCero ? "Material" : recorrido.Material,
                    WorkflowInstanceId = recorrido.WorkflowInstanceId,
                    TipoPeso = "Peso:",
                    Patente = enCero ? "" : recorrido.Patente,
                    Actividad = !enCero ? "BalanzaACero":"En Espera",
                    TipoVehiculo = enCero ? "Vehiculo" : recorrido.TipoVehiculo.ToString(),
                    Tarjeta = enCero ? "" : recorrido.Tarjeta,
                    NoRedirecciona = noRedirecciona,
                    Calle = recorrido.Calle,
                    TipoPesoOrigen = "Peso Org:",
                    PesoBrutoOrigen = "0",
                    PesoNetoOrigen = "0"
                };
                servicio.Notificar(new NotificacionDto
                {
                    Grupo = "Automaticas",
                    Mensaje = notificacion.ToJson(),
                    TipoAlerta = tipo,
                    Leido = nohayError,
                    PuestoId = recorrido.PuestoDeTrabajoId
                });
            }
            catch (Exception e)
            {
            }
        }
        private ResultadoPesaje VolverACero(IServicioRepositorio repositorio, IServicioComandos servComando, ControlRecorridoDto recorrido, CodeActivityContext context)
        {
            ResultadoPesaje resultado = null;
            try
            {
                
                var puesto = repositorio.ObtenerPuestoDeTrabajo(recorrido.PuestoDeTrabajoId);
                var balanzaId = repositorio.ObtenerIdBalanzaAutomaticaPorPuestoDeTrabajo(recorrido.PuestoDeTrabajoId);
                recorrido.Automatizado = puesto != null && (puesto.AutomatizadoFull && !puesto.PausaAutoFull);
                if (!recorrido.Automatizado)
                {
                    return resultado;
                }
                var count = 0;

                while (resultado == null || resultado.HayErrores || resultado.Peso != 0)
                {
                    try
                    {
                        var mensaje = "";
                        var leido = true;
                        var tipo = TipoAlerta.Automatica;
                        if (repositorio.BalanzaEnCero(balanzaId))
                        {
                            resultado = new ResultadoPesaje();
                            break;
                        }
                        if (count > 0 && count % 80 == 0)
                        {
                            mensaje = $"Balanza {puesto.BalanzaNombre} esta tardando mucho en volver a cero. De haber un problema, puede desactivar la autimatización del puesto para avanzar el camión.";
                            leido = false;
                            tipo = TipoAlerta.AlertaAutomatica;
                        }
                        if (count % 3 == 0 && !repositorio.EsPuestoFullAutomatizado(recorrido.PuestoDeTrabajoId))
                        {
                            try
                            {
                                EnviarNotificacion(context, recorrido, resultado, "Ejecutar Balanza a Cero", false);
                            }
                            catch (Exception e)
                            {
                                recorrido.Comentario = e.Message;
                            }
                            recorrido.Automatizado = false;
                            return resultado;
                        }
                        
                        resultado = (ResultadoPesaje)servComando.Ejecutar(new ObtenerPesada() { Recorrido = recorrido });
                        if (recorrido.PasoPorContingenciaPesosExc && recorrido.TipoDeWorkflow == TipoDeWorkflow.Ingreso)
                        {
                            ContigenciaDePesosExcedidos(repositorio, resultado, recorrido);
                        }
                        count++;
                        mensaje = mensaje == "" && resultado.HayErrores ? resultado.Errores.First().Value : mensaje;
                        leido = mensaje == "";
                        var noRedirecciona = mensaje == "";
                        EnviarNotificacion(context, recorrido, resultado, mensaje, leido, tipo);
                    }
                    finally
                    {
                        Thread.Sleep(5000);
                    }
                }

                if (resultado.HayErrores)
                {
                    servComando.Ejecutar(new CrearControlRecorrido { Dto = recorrido });
                }
                else
                {
                    var cereado = servComando.Ejecutar(new ModificarBalanzaEstaEnCero
                    {
                        BalanzaId = balanzaId,
                        EstaEnCero = true
                    });
                    if (cereado.HayErrores)
                    {
                        resultado.Error(cereado.Errores.First().Key, cereado.Errores.First().Value);

                        recorrido.Comentario = resultado.Errores.First().Value;
                        servComando.Ejecutar(new CrearControlRecorrido { Dto = recorrido });
                    }
                }
            }
            catch (Exception ex)
            {
                resultado.Error("", ex.Message);
            }
            try
            {
                EnviarNotificacion(context, recorrido, resultado, resultado.HayErrores ? resultado.Errores.First().Value : null, !resultado.HayErrores);
            }
            catch (Exception e)
            {
                recorrido.Comentario = e.Message;
            }
            return resultado;
        }

        private void ContigenciaDePesosExcedidos(IServicioRepositorio repositorio, ResultadoPesaje pesaje, ControlRecorridoDto recorrido)
        {
            var balanza = repositorio.ObtenerBalanza(pesaje.BalanzaId);
            if (balanza.ContingenciaExcedentesHabilitada && balanza.ToleranciaExcedida != null && recorrido.RecorridoId != null)
            {
                var pesoMaximoPorTipoVehiculo = repositorio.LeerPesoMaximo(recorrido.WorkflowInstanceId, recorrido.TipoDeWorkflow);
                if (pesaje.Peso > pesoMaximoPorTipoVehiculo && (pesaje.Peso - pesoMaximoPorTipoVehiculo <= balanza.ToleranciaExcedida))
                {
                    pesaje.Peso = pesoMaximoPorTipoVehiculo;
                }
            }
        }
    }
}
