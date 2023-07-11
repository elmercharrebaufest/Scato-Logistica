using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCTGWebService;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Impl
{
    public class CalculadoraDescuento : ICalculadoraDescuento
    {
        private readonly IRepositorio repositorio;

        public CalculadoraDescuento(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public decimal CalcularPorcentajeMuestraAuditoria(IList<AnalisisPorCaracteristica> analisis, Guid instanceId)
        {
            decimal retorno = 0;
            foreach(var c in analisis)
            {
                var valorMedicion = c.ValorAnalisis ?? c.ValorCalado ?? 0;
                if (EnviaACamara(c.CaracteristicaDeCalidad, valorMedicion, instanceId))
                {
                    retorno = 0;
                    break;
                }
                var descuentosOrdenados = c.CaracteristicaDeCalidad.Descuentos.OrderBy(x => x.ValorHasta);
                
                foreach (var d in descuentosOrdenados)
                {
                    if (valorMedicion <= d.ValorHasta)
                    {
                        retorno += d.PorcentajeEnvioCamaraAuditoria;
                        break;
                    }
                }
            }
            return retorno;
        }

        private bool TieneAgenteDecompras(Guid instanceId)
        {
            return repositorio.Existe<Recorrido>(x => x.InstanciaWorkflow == instanceId && x.Vehiculo.CartaPorte.AgenteCompras != null);
        }

        public bool EnviaACamara(CaladoPorCaracteristica obj, Guid instanceId)
        {
            return EnviaACamara(obj.CaracteristicaDeCalidad, obj.ValorCalado ?? 0, instanceId);
        }

        public bool EnviaACamara(AnalisisPorCaracteristica obj, Guid instanceId)
        {
            return EnviaACamara(obj.CaracteristicaDeCalidad, obj.ValorAnalisis ?? obj.ValorCalado ?? 0, instanceId);
        }


        private bool EnviaACamara(CaracteristicaDeCalidad caracteristica, decimal valorMedicion, Guid instanceId)
        {
            if (caracteristica.SituacionEnvioACamara == EnvioACamara.Siempre || ExisteExcepcionAlDescuento(instanceId, caracteristica.Id)) //No corresponde calcular descuento
            {
                return true;
            }

            if (caracteristica.SituacionEnvioACamara == EnvioACamara.SiempreSiSuperaValorCamara) //No corresponde calcular descuento
            {
                if(caracteristica.SiSuperaValorCamara <= valorMedicion)
                {
                    return true;
                }
                var descuentosOrdenados = caracteristica.Descuentos.OrderBy(x => x.ValorHasta);
                var descuento = descuentosOrdenados.FirstOrDefault(x => x.ValorHasta >= valorMedicion);
                if ((descuento != null ? descuento.MercadoATermino : false) && TieneAgenteDecompras(instanceId))
                {
                    return true;
                }
            }
            return false;
        }

        public decimal CalcularDescuentoEnPorcentaje(CaracteristicaDeCalidad caracteristica, decimal valorMedicion, Guid instanceId)
        {
            if(EnviaACamara(caracteristica, valorMedicion, instanceId))//No corresponde calcular descuento
            {
                return 0;
            }
            if (valorMedicion < caracteristica.CaladoMinimo)
            {
                throw new Exception("Valor fuera de escala");
            }
            if (valorMedicion == caracteristica.CaladoMinimo)
            {
                return 0;
            }
            if (valorMedicion > caracteristica.CaladoMaximo)
            {
                valorMedicion = caracteristica.CaladoMaximo;
            }
            var descuentosOrdenados = caracteristica.Descuentos.OrderBy(x => x.ValorHasta);

            switch (caracteristica.DescuentoEnPorcentaje)
            {
                case FormulaDescuento.DeTabla:
                    var descuento = descuentosOrdenados.FirstOrDefault(x => x.ValorHasta >= valorMedicion);
                    return descuento == null ? 0 : descuento.PorcentajeDescuento;
                
                case FormulaDescuento.Acumulado:
                    var minimo = caracteristica.CaladoMinimo;
                    decimal descuentoResultado = 0;
                    foreach (var d in descuentosOrdenados)
                    {
                        if (valorMedicion > d.ValorHasta)
                        {
                            var diferencial = d.ValorHasta - minimo;
                            descuentoResultado += diferencial * d.PorcentajeDescuento;
                            minimo = d.ValorHasta;
                            continue;
                        }
                        if (valorMedicion <= d.ValorHasta)
                        {
                            var diferencial = valorMedicion - minimo;
                            descuentoResultado += diferencial * d.PorcentajeDescuento;
                        }
                        break;
                    }
                    return descuentoResultado;
                
                case FormulaDescuento.PuntoYFraccion:
                    minimo = caracteristica.CaladoMinimo;
                    var descuentoAnterior = descuentosOrdenados.TakeWhile(d => d.ValorHasta < valorMedicion).LastOrDefault();
                    if (descuentoAnterior != null)
                    {
                        minimo = descuentoAnterior.ValorHasta;
                    }
                    var descuentoSeleccionado = descuentosOrdenados.FirstOrDefault(x => x.ValorHasta >= valorMedicion);

                    return descuentoSeleccionado != null ? descuentoSeleccionado.PorcentajeDescuento * (valorMedicion - minimo) : 0;
                
                case FormulaDescuento.SinDescuento:
                    return 0;
            }
            throw new Exception("Formula no definida en la calculadora de descuentos");
        }

        public decimal CalcularDescuentoEnKg(CaracteristicaDeCalidad caracteristica, decimal valorMedicion, int pesoNetoOrigen, Guid instanceId)
        {
            var descuentoPorcentaje = CalcularDescuentoEnPorcentaje(caracteristica, valorMedicion,instanceId);
            return (descuentoPorcentaje*pesoNetoOrigen)/100;
        }

        public bool ExisteExcepcionAlDescuento(Guid instanceId, int caracteristicaDeCalidadId)
        {
            var corredorId = repositorio.ObtenerProyeccion<Recorrido, int?>(x => x.InstanciaWorkflow == instanceId && x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.Vehiculo.CartaPorte.Corredor != null,x => x.Vehiculo.CartaPorte.Corredor.Id);
            if (corredorId.HasValue)                
            {
                var fecha = DateTime.Now;
                return repositorio.Existe<ExcepcionAlDescuento>(x => x.Proveedor.Id == corredorId.Value
                                                                     && x.CaracteristicaDeCalidad.Id == caracteristicaDeCalidadId
                                                                     && (fecha >= x.FechaDesde && fecha <= x.FechaHasta));
            }
            return false;
        }

        public decimal TotalKilosDescuento(CaladoDto calado, AnalisisDeCalidadDto analisis, int pesoNeto)
        {
            var descuentosSinMerma = TotalKilosDescuentosSinMermaDto(calado, analisis, pesoNeto);
            var descuentosPorMerma = TotalKilosDescuentoPorMermaDto(calado, analisis, pesoNeto, descuentosSinMerma);
            return descuentosSinMerma + descuentosPorMerma;
        }

        public decimal ActualizarMermaVolatil(Calado calado, AnalisisDeCalidad analisis, int pesoNeto)
        {
            decimal resultado = 0;
            var descuentosEnKilosSinMerma = TotalKilosDescuentosSinMerma(calado, analisis, pesoNeto);
            if (calado != null)
            {
                var mermaEnCalado = calado.CaladosPorCaracteristica.FirstOrDefault(x => x.CaracteristicaDeCalidad.EsMermaVolatil);
                if (mermaEnCalado != null)
                {
                    resultado = (mermaEnCalado.DescuentoEnPorcentaje ?? 0) * (pesoNeto - descuentosEnKilosSinMerma) / 100;
                    mermaEnCalado.DescuentoEnKg = resultado;
                }
            }
            if (analisis != null)
            {
                var mermaEnAnalisis = analisis.CaracteristicasAnalizadas.FirstOrDefault(x => x.CaracteristicaDeCalidad.EsMermaVolatil);
                
                if (mermaEnAnalisis != null)
                {
                    resultado = (mermaEnAnalisis.DescuentoEnPorcentaje ?? 0) * (pesoNeto - descuentosEnKilosSinMerma) / 100;
                    mermaEnAnalisis.DescuentoEnKg = resultado;
                }
            }
            return resultado;
        }

        public void ActualizarEstado(Calado calado, AnalisisDeCalidad analisis, CaracteristicasAnalizadas estado, bool tieneEntregador = false)
        {
            if (calado != null)
            {
                estado.EsHumedad = calado.CaladosPorCaracteristica.Any(x => x.CaracteristicaDeCalidad.EsHumedad && x.DescuentoEnPorcentaje.HasValue && x.DescuentoEnPorcentaje > 0);
                estado.EsGranosVerdes = calado.CaladosPorCaracteristica.Any(x => x.CaracteristicaDeCalidad.EsGranosVerdes && x.DescuentoEnPorcentaje.HasValue && x.DescuentoEnPorcentaje > 0);
                estado.EsGranosDañados = calado.CaladosPorCaracteristica.Any(x => x.CaracteristicaDeCalidad.EsGranosDañados && x.DescuentoEnPorcentaje.HasValue && x.DescuentoEnPorcentaje > 0);
                estado.EsCuerposExtranos = calado.CaladosPorCaracteristica.Any(x => x.CaracteristicaDeCalidad.EsCuerposExtranos && x.DescuentoEnPorcentaje.HasValue && x.DescuentoEnPorcentaje > 0);
                estado.EsProteinaBaja = calado.CaladosPorCaracteristica.Any(x => x.CaracteristicaDeCalidad.EsProteina && x.ValorCalado.HasValue && x.ValorCalado < x.CaracteristicaDeCalidad.ValorProteinaMedia);
                estado.EsProteinaMedia = calado.CaladosPorCaracteristica.Any(x => x.CaracteristicaDeCalidad.EsProteina && x.ValorCalado.HasValue && x.ValorCalado >= x.CaracteristicaDeCalidad.ValorProteinaMedia && x.ValorCalado < x.CaracteristicaDeCalidad.ValorProteina);
                estado.EsProteinaAlta = calado.CaladosPorCaracteristica.Any(x => x.CaracteristicaDeCalidad.EsProteina && x.ValorCalado.HasValue && x.ValorCalado >= x.CaracteristicaDeCalidad.ValorProteina);
                estado.TieneDescuentos = calado.CaladosPorCaracteristica.Any(x => x.DescuentoEnPorcentaje.HasValue && x.DescuentoEnPorcentaje > 0);
                estado.TieneInsectosVivos = calado.CaladosPorCaracteristica.Any(x => x.CaracteristicaDeCalidad.EsInsectosVivos && x.ValorCalado.HasValue && x.ValorCalado > 0);
                estado.Humedad = calado.CaladosPorCaracteristica.Where(x => x.CaracteristicaDeCalidad.EsHumedad).Select(x => x.ValorCalado).FirstOrDefault();
                estado.Grado = calado.CaladosPorCaracteristica.Where(x => x.CaracteristicaDeCalidad.DescripcionCorta == "GRADO").Select(x => x.ValorCalado).FirstOrDefault();
                estado.CaracteristicasNoCorrenspodenEspecial = calado.CaladosPorCaracteristica.Any(x => x.CaracteristicaDeCalidad.ValorEspecialMaximo != null && x.CaracteristicaDeCalidad.ValorEspecialMinimo != null && (x.CaracteristicaDeCalidad.ValorEspecialMinimo > x.ValorCalado || x.CaracteristicaDeCalidad.ValorEspecialMaximo < x.ValorCalado));
                estado.Calidad = estado.EsHumedad ? TipoCalidad.Humedo : (calado.CaladosPorCaracteristica.Any(x => x.AnalisisPreliminar) || (tieneEntregador && calado.CaladosPorCaracteristica.Any(x => x.DescuentoEnPorcentaje.HasValue && x.DescuentoEnPorcentaje > 0)) ? TipoCalidad.Analisis : TipoCalidad.Conforme);
            }
            if (analisis != null)
            {
                var humedad = analisis.CaracteristicasAnalizadas.FirstOrDefault(x => x.CaracteristicaDeCalidad.EsHumedad && x.ValorAnalisis.HasValue);
                var granosVerdes = analisis.CaracteristicasAnalizadas.FirstOrDefault(x => x.CaracteristicaDeCalidad.EsGranosVerdes && x.ValorAnalisis.HasValue);
                var granosDañados = analisis.CaracteristicasAnalizadas.FirstOrDefault(x => x.CaracteristicaDeCalidad.EsGranosDañados && x.ValorAnalisis.HasValue);
                var cuerposExtraños = analisis.CaracteristicasAnalizadas.FirstOrDefault(x => x.CaracteristicaDeCalidad.EsCuerposExtranos && x.ValorAnalisis.HasValue);
                var proteina = analisis.CaracteristicasAnalizadas.FirstOrDefault(x => x.CaracteristicaDeCalidad.EsProteina && x.ValorAnalisis.HasValue);
                var insectos = analisis.CaracteristicasAnalizadas.FirstOrDefault(x => x.CaracteristicaDeCalidad.EsInsectosVivos && x.ValorAnalisis.HasValue);
                var grado = analisis.CaracteristicasAnalizadas.FirstOrDefault(x => x.CaracteristicaDeCalidad.DescripcionCorta == "GRADO" && x.ValorAnalisis.HasValue);

                estado.EsHumedad = humedad != null ? humedad.DescuentoEnPorcentaje.HasValue && humedad.DescuentoEnPorcentaje > 0 : estado.EsHumedad;
                estado.EsGranosVerdes = granosVerdes != null ? granosVerdes.DescuentoEnPorcentaje.HasValue && granosVerdes.DescuentoEnPorcentaje > 0 : estado.EsGranosVerdes;
                estado.EsGranosDañados = granosDañados != null ? granosDañados.DescuentoEnPorcentaje.HasValue && granosDañados.DescuentoEnPorcentaje > 0 : estado.EsGranosDañados;
                estado.EsCuerposExtranos = cuerposExtraños != null ? cuerposExtraños.DescuentoEnPorcentaje.HasValue && cuerposExtraños.DescuentoEnPorcentaje > 0 : estado.EsCuerposExtranos;
                estado.EsProteinaBaja = proteina != null ? proteina.ValorAnalisis < proteina.CaracteristicaDeCalidad.ValorProteinaMedia : estado.EsProteinaBaja;
                estado.EsProteinaMedia = proteina != null ? (proteina.ValorAnalisis >= proteina.CaracteristicaDeCalidad.ValorProteinaMedia && proteina.ValorAnalisis < proteina.CaracteristicaDeCalidad.ValorProteina) : estado.EsProteinaMedia;

                estado.EsProteinaAlta = proteina != null ? proteina.ValorAnalisis >= proteina.CaracteristicaDeCalidad.ValorProteina : estado.EsProteinaAlta;
                estado.TieneDescuentos = analisis.CaracteristicasAnalizadas.Any(x => x.DescuentoEnPorcentaje.HasValue && x.DescuentoEnPorcentaje > 0);
                estado.TieneInsectosVivos = insectos != null ? insectos.ValorAnalisis >= 0 : estado.TieneInsectosVivos;
                estado.Humedad = humedad != null ? humedad.ValorAnalisis : estado.Humedad;
                estado.Grado = grado != null ? grado.ValorAnalisis : estado.Grado;
                estado.CaracteristicasNoCorrenspodenEspecial = analisis.CaracteristicasAnalizadas.Any(x => x.CaracteristicaDeCalidad.ValorEspecialMaximo != null && x.CaracteristicaDeCalidad.ValorEspecialMinimo != null && x.ValorAnalisis.HasValue && (x.CaracteristicaDeCalidad.ValorEspecialMinimo > x.ValorAnalisis || x.CaracteristicaDeCalidad.ValorEspecialMaximo < x.ValorAnalisis));
                estado.Calidad = estado.EsHumedad ? TipoCalidad.Humedo : TipoCalidad.Analisis;
            }
        }

        public void ActualizarEstadoEspecial(Calado calado, AnalisisDeCalidad analisis, CaracteristicasAnalizadas estado)
        {
            if(calado != null && analisis != null)
            {
                estado.CaracteristicasNoCorrenspodenEspecial = 
                    calado.CaladosPorCaracteristica.Any(x => x.CaracteristicaDeCalidad.ValorEspecialMaximo != null && x.CaracteristicaDeCalidad.ValorEspecialMinimo != null && (x.CaracteristicaDeCalidad.ValorEspecialMinimo > x.ValorCalado || x.CaracteristicaDeCalidad.ValorEspecialMaximo < x.ValorCalado)) ||
                    analisis.CaracteristicasAnalizadas.Any(x => x.CaracteristicaDeCalidad.ValorEspecialMaximo != null && x.CaracteristicaDeCalidad.ValorEspecialMinimo != null && x.ValorAnalisis.HasValue && (x.CaracteristicaDeCalidad.ValorEspecialMinimo > x.ValorAnalisis || x.CaracteristicaDeCalidad.ValorEspecialMaximo < x.ValorAnalisis));
            }
            if (calado != null)
            {
                estado.CaracteristicasNoCorrenspodenEspecial = calado.CaladosPorCaracteristica.Any(x => x.CaracteristicaDeCalidad.ValorEspecialMaximo != null && x.CaracteristicaDeCalidad.ValorEspecialMinimo != null && (x.CaracteristicaDeCalidad.ValorEspecialMinimo > x.ValorCalado || x.CaracteristicaDeCalidad.ValorEspecialMaximo < x.ValorCalado));
            }
            if (analisis != null)
            {
                estado.CaracteristicasNoCorrenspodenEspecial = analisis.CaracteristicasAnalizadas.Any(x => x.CaracteristicaDeCalidad.ValorEspecialMaximo != null && x.CaracteristicaDeCalidad.ValorEspecialMinimo != null && x.ValorAnalisis.HasValue && (x.CaracteristicaDeCalidad.ValorEspecialMinimo > x.ValorAnalisis || x.CaracteristicaDeCalidad.ValorEspecialMaximo < x.ValorAnalisis));
            }
        }

        private decimal TotalKilosDescuentosSinMermaDto(CaladoDto calado, AnalisisDeCalidadDto analisis, int pesoNeto)
        {
            var descuentosSinMermaVolatil = new Dictionary<string, decimal>();
            if (calado != null)
            {
                descuentosSinMermaVolatil = calado.CaladosPorCaracteristica.Where(x => !x.EsMermaVolatil).ToDictionary(x => x.CaracteristicaCodigoSap, x => x.DescuentoEnPorcentaje);
            }

            if (analisis != null)
            {
                foreach (var valorAnalisis in analisis.CaracteristicasAnalizadas.Where(x => !x.EsMermaVolatil))
                {
                    descuentosSinMermaVolatil[valorAnalisis.CaracteristicaCodigoSap] = valorAnalisis.DescuentoEnPorcentaje;
                }
            }
            var descuentoEnKilosSinMerma = descuentosSinMermaVolatil.Sum(keyValue => keyValue.Value) * pesoNeto / 100;
            return descuentoEnKilosSinMerma;
        }

        private decimal TotalKilosDescuentoPorMermaDto(CaladoDto calado, AnalisisDeCalidadDto analisis, int pesoNeto, decimal descuentosEnKilosSinMerma)
        {
            var mermaEnCalado = calado.CaladosPorCaracteristica.FirstOrDefault(x => x.EsMermaVolatil);
            var descuentoEnPorcentajeMermaVolatil = mermaEnCalado != null ? mermaEnCalado.DescuentoEnPorcentaje : 0;

            if (analisis != null)
            {
                var mermaEnAnalisis = analisis.CaracteristicasAnalizadas.FirstOrDefault(x => x.EsMermaVolatil);
                if (mermaEnAnalisis != null)
                {
                    descuentoEnPorcentajeMermaVolatil = mermaEnAnalisis.DescuentoEnPorcentaje;
                }
            }
            return descuentoEnPorcentajeMermaVolatil * (pesoNeto - descuentosEnKilosSinMerma) / 100;
        }

        private decimal TotalKilosDescuentosSinMerma(Calado calado, AnalisisDeCalidad analisis, int pesoNeto)
        {
            var descuentosSinMermaVolatil = new Dictionary<string, decimal>();
            if (calado != null)
            {
                descuentosSinMermaVolatil = calado.CaladosPorCaracteristica.Where(x => !x.CaracteristicaDeCalidad.EsMermaVolatil && x.DescuentoEnPorcentaje.HasValue).ToDictionary(x => x.CaracteristicaDeCalidad.CodigoSAP, x => x.DescuentoEnPorcentaje.Value);
            }

            if (analisis != null)
            {

                foreach (var valorAnalisis in analisis.CaracteristicasAnalizadas.Where(x => !x.CaracteristicaDeCalidad.EsMermaVolatil))
                {
                    if (valorAnalisis.DescuentoEnPorcentaje.HasValue)
                    {
                        descuentosSinMermaVolatil[valorAnalisis.CaracteristicaDeCalidad.CodigoSAP] = valorAnalisis.DescuentoEnPorcentaje.Value;
                    }
                }
            }
            var descuentoEnKilosSinMerma = descuentosSinMermaVolatil.Sum(keyValue => keyValue.Value) * pesoNeto / 100;
            return descuentoEnKilosSinMerma;
        }


    }
}
