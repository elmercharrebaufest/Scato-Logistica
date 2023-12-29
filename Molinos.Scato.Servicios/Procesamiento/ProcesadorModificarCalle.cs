using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCalle : ProcesadorModificar<ModificarCalle>
    {
        public ProcesadorModificarCalle(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCalle comando)
        {
            var calle = Repositorio.Obtener<Calle>(comando.Dto.Id);
            var automatismoTipoLlamadoId = calle.AutomatismoTipoLlamadoId;

            Conversor.Convertir(comando.Dto, calle);
            calle.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            calle.CaracteristicaDeCalidad = Repositorio.Obtener<CaracteristicaDeCalidad>(comando.Dto.CaracteristicaDeCalidadId);
            if (comando.Dto.CalleCaladoId > 0)
                calle.CalleCalado = Repositorio.Obtener<Calle>(comando.Dto.CalleCaladoId);

            if (comando.Dto.TipoCalle == TipoCalle.PlayaInterna)
            {
                if (automatismoTipoLlamadoId == null)
                {
                    var automatismoTipoLlamado = Repositorio.Obtener<AutomatismoTipoLlamado>(q => q.Codigo == Constantes.AutomatismoTipoLlamado.PorFila);
                    calle.AutomatismoTipoLlamadoId = automatismoTipoLlamado.Id;
                }
                else
                {
                    calle.AutomatismoTipoLlamadoId = automatismoTipoLlamadoId;
                }
            }
            else
            {
                calle.AutomatismoTipoLlamadoId = null;
            }

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

            var ConfiguracionGranoActivo = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica && x.Nombre == Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos);
            var ConfiguracionNoGranoActivo = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.TableroComandoPuerto && x.Nombre == Constantes.ConfiguracionGeneral.LlamadoAutomatico.NoGranos);
            if ((ConfiguracionGranoActivo.Valor.Equals("True")
                && Repositorio.Existe<AutomatismoGrano>(a => a.Activo == true && (a.CallePreBalanzaId == comando.Dto.Id || a.CallePreHidraulicaId == comando.Dto.Id)))
                || (ConfiguracionNoGranoActivo.Valor.Equals("True")
                && Repositorio.Existe<AutomatismoNoGrano>(a => a.Activo == true && (a.CallePlanta.Id == comando.Dto.Id || a.CallePlayaInterna.Id == comando.Dto.Id))))
            {
                resultado.Error("Codigo", Textos.Automatismo_CalleUtilizadaEnAutomatismoActivo);
            }

            if (!comando.Dto.Deshabilitada && comando.Dto.TipoCalle == TipoCalle.PlantaNoGranos)
            {
                if (Repositorio.Existe<Calle>(x => x.Material.Id == comando.Dto.MaterialId && x.TipoCalle == TipoCalle.PlantaNoGranos && !x.Deshabilitada && comando.Dto.Id != x.Id))
                {
                    resultado.Error("MaterialDesc", Textos.Calle_PlantaNoGranos_Existente);
                }
            }

        }
    }
}