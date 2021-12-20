using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios.ServicioImpresion;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public abstract class ProcesadorImprimirLibroMovimientosExistenciaGranos : ProcesadorComando<ImprimirLibroMovimientosExistenciaGranos>
    {
        private readonly IFirmaProvider firmaProvider;

        protected IServicioImpresion ServicioImpresion { get; }

        public ProcesadorImprimirLibroMovimientosExistenciaGranos(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresion servicioImpresion)
            : base(repositorio, conversor, log)
        {
            this.firmaProvider = firmaProvider;
            ServicioImpresion = servicioImpresion;
        }

        public override Resultado Ejecutar(ImprimirLibroMovimientosExistenciaGranos comando)
        {
            var resultado = new ResultadoPrevisualizar();
            var impresion = Repositorio.Obtener<DocumentoDeImpresionPorCentro>(x => x.DocumentoDeImpresion.Codigo == comando.CodigoDeImpresion && x.Centro.Id == comando.CentroId);

            if (impresion == null || impresion.FormatoDeImpresion == null || impresion.FormatoDeImpresion.FormatosDeCampo.Count == 0)
            {
                Log.Error("Error al imprimir en la impresora: codigo de impresion no encontrado");
                resultado.Errores.Add("", String.Format(Textos.Error_CodigoDeImpresion, comando.CodigoDeImpresion));
                return resultado;
            }

            var formatoDeImpresionDto = Conversor.Convertir<FormatoDeImpresion, FormatoDeImpresionDto>(impresion.FormatoDeImpresion);
            Log.Info("Iniciando impresión de LibroMovimientosExistenciaGranos en la impresora: {0}", impresion.Impresora.Descripcion);

            // crear los dtos, manejando el tema de las hojas a imprimir
            IList<int> materiales;
            if (comando.Dto.MaterialId != 0)
            {
                materiales = new List<int>{comando.Dto.MaterialId};
            }
            else
            {
                materiales = Repositorio.Listar<MaterialPorCentro, int>(x => x.Material.Id,
                                                                        x =>
                                                                        x.Material.Activo &&
                                                                        x.Material.DescripcionCorta ==
                                                                        comando.Dto.MaterialDescripcionCorta &&
                                                                        x.Centro.Id == comando.CentroId).ToList();
            }

            var stock = materiales.Sum(material => Repositorio.ObtenerConsultaEscalar(new ObtenerStockInicialConsulta(comando.Dto.FechaDesde, material, comando.CentroId)) ?? 0);
            Log.Info("Stock inicial: {0}", stock);

            var dtosOrdenados = Repositorio.ListarConsulta(new ListarLibroMovimientosExistenciaDeGranos(comando.CentroId,
                                                                                                        materiales,
                                                                                                        stock,
                                                                                                        comando.Dto.FechaDesde,
                                                                                                        comando.Dto.FechaHasta,
                                                                                                        firmaProvider
                                                                                                            .ObtenerFirmaSinLogo
                                                                                                            ().CodigoSAP));

            Log.Info("Registros del libro: {0}", dtosOrdenados.Count);
            try
            {
                var unCampoColumna = formatoDeImpresionDto.FormatosDeCampo.FirstOrDefault(x => x.EsColumna);

                if(dtosOrdenados.Count > 0 && unCampoColumna != null)
                {
                    Log.Info("Calculando totales");
                    CalcularTotales(dtosOrdenados);
                    Log.Info("Generando Archivo");
                    GenerarArchivo(resultado, dtosOrdenados, formatoDeImpresionDto, impresion.Impresora.Direccion);
                }
                else if (dtosOrdenados.Count == 0)
                {
                    resultado.Errores.Add("",Textos.Error_NoResultados);
                }
                else if (unCampoColumna == null)
                {
                    resultado.Errores.Add("", Textos.Error_Generico);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: {0}", impresion.Impresora.Descripcion);
                resultado.Errores.Add("", String.Format(Textos.ImpresoraNoConecta, impresion.Impresora.Descripcion));
            }
            return resultado;
        }

        private static void CalcularTotales(ICollection<ImpImpresionGenericaDto> dtos)
        {
            //Calculo totales por mes
            dtos.GroupBy(g => new { g.FechaEmision.Year, g.FechaEmision.Month })
           .ToList()
           .ForEach(f => dtos.Add(new ImpImpresionGenericaDto
           {
               Id = 2,
               FechaEmision = (new DateTime(f.Key.Year, f.Key.Month, DateTime.DaysInMonth(f.Key.Year, f.Key.Month)).FinDelDia()),
               PesoNetoIngreso = Formatted(f.Where(w => w.TipoDeWorkflow == TipoDeWorkflow.Ingreso).Sum(s => Decimal.ToInt32(s.PesoNetoNumerico))),
               PesoNetoEgreso = Formatted(f.Where(w => w.TipoDeWorkflow == TipoDeWorkflow.Egreso).Sum(s => Decimal.ToInt32(s.PesoNetoNumerico))),
               PesoNetoSinHumedad = Formatted(f.Where(w => w.TipoDeWorkflow == TipoDeWorkflow.Ingreso).Sum(s => Decimal.ToInt32(s.PesoNetoSinhumedadNumerico))),
               SaldosSTOCK = f.LastOrDefault() != null ? f.Last().SaldosSTOCK : "",
               EsTotalMes = true
           }));

            //Calculo totales por día
            dtos.Where(w => !w.EsTotalMes).GroupBy(g => g.FechaEmision.FinDelDia())
            .ToList()
            .ForEach(f => dtos.Add(new ImpImpresionGenericaDto
            {
                Id = 1,
                FechaEmision = f.Key,
                PesoNetoIngreso = Formatted(f.Where(w => !w.EsTotalMes && w.TipoDeWorkflow == TipoDeWorkflow.Ingreso).Sum(s => Decimal.ToInt32(s.PesoNetoNumerico))),
                PesoNetoEgreso = Formatted(f.Where(w => !w.EsTotalMes && w.TipoDeWorkflow == TipoDeWorkflow.Egreso).Sum(s => Decimal.ToInt32(s.PesoNetoNumerico))),
                PesoNetoSinHumedad = Formatted(f.Where(w => !w.EsTotalMes && w.TipoDeWorkflow == TipoDeWorkflow.Ingreso).Sum(s => Decimal.ToInt32(s.PesoNetoSinhumedadNumerico))),
                SaldosSTOCK = f.LastOrDefault() != null ? f.Last().SaldosSTOCK : "",
                EsTotalDia = true
            }));
        }

        private static string Formatted(decimal? number)
        {
            return number == null || number == 0 ? "" : String.Format(CultureInfo.InvariantCulture, "{0:0,0}", Decimal.ToInt32(number.Value)).Replace(',','.');
        }

        protected abstract void GenerarArchivo(ResultadoPrevisualizar resultado, List<ImpImpresionGenericaDto> dtos, FormatoDeImpresionDto formatoDeImpresion, string direccionImpresora);
    }
}
