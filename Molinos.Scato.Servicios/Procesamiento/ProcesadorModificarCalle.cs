using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCalle : ProcesadorModificar<ModificarCalle>
    {
        private readonly IServicioComandos servicioComandos;

        public ProcesadorModificarCalle(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
        }

        protected override void ModificarEntidad(ModificarCalle comando)
        {
            var calle = Repositorio.Obtener<Calle>(comando.Dto.Id);

            if (comando.Dto.TipoCalle == TipoCalle.PreBalanzaGranos && comando.Dto.EsPasoDirecto)
                LlamarCallePrebalanzaPrioritario(comando);

            if (comando.Dto.TipoCalle == TipoCalle.PreBalanzaGranos && !comando.Dto.EsPasoDirecto && calle.EsPasoDirecto)
                LiberarCallePrebalanzaPrioritario(comando);

            Conversor.Convertir(comando.Dto, calle);
            calle.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            calle.CaracteristicaDeCalidad = Repositorio.Obtener<CaracteristicaDeCalidad>(comando.Dto.CaracteristicaDeCalidadId);
            if (comando.Dto.CalleCaladoId > 0)
                calle.CalleCalado = Repositorio.Obtener<Calle>(comando.Dto.CalleCaladoId);

            //cancelar llamado de calle
            if (!comando.Llamada)
                calle.CalleCalado = null;
        }

        protected override void Validar(ModificarCalle comando, Resultado resultado)
        {
            var caracteristicaCalidad = Repositorio.Obtener<CaracteristicaDeCalidad>(comando.Dto.CaracteristicaDeCalidadId);

            if (comando.Dto.TipoCalle == TipoCalle.PostCalado && comando.Dto.TipoCalidad == TipoCalidad.Otros)
            {
                if (comando.Dto.MaterialId == 0)
                {
                    resultado.Error("MaterialDesc", string.Format(Dominio.Recursos.Textos.Error_Requerido, new[] { "Material" }));
                }
                if (caracteristicaCalidad == null)
                {
                    resultado.Error("CaracteristicaDeCalidadDesc", string.Format(Dominio.Recursos.Textos.Error_Requerido, new[] { "Caracteristicas de Calidad" }));
                }
                else
                {
                    var caracteristicaCalidadExiste = caracteristicaCalidad != null;
                    var tieneRangoMaximo = comando.Dto.RangoCaracteristicaCalidadMaximo != null;
                    var tieneRangoMinimo = comando.Dto.RangoCaracteristicaCalidadMinimo != null;

                    if (caracteristicaCalidadExiste)
                    {
                        if (!tieneRangoMaximo)
                        {
                            resultado.Error("RangoCaracteristicaCalidadMaximo", string.Format(Dominio.Recursos.Textos.Error_Requerido, new[] { "Rango Máximo" }));
                        }
                        if (!tieneRangoMinimo)
                        {
                            resultado.Error("RangoCaracteristicaCalidadMinimo", string.Format(Dominio.Recursos.Textos.Error_Requerido, new[] { "Rango Minimo" }));
                        }
                        var estaRangoMaximoFueraDeValoresPermitidos = comando.Dto.RangoCaracteristicaCalidadMaximo < caracteristicaCalidad.CaladoMinimo || comando.Dto.RangoCaracteristicaCalidadMaximo > caracteristicaCalidad.CaladoMaximo;
                        var estaRangoMinimoFueraDeValoresPermitidos = comando.Dto.RangoCaracteristicaCalidadMinimo < caracteristicaCalidad.CaladoMinimo || comando.Dto.RangoCaracteristicaCalidadMinimo > caracteristicaCalidad.CaladoMaximo;
                        var esRangoMaximoMenorQueRangoMinimo = comando.Dto.RangoCaracteristicaCalidadMaximo < comando.Dto.RangoCaracteristicaCalidadMinimo;
                        if (estaRangoMaximoFueraDeValoresPermitidos)
                        {
                            resultado.Error("RangoCaracteristicaCalidadMaximo", string.Format(Dominio.Recursos.Textos.Error_RangoCaracteristicasCalidad, caracteristicaCalidad.CaladoMinimo, caracteristicaCalidad.CaladoMaximo));
                        }
                        if (estaRangoMinimoFueraDeValoresPermitidos)
                        {
                            resultado.Error("RangoCaracteristicaCalidadMinimo", string.Format(Dominio.Recursos.Textos.Error_RangoCaracteristicasCalidad, caracteristicaCalidad.CaladoMinimo, caracteristicaCalidad.CaladoMaximo));
                        }
                        if (esRangoMaximoMenorQueRangoMinimo)
                        {
                            resultado.Error("RangoCaracteristicaCalidadMaximo", Dominio.Recursos.Textos.Error_RangoCaracteristicasCalidad_Minimo);
                        }
                    }
                }
            }

            if (comando.Dto.TipoCalle == TipoCalle.PreBalanzaGranos && comando.Dto.EsPasoDirecto)
            {
                if(Repositorio.Existe<Calle>(x => x.EsPasoDirecto && x.Id != comando.Dto.Id))
                    resultado.Error("EsPasoDirecto", "Ya existe una fila Prebalanza de Paso Directo");

                if(Repositorio.Existe<Calle>(x => x.Id == comando.Dto.Id && x.Bloqueada && x.FechaLLamada != null))
                    resultado.Error("EsPasoDirecto", $"La {comando.Dto.Nombre} está siendo llamada actualmente. Por favor libérela para poder continuar");
            }

        }

        private void LlamarCallePrebalanzaPrioritario(ModificarCalle comando)
        {
            comando.Dto.FechaLLamada = DateTime.Now;
            comando.Dto.Bloqueada = true;
            var resultadoInsertarCallePrioritarioCartelLed = servicioComandos.Ejecutar(new InsertarSlotMensajeCartelLed()
            {
                Codigo = CodigoMensajeCartelLed.CartelPreBalanza,
                CalleId = comando.Dto.Id,
                EsPrioritarioPrebalanza = true,
            }) as ResultadoMensajeCartelLed;
            EnviarFilasReordenadasPreBalanzaCartelLed(resultadoInsertarCallePrioritarioCartelLed);
        }

        private void EnviarFilasReordenadasPreBalanzaCartelLed(ResultadoMensajeCartelLed resultadoInsertarCallePrioritarioCartelLed)
        {
            if (!resultadoInsertarCallePrioritarioCartelLed.HayErrores && resultadoInsertarCallePrioritarioCartelLed.ListaDeMensajes.Any())
            {
                var cartel = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.EstadoPlayaInterna && x.Nombre == Constantes.ConfiguracionGeneral.PreBalanza.CartelLedPreBalanza);
                foreach (var mensajeCartelLed in resultadoInsertarCallePrioritarioCartelLed.ListaDeMensajes)
                {
                    servicioComandos.Ejecutar(new EnviarMensajeCartelLed
                    {
                        Mensaje = mensajeCartelLed.HistorialMensajeCartelLed?.Mensaje ?? "-",
                        Codigo = cartel?.Valor,
                        NumeroTrama = mensajeCartelLed.Trama,
                        NumeroPrograma = mensajeCartelLed.Programa,
                        NumeroVariable = mensajeCartelLed.Variable,
                    });
                }
            }
        }

        private void LiberarCallePrebalanzaPrioritario(ModificarCalle comando)
        {
            comando.Dto.FechaLLamada = null;
            comando.Dto.Bloqueada = false;
            var resultadoLimpiarCallePrioritarioCartelLed = servicioComandos.Ejecutar(new LimpiarHistorialMensajeCartelLed()
            {
                Codigo = CodigoMensajeCartelLed.CartelPreBalanza,
                CalleId = comando.Dto.Id
            }) as ResultadoMensajeCartelLedReordenado;

            var resultadoMensajeCartelLed = new ResultadoMensajeCartelLed
            {
                ListaDeMensajes = resultadoLimpiarCallePrioritarioCartelLed.ListaDeMensajes,
            };

            EnviarFilasReordenadasPreBalanzaCartelLed(resultadoMensajeCartelLed);
        }
    }
}