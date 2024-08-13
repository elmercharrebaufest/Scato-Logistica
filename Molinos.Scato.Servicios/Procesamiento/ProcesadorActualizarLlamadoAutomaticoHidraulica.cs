using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarLlamadoAutomaticoHidraulica : ProcesadorComando<ActualizarLlamadoAutomaticoHidraulica>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IServicioRepositorio servicioRepositorio;

        public ProcesadorActualizarLlamadoAutomaticoHidraulica(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioOrquestador servicioOrquestador, IServicioRepositorio servicioRepositorio)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
            this.servicioRepositorio = servicioRepositorio;

        }

        public override Resultado Ejecutar(ActualizarLlamadoAutomaticoHidraulica comando)
        {
            var resultado = new Resultado();
            Validar(comando, resultado);
            if (resultado.HayErrores)
                return resultado;

            var hidraulica = Repositorio.Obtener<LlamadoAutomaticoHidraulica>(q => q.Hidraulica.Id == comando.Id && (q.Estado != EstadoHidraulica.Inhabilitado || comando.Estado == EstadoHidraulica.Disponible));
            if (hidraulica != null)
            {
                hidraulica.Estado = comando.Estado;
                hidraulica.UltimaPatenteLlamada = comando.Patente;
                hidraulica.FechaUltimaModificacionEstado = DateTime.Now;

                switch (comando.Estado)
                {
                    case EstadoHidraulica.Disponible:
                        LlamadoAutomaticoVolcadora(hidraulica);
                        break;

                    case EstadoHidraulica.Llamando:
                        hidraulica.UltimoCartelLlamado = comando.Cartel;
                        break;

                    case EstadoHidraulica.Inhabilitado:
                        LimpiarMensajeCartel(hidraulica.UltimoCartelLlamado);
                        hidraulica.UltimoCartelLlamado = null;
                        break;
                }

                Repositorio.GuardarCambios();
            }

            return resultado;
        }

        private void LlamadoAutomaticoVolcadora(LlamadoAutomaticoHidraulica hidraulica)
        {
            var callesHidraulicas = Repositorio.Listar<ConfiguracionCalleHidraulica>();
            var primerosCamiones = new List<CamionHidraulicaDto>();
            foreach (var calleHidraulica in callesHidraulicas)
            {
                var resultado = servicioOrquestador.Ejecutar(
                    new EjecutarTomarFoto
                    {
                        CodigoDispositivo = calleHidraulica.CodigoCamaraALPR,
                        FilePath = string.Empty,
                        SubPath = string.Empty,
                        FileName = string.Empty
                    }) as ResultadoObtenerPatente;
                if (resultado == null || string.IsNullOrEmpty(resultado.Patente))
                    continue;

                var hidraulicas = Repositorio.Listar<LlamadoAutomaticoHidraulica>(x => x.UltimaPatenteLlamada == resultado.Patente);
                if (hidraulicas.Any())
                    continue;

                var datosCamion = ObtenerDatosPorPatente(resultado.Patente);
                if (datosCamion == null)
                    continue;

                var callePorRecorrido = Repositorio.Obtener<CallePorRecorrido>(x => x.Recorrido.Id == datosCamion.RecorridoId && x.FechaEgreso == null && x.Calle.TipoCalle == TipoCalle.PlayaInterna && !x.Recorrido.Terminado);
                if (callePorRecorrido == null)
                    continue;

                if (!datosCamion.HidraulicasId.Contains(hidraulica.Hidraulica.Id))
                    continue;

                datosCamion.FechaLlegadaACalleHidraulica = callePorRecorrido.FechaIngeso;
                datosCamion.CodigoCartel = calleHidraulica.CodigoCartel;
                primerosCamiones.Add(datosCamion);
            }
            if (primerosCamiones.Count > 0)
            {
                var tiempoDeIntervalo = Repositorio.Obtener<Dominio.Entidades.ConfiguracionGeneral>(q => q.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.EstadoVolcadoras && q.Nombre == Constantes.ConfiguracionGeneral.Volcadoras.CartelLedIntervalo);
                var camionLlamado = primerosCamiones.OrderBy(x => x.FechaLlegadaACalleHidraulica).FirstOrDefault();
                EnviarMensajeACartelConIntervalo(camionLlamado?.CodigoCartel, camionLlamado.Patente, hidraulica.Hidraulica.Nombre, (tiempoDeIntervalo != null) ? int.Parse(tiempoDeIntervalo.Valor) : 3000);
                hidraulica.Estado = EstadoHidraulica.Llamando;
                hidraulica.UltimaPatenteLlamada = camionLlamado.Patente;
                hidraulica.UltimoCartelLlamado = camionLlamado.CodigoCartel;
            }
        }

        private void EnviarMensajeACartelConIntervalo(string codigoCartel, string mensaje, string mensajeSecundario, int intervaloMilliseconds)
        {
            try
            {
                if (!string.IsNullOrEmpty(codigoCartel) && mensaje != null)
                {
                    var mensajeCartel = servicioRepositorio.ObtenerMensajeCartelLedPorCodigo(CodigoMensajeCartelLed.LlamadoAutomaticoVolcadoras);
                    servicioComandos.Ejecutar(new EnviarMensajeCartelLed
                    {
                        Mensaje = mensaje,
                        Codigo = codigoCartel,
                        NumeroPrograma = mensajeCartel.Programa,
                        NumeroTrama = mensajeCartel.Trama,
                        NumeroVariable = mensajeCartel.Variable,
                        SegundosDeEspera = mensajeCartel.SegundosDeEspera,
                        EsMensajeConIntervalo = true,
                        MensajeSecundario = mensajeSecundario,
                        IntervaloMilliseconds = intervaloMilliseconds,
                    });
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo mostrar el mensaje en Cartel Led");
            }
        }

        private void LimpiarMensajeCartel(string codigoCartel)
        {
            try
            {
                if (!string.IsNullOrEmpty(codigoCartel))
                {
                    var mensajeCartel = servicioRepositorio.ObtenerMensajeCartelLedPorCodigo(CodigoMensajeCartelLed.LlamadoAutomaticoVolcadoras);
                    servicioOrquestador.Ejecutar(new DetenerMensajeIntervalo
                    {
                        CodigoDispositivo = codigoCartel,
                        NumeroPrograma = mensajeCartel.Programa,
                        NumeroTrama = mensajeCartel.Trama,
                        NumeroVariable = mensajeCartel.Variable
                    });
                    servicioComandos.Ejecutar(new EnviarMensajeCartelLed
                    {
                        Mensaje = "PARE AQUI",
                        Codigo = codigoCartel,
                        NumeroPrograma = mensajeCartel.Programa,
                        NumeroTrama = CartelTramaPare.LlamadoAutomaticoVolcadoras,
                        NumeroVariable = mensajeCartel.Variable,
                    });
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo mostrar el mensaje en Cartel Led");
            }
        }

        private CamionHidraulicaDto ObtenerDatosPorPatente(string patente)
        {
            CamionHidraulicaDto datosCamion = null;
            try
            {
                var recorrido = servicioRepositorio.ObtenerRecorridoActivoPorPatente(patente);
                if (recorrido != null)
                {
                    datosCamion = new CamionHidraulicaDto()
                    {
                        Patente = patente,
                        HidraulicasId = recorrido.HidraulicasId,
                        RecorridoId = recorrido.Id
                    };
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "No se obtener datos por patente {0}", patente);
            }
            return datosCamion;
        }
        private IList<AutomatismoGrano> TodosLosAutomatismosActivos()
        {
            
            var includesGrano = new List<Expression<Func<AutomatismoGrano, object>>> {
                x => x.Material,
                x => x.CallePreBalanza,
                x => x.CallePreHidraulica,
                x => x.TipoVariedades,
                x => x.Almacen,
                x => x.Hidraulicas };
            return Repositorio.Listar<AutomatismoGrano>(includesGrano, c => c.Activo == true);
        }

        private void Validar(ActualizarLlamadoAutomaticoHidraulica comando, Resultado resultado)
        {
            var configuracion = Repositorio.Obtener<Dominio.Entidades.ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica && x.Nombre == Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos);

            if (comando.Estado == EstadoHidraulica.Inhabilitado && configuracion.Valor.Equals("True") && Repositorio.Existe<AutomatismoGrano>(a => a.Hidraulicas.Any(h => h.Id == comando.Id) && a.Activo))
            {
                var todosLosAutomatismosActivos = TodosLosAutomatismosActivos();

                // Obtener los automatismos activos que contienen la hidráulica
                var automatismosActivosConLaHidraulica = todosLosAutomatismosActivos
                    .Where(a => a.Hidraulicas.Any(h => h.Id == comando.Id))
                    .ToList();

                // Verificar si la hidráulica es la única en algún automatismo activo
                bool hidraulicaUnicaEnAlgunAutomatismoActivo = automatismosActivosConLaHidraulica
                    .Any(a => a.Hidraulicas.Count(h => h.ActivoAutomatico) == 1);

                // Verificar si la hidráulica está en varios automatismos activos y es la única en cada uno
                bool hidraulicaUnicaEnVariosAutomatismosActivos = automatismosActivosConLaHidraulica.Count > 1 &&
                    automatismosActivosConLaHidraulica.All(a => a.Hidraulicas.Count(h => h.ActivoAutomatico) == 1);

                // Obtener una lista de IDs de los automatismos que contienen la hidráulica
                var idsAutomatismosConLaHidraulica = string.Join(", ", automatismosActivosConLaHidraulica.Where(c => c.Hidraulicas.Count(h => h.ActivoAutomatico) == 1).Select(a => a.Id.ToString()));

                // Generar error si se cumplen las condiciones para no permitir la deshabilitación
                if (hidraulicaUnicaEnAlgunAutomatismoActivo || hidraulicaUnicaEnVariosAutomatismosActivos)
                {
                    resultado.Error("MensajeError", string.Format(Textos.HidraulicaUtilizadaEnVariosAutomatismoActivo, idsAutomatismosConLaHidraulica));
                }
            }
        }
    }
}