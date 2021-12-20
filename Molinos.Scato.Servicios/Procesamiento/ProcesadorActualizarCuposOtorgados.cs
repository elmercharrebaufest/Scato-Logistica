using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarCuposOtorgados : ProcesadorComando<ActualizarCuposOtorgados>
    {
        private readonly ZSDWS_SCATO servicioSap;
        
        private Dictionary<int, string> Especiales = new Dictionary<int, string>
        {
            { 4, " Sust." },
            { 13, " Esp." }
        };

        public ProcesadorActualizarCuposOtorgados(IRepositorio repositorio, IConversor conversor, ILogger log, IConfiguracionProvider configuracion, ZSDWS_SCATO servicioSap)
            : base(repositorio, conversor, log)
        {
            this.servicioSap = servicioSap;
        }

        public override Resultado Ejecutar(ActualizarCuposOtorgados comando)
        {
            var resultado = new Resultado();
            var cuposOtorgados = ObtenerCuposOtorgadosSAP(comando, resultado);
            var estados = GenerarEstadoCupos(cuposOtorgados);

            var estadoViejo = Repositorio.Listar<CupoMobile>();
            foreach(var estado in estados)
            {
                Repositorio.Agregar(estado);
            }
            Repositorio.RemoverTodos(estadoViejo);
            Repositorio.GuardarCambios();
            return resultado;
        }

        private List<CuposOtorgadosDto> ObtenerCuposOtorgadosSAP(ActualizarCuposOtorgados comando, Resultado result){
            var resultado = new List<CuposOtorgadosDto>();
            try
            {
                var fecha = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                var response = servicioSap.Z_SDMF_RFC_Z2100T(new Z_SDMF_RFC_Z2100TRequest
                {
                    Z_SDMF_RFC_Z2100T = new Z_SDMF_RFC_Z2100T()
                    {
                        IM_CENTRO = comando.CentrosCodigoSap.Where(x => !string.IsNullOrEmpty(x)).Select(x => new ZMPES5210 { CENTRO = x }).ToArray(),
                        IM_FECHA = new ZMPES5230[] { new ZMPES5230 { FECHA = fecha } }
                    }
                });

                if (response != null && response.Z_SDMF_RFC_Z2100TResponse != null && response.Z_SDMF_RFC_Z2100TResponse.EX_TOTAL_CUPOS.Any())
                {
                    foreach (var cuposPorMaterial in response.Z_SDMF_RFC_Z2100TResponse.EX_TOTAL_CUPOS)
                    {
                        Log.Debug("ActualizarCuposOtorgados Material {0} Centro{1}", cuposPorMaterial.MATERIAL, cuposPorMaterial.CENTRO);
                        var codigoSap = cuposPorMaterial.MATERIAL.TrimStart(new[] { '0' });
                        var material = Repositorio.Listar<Material, int>(x => x.Id, x => x.CodigoSAP == codigoSap).FirstOrDefault();
                        var centro = Repositorio.Listar<Centro, CentroDto>(x => new CentroDto { Id = x.Id , CodigoSAP = x.CodigoSAP} ,
                                        x => x.CodigoSAP == cuposPorMaterial.CENTRO || 
                                        x.CodigoSAPEspecial.Contains(cuposPorMaterial.CENTRO)).FirstOrDefault();

                        if (centro == null)
                        {
                            Log.Error("ActualizarCuposOtorgados Centro no encontrado Centro{0}", cuposPorMaterial.CENTRO);
                            continue;
                        }
                        var esEspecial = centro.CodigoSAP != cuposPorMaterial.CENTRO;

                        resultado.Add(new CuposOtorgadosDto
                        {
                            CentroId = centro.Id,
                            Cupos = int.Parse(cuposPorMaterial.CUPOS),
                            Fecha = DateTime.Now,
                            MaterialId = material,
                            Especial = esEspecial
                        });

                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "ActualizarCuposOtorgados: ");
                result.Errores.Add("", Textos.Error_GenericoSap);
            }
            return resultado;
        }
        
        private List<CupoMobile> GenerarEstadoCupos(List<CuposOtorgadosDto> cuposOtorgados)
        {
            var resultado = new List<CupoMobile>();
            var date = DateTime.Now.Date;
            var materiales = Repositorio.Listar<MaterialPorCentro, MaterialIdYDescripcionDto>(
                x => new MaterialIdYDescripcionDto { MaterialId = x.Material.Id, Descripcion = x.Material.Descripcion, DescripcionWebMobile = x.DescripcionWebMobile, CentroId = x.Centro.Id },
                x => x.MostrarEnWebMobile
                );
            var materialesEspeciales = materiales.Where(x => Especiales.ContainsKey(x.MaterialId)).ToList();
            var cuposIngresadosEnElDiaOActivos = Repositorio.Listar<CargaDeCupo>(x => (x.Recorrido == null || !x.Recorrido.Rechazado) && (x.Fecha >= date || !x.Recorrido.Terminado || (x.Recorrido.Terminado && x.Recorrido.PesoTaraFecha > date)));

            foreach (var cuposOtorgadosPorCentro in cuposOtorgados.GroupBy(x => x.CentroId))
            {
                GenerarEstadoCuposPorCentro(cuposOtorgadosPorCentro, resultado, materiales, materialesEspeciales, cuposIngresadosEnElDiaOActivos);
            }
            return resultado.OrderBy(x => x.Orden).ThenBy(x => x.Material).ToList();
        }

        private void GenerarEstadoCuposPorCentro(IGrouping<int,CuposOtorgadosDto> cuposOtorgadosPorCentro, List<CupoMobile> resultado, IList<MaterialIdYDescripcionDto> materiales, List<MaterialIdYDescripcionDto> materialesEspeciales, IList<CargaDeCupo> cuposIngresadosEnElDiaOActivos)
        {
            var centroId = cuposOtorgadosPorCentro.Key;
            var materialesPorCentro = materiales.Where(x => x.CentroId == centroId);
            var materialesEspecialesPorCentro = materialesEspeciales.Where(x => x.CentroId == centroId);

            var cuposOtorgadosNoEspeciales = cuposOtorgadosPorCentro.Where(x => !x.Especial);
            var cuposOtorgadosEspeciales = cuposOtorgadosPorCentro.Where(x => x.Especial);

            var cuposIngresadosEnElDiaOActivosPorCentro = cuposIngresadosEnElDiaOActivos.Where(x => x.Centro.Id == centroId);
            var cuposIngresadosEnElDiaOActivosNoEspeciales = cuposIngresadosEnElDiaOActivosPorCentro.Where(x => !x.Especial);
            var cuposIngresadosEnElDiaOActivosEspeciales = cuposIngresadosEnElDiaOActivosPorCentro.Where(x => x.Especial);

            var cuposAInsertar = new List<CupoMobile>();
            Cargarcupos(centroId, cuposAInsertar, cuposIngresadosEnElDiaOActivosNoEspeciales, cuposOtorgadosNoEspeciales, materialesPorCentro, false);
            Cargarcupos(centroId, cuposAInsertar, cuposIngresadosEnElDiaOActivosEspeciales, cuposOtorgadosEspeciales, materialesEspecialesPorCentro, true);
            resultado.AddRange(cuposAInsertar);
        }

        private void Cargarcupos(int centroId, List<CupoMobile> resultado, IEnumerable<CargaDeCupo> cuposIngresadosEnElDia, IEnumerable<CuposOtorgadosDto> cuposOtorgados, IEnumerable<MaterialIdYDescripcionDto> materiales, bool especiales)
        {
            CuposOtorgadosDto select(IGrouping<Material, CargaDeCupo> x) => new CuposOtorgadosDto { MaterialId = x.Key == null ? 0 : x.Key.Id, Cupos = x.Count() };

            var dateMin = DateTime.Now.Date;
            var dateMax = DateTime.Now.Date.AddDays(1);
            var cuposArribados = cuposIngresadosEnElDia.Where(x => !x.SinCupo && x.Material != null);
            var cuposArribadosVencidos = cuposArribados.Where(x => x.FechaSap < dateMin).GroupBy(x => x.Material).Select(select);
            var cuposArribadosFuturos = cuposArribados.Where(x => dateMax <= x.FechaSap).GroupBy(x => x.Material).Select(select);
            var cuposArribadosDelDia = cuposArribados.Where(x => dateMin <= x.FechaSap && x.FechaSap < dateMax).GroupBy(x => x.Material).Select(select);

            var cuposDescargadosDelDia = cuposIngresadosEnElDia.Where(x => !x.SinCupo && dateMin <= x.FechaSap && x.FechaSap < dateMax && x.Material != null && x.Recorrido != null && x.Recorrido.PesoTaraFecha.HasValue && !x.Recorrido.Rechazado && x.Recorrido.Terminado).GroupBy(x => x.Material).Select(select);
            var cuposDescargadosTodos = cuposIngresadosEnElDia.Where(x => x.Material != null && x.Recorrido != null && x.Recorrido.PesoTaraFecha.HasValue && !x.Recorrido.Rechazado && x.Recorrido.Terminado).GroupBy(x => x.Material).Select(select);
            var cuposPendientes = cuposIngresadosEnElDia.Where(x => x.EstuvoPendiente && x.Recorrido == null).GroupBy(x => x.Material).Select(select);
            var cuposSinRecorrido = cuposIngresadosEnElDia.Where(x => x.Recorrido == null).GroupBy(x => x.Material).Select(select);
            var sinCupo = cuposIngresadosEnElDia.Where(x => x.SinCupo).GroupBy(x => x.Material).Select(select);

            foreach (var material in materiales)
            {
                var descripcionMaterial = !string.IsNullOrEmpty(material.DescripcionWebMobile) ? material.DescripcionWebMobile : material.Descripcion;
                descripcionMaterial += especiales ? Especiales[material.MaterialId] : string.Empty;

                InsertarCupoMobile(resultado,
                    cuposArribadosDelDia.Where(x => x.MaterialId == material.MaterialId).Sum(x => x.Cupos),
                    cuposArribadosVencidos.Where(x => x.MaterialId == material.MaterialId).Sum(x => x.Cupos),
                    cuposArribadosFuturos.Where(x => x.MaterialId == material.MaterialId).Sum(x => x.Cupos),
                    cuposDescargadosDelDia.Where(x => x.MaterialId == material.MaterialId).Sum(x => x.Cupos),
                    cuposPendientes.Where(x => x.MaterialId == material.MaterialId).Sum(x => x.Cupos),
                    cuposSinRecorrido.Where(x => x.MaterialId == material.MaterialId).Sum(x => x.Cupos),
                    cuposDescargadosTodos.Where(x => x.MaterialId == material.MaterialId).Sum(x => x.Cupos),
                    sinCupo.Where(x => x.MaterialId == material.MaterialId).Sum(x => x.Cupos),
                    descripcionMaterial,
                    cuposOtorgados.Where(x => x.MaterialId == material.MaterialId).Sum(x => x.Cupos),
                    cuposOtorgados.Any() ? cuposOtorgados.Max(x => x.Fecha) : DateTime.MinValue,
                    1,
                    centroId
                );
            }

            var materialesQueNoSonOtros = materiales.Select(x => x.MaterialId).ToList();
            InsertarCupoMobile(resultado,
                cuposArribadosDelDia.Where(x => !materialesQueNoSonOtros.Contains(x.MaterialId)).Sum(x => x.Cupos),
                cuposArribadosVencidos.Where(x => !materialesQueNoSonOtros.Contains(x.MaterialId)).Sum(x => x.Cupos),
                cuposArribadosFuturos.Where(x => !materialesQueNoSonOtros.Contains(x.MaterialId)).Sum(x => x.Cupos),
                cuposDescargadosDelDia.Where(x => !materialesQueNoSonOtros.Contains(x.MaterialId)).Sum(x => x.Cupos),
                cuposPendientes.Where(x => !materialesQueNoSonOtros.Contains(x.MaterialId)).Sum(x => x.Cupos),
                cuposSinRecorrido.Where(x => !materialesQueNoSonOtros.Contains(x.MaterialId)).Sum(x => x.Cupos),
                cuposDescargadosTodos.Where(x => !materialesQueNoSonOtros.Contains(x.MaterialId)).Sum(x => x.Cupos),
                sinCupo.Where(x => !materialesQueNoSonOtros.Contains(x.MaterialId)).Sum(x => x.Cupos),
                "Otros",
                cuposOtorgados.Where(x => !materialesQueNoSonOtros.Contains(x.MaterialId)).Sum(x => x.Cupos),
                cuposOtorgados.Any() ? cuposOtorgados.Max(x => x.Fecha) : DateTime.MinValue,
                99,
                centroId
            );

        }

        private void InsertarCupoMobile(List<CupoMobile> resultado,
            int cuposArribadosDelDia,
            int cuposArribadosVencidos,
            int cuposArribadosFuturos,
            int cuposDescargadosDelDia,
            int cuposPendientes,
            int cuposSinRecorrido,
            int cuposdescargados,
            int sinCupo,
            string material,
            int otorgados,
            DateTime fechaActualizacion,
            int orden,
            int centroId
            )
        {
            var existente = resultado.FirstOrDefault(x => x.Material == material);
            if (existente != null)
            {
                existente.ArribadosDia += cuposArribadosDelDia;
                existente.ArribadosVencidos += cuposArribadosVencidos;
                existente.ArribadosFuturos += cuposArribadosFuturos;

                existente.Descargados += cuposDescargadosDelDia;
                existente.DescargadosTodos += cuposdescargados;
                existente.Pendiente += cuposPendientes;
                existente.SinRecorrido += cuposSinRecorrido;
                existente.Excedente += cuposArribadosDelDia + cuposArribadosVencidos + cuposArribadosFuturos + cuposPendientes + sinCupo - cuposdescargados;
                existente.SinCupo += sinCupo;
                existente.Otorgados += otorgados;
            }
            else
            {
                resultado.Add(new CupoMobile
                {
                    ArribadosDia = cuposArribadosDelDia,
                    ArribadosVencidos = cuposArribadosVencidos,
                    ArribadosFuturos = cuposArribadosFuturos,

                    Descargados = cuposDescargadosDelDia,
                    DescargadosTodos = cuposdescargados,
                    Pendiente = cuposPendientes,
                    SinRecorrido = cuposSinRecorrido,
                    Excedente = cuposArribadosDelDia + cuposArribadosVencidos + cuposArribadosFuturos + cuposPendientes + sinCupo - cuposdescargados,
                    SinCupo = sinCupo,
                    Material = material,
                    Otorgados = otorgados,
                    FechaActualizacion = fechaActualizacion,
                    Orden = orden,
                    CentroId = centroId
                });
            }
        }

    }
}