using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Helpers;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Configuration;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCargaDeCupoNoGrano : ProcesadorComando<CrearCargaDeCupoNoGrano>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IServicioRepositorio servicioRepositorio;

        public ProcesadorCrearCargaDeCupoNoGrano(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioOrquestador servicioOrquestador, IServicioRepositorio servicioRepositorio)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
            this.servicioRepositorio = servicioRepositorio;
        }

        public override Resultado Ejecutar(CrearCargaDeCupoNoGrano comando)
        {
            var resultado = new ResultadoCrearCargaDeCupo();
            CargaDeCupo nuevoCupo = null;

            try
            {
                ValidarCupo(comando);
                nuevoCupo = CrearEntidad(comando);

                if (comando.Dto.ImprimeTarjetaDeAcceso)
                    ImprimirTarjetaDeAcceso(nuevoCupo.Numero, nuevoCupo.Centro.Id, nuevoCupo.PuestoDeTrabajo.Id);

                if (!comando.Dto.NoAsignaCalleEnGaritaEntrada) {
                    if (nuevoCupo.Material != null)
                    {
                        var resultadoCrearCalle = AsignarCalle(nuevoCupo.Id, nuevoCupo.Centro.Id);
                        MostrarMensajeEnCartel(resultadoCrearCalle.CalleNombre, nuevoCupo.Patente, nuevoCupo.PuestoDeTrabajo.Id);
                        resultado.Mensaje = resultadoCrearCalle.Disponibilidad <= 5 && resultadoCrearCalle.Disponibilidad > 0
                            ? $"Carga Exitosa, se asignó {resultadoCrearCalle.CalleNombre}, ESPACIO DISPONIBLE: {resultadoCrearCalle.Disponibilidad} camiones"
                            : $"Carga Exitosa, se asignó {resultadoCrearCalle.CalleNombre}";
                    } else
                    {
                        MostrarMensajeEnCartel("Mesa FAS", nuevoCupo.Patente, nuevoCupo.PuestoDeTrabajo.Id);
                        resultado.Mensaje = $"Carga Exitosa, Camión debe dirigirse a Mesa FAS.";
                    }
                }

                resultado.Id = nuevoCupo.Id;
                if (comando.Dto.TipoOrdenCargaNoGranos.HasValue)
                {
                    resultado.FastPassValido = ValidarFastPass(comando, resultado);
                    resultado.FastPassWorkflowDefinicionId = ObtenerWorkflowDefinicionId(comando.Dto.TipoOrdenCargaNoGranos.Value);
                }
                AbrirBarrera(nuevoCupo.PuestoDeTrabajo.Entrada);
            }
            catch (ErrorCrearCupoNoGranoExcepcion ex)
            {
                Log.Error(ex, "Error de aplicación en CrearCargaDeCupoNoGrano");
                EliminarCupoSiFueCreado(nuevoCupo);
                resultado.Error("Error", ex.Message);
            }
            catch (ErrorNoBloqueanteCrearCupoNoGranoExcepcion ex)
            {
                Log.Info(ex, "Error No Bloqueante de aplicación en CrearCargaDeCupoNoGrano");
                resultado.Error("Advertencia", "La carga se creó correctamente, pero " + ex.Message + " Camión debe dirigirse a Mesa FAS.");

                AbrirBarrera(nuevoCupo.PuestoDeTrabajo.Entrada);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error de aplicación no esperado en CrearCargaDeCupoNoGrano");
                EliminarCupoSiFueCreado(nuevoCupo);
                resultado.Error("Error", "Ocurrió un error al crear cupo no grano.");
            }

            return resultado;
        }

        private int ObtenerWorkflowDefinicionId(TipoOrdenCargaNoGranos tipoOrdenCarga)
        {
            var codigoWorkflow = tipoOrdenCarga == TipoOrdenCargaNoGranos.Insumos ? Constantes.WorkFlow.workflowMaterialNoProductivo 
                : tipoOrdenCarga == TipoOrdenCargaNoGranos.FasonConFlete ? Constantes.WorkFlow.workflowFason
                : tipoOrdenCarga == TipoOrdenCargaNoGranos.FasonSinFlete ? Constantes.WorkFlow.workflowFasonSinFlete
                : string.Empty;
            return Repositorio.ObtenerMayor<WorkflowDefinicion, int, int>(x => x.Workflow.Codigo == codigoWorkflow && x.Activa && x.FechaActivacion <= DateTime.Now, x => x.Id , x => x.Id);

        }

        private CargaDeCupo CrearEntidad(CrearCargaDeCupoNoGrano comando)
        {
            EliminarDuplicadas(comando);
            var cupo = Conversor.Convertir<CargaDeCupoDto, CargaDeCupo>(comando.Dto);
            if (comando.Dto.TipoOrdenCargaNoGranos == TipoOrdenCargaNoGranos.FasonConFlete || comando.Dto.TipoOrdenCargaNoGranos == TipoOrdenCargaNoGranos.FasonSinFlete)
                cupo.FleteMOA = comando.Dto.TipoOrdenCargaNoGranos == TipoOrdenCargaNoGranos.FasonConFlete ? "true" : "false";

            cupo.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            cupo.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            cupo.PuestoDeTrabajo = Repositorio.Obtener<PuestoDeTrabajo>(comando.Dto.PuestoDeTrabajoId);
            cupo.TipoVehiculo = comando.Dto.TipoVehiculo;
            Repositorio.Agregar(cupo);
            Repositorio.GuardarCambios();
            return cupo;
        }

        private void EliminarDuplicadas(CrearCargaDeCupoNoGrano comando)
        {
            var cuposDuplicadosPorTarjeta = Repositorio.Listar<CargaDeCupo>(x => x.Numero == comando.Dto.Numero && x.Recorrido == null && x.Centro.Id == comando.Dto.CentroId);
            foreach (var i in cuposDuplicadosPorTarjeta)
            {
                Repositorio.Remover(i);
            }

            var cuposDuplicadosPorPatente = Repositorio.Listar<CargaDeCupo>(x => x.Patente == comando.Dto.Patente && x.Recorrido == null && x.Centro.Id == comando.Dto.CentroId);
            foreach (var i in cuposDuplicadosPorPatente)
            {
                Repositorio.Remover(i);
            }
        }

        private void ValidarCupo(CrearCargaDeCupoNoGrano comando)
        {
            if (!string.IsNullOrEmpty(comando.Dto.Patente) 
                && Repositorio.Existe<CargaDeCupo>(e => e.Patente == comando.Dto.Patente && e.Centro.Id == comando.Dto.CentroId && e.Recorrido != null && !e.Recorrido.Terminado))
                throw new ErrorCrearCupoNoGranoExcepcion($"El camion {comando.Dto.Patente} ya se encuentra en circuito");

            if (servicioRepositorio.EsTarjetaBloqueada(comando.Dto.Numero, comando.Dto.CentroId))
                throw new ErrorCrearCupoNoGranoExcepcion(Textos.AsignacionTarjetaDeAcceso_TarjetaBloqueada);

            if (!servicioRepositorio.EsTarjetaEnRangoValido(comando.Dto.Numero, comando.Dto.CentroId))
                throw new ErrorCrearCupoNoGranoExcepcion(Textos.AsignacionTarjetaDeAcceso_TarjetaSinRango);

            if (Repositorio.Existe<Recorrido>(x => x.TarjetaDeAcceso == comando.Dto.Numero && x.Centro.Id == comando.Dto.CentroId && x.Terminado == false))
                throw new ErrorCrearCupoNoGranoExcepcion(Textos.ImpresionTarjetaDeAcceso_EnUso);

            var validarTarjetaEnUsoPendienteSinRecorridoConfig = ConfigurationManager.AppSettings["ValidarTarjetaEnUsoEtapaPendiente"];
            if (bool.TryParse(validarTarjetaEnUsoPendienteSinRecorridoConfig, out bool validarTarjetaEnUsoPendienteSinRecorrido) && validarTarjetaEnUsoPendienteSinRecorrido)
            {
                var workflowsPendientes = servicioRepositorio.ListarDatosDeWorkflowsPendientes(comando.Dto.CentroId, 0).Where(x => x.NumeroDeTarjeta == comando.Dto.Numero);
                if (workflowsPendientes.Any())
                    throw new ErrorCrearCupoNoGranoExcepcion(string.Format(Textos.TarjetaDeAcceso_EnUso_Pendiente, workflowsPendientes.FirstOrDefault().Patente));
            }

            if (!Repositorio.Existe<PuestoDeTrabajo>(x => x.Id == comando.Dto.PuestoDeTrabajoId))
                throw new ErrorCrearCupoNoGranoExcepcion(Textos.RequierePuestoDeTrabajo);
        }

        private ResultadoCrearCalle AsignarCalle(int cargaCupoId, int centroId)
        {
            var resultadoAsignarCalle = servicioComandos.Ejecutar(new CrearCallePorRecorrido
            {
                TipoCalle = TipoCalle.NoGranos,
                CargaDeCupoId = cargaCupoId,
                CentroId = centroId,
            }) as ResultadoCrearCalle;
            if (resultadoAsignarCalle.HayErrores)
                throw new ErrorCrearCupoNoGranoExcepcion("Ocurrió un error al asignar la calle.");

            return resultadoAsignarCalle;
        }

        private void MostrarMensajeEnCartel(string mensaje, string patente, int puestoId)
        {
            try
            {
                var mensajesCartelLedConfig = Repositorio.Listar<MensajeCartelLed>(x => x.Codigo == CodigoMensajeCartelLed.GaritaIngresoAsignarCalle && x.Habilitado);
                var mensajes = mensajesCartelLedConfig.Select(s =>
                    new EnviarMensajeCartelLed
                    {
                        Mensaje = string.Format(s.Mensaje, mensaje, patente),
                        PuestoDeTrabajoId = puestoId,
                        NumeroPrograma = s.Programa,
                        NumeroTrama = s.Trama,
                        NumeroVariable = s.Variable,
                        SegundosDeEspera = s.SegundosDeEspera
                    }
                ).ToList();
                servicioComandos.Ejecutar(new EnviarMensajesAsincronoCartelLed { Mensajes = mensajes });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ProcesadorCrearCargaDeCupoNoGrano MostrarMensajeEnCartel");
                throw new ErrorNoBloqueanteCrearCupoNoGranoExcepcion("falló la comunicación con el cartel.");
            }
        }

        private void ImprimirTarjetaDeAcceso(string numero, int centroId, int puestoId)
        {
            var resultadoImpresion = servicioComandos.Ejecutar(new ImprimirTarjetaDeAcceso
            {
                Dto = new ImpTarjetaDeAccesoDto
                {
                    Codigo = "ImpresionTarjetaDeAcceso",
                    Numero = numero,
                    Fecha = DateTime.Now.Formatted(),
                    CentroId = centroId,
                    PuestoDeTrabajoId = puestoId,
                },
                OrigenImpresion = "CargaDeCupoController"
            });
            if (resultadoImpresion.HayErrores)
                throw new ErrorCrearCupoNoGranoExcepcion("Ocurrió un error al imprimir la tarjeta de acceso.");
        }

        private void AbrirBarrera(string codigoBarrera)
        {
            try
            {
                servicioOrquestador.Ejecutar(new EjecutarAperturaBarrera { CodigoDispositivo = codigoBarrera });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ProcesadorCrearCargaDeCupoNoGrano AbrirBarrera");
                throw new ErrorNoBloqueanteCrearCupoNoGranoExcepcion("falló la apertura de la barrera.");
            }
        }

        private bool ValidarFastPass(CrearCargaDeCupoNoGrano comando, ResultadoCrearCargaDeCupo resultadoCrearCargaDeCupo)
        {
            var fastPassValido = false;
            switch (comando.Dto.TipoOrdenCargaNoGranos)
            {
                case TipoOrdenCargaNoGranos.FasonConFlete:
                case TipoOrdenCargaNoGranos.FasonSinFlete:
                    var resultadoFastPassFason = ValidarFastPassFason(comando);
                    resultadoCrearCargaDeCupo.OrdenCargaInternaFason = resultadoFastPassFason.Dto;
                    resultadoCrearCargaDeCupo.ControlRecorrido = resultadoFastPassFason.ControlRecorrido;
                    fastPassValido = !resultadoFastPassFason.HayErrores;
                    break;
                case TipoOrdenCargaNoGranos.Insumos:
                    var resultadoFastPassInsumos = ValidarFastPassInsumos(comando);
                    resultadoCrearCargaDeCupo.OrdenCargaInterna = resultadoFastPassInsumos.Dto;
                    resultadoCrearCargaDeCupo.ControlRecorrido = resultadoFastPassInsumos.ControlRecorrido;
                    fastPassValido = !resultadoFastPassInsumos.HayErrores;
                    break;
                case TipoOrdenCargaNoGranos.Fas:
                    var resultadoFastPassFas = ValidarFastPassFas(comando);
                    resultadoCrearCargaDeCupo.OrdenCargaFasDto = resultadoFastPassFas.Dto;
                    resultadoCrearCargaDeCupo.ControlRecorrido = resultadoFastPassFas.ControlRecorrido;
                    fastPassValido = !resultadoFastPassFas.HayErrores;
                    break;
            }

            if (!fastPassValido)
                throw new ErrorNoBloqueanteCrearCupoNoGranoExcepcion("falló el fast pass.");


            return fastPassValido;
        }

        private ResultadoValidarOrdenCargaInterna ValidarFastPassInsumos(CrearCargaDeCupoNoGrano comando)
        {
            var comandoValidacion = new ValidarOrdenCargaInterna { 
                Dto = new OrdenCargaInternaDto { 
                    PatenteCamion = comando.Dto.Patente,
                    MaterialId = comando.Dto.MaterialId.Value,
                },
                AplicaFastPass = true,
                CentroId = comando.Dto.CentroId,
                PuestoDeTrabajoId = comando.Dto.PuestoDeTrabajoId,
                Usuario = comando.Usuario,
            };
            return servicioComandos.Ejecutar(comandoValidacion) as ResultadoValidarOrdenCargaInterna;
        }

        private ResultadoOrdenFason ValidarFastPassFason(CrearCargaDeCupoNoGrano comando)
        {
            var respuesta = servicioComandos.Ejecutar(new ValidarOrdenCargaInternaFason()
            {
                CentroId = comando.Dto.CentroId,
                Usuario = comando.Usuario,
                AplicaFastPass = true,
                Orden = new OrdenDeCargaDto
                {
                    PatenteChasis = comando.Dto.Patente,
                    Id = comando.OrdenOperacionesId.GetValueOrDefault(),
                    MaterialId = comando.Dto.MaterialId.ToString(),
                }
            }) as ResultadoOrdenFason;

            if (respuesta.HayErrores)
            {
                Log.Error("Error al validar FastPass Fason: {0}", respuesta.Errores.Values.FirstOrDefault());
            }

            return respuesta;
        }

        private ResultadoOrdenFas ValidarFastPassFas(CrearCargaDeCupoNoGrano comando)
        {
            var respuesta = servicioComandos.Ejecutar(new ValidarOrdenCargaInternaFas()
            {
                CentroId = comando.Dto.CentroId,
                Usuario = comando.Usuario,
                AplicaFastPass = true,
                Orden = new OrdenDeCargaDto
                {
                    PatenteChasis = comando.Dto.Patente,
                    Id = comando.OrdenOperacionesId.GetValueOrDefault(),
                    MaterialId = comando.Dto.MaterialId.ToString(),
                }
            }) as ResultadoOrdenFas;

            if (respuesta.HayErrores)
            {
                Log.Error("Error al validar FastPass Fas: {0}", respuesta.Errores.Values.FirstOrDefault());
            }

            return respuesta;
        }

        private void EliminarCupoSiFueCreado(CargaDeCupo cupo)
        {
            if (cupo != null)
            {
                Repositorio.Remover(cupo);
                Repositorio.GuardarCambios();
            }
        }
    }

    public class ErrorCrearCupoNoGranoExcepcion : Exception
    {
        public ErrorCrearCupoNoGranoExcepcion(string message) : base(message)
        {
        }
    }

    public class ErrorNoBloqueanteCrearCupoNoGranoExcepcion : Exception
    {
        public ErrorNoBloqueanteCrearCupoNoGranoExcepcion(string message) : base(message)
        {
        }
    }
}
