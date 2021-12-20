using System;
using System.Collections.Generic;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ComandosEF;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLecturaBalanzada : ProcesadorComando<CrearLecturaBalanzada>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador orquestador;

        public ProcesadorCrearLecturaBalanzada(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioOrquestador orquestador) : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.orquestador = orquestador;
        }

        public override Resultado Ejecutar(CrearLecturaBalanzada comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se está ejecutando ProcesadorCrearLecturaBalanzada con balanza: {0}", comando.CodigoDispositivo);

                var numeroBalanza = comando.Informacion["numeroBalanza"];
                var tipoBalanzada = comando.Informacion["tipoBalanzada"]; // nos va a definir que tipo es
                var balanza = Repositorio.Obtener<BalanzaPuerto>(x => x.CodigoBalanza == numeroBalanza);
                var offsetBalanza = balanza.OffSetPlc;
                var id = Int32.Parse(comando.Informacion["id"]) + offsetBalanza;
                var fecha = DateTime.ParseExact(comando.Informacion["fecha"], "dd-MM-yyyyHH:mm", CultureInfo.InvariantCulture);

                if (Repositorio.Existe<RegistroBalanzaPuerto>(e => e.Id == id && e.NumeroBalanza == numeroBalanza)) //error, creo que no puede pasar 
                {
                    resultado.Error("IdDeBalanzada: " + id, Textos.BalanzaPuerto_IdExistente);
                    return resultado;
                }

                if (tipoBalanzada == "inicio")
                {
                    if (EsInicioFalso(id, numeroBalanza, balanza.CodigoDispositivo))
                    {
                        Repositorio.Agregar(new RegistroBalanzaPuerto
                        {
                            EnviadoASap = false,
                            Fecha = fecha,
                            Id = id,
                            NumeroBalanza = numeroBalanza,
                            Tipo = "inicioError"
                        });
                        var actualizarCargaInicio = Repositorio.ObtenerMayor<Carga, int>(x => 
                            x.Id < id && 
                            x.NumeroBalanza == numeroBalanza && 
                            x.Tipo == "inicio" && 
                            !x.CargaOpuesta_Id.HasValue &&
                            x.Bodega.Nombre == comando.Informacion["vapor"] &&
                            x.Vapor.Nombre == comando.Informacion["bodega"] &&
                            x.Destino.Nombre == comando.Informacion["destino"] &&
                            x.Exportador.Nombre == comando.Informacion["exportador"] &&
                            x.Material.Descripcion == comando.Informacion["commodity"]
                            , x => x.Id);
                        if (actualizarCargaInicio != null)
                        {
                            actualizarCargaInicio.PesoProgramado = string.IsNullOrEmpty(comando.Informacion["pesoProgramado"]) ? 0 : int.Parse(comando.Informacion["pesoProgramado"]);
                        }
                    }
                    else
                    {
                        var fin = Repositorio.ObtenerMayor<Carga, int>(x => x.NumeroBalanza == numeroBalanza && x.Tipo == "fin" && x.FechaInicio == fecha && x.Id > id && !x.CargaOpuesta_Id.HasValue, x => x.Id);
                        var carga = CrearCarga(comando, id, numeroBalanza, fecha, tipoBalanzada, fin);
                        if (carga != null)
                        {
                            Repositorio.Agregar(carga);
                            Repositorio.GuardarCambios();
                        }

                        if (fin != null)
                        {
                            fin.CargaOpuesta = carga;
                            var balanzadasModificadas = Repositorio.EjecutarComando(new AsignarBalanzadas(id, numeroBalanza, id, fin.Id));
                            EnviarASap(balanzadasModificadas);
                        }
                        else
                        {
                            var siguienteCarga = Repositorio.ObtenerMenor<Carga, int>(x => x.NumeroBalanza == numeroBalanza && x.Id > id, x => x.Id);
                            if (siguienteCarga == null || siguienteCarga.Tipo == "inicio")
                            {
                                //si la siguienteCarga es un fin y no er3a el fin de este inicio, se perdió otro inicio en el medio.
                                //en ese caso preferimos no hacer nada
                                var balanzadasModificadas = Repositorio.EjecutarComando(new AsignarBalanzadas(id, numeroBalanza, id, siguienteCarga != null ? siguienteCarga.Id : -1));
                                EnviarASap(balanzadasModificadas);
                            }
                        }
                    }
                    Repositorio.GuardarCambios();
                    BorrarRegistroEnOrquestador(comando.CodigoDispositivo, id - offsetBalanza);
                }
                else if (tipoBalanzada == "fin")
                {
                    var parseFechaInicio = DateTime.ParseExact(comando.Informacion["fechaInicio"], "dd-MM-yyyyHH:mm", CultureInfo.InvariantCulture);

                    var inicio = Repositorio.ObtenerMayor<Carga, int>(x => x.NumeroBalanza == numeroBalanza && x.Tipo == "inicio" && x.Fecha == parseFechaInicio && x.Id < id && !x.CargaOpuesta_Id.HasValue, x => x.Id);
                    var carga = CrearCarga(comando, id, numeroBalanza, fecha, tipoBalanzada, inicio);

                    if (inicio != null)
                    {
                        inicio.CargaOpuesta = carga;
                        inicio.EnviadoASap = !Repositorio.Existe<Balanzada>(x => x.CargaInicial_Id == inicio.Id && x.NumeroBalanza == inicio.NumeroBalanza && !x.EnviadoASap);
                    }
                    Repositorio.Agregar(carga);
                    Repositorio.GuardarCambios();
                    BorrarRegistroEnOrquestador(comando.CodigoDispositivo, id - offsetBalanza);
                }
                else if (tipoBalanzada == "balanzada")
                {
                    var inicio = Repositorio.ObtenerMayor<Carga, int>(x => x.Id < id && x.NumeroBalanza == numeroBalanza && x.Tipo == "inicio", x => x.Id);
                    if(inicio != null)
                    {
                        var fin = Repositorio.ObtenerMayor<Carga, int>(x => x.Id > inicio.Id  && x.NumeroBalanza == numeroBalanza && x.Tipo == "fin" && x.Id < id, x => x.Id);
                        inicio = fin == null ? inicio : null;
                    }
                    
                    var balanzada = new Balanzada
                    {
                        Id = id,
                        NumeroBalanza = numeroBalanza,
                        Fecha = fecha,
                        Tipo = tipoBalanzada,
                        PesoBruto = string.IsNullOrEmpty(comando.Informacion["pesoBruto"]) ? 0 : int.Parse(comando.Informacion["pesoBruto"]),
                        PesoTara = string.IsNullOrEmpty(comando.Informacion["pesoTara"]) ? 0 : int.Parse(comando.Informacion["pesoTara"]),
                        PesoNeto = string.IsNullOrEmpty(comando.Informacion["pesoNeto"]) ? 0 : int.Parse(comando.Informacion["pesoNeto"]),
                        Capacidad = comando.Informacion["capacidad"],
                        CargaInicial = inicio != null && inicio.CargaOpuesta_Id.HasValue && inicio.CargaOpuesta_Id < id ? null : inicio
                    };
                    Repositorio.Agregar(balanzada);
                    Repositorio.GuardarCambios();
                    EnviarASap(balanzada);
                }
                else
                {
                    var registroError = new RegistroBalanzaPuerto
                    {
                        Id = id,
                        NumeroBalanza = numeroBalanza,
                        Fecha = fecha,
                        EnviadoASap = false,
                        Tipo = tipoBalanzada
                    };
                    Repositorio.Agregar(registroError);
                    Repositorio.GuardarCambios();
                    Log.Error("La balanzada " + id + " fue recibida con error: " + string.Join(";", comando.Informacion));
                    BorrarRegistroEnOrquestador(comando.CodigoDispositivo, id - offsetBalanza);
                }

                Log.Info("Se ejecutó correctamente ProcesadorCrearLecturaBalanzada con balanza: {0} para la balanzada: {1}", comando.CodigoDispositivo, id);
                resultado.Id = id;
                return resultado;
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorCrearLecturaBalanzada con balanza: {0}", comando.CodigoDispositivo);
                resultado.Error("", e.Message);
            }

            return resultado;
        }

        private bool EsInicioFalso(int id, string numeroBalanza, string codigoDispositivo)
        {
            var registroAnterior = Repositorio.Obtener<RegistroBalanzaPuerto>(x => x.Id == id - 1 && x.NumeroBalanza == numeroBalanza);
            if(registroAnterior == null)
            {
                servicioComandos.Ejecutar(new ValidarConsistenciaBalanzadas { Balanza = codigoDispositivo, CodigoDispositivo = codigoDispositivo, Desde = id - 1, Hasta = id});
                registroAnterior = Repositorio.Obtener<RegistroBalanzaPuerto>(x => x.Id == id - 1 && x.NumeroBalanza == numeroBalanza);
            }

            return registroAnterior != null && (registroAnterior.Tipo == "error41" || (registroAnterior.Tipo == "error" && EsInicioFalso(registroAnterior.Id, numeroBalanza, codigoDispositivo)));
        }

        private void EnviarASap(List<Balanzada> balanzadas)
        {
            if(balanzadas != null)
            {
                foreach (var b in balanzadas)
                {
                    EnviarASap(b);
                }
            }            
        }

        private void EnviarASap(Balanzada b)
        {
            if (!b.EnviadoASap && b.CargaInicial != null)
            {
                servicioComandos.Ejecutar(new EnviarLecturaBalanzadaTransmisionASap { Id = b.Id, NumeroBalanza = b.NumeroBalanza });
            }
        }

        private void BorrarRegistroEnOrquestador(string codigoDispositivo, int id)
        {
            orquestador.Ejecutar(new EjecutarBorrarBalanzada { CodigoDispositivo = codigoDispositivo, IdBorrado = id });
        }

        private Carga CrearCarga(CrearLecturaBalanzada comando, int id, string numeroBalanza, DateTime fecha, string tipoBalanzada, Carga cargaOpuesta)
        {
            var nombreVapor = ExisteVapor(comando);
            var nombreBodega = ExisteBodega(comando);
            var nombreDestino = ExisteDestino(comando);
            var nombreExportador = ExisteExportador(comando);
            var nombreCommodity = ExisteMaterial(comando);
            var parseFechaInicio = comando.Informacion.ContainsKey("fechaInicio") && !string.IsNullOrEmpty(comando.Informacion["fechaInicio"]) ?
                                        DateTime.ParseExact(comando.Informacion["fechaInicio"], "dd-MM-yyyyHH:mm", CultureInfo.InvariantCulture) :
                                        (DateTime?)null;
            var pesoProgramado = string.IsNullOrEmpty(comando.Informacion["pesoProgramado"]) ? 0 : int.Parse(comando.Informacion["pesoProgramado"]);

            if (tipoBalanzada == "inicio")
            {
                //si encontramos un inicio que cumpla estas condiciones, entonces este inicio se trata de una actualización de ese
                var inicioAActualizar = Repositorio.ObtenerMayor<Carga, int>(x =>
                        x.NumeroBalanza == numeroBalanza &&
                        x.Tipo == "inicio" &&
                        !x.CargaOpuesta_Id.HasValue &&
                        x.Destino.Nombre == nombreBodega &&
                        x.Exportador.Nombre == nombreExportador &&
                        x.Material.Descripcion == nombreCommodity &&
                        x.Vapor.Nombre == nombreVapor &&
                        x.Id < id, x => x.Id);
                if (inicioAActualizar != null)
                {
                    inicioAActualizar.PesoProgramado = pesoProgramado;
                    return null;
                }
            }
            var esPatron = false;
            if(nombreVapor == string.Empty && 
                nombreBodega == string.Empty && 
                nombreDestino == string.Empty && 
                nombreExportador == string.Empty &&
                nombreCommodity == string.Empty)
            {
                esPatron = true;
            }

            return new Carga
            {
                Id = id,
                NumeroBalanza = numeroBalanza,
                Fecha = fecha,
                Tipo = tipoBalanzada,
                Vapor = Repositorio.Obtener<Vapor>(e => e.Nombre == nombreVapor),
                Bodega = Repositorio.Obtener<Bodega>(e => e.Nombre == nombreBodega),
                Destino = Repositorio.Obtener<Destino>(e => e.Nombre == nombreDestino),
                Exportador = Repositorio.Obtener<Exportador>(e => e.Nombre == nombreExportador),
                Material = !esPatron ? Repositorio.Obtener<MaterialPuerto>(e => e.Descripcion == nombreCommodity) :
                    Repositorio.ObtenerMenor<MaterialPuerto, int>(e => e.Descripcion.Contains("patron"), x => x.Id),
                PesoProgramado = pesoProgramado,
                ToneladasAW = string.IsNullOrEmpty(comando.Informacion["toneladasaw"]) ? 0 : int.Parse(comando.Informacion["toneladasaw"]),
                FechaInicio = parseFechaInicio,
                CargaOpuesta = cargaOpuesta
            };
        }

        private string ExisteVapor(CrearLecturaBalanzada comando)
        {
            var vapor = string.Empty;
            if (comando.Informacion.ContainsKey("vapor"))
            {
                vapor = comando.Informacion["vapor"];
                if (!Repositorio.Existe<Vapor>(e => e.Nombre == vapor))
                {
                    var nuevoVapor = new Vapor
                    {
                        Nombre = vapor
                    };
                    Repositorio.Agregar(nuevoVapor);
                    Repositorio.GuardarCambios();
                }
            }
            return vapor;
        }

        private string ExisteBodega(CrearLecturaBalanzada comando)
        {
            var resultado = string.Empty;
            if (comando.Informacion.ContainsKey("bodega"))
            {
                resultado = comando.Informacion["bodega"];
                if (!Repositorio.Existe<Bodega>(e => e.Nombre == resultado))
            {
                var nuevaBodega = new Bodega
                {
                    Nombre = resultado
                };
                Repositorio.Agregar(nuevaBodega);
                Repositorio.GuardarCambios();
                }
            }
            return resultado;
        }

        private string ExisteDestino(CrearLecturaBalanzada comando)
        {
            var resultado = string.Empty;
            if (comando.Informacion.ContainsKey("destino"))
            {
                resultado = comando.Informacion["destino"];
                if (!Repositorio.Existe<Destino>(e => e.Nombre == resultado))
                {
                    var nuevaBodega = new Destino
                    {
                        Nombre = resultado
                    };
                    Repositorio.Agregar(nuevaBodega);
                    Repositorio.GuardarCambios();
                }
            }
            return resultado;
        }

        private string ExisteExportador(CrearLecturaBalanzada comando)
        {
            var resultado = string.Empty;
            if (comando.Informacion.ContainsKey("exportador"))
            {
                resultado = comando.Informacion["exportador"];
                if (!Repositorio.Existe<Exportador>(e => e.Nombre == resultado))
                {
                    var nuevoExportador = new Exportador
                    {
                        Nombre = resultado
                    };
                    Repositorio.Agregar(nuevoExportador);
                    Repositorio.GuardarCambios();
                }
            }
            return resultado;
        }

        private string ExisteMaterial(CrearLecturaBalanzada comando)
        {
            var resultado = string.Empty;
            if (comando.Informacion.ContainsKey("commodity"))
            {
                resultado = comando.Informacion["commodity"];
                if (!Repositorio.Existe<MaterialPuerto>(e => e.Descripcion == resultado))
                {
                    var nuevoMaterial = new MaterialPuerto
                    {
                        Descripcion = resultado,
                        CodigoSAP = "Nuevo",
                        Almacen = null
                    };
                    Repositorio.Agregar(nuevoMaterial);
                    Repositorio.GuardarCambios();
                }
            }
            return resultado;
        }
    }
}
