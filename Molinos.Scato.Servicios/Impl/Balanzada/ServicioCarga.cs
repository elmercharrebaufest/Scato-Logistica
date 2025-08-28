using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ComandosEF;
using Molinos.Scato.Repositorio.ComandosEF.BalanzasPuerto;
using Molinos.Scato.Servicios.Estrategias;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Servicios.Impl
{
    public class ServicioCarga : IServicioCarga
    {
        private readonly IRepositorio _repositorio;
        private readonly IServicioOrquestador _orquestador;
        private readonly IServicioComandos _servicioComandos;
        protected ILogger Log { get; private set; }

        public ServicioCarga(IRepositorio repositorio, IServicioOrquestador orquestador, IServicioComandos servicioComandos, ILogger log)
        {
            _repositorio = repositorio;
            _orquestador = orquestador;
            _servicioComandos = servicioComandos;
            Log = log;
        }

        public BalanzadaRecibidaDTO ConvertirDatosABalanazadaRecibida(Dictionary<string, string> datos)
        {
            var fecha = datos["fecha"];
            if (fecha.Length == 13) // Si llegara dd-MM-yyHH:mm se ignora el año y usa el actual
            {
                var anioActual = DateTime.Now.Year.ToString();
                fecha = fecha.Remove(6, 2).Insert(6, anioActual);
            }

            var balanzadaRecibida = new BalanzadaRecibidaDTO
            {
                Id = int.Parse(datos["id"]),
                TipoBalanzada = datos["tipoBalanzada"],
                NumeroBalanza = datos["numeroBalanza"],
                Fecha = DateTime.ParseExact(fecha, "dd-MM-yyyyHH:mm", CultureInfo.InvariantCulture),
            };

            var balanza = _repositorio.Obtener<BalanzaPuerto>(q => q.CodigoBalanza == balanzadaRecibida.NumeroBalanza);

            balanzadaRecibida.CodigoDispositivo = balanza.CodigoDispositivo;
            balanzadaRecibida.OffsetBalanza = balanza.OffSetPlc;
            balanzadaRecibida.IdOffset = balanzadaRecibida.Id + balanzadaRecibida.OffsetBalanza;
            balanzadaRecibida.UltimaValidacion = balanza.UltimaValidacion;
            balanzadaRecibida.IntentosValidacion = balanza.IntentosValidacion;

            balanzadaRecibida.Commodity = datos.ContainsKey("commodity") ? datos["commodity"] : string.Empty;

            balanzadaRecibida.Bodega = datos.ContainsKey("bodega") ? datos["bodega"] : string.Empty;

            balanzadaRecibida.Vapor = datos.ContainsKey("vapor") ? datos["vapor"] : string.Empty;

            balanzadaRecibida.Exportador = datos.ContainsKey("exportador") ? datos["exportador"] : string.Empty;

            balanzadaRecibida.Destino = datos.ContainsKey("destino") ? (datos["destino"]) : string.Empty;

            balanzadaRecibida.PesoProgramado = datos.ContainsKey("pesoProgramado") ? int.Parse(datos["pesoProgramado"]) : 0;

            balanzadaRecibida.ToneladasAW = datos.ContainsKey("toneladasaw") ? int.Parse(datos["toneladasaw"]) : 0;

            balanzadaRecibida.PesoBruto = datos.ContainsKey("pesoBruto") ? int.Parse(datos["pesoBruto"]) : 0;

            balanzadaRecibida.PesoTara = datos.ContainsKey("pesoTara") ? int.Parse(datos["pesoTara"]) : 0;

            balanzadaRecibida.PesoNeto = datos.ContainsKey("pesoNeto") ? int.Parse(datos["pesoNeto"]) : 0;

            balanzadaRecibida.Capacidad = datos.ContainsKey("capacidad") ? datos["capacidad"] : string.Empty;

            return balanzadaRecibida;
        }

        public ResultadoCrear CrearCargaPendiente(BalanzadaRecibidaDTO balanzada)
        {
            Log.Debug("Validando balanza {0} balanzada {1} {2}", balanzada.NumeroBalanza, balanzada.TipoBalanzada, balanzada.Id);
            var resultado = new ResultadoCrear();
            var ok = ValidarCrearCargaPendiente(balanzada);
            if (!ok)
            {
                resultado.Error("", "No se han podido restaurar todos los pendientes");
            }
            return resultado;
        }

        public bool ExisteRegistroBalanzaPuerto(BalanzadaRecibidaDTO balanzada)
        {
            return _repositorio.Existe<RegistroBalanzaPuerto>(e => e.Id == balanzada.IdOffset && e.NumeroBalanza == balanzada.NumeroBalanza);
        }

        private bool ValidarCrearCargaPendiente(BalanzadaRecibidaDTO balanzada)
        {
            var desde = balanzada.UltimaValidacion;
            var cuantos = balanzada.IdOffset - desde;

            // Registros que ya están en la db por lo que no deben consultarse al orquestador
            var idsAExcluir = _repositorio.Listar<RegistroBalanzaPuerto, int>(x => x.Id,
                c => c.Id >= desde &&
                c.Id < balanzada.IdOffset &&
                (c.NumeroBalanza == balanzada.NumeroBalanza)
            ).OrderBy(x => x).ToList();

            idsAExcluir.Add(0); // Se añade el id 0 ya que no es un id válido en el orquestador

            List<int> balanzadasPendientes = Enumerable.Range(desde, cuantos).Except(idsAExcluir).ToList();

            balanzadasPendientes = balanzadasPendientes.OrderBy(x => x).ToList();
            Log.Debug("Faltan las siguientes balanzadas en la balanza {0}: {1}", balanzada.NumeroBalanza, string.Join(",", balanzadasPendientes));

            foreach (int balanzadaId in balanzadasPendientes)
            {
                int reintentos = 0;
                bool validado = false;
                while (reintentos < balanzada.IntentosValidacion && !validado)
                {
                    var balanzadaPendiente = _orquestador.Ejecutar(new EjecutarConsultaBalanzada { CodigoDispositivo = balanzada.CodigoDispositivo, IdBalanzada = balanzadaId - balanzada.OffsetBalanza });

                    if (balanzadaPendiente.Mensaje.Codigo == 0)
                    {
                        var valores = ObtenerValoresBalanzada(balanzadaPendiente);
                        if (valores.Any())
                        {
                            var balanzadaACrear = ConvertirDatosABalanazadaRecibida(valores);
                            switch (balanzadaACrear.TipoBalanzada)
                            {
                                case TipoBalanzada.Inicio:
                                    CrearCargaInicio(balanzadaACrear);
                                    break;

                                case TipoBalanzada.Balanzada:
                                    CrearBalanzada(balanzadaACrear);
                                    break;

                                case TipoBalanzada.Fin:
                                    CrearCargaFin(balanzadaACrear);
                                    break;

                                default:
                                    CrearRegistroBalanzaPuerto(balanzadaACrear);
                                    break;
                            }
                            validado = true;
                        }
                    }
                    reintentos++;
                }
                // Si falla los 3 reintentos se aborta
                if (!validado)
                {
                    Log.Info("No se pudo obtener la balanzada {0} de la balanza {1}", balanzadaId, balanzada.NumeroBalanza);
                    return false;
                }
            }
            return true;
        }

        private Dictionary<string, string> ObtenerValoresBalanzada(ResultadoEjecutar resultadoEjecutar)
        {
            Type type = resultadoEjecutar.GetType();
            IEnumerable props = type.GetRuntimeProperties();
            var valoresBalanzada = new Dictionary<string, string>();

            foreach (PropertyInfo prop in props)
            {
                if (string.Equals(prop.Name, "ValoresBalanzada", StringComparison.OrdinalIgnoreCase))
                {
                    valoresBalanzada = (Dictionary<string, string>)prop.GetValue(resultadoEjecutar);
                }
            }
            return valoresBalanzada;
        }

        public void ActualizarUltimaValidacion(BalanzadaRecibidaDTO balanzada)
        {
            var desde = balanzada.UltimaValidacion;
            var hasta = balanzada.IdOffset;
            var ultimaValidacion = _repositorio.EjecutarComando(new ObtenerUltimoIdValidoBalanza(desde, hasta, balanzada.NumeroBalanza));
            var balanzaPuerto = _repositorio.Obtener<BalanzaPuerto>(x => x.CodigoDispositivo == balanzada.CodigoDispositivo);
            balanzaPuerto.UltimaValidacion = ultimaValidacion;
            _repositorio.GuardarCambios();
        }

        public ResultadoCrear CrearCargaInicio(BalanzadaRecibidaDTO balanzada)
        {
            var resultado = new ResultadoCrear();
            try
            {
                if (ExisteRegistroBalanzaPuerto(balanzada))
                {
                    return resultado;
                }

                if (EsInicioFalso(balanzada))
                {
                    var registroBalanzaPuerto = ConstruirRegistroBalanzaPuerto(balanzada, TipoBalanzada.InicioError);
                    _repositorio.Agregar(registroBalanzaPuerto);
                    var inicioActualizar = ObtenerInicioActualizar(balanzada);
                    if (inicioActualizar != null)
                    {
                        inicioActualizar.PesoProgramado = balanzada.PesoProgramado;
                    }
                    _repositorio.GuardarCambios();
                }
                else
                {
                    Carga cargaFin = null;

                    bool hayRegistrosPosteriores = _repositorio.Existe<RegistroBalanzaPuerto>(r => r.Id > balanzada.IdOffset && r.NumeroBalanza == balanzada.NumeroBalanza);
                    if (hayRegistrosPosteriores)
                    {
                        cargaFin = _repositorio.EjecutarComando(new ObtenerCargaFin(balanzada.IdOffset, balanzada.NumeroBalanza));
                    }

                    var carga = ConstruirCarga(balanzada, cargaFin);
                    _repositorio.Agregar(carga);

                    if (cargaFin != null)
                    {
                        cargaFin.CargaOpuesta = carga;
                    }
                    _repositorio.GuardarCambios();

                    // Asignar balanzadas huerfanas a inicio
                    if (hayRegistrosPosteriores)
                    {
                        // Si no hay fin, se toma como fin el próximo inicio.
                        var cargaHasta = cargaFin ?? _repositorio.EjecutarComando(new ObtenerSiguienteCargaInicio(balanzada.IdOffset, balanzada.NumeroBalanza));
                        var hasta = cargaHasta?.Id ?? -1; // Si no hay un hasta, el -1 indica que se actualiza hasta la ultima balanzada.
                        var balanzadasModificadas = _repositorio.EjecutarComando(new AsignarBalanzadas(balanzada.IdOffset, balanzada.NumeroBalanza, hasta));


                        foreach (var balanzadasModificada in balanzadasModificadas)
                        {
                            EnviarASap(balanzadasModificada.EnviadoASap, balanzadasModificada.CargaInicial, balanzadasModificada.Id, balanzadasModificada.NumeroBalanza);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                resultado.Error("", e.Message);
            }
            return resultado;
        }

        public ResultadoCrear CrearBalanzada(BalanzadaRecibidaDTO balanzada)
        {
            var resultado = new ResultadoCrear();
            try
            {
                if (ExisteRegistroBalanzaPuerto(balanzada))
                {
                    return resultado;
                }

                var inicio = _repositorio.EjecutarComando(new ObtenerCargaInicio(balanzada.IdOffset, balanzada.NumeroBalanza));
                var ultimoIdFin = _repositorio.EjecutarComando(new ObtenerIdFinAnterior(balanzada.IdOffset, balanzada.NumeroBalanza));
                if (inicio != null && ultimoIdFin > inicio.Id)
                {
                    inicio = null;
                }

                var balanzadaEntity = ConstruirBalanzada(balanzada, inicio);
                _repositorio.Agregar(balanzadaEntity);
                _repositorio.GuardarCambios();

                EnviarASap(balanzadaEntity.EnviadoASap, balanzadaEntity.CargaInicial, balanzadaEntity.Id, balanzadaEntity.NumeroBalanza);
            }
            catch (Exception e)
            {
                resultado.Error("", e.Message);
                throw;
            }
            return resultado;
        }

        public ResultadoCrear CrearCargaFin(BalanzadaRecibidaDTO balanzada)
        {
            var resultado = new ResultadoCrear();
            try
            {
                if (ExisteRegistroBalanzaPuerto(balanzada))
                {
                    return resultado;
                }

                var cargaInicio = _repositorio.EjecutarComando(new ObtenerCargaInicio(balanzada.IdOffset, balanzada.NumeroBalanza, true));
                var idFinAnterior = _repositorio.EjecutarComando(new ObtenerIdFinAnterior(balanzada.IdOffset, balanzada.NumeroBalanza));
                if (cargaInicio != null && idFinAnterior > cargaInicio.Id)
                {
                    cargaInicio = null;
                }

                var cargaFin = ConstruirCarga(balanzada, cargaInicio);

                if (cargaInicio != null)
                {
                    cargaFin.FechaInicio = cargaInicio.Fecha;
                    cargaInicio.CargaOpuesta = cargaFin;
                }

                _repositorio.Agregar(cargaFin);
                _repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                resultado.Error("", e.Message);
            }
            return resultado;
        }

        public ResultadoCrear CrearRegistroBalanzaPuerto(BalanzadaRecibidaDTO balanzada)
        {
            var resultado = new ResultadoCrear();
            try
            {
                if (ExisteRegistroBalanzaPuerto(balanzada))
                {
                    return resultado;
                }

                var registroBalanzaPuerto = ConstruirRegistroBalanzaPuerto(balanzada, balanzada.TipoBalanzada);
                _repositorio.Agregar(registroBalanzaPuerto);
                _repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                resultado.Error("", e.Message);
            }
            return resultado;
        }

        public void RestaurarBalanzadasPerdidas(string numeroBalanza, int desde, int hasta)
        {
            var balanza = _repositorio.Obtener<BalanzaPuerto>(q => q.CodigoBalanza == numeroBalanza);
            var balanzadaRecibida = new BalanzadaRecibidaDTO
            {
                NumeroBalanza = numeroBalanza,
                CodigoDispositivo = balanza.CodigoDispositivo,
                OffsetBalanza = balanza.OffSetPlc,
                IntentosValidacion = balanza.IntentosValidacion,
                IdOffset = hasta + 1,
                UltimaValidacion = desde
            };

            ValidarCrearCargaPendiente(balanzadaRecibida);
        }

        // Un inicio falso se da cuando un lote se detiene y se vuelve a reanudar. No existe un fin para el ultimo inicio,
        // por lo que éste no puede ser tratado como un inicio válido. Por lo visto siempre contienen valores mayores a 0 para ToneladasAW.
        // Estos inicios falsos luego actualizarán el peso programado del inicio verdadero.
        private bool EsInicioFalso(BalanzadaRecibidaDTO balanzada)
        {
            if (balanzada.ToneladasAW > 0)
            {
                return true;
            }

            var inicioAnterior = _repositorio.EjecutarComando(new ObtenerCargaInicio(balanzada.IdOffset, balanzada.NumeroBalanza));

            if (inicioAnterior == null)
            {
                return false;
            }
            
            return inicioAnterior.CargaOpuesta == null; // Si CargaOpuesta es null, el último inicio no tuvo fin, por lo que el nuevo inicio es falso.
        }

        private Carga ObtenerInicioActualizar(BalanzadaRecibidaDTO balanzada)
        {
            var ultimoInicio = _repositorio.EjecutarComando(new ObtenerCargaInicio(balanzada.IdOffset, balanzada.NumeroBalanza));
            return ultimoInicio.CargaOpuesta == null ? ultimoInicio : null;
        }

        private Carga ConstruirCarga(BalanzadaRecibidaDTO balanzada, Carga cargaOpuesta)
        {
            var balanza = _repositorio.Obtener<BalanzaPuerto>(x => x.CodigoBalanza == balanzada.NumeroBalanza);

            var vapor = ObtenerVapor(balanzada.Vapor);
            var bodega = ObtenerBodega(balanzada.Bodega);
            var destino = ObtenerDestino(balanzada.Destino);
            var exportador = ObtenerExportador(balanzada.Exportador);

            var commodity = new MaterialPuerto();
            if (balanzada.Vapor == string.Empty && balanzada.Bodega == string.Empty && balanzada.Destino == string.Empty && balanzada.Exportador == string.Empty && balanzada.Commodity == string.Empty)
            {
                commodity = _repositorio.ObtenerMenor<MaterialPuerto, int>(e => e.Descripcion.Contains("patron"), x => x.Id);
            }
            else
            {
                commodity = ObtenerMaterial(balanzada.Commodity);
            }

            return new Carga
            {
                Id = balanzada.IdOffset,
                NumeroBalanza = balanzada.NumeroBalanza,
                Fecha = balanzada.Fecha,
                Tipo = balanzada.TipoBalanzada,
                Vapor = vapor,
                Bodega = bodega,
                Destino = destino,
                Exportador = exportador,
                Material = commodity,
                PesoProgramado = balanzada.PesoProgramado,
                ToneladasAW = balanzada.ToneladasAW,
                FechaInicio = balanzada.FechaInicio,
                CargaOpuesta = cargaOpuesta
            };
        }

        private RegistroBalanzaPuerto ConstruirRegistroBalanzaPuerto(BalanzadaRecibidaDTO balanzada, string tipoBalanzada)
        {
            return new RegistroBalanzaPuerto
            {
                EnviadoASap = false,
                Fecha = balanzada.Fecha,
                Id = balanzada.IdOffset,
                NumeroBalanza = balanzada.NumeroBalanza,
                Tipo = tipoBalanzada
            };
        }

        private Balanzada ConstruirBalanzada(BalanzadaRecibidaDTO balanzada, Carga inicio)
        {
            return new Balanzada
            {
                Id = balanzada.IdOffset,
                NumeroBalanza = balanzada.NumeroBalanza,
                Fecha = balanzada.Fecha,
                Tipo = balanzada.TipoBalanzada,
                PesoBruto = balanzada.PesoBruto,
                PesoTara = balanzada.PesoTara,
                PesoNeto = balanzada.PesoNeto,
                Capacidad = balanzada.Capacidad,
                CargaInicial = inicio
            };
        }

        private Vapor ObtenerVapor(string vapor)
        {
            if (vapor != null)
            {
                Vapor registro;

                if (vapor.Length == 15)
                {
                    registro = _repositorio.Obtener<Vapor>(v => v.Nombre.StartsWith(vapor));
                }
                else
                {
                    registro = _repositorio.Obtener<Vapor>(e => e.Nombre == vapor);
                }

                if (registro == null)
                {
                    registro = new Vapor
                    {
                        Nombre = vapor
                    };
                    _repositorio.Agregar(registro);
                    _repositorio.GuardarCambios();
                }

                return registro;
            }
            return null;
        }

        private Bodega ObtenerBodega(string bodega)
        {
            if (bodega != null)
            {
                var registro = _repositorio.Obtener<Bodega>(e => e.Nombre == bodega);

                if (registro == null)
                {
                    registro = new Bodega
                    {
                        Nombre = bodega
                    };
                    _repositorio.Agregar(registro);
                    _repositorio.GuardarCambios();
                }

                return registro;
            }
            return null;
        }

        private Destino ObtenerDestino(string destino)
        {
            if (destino != null)
            {
                var registro = _repositorio.Obtener<Destino>(e => e.Nombre == destino);
                if (registro == null)
                {
                    registro = new Destino
                    {
                        Nombre = destino
                    };
                    _repositorio.Agregar(registro);
                    _repositorio.GuardarCambios();
                }

                return registro;
            }
            return null;
        }

        private Exportador ObtenerExportador(string exportador)
        {
            if (exportador != null)
            {
                var registro = _repositorio.Obtener<Exportador>(e => e.Nombre == exportador);
                if (registro == null)
                {
                    registro = new Exportador
                    {
                        Nombre = exportador
                    };
                    _repositorio.Agregar(registro);
                    _repositorio.GuardarCambios();
                }

                return registro;
            }
            return null;
        }

        private MaterialPuerto ObtenerMaterial(string commodity)
        {
            if (commodity != null)
            {
                var registro = _repositorio.Obtener<MaterialPuerto>(e => e.Descripcion == commodity);
                if (registro == null)
                {
                    registro = new MaterialPuerto
                    {
                        Descripcion = commodity,
                        CodigoSAP = "Nuevo",
                        Almacen = null
                    };
                    _repositorio.Agregar(registro);
                    _repositorio.GuardarCambios();
                }
                return registro;
            }
            return null;
        }

        private void EnviarASap(bool enviadoASap, Carga cargaInicial, int idBalanzada, string numeroBalanza)
        {
            if (!enviadoASap && cargaInicial != null)
            {
                _servicioComandos.Ejecutar(new EnviarLecturaBalanzadaTransmisionASap { Id = idBalanzada, NumeroBalanza = numeroBalanza });
            }
        }
    }
}