using Microsoft.Web.Administration;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios.Behavior;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Helpers;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Printing;
using System.ServiceModel.Configuration;
using WebConfigurationManager = System.Web.Configuration.WebConfigurationManager;

namespace Molinos.Scato.Servicios.Impl
{
    [AiErrorHandlerBehaviorAttribute]
    public class ServicioRepositorio : IServicioRepositorio
    {
        private readonly IRepositorio repositorio;
        private readonly IConversor conversor;
        private readonly ILogger log;
        private readonly IFirmaProvider firmaProvider;
        private readonly ICalculadoraDescuento calculadora;
        private readonly IConfiguracionProvider configuracion;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IAdministradorDeCalles administradorDeCalles;

        public ServicioRepositorio(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider,
            ICalculadoraDescuento calculadora, IConfiguracionProvider configuracion, IServicioOrquestador servicioOrquestador
            , IAdministradorDeCalles administradorDeCalles)
        {
            this.repositorio = repositorio;
            this.conversor = conversor;
            this.log = log;
            this.firmaProvider = firmaProvider;
            this.calculadora = calculadora;
            this.servicioOrquestador = servicioOrquestador;
            this.configuracion = configuracion;
            this.administradorDeCalles = administradorDeCalles;
        }

        public TipoDocumentoIdentidadDto ObtenerTipoDocumentoIdentidad(int id)
        {
            return Obtener<TipoDocumentoIdentidad, TipoDocumentoIdentidadDto>(id);
        }

        public IList<VideoCamaraDto> ListarVideoCamarasPuerto()
        {
            return Listar<VideoCamara, VideoCamaraDto>(x => x.PuestoDeTrabajo.NombrePuesto.Contains("puerto"));
        }

        public VariedadPorVinedoDto ObtenerVariedadPorVinedo(int id)
        {
            var variedadPorVinedo = Obtener<VariedadPorVinedo, VariedadPorVinedoDto>(id);
            variedadPorVinedo.KgRecibidos =
                repositorio.ObtenerConsultaEscalar(new KgRecibidosPorFincaViñedoAño(variedadPorVinedo.VinedoId,
                                                                                    variedadPorVinedo.VariedadId,
                                                                                    variedadPorVinedo.Cosecha));
            return variedadPorVinedo;
        }

        public string ObtenerCodigoEstablecimientoPorGuid(Guid instanceId)
        {
            var codigo = repositorio.ObtenerProyeccion<Recorrido, string>(x => x.InstanciaWorkflow == instanceId,
                                                             x => x.Establecimiento.CodigoDeEstablecimiento);
            return codigo ?? "";
        }

        public VariedadDto ObtenerVariedadPorMaterial(int id)
        {
            var variedad = repositorio.ObtenerProyeccion<Material, Variedad>(x => x.Id == id, x => x.Variedad);
            if (variedad != null)
            {
                return conversor.Convertir<Variedad, VariedadDto>(variedad);
            }
            return null;
        }

        public VinedoPropioDto ObtenerVinedoPropio(int id)
        {
            return Obtener<VinedoPropio, VinedoPropioDto>(id);
        }

        public VinedoTercerosDto ObtenerVinedoTerceros(int id)
        {
            return Obtener<VinedoTerceros, VinedoTercerosDto>(id);
        }

        public ListaPaginada<TipoDocumentoIdentidadDto> ListarPaginadoTiposDocumentoIdentidad(Paginacion paginacion)
        {
            return Listar<TipoDocumentoIdentidad, TipoDocumentoIdentidadDto>(null, paginacion);
        }

        public ListaPaginada<VariedadPorVinedoDto> ListarPaginadoVariedadPorVinedo(int vinedoId, Paginacion paginacion)
        {
            var variedadesPorVinedo = Listar<VariedadPorVinedo, VariedadPorVinedoDto>(x => x.Vinedo.Id == vinedoId,
                                                                                      paginacion);
            foreach (var variedadPorVinedo in variedadesPorVinedo)
            {
                variedadPorVinedo.KgRecibidos =
                    repositorio.ObtenerConsultaEscalar(new KgRecibidosPorFincaViñedoAño(vinedoId,
                                                                                        variedadPorVinedo.VariedadId,
                                                                                        variedadPorVinedo.Cosecha));
            }
            return variedadesPorVinedo;
        }

        public IList<TipoDocumentoIdentidadDto> ListarTiposDocumentoIdentidad()
        {
            return Listar<TipoDocumentoIdentidad, TipoDocumentoIdentidadDto>();
        }

        public IList<TipoVehiculoBodegaDto> ListarTiposVehiculoBodega()
        {
            return Listar<TipoVehiculoBodega, TipoVehiculoBodegaDto>();
        }

        public MaterialDto ObtenerMaterial(int id)
        {
            return Obtener<Material, MaterialDto>(id);
        }

        public MaterialDto ObtenerMaterialPorCodigoSap(string codigo)
        {
            string codigoTrim = codigo.TrimStart('0');
            return Obtener<Material, MaterialDto>(x => x.CodigoSAP.Equals(codigoTrim));
        }

        public int ObtenerMaterialIdPorCodigoSap(string codigo)
        {
            return repositorio.Listar<Material, int>(x => x.Id, x => x.CodigoSAP == codigo).FirstOrDefault();
        }

        public AlmacenDto ObtenerAlmacen(int id)
        {
            return Obtener<Almacen, AlmacenDto>(id);
        }

        public string ObtenerAlmacenDescripcion(int id)
        {
            var almacen = repositorio.Obtener<Almacen>(id);
            return almacen != null ? almacen.Descripcion : "";
        }

        public TaraRomaneoDto ObtenerTaraRomaneo(int id)
        {
            return Obtener<TaraRomaneo, TaraRomaneoDto>(id);
        }

        public TalonarioDto ObtenerTalonario(int id)
        {
            return Obtener<Talonario, TalonarioDto>(id);
        }

        public CentroDto ObtenerCentro(int id)
        {
            return Obtener<Centro, CentroDto>(id);
        }

        public int? ObtenerTiempoMaximoCentro(int id)
        {
            return repositorio.ObtenerProyeccion<Centro, int?>(y => y.Id == id, x => x.TiempoMaxEntreActividades);
        }

        public string ObteneCodigoSapPorCentroId(int id)
        {
            return repositorio.ObtenerProyeccion<Centro, string>(x => x.Id == id, x => x.CodigoSAP);
        }

        public int ObtenerCentroIdPorInstanceId(Guid instanceId)
        {
            return repositorio.ObtenerProyeccion<Recorrido, int>(x => x.InstanciaWorkflow == instanceId, x => x.Centro.Id);
        }

        public bool TieneDescuentoPorHumedad(Guid instanceId)
        {
            var analisis = repositorio.ObtenerProyeccion<Recorrido, AnalisisDeCalidad>(x => x.InstanciaWorkflow == instanceId, x => x.AnalisisDeCalidad);
            if (analisis != null)
            {
                var humedad = analisis.CaracteristicasAnalizadas.FirstOrDefault(x => x.CaracteristicaDeCalidad.EsHumedad);
                if (humedad != null)
                {
                    return humedad.DescuentoEnPorcentaje > 0;
                }
            }
            var calado = repositorio.ObtenerProyeccion<Recorrido, Calado>(x => x.InstanciaWorkflow == instanceId, x => x.Calado);
            if (calado != null)
            {
                var humedad = calado.CaladosPorCaracteristica.FirstOrDefault(x => x.CaracteristicaDeCalidad.EsHumedad);
                if (humedad != null)
                {
                    return humedad.DescuentoEnPorcentaje > 0;
                }
            }
            return false;
        }

        public bool TieneAnalisisDeCalidad(Guid instanceId)
        {
            return repositorio.Existe<Recorrido>(x => x.InstanciaWorkflow == instanceId && x.AnalisisDeCalidad != null);
        }

        public BalanzaDto ObtenerBalanza(int id)
        {
            return Obtener<Balanza, BalanzaDto>(id);
        }

        public string ObtenerBalanzaNombre(int id)
        {
            var balanza = repositorio.Obtener<Balanza>(id);
            return balanza != null ? balanza.Nombre : "";
        }

        public IList<BalanzaDto> ListarTodasLasBalanzasActivas(int centroId)
        {
            return Listar<Balanza, BalanzaDto>(x => x.Centro.Id == centroId && x.Desactivado == false, 20);
        }

        public IList<BalanzaDto> ListarTodasLasBalanzas(int centroId)
        {
            return Listar<Balanza, BalanzaDto>(x => x.Centro.Id == centroId, 20);
        }

        public IList<BalanzaDto> ListarBalanzasActivas(int centroId, TipoVehiculo tipoVehiculo)
        {
            var tipo = tipoVehiculo == TipoVehiculo.Tren ? tipoVehiculo : TipoVehiculo.Camión;
            return Listar<Balanza, BalanzaDto>(x => x.Centro.Id == centroId && x.TipoVehiculo == tipo && x.Desactivado == false, 20);
        }

        public IList<BalanzaDto> ListarBalanzasActivasPorNombrePc(int centroId, string nombrePc, TipoVehiculo tipoVehiculo)
        {
            var tipo = tipoVehiculo == TipoVehiculo.Tren ? tipoVehiculo : TipoVehiculo.Camión;
            return Listar<Balanza, BalanzaDto>(x => x.Centro.Id == centroId && x.PuestoDeTrabajo == nombrePc && x.TipoVehiculo == tipo
                && x.Desactivado == false);
        }

        public IList<BalanzaAutomaticaDto> ListarPuestosAutomaticosporCentro(int centroId)
        {
            var balanzas = repositorio.Listar<PuestoDeTrabajo, BalanzaAutomaticaDto>(x => new BalanzaAutomaticaDto
            {
                NombreBalanza = x.Balanza.Nombre,
                BalanzaId = x.Balanza.Id,
                PuestoId = x.Id,
                PausaFullAuto = x.PausaAutoFull,
                EsVagon = x.Balanza.TipoVehiculo == TipoVehiculo.Tren || x.Balanza.TipoVehiculo == TipoVehiculo.Bitren,
                EsExpo = x.Balanza.EsExportacion,
                Camara = x.VideoCamaras.FirstOrDefault().Codigo,
                Orden = x.OrdenBalanza,
                IntercomunicadorCodigo = x.IntercomunicadorCodigo
            }, x => x.AutomatizadoFull && x.Balanza != null && x.Centro.Id == centroId);
            foreach (var b in balanzas)
            {
                b.CamaraUrl = servicioOrquestador.ObtenerUrlPorCamara(new string[] { b.Camara }).FirstOrDefault();
                b.RutaNotificacion = configuracion.AppSettings["UrlNotificacionesWeb"];
                //log.Debug($"Url: {b.CamaraUrl} para camara {b.Camara}");
            }
            return balanzas;
        }

        public BalanzaDto ObtenerBalanzaPorPuestoDeTrabajoAutomatico(int puestoId)
        {
            return repositorio.ObtenerProyeccion<PuestoDeTrabajo, BalanzaDto>(x => x.Id == puestoId, x => new BalanzaDto
            {
                Id = x.Balanza.Id,
                Nombre = x.Balanza.Nombre
            });
        }

        public IList<NotificacionDto> ListarErrorBalanzas(List<int> puestosId)
        {
            var resultado = new List<NotificacionDto>();
            foreach (var puestoid in puestosId)
            {
                var error = repositorio.ObtenerMayor<Notificacion, int>(x => x.TipoAlerta == TipoAlerta.Automatica && x.PuestoId == puestoid, x => x.Id);
                if (error != null && !error.Leido)
                {
                    var errorDto = conversor.Convertir<Notificacion, NotificacionDto>(error);
                    if (errorDto.Mensaje.Contains("\"Actividad\":\"\""))
                    {
                        var notificacionConDatos = repositorio.ObtenerMayor<Notificacion, int>(x => x.TipoAlerta == TipoAlerta.Automatica && x.PuestoId == puestoid && x.Hora < errorDto.Hora && !x.Leido && !x.Mensaje.StartsWith("{\"Actividad\":\"\""), x => x.Id);
                        if (notificacionConDatos != null)
                        {
                            var notificacionDtoConDatos = conversor.Convertir<Notificacion, NotificacionDto>(notificacionConDatos);
                            var balanzaConDatos = ExtensionesSerializacion.FromJson<NotificacionPesadaAutomaticaDto>(notificacionDtoConDatos.Mensaje);
                            var balanza = ExtensionesSerializacion.FromJson<NotificacionPesadaAutomaticaDto>(errorDto.Mensaje);
                            balanzaConDatos.Error = balanza.Error;
                            notificacionConDatos.Mensaje = ExtensionesSerializacion.ToJson<NotificacionPesadaAutomaticaDto>(balanzaConDatos);
                            resultado.Add(notificacionDtoConDatos);
                        }
                    }
                    else
                    {
                        resultado.Add(errorDto);
                    }
                }
            }
            return resultado;
        }

        public IList<CentroInfoDto> BuscarCentros(string criteria)
        {
            return repositorio.Listar<Centro, CentroInfoDto>(x => new CentroInfoDto
            {
                Descripcion = x.Descripcion,
                Id = x.Id
            },
                                                             f =>
                                                             f.Descripcion.Contains(criteria) ||
                                                             f.CodigoSAP.Contains(criteria), 20);
        }

        public CentroInfoDto BuscarCentro(string criteria)
        {
            CentroInfoDto proveedor;
            try
            {
                proveedor = repositorio.ObtenerProyeccion<Centro, CentroInfoDto>(
                    f =>
                    f.Descripcion.Contains(criteria) ||
                    f.CodigoSAP.Contains(criteria),
                    x => new CentroInfoDto
                    {
                        Descripcion = x.Descripcion,
                        Id = x.Id
                    });
            }
            catch
            {
                proveedor = null;
            }
            return proveedor;
        }

        public IList<CentroDto> BuscarCentrosBodega(string criteria)
        {
            return Listar<Centro, CentroDto>(c => (c.Descripcion.Contains(criteria) || c.NumeroINV.Contains(criteria)) && c.NumeroINV != null);
        }

        public CentroDto BuscarCentroBodega(string criteria)
        {
            Centro centro;
            try
            {
                centro = repositorio.Obtener<Centro>(f => (f.NumeroINV.Contains(criteria) || f.Descripcion.Contains(criteria)) && f.NumeroINV != null);
            }
            catch
            {
                centro = null;
            }
            return centro == null ? null : conversor.Convertir<Centro, CentroDto>(centro);
        }

        public IList<ProveedorInfoDto> BuscarProveedoresPorCuit(string cuit, TiposProveedor tipo)
        {
            return repositorio.Listar<Proveedor, ProveedorInfoDto>(x => new ProveedorInfoDto
            {
                Cuil = x.Cuil,
                Descripcion = x.Descripcion,
                Id = x.Id
            }, f => f.Activo
                    && (!tipo.PR || f.PR) && (!tipo.AM || f.AM) && (!tipo.CM || f.CM)
                    && f.Cuil == cuit, 20);
        }

        public MaterialDto BuscarMaterial(int centroId, string criteria)
        {
            Material material;
            try
            {
                material =
                    repositorio.Obtener<MaterialPorCentro>(
                        f => f.Material.Activo && (f.Material.Descripcion.Contains(criteria)) && f.Centro.Id == centroId)
                               ?.Material;
            }
            catch
            {
                material = null;
            }
            return material == null ? null : conversor.Convertir<Material, MaterialDto>(material);
        }

        public MaterialDto BuscarMaterialTodosLosCentros(string criteria)
        {
            Material material;
            try
            {
                material = repositorio.Obtener<Material>(f => f.Activo && (f.Descripcion.Contains(criteria)));
            }
            catch
            {
                material = null;
            }
            return material == null ? null : conversor.Convertir<Material, MaterialDto>(material);
        }

        public IList<MaterialDto> BuscarMaterialesTodosLosCentros(string criteria)
        {
            return Listar<Material, MaterialDto>(m => m.Activo && m.Descripcion.Contains(criteria));
        }

        public MaterialPorCentroDto BuscarMaterialPorCentro(int centroId, string criteria)
        {
            MaterialPorCentro material;
            try
            {
                material =
                    repositorio.Obtener<MaterialPorCentro>(
                        f => f.Material.Activo && f.Material.Descripcion.Contains(criteria) && f.Centro.Id == centroId);
            }
            catch
            {
                material = null;
            }
            return material == null ? null : conversor.Convertir<MaterialPorCentro, MaterialPorCentroDto>(material);
        }

        public VinedoDto BuscarVinedo(string criteria)
        {
            Vinedo vinedo;
            try
            {
                vinedo =
                    repositorio.Obtener<Vinedo>(f => f.NumeroINV.Contains(criteria) || f.Descripcion.Contains(criteria));
            }
            catch
            {
                vinedo = null;
            }
            return vinedo == null ? null : conversor.Convertir<Vinedo, VinedoDto>(vinedo);
        }

        public IList<VinedoDto> BuscarVinedos(string criteria)
        {
            return
                Listar<Vinedo, VinedoDto>(
                    f => f.NumeroINV.Contains(criteria) || f.Descripcion.Contains(criteria), 20);
        }

        public PrecintoDto ObtenerPrecinto(int id)
        {
            return Obtener<Precinto, PrecintoDto>(id);
        }

        public IList<AlmacenDto> ObtenerAlmacenesPorCentro(int centroId)
        {
            Expression<Func<Almacen, bool>> expresionFiltro = x => x.Centro.Id == centroId;
            var almacenes = repositorio.Listar(expresionFiltro);
            return conversor.ConvertirList<Almacen, AlmacenDto>(almacenes);
        }

        public IList<TipoEmbalajeDto> ListarEmbalaje()
        {
            return Listar<TipoEmbalaje, TipoEmbalajeDto>();
        }

        //public IList<TipoEmbalajeDto> ObtenerTipoEmbalaje()
        //{
        //    Expression<Func<TipoEmbalaje, bool>> expresionFiltro = x => x.Activo == true;
        //    var embalajes = repositorio.Listar(expresionFiltro);
        //    return conversor.ConvertirList<TipoEmbalaje, TipoEmbalajeDto>(embalajes);
        //}

        public MaterialPorCentroDto ObtenerMaterialPorCentro(int centroId, int materialId)
        {
            return
                Obtener<MaterialPorCentro, MaterialPorCentroDto>(
                    x => x.Centro.Id == centroId && x.Material.Id == materialId);
        }

        public MaterialPorCentroDto ObtenerMaterialPorCentroPorInstanceId(Guid instanceId)
        {
            //TODO: se podría hacer con una sola consulta?
            var materialCentro = repositorio.ObtenerProyeccion((Recorrido x) => x.InstanciaWorkflow == instanceId,
                                          x => new { MaterialId = x.Material.Id, CentroId = x.Centro.Id });
            return materialCentro == null
                       ? null
                       : Obtener<MaterialPorCentro, MaterialPorCentroDto>(
                           x => x.Centro.Id == materialCentro.CentroId && x.Material.Id == materialCentro.MaterialId);
        }

        public Dominio.Dto.CamaraDto ObtenerCamaraPorMaterialPorCentro(Guid instanceId)
        {
            var materialCentro = repositorio.ObtenerProyeccion((Recorrido x) => x.InstanciaWorkflow == instanceId,
                                          x => new { MaterialId = x.Material.Id, CentroId = x.Centro.Id });
            var camara = materialCentro == null
                       ? null
                       : repositorio.ObtenerProyeccion<MaterialPorCentro, Camara>(x => x.Centro.Id == materialCentro.CentroId && x.Material.Id == materialCentro.MaterialId, x => x.Camara);

            return camara != null ? conversor.Convertir<Camara, Dominio.Dto.CamaraDto>(camara) : null;
        }

        public ListaPaginada<MaterialDto> ListarPaginadoMateriales(string filtro, int centroId, Paginacion paginacion)
        {
            Expression<Func<MaterialPorCentro, bool>> expresionFiltro =
                x => x.Material.Activo && x.Centro.Id == centroId;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = (x => (x.Material.Descripcion.Contains(filtro)
                                         || (x.Camara.Descripcion.Contains(filtro))
                                         || (x.Material.CodigoSAP.Contains(filtro))
                                         /*|| (x.Material.Cosecha.Contains(filtro))*/) && x.Material.Activo &&
                                        x.Centro.Id == centroId);
            }

            return Listar<MaterialPorCentro, MaterialDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<TaraRomaneoDto> ListarTaraRomaneos(string filtro, Paginacion paginacion)
        {
            Expression<Func<TaraRomaneo, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                int filtroInt;
                if (!int.TryParse(filtro, out filtroInt))
                {
                    filtroInt = int.MaxValue;
                }

                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    x.Codigo.Equals(filtroInt) || x.Descripcion.Contains(filtro) || x.Id.Equals(filtroInt) ||
                    x.Peso.Equals(filtroInt);
            }

            return Listar<TaraRomaneo, TaraRomaneoDto>(expresionFiltro, paginacion);
        }

        public IList<TaraRomaneoDto> ListarTaraRomaneosPorCentro(int centroId)
        {
            return Listar<TaraRomaneo, TaraRomaneoDto>(m => m.Centro.Id == centroId);
        }

        public IList<MaterialPorCentroDto> BuscarMaterialesPorCentro(int centroId, string filtro, int cantidad = 20, string tipoCalle = null)
        {
            if (!string.IsNullOrEmpty(tipoCalle))
            {
                var tipo = (TipoCalle)Enum.Parse(typeof(TipoCalle), tipoCalle);
                return cantidad > 0 ? Listar<MaterialPorCentro, MaterialPorCentroDto>(
                                        m => m.Material.Activo && m.Centro.Id == centroId && m.Material.Descripcion.Contains(filtro)
                                        && ((tipo == TipoCalle.NoGranos || tipo == TipoCalle.PlantaNoGranos) ? !m.Material.EsGrano : m.Material.EsGrano), cantidad) :
                                      Listar<MaterialPorCentro, MaterialPorCentroDto>(
                                        m => m.Material.Activo && m.Centro.Id == centroId && m.Material.Descripcion.Contains(filtro));
            }
            else
            {
                return cantidad > 0 ? Listar<MaterialPorCentro, MaterialPorCentroDto>(
                                        m => m.Material.Activo && m.Centro.Id == centroId && m.Material.Descripcion.Contains(filtro), cantidad) :
                                      Listar<MaterialPorCentro, MaterialPorCentroDto>(
                                        m => m.Material.Activo && m.Centro.Id == centroId && m.Material.Descripcion.Contains(filtro));
            }
        }

        public IList<MaterialDto> ListarMaterialesPorCamara(int camaraId)
        {
            var materiales = repositorio.Listar<MaterialPorCentro, Material>(x => x.Material,
                                                                             m =>
                                                                             m.Material.Activo &&
                                                                             m.Camara.Id == camaraId);
            return
                conversor.ConvertirList<Material, MaterialDto>(
                    materiales.GroupBy(g => g.Id).Select(s => s.First()).OrderBy(x => x.Descripcion).ToList());
        }

        public IList<ConversionGrupoDto> ListarGruposPorCamara(int camaraId)
        {
            return Listar<ConversionGrupo, ConversionGrupoDto>(m => m.Camara.Id == camaraId);
        }

        public ListaPaginada<AlmacenDto> ListarPaginadoAlmacenes(string filtro, Paginacion paginacion, int centroId)
        {
            Expression<Func<Almacen, bool>> expresionFiltro;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = (x => ((x.Descripcion.Contains(filtro))
                                         || (x.DescripcionCorta.Contains(filtro))
                                         || (x.CodigoSAP.Contains(filtro))
                                         || (x.CodigoONCCA.Contains(filtro)))
                                        && (x.Centro.Id == centroId));
            }
            else
            {
                expresionFiltro = x => x.Centro.Id == centroId;
            }

            return Listar<Almacen, AlmacenDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<TaraRomaneoDto> ListarPaginadoTaraRomaneo(string filtro, Paginacion paginacion,
                                                                       int centroId)
        {
            Expression<Func<TaraRomaneo, bool>> expresionFiltro;
            if (!string.IsNullOrEmpty(filtro))
            {
                int filtroInt;
                if (!int.TryParse(filtro, out filtroInt))
                {
                    filtroInt = int.MaxValue;
                }
                expresionFiltro = (x => ((x.Descripcion.Contains(filtro))
                                         || (x.Peso.Equals(filtroInt))
                                         || (x.Codigo.Equals(filtroInt))
                                         && (x.Centro.Id == centroId)));
            }
            else
            {
                expresionFiltro = x => x.Centro.Id == centroId;
            }

            return Listar<TaraRomaneo, TaraRomaneoDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<CalidadMaterialDto> ListarPaginadoCalidadMaterial(int materialId, int centroId,
                                                                               Paginacion paginacion)
        {
            return
                Listar<CalidadMaterial, CalidadMaterialDto>(
                    x => x.MaterialPorCentro.Material.Id == materialId && x.MaterialPorCentro.Centro.Id == centroId,
                    paginacion);
        }

        public ListaPaginada<TalonarioDto> ListarPaginadoTalonario(string filtro, Paginacion paginacion, int centroId)
        {
            Expression<Func<Talonario, bool>> expresionFiltro;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = x => x.Descripcion.Contains(filtro);
            }
            else
            {
                expresionFiltro = x => x.Centro.Id == centroId;
            }

            return Listar<Talonario, TalonarioDto>(expresionFiltro, paginacion);
        }

        public IList<AlmacenDto> ListarAlmacenes()
        {
            return Listar<Almacen, AlmacenDto>();
        }

        public IList<AlmacenDto> ListarAlmacenesPorCentroYesSustentable(int centroId, bool esSustentable)
        {
            return Listar<Almacen, AlmacenDto>(f => f.Centro.Id == centroId && f.EsSojaSustentable == esSustentable);
        }

        public IList<AlmacenDto> ListarAlmacenesPorCentro(int centroId)
        {
            return Listar<Almacen, AlmacenDto>(f => f.Centro.Id == centroId);
        }

        public IList<AlmacenDto> ListarAlmacenesPorMaterialYCentro(int centroId, int materialId, bool esSustentable)
        {
            return
                Listar<Almacen, AlmacenDto>(
                    f =>
                    f.Centro.Id == centroId && f.Materiales.Any(x => x.Id == materialId) &&
                    f.EsSojaSustentable == esSustentable && !f.EPA);
        }

        public IList<CentroDto> ListarCentros()
        {
            return Listar<Centro, CentroDto>();
        }

        public IList<CentroDto> ListarCentrosPorUsuario(string nombreUsuario)
        {
            return Listar<Centro, CentroDto>(f => f.UsuariosAsociados.Any(x => x.NombreUsuario == nombreUsuario));
        }

        public ListaPaginada<CentroDto> ListarPaginadoCentros(Paginacion paginacion)
        {
            return Listar<Centro, CentroDto>(null, paginacion);
        }

        public ListaPaginada<CentroDto> ListarPaginadoCentrosPorUsuario(string nombreUsuario, Paginacion paginacion)
        {
            return Listar<Centro, CentroDto>(f => f.UsuariosAsociados.Any(x => x.NombreUsuario == nombreUsuario),
                                             paginacion);
        }

        public IList<Dominio.Dto.CamaraDto> ListarCamaras()
        {
            return Listar<Camara, Dominio.Dto.CamaraDto>();
        }

        public ListaPaginada<Dominio.Dto.CamaraDto> ListarPaginadoCamaras(string filtro, Paginacion paginacion)
        {
            Expression<Func<Camara, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro = x => x.CodigoSAP.Contains(filtro)
                                       || x.Descripcion.Contains(filtro)
                                       || x.DescripcionCorta.Contains(filtro);
            }
            return Listar<Camara, Dominio.Dto.CamaraDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<CategoriaDto> ListarPaginadoCategoria(string filtro, Paginacion paginacion)
        {
            Expression<Func<Categoria, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro = x => x.Clasificacion.Contains(filtro);
            }
            return Listar<Categoria, CategoriaDto>(expresionFiltro, paginacion);
        }

        public Dominio.Dto.CamaraDto ObtenerCamara(int id)
        {
            return Obtener<Camara, Dominio.Dto.CamaraDto>(id);
        }

        public CategoriaDto ObtenerCategoria(int id)
        {
            return Obtener<Categoria, CategoriaDto>(id);
        }

        public CasilleroDto ObtenerCasillero(int id)
        {
            return Obtener<Casillero, CasilleroDto>(id);
        }

        public CasilleroDto ObtenerCasilleroPorNumeroYCentro(string numero, int centroId)
        {
            return Obtener<Casillero, CasilleroDto>(x => x.Numero == numero && x.Centro.Id == centroId);
        }

        public ListaPaginada<CasilleroDto> ListarPaginadoCasilleros(Paginacion paginacion, int centroId)
        {
            Expression<Func<Casillero, bool>> expresionFiltro = x => x.Centro.Id.Equals(centroId);

            return Listar<Casillero, CasilleroDto>(expresionFiltro, paginacion);
        }

        public IList<CasilleroDto> ListarCasillerosPorCentro(int centroId)
        {
            return Listar<Casillero, CasilleroDto>(x => x.Centro.Id.Equals(centroId));
        }

        public IList<MicroMuestrasPorCasilleroDto> ListarMicroMuestrasPorCasilleroPorCentro(int centroId)
        {
            return
                Listar<MicroMuestrasPorCasillero, MicroMuestrasPorCasilleroDto>(
                    x => x.Casillero.Centro.Id.Equals(centroId));
        }

        public ListaPaginada<LiberacionDeCasillerosDto> ListarLiberacionDeCasilleros(LiberacionDeCasillerosDto filtro,
                                                                                     Paginacion paginacion)
        {
            filtro.Hasta = filtro.Hasta ?? "9999-999999";
            filtro.Desde = filtro.Desde ?? "0000-000000";

            var consulta = new LiberacionDeCasillerosConsulta(filtro, paginacion);
            return repositorio.ListarConsultaPaginada(consulta);
        }

        public ListaPaginada<ConsultaCasilleroAntiguedadDto> ListarConsultaCasillerosPorAntiguedad(
            ConsultaCasilleroAntiguedadDto filtro, Paginacion paginacion)
        {
            var consulta = new CasillerosPorAntiguedadConsulta(filtro, paginacion);
            return repositorio.ListarConsultaPaginada(consulta);
        }

        public ListaPaginada<ConsultaCasilleroDto> ListarConsultaCasillerosPorCasillero(ConsultaCasilleroDto filtro,
                                                                                        Paginacion paginacion)
        {
            filtro.Desde = filtro.Desde ?? "0000-000000";
            filtro.Hasta = filtro.Hasta ?? "9999-999999";

            var consulta = new CasillerosPorCasilleroConsulta(filtro, paginacion);
            return repositorio.ListarConsultaPaginada(consulta);
        }

        public ListaPaginada<ConsultaCasilleroMuestraDto> ListarConsultaCasillerosPorMuestra(
            ConsultaCasilleroMuestraDto filtro, Paginacion paginacion)
        {
            var consulta = new CasillerosPorMuestraConsulta(filtro, paginacion);
            return repositorio.ListarConsultaPaginada(consulta);
        }

        public IList<ProvinciaDto> ListarProvincias()
        {
            return Listar<Provincia, ProvinciaDto>();
        }

        public IList<PaisDto> ListarPaises()
        {
            return Listar<Pais, PaisDto>();
        }

        public IList<ProvinciaDto> ListarProvinciasPorPais(int paisId)
        {
            return Listar<Provincia, ProvinciaDto>(x => x.Pais.Id == paisId);
        }

        public MuestraEnvioACamaraDto ObtenerUltimaMuestraEnvioACamaraPorCaladoId(int caladoId)
        {
            return ObtenerUltimo<MuestraEnvioACamara, MuestraEnvioACamaraDto>(x => x.Calado.Id == caladoId, x => x.Id);
        }

        public ListaPaginada<MuestraEnvioACamaraDto> ListarMuestrasPorLote(int id, Paginacion paginacion)
        {
            var muestras = repositorio.ListarConsulta(new ListarMuestraEnvioACamaraConsulta(0, loteId: id, paginacion: paginacion));

            return new ListaPaginada<MuestraEnvioACamaraDto>(muestras, 0, muestras.Count == 0 ? 1 : muestras.Count, muestras.Count);
        }

        public int ObtenerNumeroDocumentoGenerado()
        {
            return repositorio.ObtenerNumeroDocumentoGenerado();
        }

        public int ObtenerNumeroMuestraAuditoriaGenerado()
        {
            return repositorio.ObtenerNumeroMuestraAuditoriaGenerado();
        }

        public int ObtenerNumeroDeTicketGenerado(int puestoDeTrabajoId, bool pagoConMercadoPago)
        {
            return repositorio.ObtenerNumeroDeTicketGenerado(puestoDeTrabajoId, pagoConMercadoPago);
        }

        public int ObtenerNumeroDeTicketImportacionGenerado()
        {
            return repositorio.ObtenerNumeroDeTicketImportacionGenerado();
        }

        public int ObtenerNumeroControlDeCargaGenerado()
        {
            return repositorio.ObtenerNumeroDeControlDeCargaGenerado();
        }

        public int ObtenerSecuenciaEnvioACamara()
        {
            return repositorio.ObtenerNumeroDocumentoGenerado();
        }

        public int ObtenerNumeroDocumentoFasonGenerado()
        {
            return repositorio.ObtenerNumeroDocumentoFasonGenerado();
        }

        public int ObtenerNumeroInformeGenerado()
        {
            return repositorio.ObtenerNumeroOrdenDeDescargaGenerado();
        }

        public string ObtenerNumeroOrdenDeDescargaGenerado(int centroId)
        {
            var codigoCentro = ObteneCodigoSapPorCentroId(centroId) ?? "";
            var numero = repositorio.ObtenerNumeroOrdenDeDescargaGenerado();
            return codigoCentro + "-" + numero.ToString(CultureInfo.InvariantCulture).PadLeft(8, '0');
        }

        public string ObtenerNumeroOrdenEntrePlantasGenerado(int centroId)
        {
            var codigoCentro = ObteneCodigoSapPorCentroId(centroId) ?? "";
            var numero = repositorio.ObtenerNumeroOrdenEntrePlantasGenerado();
            return codigoCentro + "-" + numero.ToString(CultureInfo.InvariantCulture).PadLeft(8, '0');
        }

        public string ObtenerNumeroOrdenDeDescargaFasonGenerado(int centroId)
        {
            var codigoCentro = ObteneCodigoSapPorCentroId(centroId) ?? "";
            var numero = repositorio.ObtenerNumeroOrdenDeDescargaFasonGenerado();
            return codigoCentro + "-" + numero.ToString(CultureInfo.InvariantCulture).PadLeft(8, '0');
        }

        public int ObtenerNumeroHojaDeRutaGenerado()
        {
            return repositorio.ObtenerNumeroHojaDeRutaGenerado();
        }

        public int ObtenerNumeroRemitoGenerado()
        {
            return repositorio.ObtenerNumeroOrdenDeDescargaFasonGenerado();
        }

        public IList<LocalidadDto> ListarLocalidades()
        {
            return Listar<Localidad, LocalidadDto>();
        }

        public LocalidadDto ObtenerLocalidad(int id)
        {
            return Obtener<Localidad, LocalidadDto>(id);
        }

        public IList<LocalidadDto> ListarLocalidadesPorProvincia(int provinciaId)
        {
            return Listar<Localidad, LocalidadDto>(x => x.Provincia.Id == provinciaId);
        }

        public ListaPaginada<ChoferDto> ListarChoferes(string filtro, Paginacion paginacion)
        {
            Expression<Func<Chofer, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    x.Apellido.Contains(filtro) || x.Nombre.Contains(filtro) ||
                    x.TipoDocumentoIdentidad.DescripcionCorta.Contains(filtro) || x.NumeroDeDocumento.Contains(filtro);
            }

            return Listar<Chofer, ChoferDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<CategoriaVehiculoDto> ListarCategoriaCamiones(string filtro, Paginacion paginacion)
        {
            Expression<Func<CategoriaVehiculo, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    x.Patente.Contains(filtro) ||
                    x.PatenteAcoplado.Contains(filtro) ||
                    x.PatenteAcoplado2.Contains(filtro);
            }

            return Listar<CategoriaVehiculo, CategoriaVehiculoDto>(expresionFiltro, paginacion);
        }

        public CategoriaVehiculoDto BuscarCategoriaVehiculo(string patente, string acoplado, string acoplado2)
        {
            return repositorio.ObtenerConsultaEscalar(new ObtenerVehiculo(patente, acoplado, acoplado2));
        }

        public ListaPaginada<DocumentoExternoDto> ListarDocumentos(string filtro, Paginacion paginacion)
        {
            Expression<Func<DocumentoExterno, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    x.NumeroDeDocumento.Contains(filtro) ||
                    x.ArchivoRutaDestino.Contains(filtro);
            }

            return Listar<DocumentoExterno, DocumentoExternoDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<KmPorProveedorDto> ListarKmPorProveedor(string filtro, Paginacion paginacion)
        {
            Expression<Func<KmPorProveedor, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x => x.Cliente.Descripcion.Contains(filtro) || x.Localidad.Descripcion.Contains(filtro) || x.Centro.Descripcion.Contains(filtro)
                    || x.Cliente.Cuit.Contains(filtro);
            }

            return Listar<KmPorProveedor, KmPorProveedorDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<VinedoPropioDto> ListarVinedoPropio(string filtro, Paginacion paginacion)
        {
            Expression<Func<VinedoPropio, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    x.Descripcion.Contains(filtro);
            }

            return Listar<VinedoPropio, VinedoPropioDto>(expresionFiltro, paginacion);
        }

        public IList<VinedoPropioDto> ListarVinedosPropios()
        {
            return Listar<VinedoPropio, VinedoPropioDto>();
        }

        public IList<VinedoTercerosDto> ListarVinedosTerceros(int proveedorId)
        {
            return Listar<VinedoTerceros, VinedoTercerosDto>(x => x.Proveedor.Id == proveedorId);
        }

        public IList<ChoferDto> BuscarChoferes(ChoferFiltro filtro)
        {
            filtro.Cuil = filtro.Cuil ?? string.Empty;
            filtro.Nombre = filtro.Nombre ?? string.Empty;
            filtro.Apellido = filtro.Apellido ?? string.Empty;
            filtro.Documento = filtro.Documento ?? string.Empty;

            if (filtro.Cuil.Contains("-"))
            {
                filtro.Cuil = filtro.Cuil.Split('-')[1];
                if (filtro.Cuil.Contains("_"))
                {
                    filtro.Cuil = filtro.Cuil.Replace("_", "");
                }
            }

            Expression<Func<Chofer, bool>> expresionFiltro = x => x.Apellido.Contains(filtro.Apellido) &&
                                                                  x.Nombre.Contains(filtro.Nombre) &&
                                                                  x.Cuil.Contains(filtro.Cuil) &&
                                                                  x.NumeroDeDocumento.Contains(filtro.Documento);
            return Listar<Chofer, ChoferDto>(expresionFiltro, 20);
        }

        public ChoferDto BuscarChofer(ChoferFiltro filtro)
        {
            Chofer entidad;
            filtro.Cuil = filtro.Cuil ?? string.Empty;
            filtro.Nombre = filtro.Nombre ?? string.Empty;
            filtro.Apellido = filtro.Apellido ?? string.Empty;
            filtro.Documento = filtro.Documento ?? string.Empty;

            if (filtro.Cuil.Contains("-"))
            {
                filtro.Cuil = filtro.Cuil.Split('-')[1];
                if (filtro.Cuil.Contains("_"))
                {
                    filtro.Cuil = filtro.Cuil.Replace("_", "");
                }
            }

            Expression<Func<Chofer, bool>> expresionFiltro = x => x.Apellido.Contains(filtro.Apellido) &&
                                                                  x.Nombre.Contains(filtro.Nombre) &&
                                                                  x.Cuil.Contains(filtro.Cuil) &&
                                                                  x.NumeroDeDocumento.Contains(filtro.Documento);
            try
            {
                entidad = repositorio.Obtener(expresionFiltro);
            }
            catch
            {
                entidad = null;
            }
            return entidad == null ? null : conversor.Convertir<Chofer, ChoferDto>(entidad);
        }

        public IList<ChoferDto> BuscarChoferesGeneral(string filtro)
        {
            return Listar<Chofer, ChoferDto>(x => x.Nombre.Contains(filtro) || x.Apellido.Contains(filtro)
                                                  || (x.Nombre + " " + x.Apellido).Contains(filtro)
                                                  || x.Cuil.Contains(filtro) || x.NumeroDeDocumento.Contains(filtro));
        }

        public ChoferDto ObtenerChofer(int id)
        {
            return Obtener<Chofer, ChoferDto>(id);
        }

        public CategoriaVehiculoDto ObtenerCategoriaVehiculo(int id)
        {
            return Obtener<CategoriaVehiculo, CategoriaVehiculoDto>(id);
        }

        public KmPorProveedorDto ObtenerKmPorProveedor(int id)
        {
            return Obtener<KmPorProveedor, KmPorProveedorDto>(id);
        }

        public GraficoDePlantaDto ObtenerGraficoDePlanta(string nombreActividad, int centroId)
        {
            return Obtener<GraficoDePlanta, GraficoDePlantaDto>(x => x.NombreActividad == nombreActividad && x.Centro.Id == centroId);
        }

        public IList<GraficoDePlantaDto> ListarGraficosDePlanta(int centroId)
        {
            return Listar<GraficoDePlanta, GraficoDePlantaDto>(x => x.Centro.Id == centroId);
        }

        public ChoferDto ObtenerChoferPorCuit(string cuit)
        {
            return Obtener<Chofer, ChoferDto>(x => x.Cuil.Equals(cuit));
        }

        public IList<TransportistaDto> BuscarTransportistas(string criteria)
        {
            return
                Listar<Transportista, TransportistaDto>(
                    f => f.RazonSocial.Contains(criteria) || f.Cuit.Contains(criteria), 20);
        }

        public TransportistaInfoDto BuscarTransportista(string criteria)
        {
            TransportistaInfoDto proveedor;
            try
            {
                proveedor =
                    repositorio.ObtenerProyeccion<Transportista, TransportistaInfoDto>(
                        f => f.RazonSocial.Contains(criteria) || f.Cuit.Contains(criteria),
                        x => new TransportistaInfoDto
                        {
                            Id = x.Id,
                            Cuit = x.Cuit,
                            RazonSocial = x.RazonSocial
                        }
                        );
            }
            catch
            {
                proveedor = null;
            }
            return proveedor;
        }

        public IList<TransportistaInfoDto> BuscarTransportistasPorCuit(string criteria)
        {
            return repositorio.Listar<Transportista, TransportistaInfoDto>(x => new TransportistaInfoDto
            {
                Id = x.Id,
                Cuit = x.Cuit,
                RazonSocial = x.RazonSocial
            }, f => f.Cuit.Contains(criteria));
        }

        public IList<TransportistaDto> ListarTransportistas()
        {
            return Listar<Transportista, TransportistaDto>();
        }

        public ListaPaginada<TransportistaDto> ListarPaginadoTransportistas(string filtro, Paginacion paginacion)
        {
            Expression<Func<Transportista, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    x.RazonSocial.Contains(filtro) || x.Cuit.Contains(filtro) || x.Domicilio.Contains(filtro) ||
                    x.Provincia.Descripcion.Contains(filtro) || x.Localidad.Descripcion.Contains(filtro);
            }

            return Listar<Transportista, TransportistaDto>(expresionFiltro, paginacion);
        }

        public TransportistaDto ObtenerTransportista(int id)
        {
            return Obtener<Transportista, TransportistaDto>(id);
        }

        public TransportistaDto ObtenerTransportistaPorCuit(string cuit)
        {
            return Obtener<Transportista, TransportistaDto>(x => x.Cuit.Equals(cuit));
        }

        public TransportistaDto ObtenerTransportistaPorRazonSocial(string razonSocial)
        {
            return ObtenerPrimero<Transportista, TransportistaDto>(x => x.RazonSocial.Equals(razonSocial));
        }

        public ClienteDto ObtenerCliente(int id)
        {
            return Obtener<Cliente, ClienteDto>(id);
        }

        public ClienteDto ObtenerClientePorCodigoSap(string codigo)
        {
            return Obtener<Cliente, ClienteDto>(x => x.CodigoSap.Equals(codigo));
        }

        public ExcepcionAlControlDto ObtenerExcepcionAlControl(int id)
        {
            return Obtener<ExcepcionAlControl, ExcepcionAlControlDto>(id);
        }

        public bool BuscarExcepcionAlControl(int materialId, int transportistaId, int centroId, DateTime fecha, int? centroDestinoId, int? clienteDestinoId)
        {
            return repositorio.Existe<ExcepcionAlControl>(x =>
                                                          x.Material.Id == materialId &&
                                                          x.Transportista.Id == transportistaId &&
                                                          x.Centro.Id == centroId &&
                                                          x.FechaDesde <= fecha && x.FechaHasta >= fecha &&
                                                          x.Motivo != MotivoExcepcionAlControl.B &&
                                                          ((centroDestinoId.HasValue && x.CentroDestino.Id == centroDestinoId.Value) || (clienteDestinoId.HasValue && x.ClienteDestino.Id == clienteDestinoId.Value) || (!centroDestinoId.HasValue && !clienteDestinoId.HasValue))
                                                          );
        }

        public ListaPaginada<ExcepcionAlControlDto> ListarExcepcionesAlControl(string filtro, Paginacion paginacion)
        {
            Expression<Func<ExcepcionAlControl, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x => x.Motivo != MotivoExcepcionAlControl.B && (
                                                                       x.Transportista.RazonSocial.Contains(filtro) ||
                                                                       x.Transportista.Cuit.Contains(filtro) ||
                                                                       x.Material.Descripcion.Contains(filtro) ||
                                                                       x.Material.DescripcionCorta.Contains(filtro) ||
                                                                       x.Material.CodigoSAP.Contains(filtro) ||
                                                                       x.Centro.Descripcion.Contains(filtro) ||
                                                                       (x.CentroDestino != null && x.CentroDestino.Descripcion.Contains(filtro)) ||
                                                                       (x.ClienteDestino != null && x.ClienteDestino.Descripcion.Contains(filtro)));
            }
            else
            {
                expresionFiltro = x => x.Motivo != MotivoExcepcionAlControl.B;
            }

            return Listar<ExcepcionAlControl, ExcepcionAlControlDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<ExcepcionAlDescuentoDto> ListarExcepcionesAlDescuento(string filtro, Paginacion paginacion, int centroId)
        {
            Expression<Func<ExcepcionAlDescuento, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                        x.CaracteristicaDeCalidad.MaterialPorCentro.Centro.Id == centroId &&
                        (x.Proveedor.RazonSocial.Contains(filtro) ||
                         x.CaracteristicaDeCalidad.MaterialPorCentro.Material.Descripcion.Contains(filtro) ||
                         x.Camara.Descripcion.Contains(filtro) ||
                         x.CaracteristicaDeCalidad.DescripcionCorta.Contains(filtro) ||
                         x.Usuario.Equals(filtro));
            }
            else
            {
                expresionFiltro = (x => x.CaracteristicaDeCalidad.MaterialPorCentro.Centro.Id == centroId);
            }
            return Listar<ExcepcionAlDescuento, ExcepcionAlDescuentoDto>(expresionFiltro, paginacion);
        }

        public ExcepcionAlDescuentoDto ObtenerExcepcionAlDescuento(int id)
        {
            return Obtener<ExcepcionAlDescuento, ExcepcionAlDescuentoDto>(id);
        }

        public ListaPaginada<InhabilitacionChoferDto> ListarInhabilitacionChoferes(string filtro, int centroId,
                                                                                   Paginacion paginacion)
        {
            DateTime filtroDate;
            if (!DateTime.TryParse(filtro, out filtroDate))
            {
                filtroDate = DateTime.MaxValue;
            }
            Expression<Func<InhabilitacionChofer, bool>> expresionFiltro = x => true;

            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro = x => ((x.Chofer.Nombre.Contains(filtro))
                                        || (x.Chofer.Apellido.Contains(filtro))
                                        || (x.Chofer.NumeroDeDocumento.Contains(filtro))
                                        || (x.FechaDesde == filtroDate)
                                        || (x.FechaHasta == filtroDate)
                                        || (x.NombreUsuarioResponsable.Contains(filtro))
                                        || (x.Motivo.Contains(filtro))
                                        || (x.Chofer.TipoDocumentoIdentidad.Descripcion.Contains(filtro)));
            }

            return Listar<InhabilitacionChofer, InhabilitacionChoferDto>(expresionFiltro, paginacion);
        }

        public InhabilitacionChoferDto ObtenerInhabilitacionChofer(int id)
        {
            return Obtener<InhabilitacionChofer, InhabilitacionChoferDto>(id);
        }

        public bool ChoferInhabilitado(int choferId, int centroId)
        {
            Expression<Func<InhabilitacionChofer, bool>> expresionFiltro =
                x =>
                (x.Chofer.Id == choferId && x.FechaDesde <= DateTime.Now &&
                 DateTime.Now <= x.FechaHasta);
            return !repositorio.Existe<Chofer>(x => x.Id == choferId) ||
                   repositorio.Existe(expresionFiltro);
        }

        public IEnumerable<InhabilitacionChoferDto> ListarInhabilitacionChofer(int choferId, int centroId)
        {
            Expression<Func<InhabilitacionChofer, bool>> expresionFiltro =
                x =>
                (x.Chofer.Id == choferId && x.FechaDesde <= DateTime.Now &&
                 DateTime.Now <= x.FechaHasta);
            return Listar<InhabilitacionChofer, InhabilitacionChoferDto>(expresionFiltro);
        }

        public ListaPaginada<InhabilitacionChoferDto> ListarInhabilitacionChoferPaginada(int choferId, int centroId,
                                                                                         Paginacion paginacion)
        {
            Expression<Func<InhabilitacionChofer, bool>> expresionFiltro =
                x =>
                (x.Chofer.Id == choferId && x.FechaDesde <= DateTime.Now &&
                 DateTime.Now <= x.FechaHasta);
            return Listar<InhabilitacionChofer, InhabilitacionChoferDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<TipoComercialDto> ListarPaginadoTiposComerciales(string filtro, Paginacion paginacion)
        {
            Expression<Func<TipoComercial, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro = x => x.Descripcion.Contains(filtro) || x.CodigoSap.Contains(filtro);
            }

            return Listar<TipoComercial, TipoComercialDto>(expresionFiltro, paginacion);
        }

        public IList<TipoComercialDto> ListarTiposComerciales()
        {
            return Listar<TipoComercial, TipoComercialDto>();
        }

        public IList<TipoComercialDto> ListarTiposComercialesPorCentro(int centroId)
        {
            return Listar<TipoComercial, TipoComercialDto>(f => f.WorkflowsAsociados.Any(w => w.Centro.Id == centroId));
        }

        public IList<TipoComercialDto> ListarTiposComercialesPorWfCodigo(string workflowCodigo)
        {
            return
                Listar<TipoComercial, TipoComercialDto>(f => f.WorkflowsAsociados.Any(a => a.Codigo == workflowCodigo));
        }

        public IList<ExportacionDeArchivosSelectObjDto> ListarTiposComercialesConFiltro(int cantResultados, int pagina)
        {
            return repositorio.ListarConsulta(new ListarTipoComercialConFiltroYCantidadMaxima(cantResultados, pagina));
        }

        public IList<ExportacionDeArchivosSelectObjDto> ListarMaterialesConFiltro(int cantResultados, int pagina)
        {
            return repositorio.ListarConsulta(new ListarMaterialConFiltroYCantidadMaxima(cantResultados, pagina));
        }

        public IList<ExportacionDeArchivosSelectObjDto> ListarCentrosConFiltro(int cantResultados, int pagina)
        {
            return repositorio.ListarConsulta(new ListarCentroConFiltroYCantidadMaxima(cantResultados, pagina));
        }

        public ListaPaginada<TipoComercialPorWfDto> ListarTiposComercialesPorWf(string filtro, int centroId,
                                                                                Paginacion paginacion)
        {
            return repositorio.ListarConsultaPaginada(new TipoComercialPorWfConsulta(filtro, centroId, paginacion));
        }

        public TipoComercialDto ObtenerTipoComercial(int id)
        {
            return Obtener<TipoComercial, TipoComercialDto>(id);
        }

        public IList<TipoComprobanteOnccaDto> ListarTiposComprobantesOncca()
        {
            return Listar<TipoComprobanteOncca, TipoComprobanteOnccaDto>();
        }

        public IList<WorkflowDto> ListarWorkflowsPorCentro(int centroId)
        {
            return Listar<Workflow, WorkflowDto>(x => x.Centro.Id == centroId);
        }

        public IList<WorkflowDto> ListarWorkflowsCodigoPorCentro(int centroId)
        {
            return repositorio.Listar<Workflow, WorkflowDto>(
                x => new WorkflowDto { Codigo = x.Codigo, Descripcion = x.Descripcion },
                x => x.Centro.Id == centroId && x.Activo);
        }

        public WorkflowDto ObtenerWorkflow(int id)
        {
            return Obtener<Workflow, WorkflowDto>(id);
        }

        public WorkflowDto ObtenerWorkflowPorCodigo(string codigo)
        {
            return Obtener<Workflow, WorkflowDto>(f => f.Codigo == codigo);
        }

        public int ObtenerUltimaWorkflowDefinicionPorCordigo(string codigo)
        {
            return repositorio.ObtenerMayor<WorkflowDefinicion, int, int>(
                x => x.Workflow.Codigo == codigo && x.Activa && x.FechaActivacion <= DateTime.Now,
                x => x.Id, x => x.Id);
        }

        public WorkflowDefinicionDto ObtenerWorkflowDefinicion(int id)
        {
            return Obtener<WorkflowDefinicion, WorkflowDefinicionDto>(id);
        }

        public bool WorkflowActivoConDefinicionActiva(string codigoWorkflow)
        {
            return repositorio.Existe<WorkflowDefinicion>(
                wfd => wfd.Activa && wfd.FechaActivacion <= DateTime.Now
                       && wfd.Workflow.Codigo == codigoWorkflow
                       && wfd.Workflow.Activo);
        }

        public TipoDocumentoIngreso ObtenerTipoDocumentoIngresoPorRecorrido(int id)
        {
            return repositorio.ObtenerProyeccion<Recorrido, TipoDocumentoIngreso>(r => r.Id == id, x => x.TipoDocumentoIngreso);
        }

        public RecorridoDto ObtenerRecorridoPorGuid(Guid guid)
        {
            return Obtener<Recorrido, RecorridoDto>(r => r.InstanciaWorkflow == guid);
        }

        public TipoDeWorkflow ObtenerTipoDeWorkflowPorGuid(Guid guid)
        {
            return repositorio.ObtenerProyeccion<Recorrido, TipoDeWorkflow>(x => x.InstanciaWorkflow == guid, x => x.Workflow.TipoDeWorkflow);
        }

        public DatosDeInstanciaDto ObtenerDatosDeInstanciaPorGuid(Guid guid)
        {
            return repositorio.ObtenerProyeccion<Recorrido, DatosDeInstanciaDto>(x => x.InstanciaWorkflow == guid,
                                                                                 x => new DatosDeInstanciaDto
                                                                                 {
                                                                                     Id = x.Id,
                                                                                     WorkflowCodigo =
                                                                                             x.Workflow.Codigo,
                                                                                     WorkflowDefinicionId =
                                                                                             x.WorkflowDefinicion.Id,
                                                                                     WorkflowId = x.Workflow.Id,
                                                                                     DatosProximaActividad = x.DatosProximaActividad,
                                                                                     TipoVehiculo = x.TipoVehiculo
                                                                                 });
        }

        public DatosDeInstanciaAltaCTGDto ObtenerDatosDeInstanciaAltaCTGPorGuid(Guid guid)
        {
            return repositorio.ObtenerProyeccion<Recorrido, DatosDeInstanciaAltaCTGDto>(
                x => x.InstanciaWorkflow == guid, x => new DatosDeInstanciaAltaCTGDto
                {
                    Id = x.Id,
                    WorkflowCodigo = x.Workflow.Codigo,
                    WorkflowDefinicionId = x.WorkflowDefinicion.Id,
                    SolicitaConfirmarCTG = x.Centro.SolicitaConfirmarCTG
                });
        }

        public string ObtenerPatentePorGuid(Guid guid)
        {
            return repositorio.ObtenerProyeccion<Recorrido, string>(x => x.InstanciaWorkflow == guid, x => x.Patente);
        }

        public string ObtenerNumeroCartaPortePorGuid(Guid guid)
        {
            return repositorio.ObtenerProyeccion<Recorrido, string>(x => x.InstanciaWorkflow == guid, x => x.NumeroDocumentoIngreso);
        }

        public ValoresSapDto ObtenerRecorridoValoresSapPorGuid(Guid guid)
        {
            return repositorio.ObtenerProyeccion<Recorrido, ValoresSapDto>(x => x.InstanciaWorkflow == guid,
                                                                           x => new ValoresSapDto
                                                                           {
                                                                               NumeroDeDocumentoSap =
                                                                                       x.NumeroDeDocumentoSap,
                                                                               DocumentoInternoSap =
                                                                                       x.DocumentoInternoSap
                                                                           });
        }

        public List<string> ObtenerUsuarioRechazoPorWorkflowInstance(Guid workflowInsance)
        {
            List<string> usuarioDto = new List<string>();

            var controlrecorrido = repositorio.ObtenerMayor<ControlRecorrido, int>(x => x.WorkflowInstanceId == workflowInsance && x.NombreUsuario != null, x => x.Id);
            var usuario = repositorio.ObtenerPrimero<Usuario>(x => x.NombreUsuario == controlrecorrido.NombreUsuario);

            if (!string.IsNullOrEmpty(usuario.Nombre) && !string.IsNullOrEmpty(usuario.Apellido))
            {
                usuarioDto.Add(usuario.Nombre + " " + usuario.Apellido);
            }
            else
            {
                usuarioDto.Add("Sin Datos");
            }

            if (!string.IsNullOrEmpty(usuario.NombreUsuario))
            {
                usuarioDto.Add(usuario.NombreUsuario);
            }
            else
            {
                usuarioDto.Add("Sin Datos");
            }

            return usuarioDto;
        }

        public int ObtenerRecorridoIdPorGuid(Guid guid)
        {
            return repositorio.ObtenerProyeccion<Recorrido, int>(r => r.InstanciaWorkflow == guid, x => x.Id);
        }

        public int ObtenerPesoMaximo(TipoVehiculo tipoVehiculo, int centroId)
        {
            var pesoMaximoPorTipoVechiculo = repositorio.Obtener<PesoMaximoPorTipoVehiculo>(x => x.TipoVehiculo == tipoVehiculo && x.Centro.Id == centroId);
            return pesoMaximoPorTipoVechiculo != null ? pesoMaximoPorTipoVechiculo.PesoMaxIngreso : 0;
        }

        public ListaPaginada<InhabilitacionCamionDto> ListarInhabilitacionCamiones(string filtro, int centroId,
                                                                                   Paginacion paginacion)
        {
            DateTime filtroDate;
            if (!DateTime.TryParse(filtro, out filtroDate))
            {
                filtroDate = DateTime.MaxValue;
            }

            Expression<Func<InhabilitacionCamion, bool>> expresionFiltro = x => true;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro = x => (x.Patente.Contains(filtro)
                                        || x.Motivo.Contains(filtro)
                                        || x.FechaDesde == filtroDate
                                        || x.FechaHasta == filtroDate
                                        || x.NombreUsuarioResponsable.Contains(filtro));
            }

            return Listar<InhabilitacionCamion, InhabilitacionCamionDto>(expresionFiltro, paginacion);
        }

        public InhabilitacionCamionDto ObtenerInhabilitacionCamion(int id)
        {
            return Obtener<InhabilitacionCamion, InhabilitacionCamionDto>(id);
        }

        public bool CamionInhabilitado(string patente, int centroId)
        {
            if (patente == "") return false;

            Expression<Func<InhabilitacionCamion, bool>> expresionFiltro =
                x =>
                (x.Patente.Contains(patente) && x.FechaDesde <= DateTime.Now &&
                 DateTime.Now <= x.FechaHasta);
            return repositorio.Existe(expresionFiltro);
        }

        public IEnumerable<InhabilitacionCamionDto> ListarInhabilitacionCamion(string patente, int centroId)
        {
            Expression<Func<InhabilitacionCamion, bool>> expresionFiltro =
                x =>
                (x.Patente.Contains(patente) && x.FechaDesde <= DateTime.Now &&
                 DateTime.Now <= x.FechaHasta);
            return Listar<InhabilitacionCamion, InhabilitacionCamionDto>(expresionFiltro);
        }

        public ListaPaginada<InhabilitacionCamionDto> ListarInhabilitacionCamionPaginada(string patente, string patenteAcoplado, int centroId,
                                                                                         Paginacion paginacion)
        {
            Expression<Func<InhabilitacionCamion, bool>> expresionFiltro =
                x =>
                ((x.Patente == patente || x.Patente == patenteAcoplado) && x.FechaDesde <= DateTime.Now &&
                 DateTime.Now <= x.FechaHasta);
            return Listar<InhabilitacionCamion, InhabilitacionCamionDto>(expresionFiltro, paginacion);
        }

        public IList<PrecintoDto> ListarPrecintos(Guid instanceId)
        {
            Expression<Func<Precinto, bool>> expresionFiltro = x => x.WorkflowInstanceId == instanceId;

            var precintos = repositorio.Listar(expresionFiltro);
            return conversor.ConvertirList<Precinto, PrecintoDto>(precintos);
        }

        public IList<NotificacionDto> ListarNotificacionesPorGrupo(string grupo, int cantidad)
        {
            var lista = Listar<Notificacion, NotificacionDto>(x => x.Grupo == grupo, cantidad);
            foreach (var t in lista)
            {
                t.HoraServidor = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            }
            return lista;
        }

        public ObservacionDto ObtenerObservacion(Guid instanceId)
        {
            Expression<Func<Observacion, bool>> expresionFiltro = x => x.WorkflowInstanceId == instanceId;

            var observacion = repositorio.Obtener(expresionFiltro);
            return conversor.Convertir<Observacion, ObservacionDto>(observacion);
        }

        public RecorridoDto ObtenerRecorrido(int id)
        {
            return Obtener<Recorrido, RecorridoDto>(id);
        }

        public IList<RecorridoDto> ObtenerRecorridoPorNumeroDocumento(string numero)
        {
            return Listar<Recorrido, RecorridoDto>(x => x.NumeroDocumentoIngreso == numero);
        }

        public IList<RecorridoDto> ObtenerRecorridoNoRechazadoPorNumeroDocumento(string numero)
        {
            return Listar<Recorrido, RecorridoDto>(x => x.NumeroDocumentoIngreso == numero && !x.Rechazado);
        }

        public RecorridoDto ObtenerRecorridoPorNumeroDocumentoSap(string numero)
        {
            return ObtenerPrimero<Recorrido, RecorridoDto>(x => x.NumeroDeDocumentoSap.Equals(numero));
        }

        public RecorridoDto ObtenerRecorridoPorNumeroCiu(string numero)
        {
            return ObtenerPrimero<Recorrido, RecorridoDto>(x => x.NumeroCiu.Equals(numero));
        }

        public Guid ObtenerRecorridoGuidPorNumeroCiu(string numero)
        {
            var recorridos = repositorio.Listar<Recorrido, Guid>(x => x.InstanciaWorkflow,
                                                                 x => x.NumeroCiu.Equals(numero) && !x.Rechazado);
            return recorridos.FirstOrDefault();
        }

        public Guid ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(string numero, int centroId)
        {
            var recorrido =
                repositorio.Listar<Recorrido>(
                    x => x.TarjetaDeAcceso == numero && x.Terminado == false && x.Centro.Id == centroId).LastOrDefault();
            return recorrido != null ? recorrido.InstanciaWorkflow : new Guid();
        }

        public IList<RecorridoDto> ListarRecorridoPorNumeroDocumentoYWorkflow(string numero, int workflowDefinicion)
        {
            return
                Listar<Recorrido, RecorridoDto>(
                    x =>
                    x.NumeroDocumentoIngreso == numero && x.WorkflowDefinicion.Id == workflowDefinicion && !x.Terminado)
                as List<RecorridoDto>;
        }

        public OrdenCargaInternaDto ObtenerOrdenCargaInterna(int id)
        {
            return Obtener<OrdenCargaInterna, OrdenCargaInternaDto>(id);
        }

        public EmbarqueDto ObtenerEmbarque(int id)
        {
            return Obtener<Embarque, EmbarqueDto>(id);
        }

        public OrdenCargaInternaFasonDto ObtenerOrdenCargaInternaFason(int id)
        {
            return Obtener<OrdenCargaInternaFason, OrdenCargaInternaFasonDto>(id);
        }

        public OrdenCargaInternaFasonDto ObtenerOrdenCargaInternaFasonPorInstanceId(Guid id)
        {
            var orden = Obtener<OrdenCargaInternaFason, OrdenCargaInternaFasonDto>(x => x.Recorrido.InstanciaWorkflow == id);
            var cartaPorteDerivadoGranario = repositorio.Obtener<CartaPorteDerivadoGranario>(x => x.Recorrido.InstanciaWorkflow == id);
            if (cartaPorteDerivadoGranario != null)
            {
                orden.NumeroCTG = cartaPorteDerivadoGranario.NroCTG;
                orden.NumeroCPE = string.Concat(cartaPorteDerivadoGranario.Sucursal, "-", cartaPorteDerivadoGranario.NroOrden);
            }
            return orden;
        }

        public OrdenCargaInternaFasonDto ObtenerOrdenCargaInternaFasonPorNumero(string numero)
        {
            return Obtener<OrdenCargaInternaFason, OrdenCargaInternaFasonDto>(x => x.NumeroOrden == numero);
        }

        public OrdenCargaInternaDto ObtenerOrdenCargaInternaPorInstanceId(Guid id)
        {
            var orden = Obtener<OrdenCargaInterna, OrdenCargaInternaDto>(x => x.Recorrido.InstanciaWorkflow == id);
            var cartaPorteDerivadoGranario = repositorio.Obtener<CartaPorteDerivadoGranario>(x => x.Recorrido.InstanciaWorkflow == id);
            if (cartaPorteDerivadoGranario != null)
            {
                orden.NumeroCTG = cartaPorteDerivadoGranario.NroCTG;
                orden.NumeroCPE = string.Concat(cartaPorteDerivadoGranario.Sucursal, "-", cartaPorteDerivadoGranario.NroOrden);
            }
            return orden;
        }

        public ClienteDto ObtenerClientePorInstanceId(Guid id, TipoDocumentoIngreso tipo)
        {
            Cliente cliente = null;

            switch (tipo)
            {
                case TipoDocumentoIngreso.OrdenCargaFas:
                    cliente = repositorio.ObtenerProyeccion<OrdenCargaFas, Cliente>(x => x.Recorrido.InstanciaWorkflow == id, x => x.Cliente);
                    break;

                case TipoDocumentoIngreso.OrdenCargaInterna:
                    cliente =
                        repositorio.ObtenerProyeccion<OrdenCargaInterna, Cliente>(
                            x => x.Recorrido.InstanciaWorkflow == id, x => x.Destino);
                    break;

                case TipoDocumentoIngreso.OrdenCargaInternaFason:
                    cliente =
                        repositorio.ObtenerProyeccion<OrdenCargaInternaFason, Cliente>(
                            x => x.Recorrido.InstanciaWorkflow == id, x => x.Cliente);
                    break;

                case TipoDocumentoIngreso.OrdenDeDescargaFason:
                    cliente =
                        repositorio.ObtenerProyeccion<OrdenDeDescargaFason, Cliente>(
                            x => x.Recorrido.InstanciaWorkflow == id, x => x.Cliente);
                    break;

                case TipoDocumentoIngreso.OrdenDeCargaContenedor:
                    cliente =
                        repositorio.ObtenerProyeccion<OrdenDeCargaContenedor, Cliente>(
                            x => x.Recorrido.InstanciaWorkflow == id, x => x.Destino);
                    break;

                case TipoDocumentoIngreso.CartaPorte:
                    var cartaPorte = repositorio.ObtenerProyeccion((Recorrido x) => x.InstanciaWorkflow == id, x => new { Centro = x.Vehiculo.CartaPorte.CentroDestino, Cliente = x.Vehiculo.CartaPorte.ClienteDestino });
                    if (cartaPorte.Centro != null)
                    {
                        cliente = new Cliente
                        {
                            Descripcion = cartaPorte.Centro.Descripcion,
                            CodigoSap = cartaPorte.Centro.CodigoSAP
                        };
                    }
                    else if (cartaPorte.Cliente != null)
                    {
                        cliente = cartaPorte.Cliente;
                    }
                    break;
            }

            return cliente != null ? conversor.Convertir<Cliente, ClienteDto>(cliente) : null;
        }

        public ListaPaginada<BalanzaDto> ListarPaginadoBalanza(string filtro, int centroId, Paginacion paginacion)
        {
            Expression<Func<Balanza, bool>> expresionFiltro;
            if (!string.IsNullOrEmpty(filtro))
            {
                int filtroInt;
                if (!int.TryParse(filtro, out filtroInt))
                {
                    filtroInt = int.MaxValue;
                }

                expresionFiltro = (x => ((x.Id == filtroInt)
                                         || (x.Nombre.Contains(filtro))
                                         && (x.Centro.Id == centroId)));
            }
            else
            {
                expresionFiltro = x => x.Centro.Id == centroId;
            }

            return Listar<Balanza, BalanzaDto>(expresionFiltro, paginacion);
        }

        public IList<LocalidadDto> BuscarProcedencias(string criteria)
        {
            return
                Listar<Localidad, LocalidadDto>(
                    f => f.Descripcion.Contains(criteria) || f.CodigoAfip.Contains(criteria), 30);
        }

        public LocalidadDto BuscarProcedencia(string criteria)
        {
            Localidad entidad;
            try
            {
                entidad =
                    repositorio.Obtener<Localidad>(
                        f => f.Descripcion.Contains(criteria) || f.CodigoAfip.Contains(criteria));
            }
            catch
            {
                entidad = null;
            }
            return entidad == null ? null : conversor.Convertir<Localidad, LocalidadDto>(entidad);
        }

        public ProvinciaDto ObtenerProvincia(int id)
        {
            return Obtener<Provincia, ProvinciaDto>(id);
        }

        public List<ProveedorInfoDto> BuscarProveedoresVinedosTerceros(string criteria)
        {
            List<int> listaProveedoresVinedosId = repositorio.Listar<VinedoTerceros, int>(x => x.Proveedor.Id, x => x.Proveedor != null).ToList();

            return
                repositorio.Listar<Proveedor, ProveedorInfoDto>(
                    x => new ProveedorInfoDto { Id = x.Id, Cuil = x.Cuil, Descripcion = x.Descripcion },
                    f =>
                    f.Activo && (f.Descripcion.Contains(criteria) || f.Cuil.Contains(criteria)) &&
                     listaProveedoresVinedosId.Contains(f.Id), 20);
        }

        public IList<VinedoDto> BuscarVinedosPropios(string criteria)
        {
            return Listar<VinedoPropio, VinedoDto>(f => f.NumeroINV.Contains(criteria) || f.Descripcion.Contains(criteria), 20);
        }

        public ProveedorInfoDto BuscarProveedorVinedoTercero(string criteria)
        {
            List<int> listaProveedoresVinedosId = repositorio.Listar<VinedoTerceros, int>(x => x.Proveedor.Id, x => x.Proveedor != null).ToList();
            ProveedorInfoDto proveedor = null;
            try
            {
                proveedor = repositorio.ObtenerProyeccion<Proveedor, ProveedorInfoDto>(
                    f => f.Activo && (f.Descripcion.Contains(criteria) || f.Cuil.Contains(criteria)) && listaProveedoresVinedosId.Contains(f.Id),
                    x => new ProveedorInfoDto
                    {
                        Id = x.Id,
                        Cuil = x.Cuil,
                        Descripcion = x.Descripcion
                    });
            }
            catch
            {
                proveedor = null;
            }
            return proveedor;
        }

        public VinedoDto BuscarVinedoPropio(string criteria)
        {
            VinedoPropio vinedo;
            try
            {
                vinedo = repositorio.Obtener<VinedoPropio>(f => f.NumeroINV.Contains(criteria) || f.Descripcion.Contains(criteria));
            }
            catch
            {
                vinedo = null;
            }
            return vinedo == null ? null : conversor.Convertir<VinedoPropio, VinedoDto>(vinedo);
        }

        public IList<ProveedorInfoDto> BuscarProveedores(string criteria, TiposProveedor tipo)
        {
            return repositorio.Listar<Proveedor, ProveedorInfoDto>(x => new ProveedorInfoDto
            {
                Id = x.Id,
                Cuil = x.Cuil,
                Descripcion = x.Descripcion
            },
                                                                   f => f.Activo
                                                                        && (!tipo.PR || f.PR) && (!tipo.AM || f.AM) &&
                                                                        (!tipo.CM || f.CM)
                                                                        &&
                                                                        (f.Descripcion.Contains(criteria) ||
                                                                        f.Cuil.Contains(criteria) ||
                                                                         f.Cuil.Replace("-", "").Contains(criteria)), 20);
        }

        public ProveedorInfoDto BuscarProveedor(string criteria, TiposProveedor tipo)
        {
            ProveedorInfoDto proveedor;
            try
            {
                proveedor = repositorio.ObtenerProyeccion<Proveedor, ProveedorInfoDto>(
                    f => f.Activo
                         && (!tipo.PR || f.PR) && (!tipo.AM || f.AM) && (!tipo.CM || f.CM)
                         && (f.Descripcion.Contains(criteria) || f.Cuil.Contains(criteria)),
                    x => new ProveedorInfoDto
                    {
                        Id = x.Id,
                        Cuil = x.Cuil,
                        Descripcion = x.Descripcion
                    });
            }
            catch
            {
                proveedor = null;
            }
            return proveedor;
        }

        public IList<ClienteDto> BuscarClientes(string criteria)
        {
            return
                Listar<Cliente, ClienteDto>(
                    f => f.Activo && (f.Descripcion.Contains(criteria) || f.Cuit.Contains(criteria)), 20);
        }

        public ClienteDto BuscarCliente(string criteria)
        {
            Cliente entidad;
            try
            {
                entidad =
                    repositorio.Obtener<Cliente>(
                        f => f.Activo && (f.Descripcion.Contains(criteria) || f.Cuit.Contains(criteria)));
            }
            catch
            {
                entidad = null;
            }
            return entidad == null ? null : conversor.Convertir<Cliente, ClienteDto>(entidad);
        }

        public IList<ClienteDto> BuscarClientesPorCuit(string cuit)
        {
            return Listar<Cliente, ClienteDto>(f => f.Activo && f.Cuit == cuit, 20);
        }

        public int ObtenerCantidadCalados(Guid instanceId)
        {
            return repositorio.Contar<Calado>(x => x.WorkflowInstanceId == instanceId);
        }

        public CaladoDto ObtenerCaladoAnterior(Guid instanceId)
        {
            return ObtenerPrimero<Calado, CaladoDto>(c => c.WorkflowInstanceId == instanceId && c.NumeroOrden != "");
        }

        public CaladoDto ObtenerCaladoPorGuid(Guid instanceId)
        {
            var calado = repositorio.ObtenerProyeccion<Recorrido, Calado>(x => x.InstanciaWorkflow == instanceId,
                                                                          x => x.Calado);
            return calado != null ? conversor.Convertir<Calado, CaladoDto>(calado) : null;
        }

        public ListaPaginada<CaracteristicaDeCalidadDto> ListarCaracteristicasDeCalidadPaginado(Paginacion paginacion,
                                                                                                string term,
                                                                                                int centroId)
        {
            Expression<Func<CaracteristicaDeCalidad, bool>> expresionFiltro =
                x =>
                (x.DescripcionCorta.Contains(term) || x.MaterialPorCentro.Material.Descripcion.Contains(term) || String.IsNullOrEmpty(term)) &&
                x.MaterialPorCentro.Material.Activo && x.MaterialPorCentro.Centro.Id == centroId;

            return Listar<CaracteristicaDeCalidad, CaracteristicaDeCalidadDto>(expresionFiltro, paginacion);
        }

        public CaracteristicaDeCalidadDto ObtenerCaracteristicaDeCalidad(int id)
        {
            return Obtener<CaracteristicaDeCalidad, CaracteristicaDeCalidadDto>(id);
        }

        public ListaPaginada<CaracteristicaDeCalidadDto> ListarCaracteristicasDeCalidadPorMaterialPaginado(
            int materialId, int centroId, Paginacion paginacion)
        {
            return
                Listar<CaracteristicaDeCalidad, CaracteristicaDeCalidadDto>(
                    x => x.MaterialPorCentro.Material.Id == materialId && x.MaterialPorCentro.Centro.Id == centroId,
                    paginacion);
        }

        public IList<CaracteristicaDeCalidadDto> ListarCaracteristicasDeCalidadPorMaterial(int materialId, int centroId)
        {
            return
                Listar<CaracteristicaDeCalidad, CaracteristicaDeCalidadDto>(
                    x => x.MaterialPorCentro.Material.Id == materialId && x.MaterialPorCentro.Centro.Id == centroId && !x.NoObservableEnCalado);
        }

        public IList<CaracteristicaDeCalidadDto> ListarCaracteristicasDeCalidadPorMaterialWorkflow(int materialId, int centroId, int workflowId)
        {
            var caracteristicas = repositorio.Listar<CaracteristicaDeCalidadPorWorkflow, CaracteristicaDeCalidad>(
                x => x.CaracteristicaDeCalidad,
                x => x.CaracteristicaDeCalidad.MaterialPorCentro.Material.Id == materialId && x.CaracteristicaDeCalidad.MaterialPorCentro.Centro.Id == centroId &&
                     !x.CaracteristicaDeCalidad.NoObservableEnCalado && x.Workflow.Id == workflowId);
            return conversor.ConvertirList<CaracteristicaDeCalidad, CaracteristicaDeCalidadDto>(caracteristicas);
        }

        public IList<CaracteristicaDeCalidadPorWorkflowDto> ListarCaracteristicasDeCalidadPorWorkflow(int materialId, int centroId, int workflowId)
        {
            return Listar<CaracteristicaDeCalidadPorWorkflow, CaracteristicaDeCalidadPorWorkflowDto>(
                x => x.CaracteristicaDeCalidad.MaterialPorCentro.Material.Id == materialId && x.CaracteristicaDeCalidad.MaterialPorCentro.Centro.Id == centroId &&
                     !x.CaracteristicaDeCalidad.NoObservableEnCalado && x.Workflow.Id == workflowId);
        }

        public IList<CaracteristicaDeCalidadDto> ListarCaracteristicasDeCalidadPorMaterialSinHumedad(int materialId,
                                                                                                     int centroId)
        {
            return
                Listar<CaracteristicaDeCalidad, CaracteristicaDeCalidadDto>(
                    x =>
                    x.MaterialPorCentro.Material.Id == materialId && !x.EsHumedad &&
                    x.MaterialPorCentro.Centro.Id == centroId);
        }

        public IList<CaracteristicaDeCalidadDto> ListarCaracteristicasDeCalidadObligatorias(int materialId, int centroId)
        {
            return
                Listar<CaracteristicaDeCalidad, CaracteristicaDeCalidadDto>(
                    q =>
                    q.MaterialPorCentro.Material.Id == materialId && q.Obligatorio &&
                    q.MaterialPorCentro.Centro.Id == centroId);
        }

        public CaracteristicaDeCalidadDto ObtenerCaracteristicaDeCalidadHumedad(int materialId, int centroId)
        {
            return
                Obtener<CaracteristicaDeCalidad, CaracteristicaDeCalidadDto>(
                    q =>
                    q.MaterialPorCentro.Material.Id == materialId && q.EsHumedad &&
                    q.MaterialPorCentro.Centro.Id == centroId);
        }

        public HumedimetroDto ObtenerHumedimetro(int id)
        {
            return Obtener<Humedimetro, HumedimetroDto>(id);
        }

        public IList<HumedimetroDto> ListarHumedimetros()
        {
            return Listar<Humedimetro, HumedimetroDto>();
        }

        public IList<HumedimetroDto> ListarHumedimetrosPorCentro(int centroId)
        {
            return Listar<Humedimetro, HumedimetroDto>(x => x.Centro.Id == centroId);
        }

        public ListaPaginada<HumedimetroDto> ListarPaginadoHumedimetros(string filtro, int centroId,
                                                                        Paginacion paginacion)
        {
            Expression<Func<Humedimetro, bool>> expresionFiltro;
            if (!string.IsNullOrEmpty(filtro))
            {
                int filtroInt;
                if (!int.TryParse(filtro, out filtroInt))
                {
                    filtroInt = int.MaxValue;
                }

                expresionFiltro = (x => ((x.Id == filtroInt)
                                         || ((x.Descripcion.Contains(filtro))
                                         || (x.DescripcionCorta.Contains(filtro)))
                                         && (x.Centro.Id == centroId)));
            }
            else
            {
                expresionFiltro = x => x.Centro.Id == centroId;
            }

            return Listar<Humedimetro, HumedimetroDto>(expresionFiltro, paginacion);
        }

        public MotivoDto ObtenerMotivo(int id)
        {
            return Obtener<Motivo, MotivoDto>(id);
        }

        public ListaPaginada<MotivoDto> ListarPaginadoMotivos(Paginacion paginacion)
        {
            return Listar<Motivo, MotivoDto>(null, paginacion);
        }

        public IList<MotivoDto> ListarMotivos()
        {
            var precintos = repositorio.Listar<Motivo>();
            return conversor.ConvertirList<Motivo, MotivoDto>(precintos);
        }

        public IList<MotivoReasignacionDeTarjetaDto> ListarMotivosReasignacionDeTarjeta()
        {
            var precintos = repositorio.Listar<MotivoReasignacionDeTarjeta>();
            return conversor.ConvertirList<MotivoReasignacionDeTarjeta, MotivoReasignacionDeTarjetaDto>(precintos);
        }

        public ListaPaginada<BocaDestinoDto> ListarPaginadoBocasDestino(string filtro, Paginacion paginacion)
        {
            Expression<Func<BocaDestino, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = x => x.Proveedor.Activo && (x.Proveedor.Descripcion.Contains(filtro)
                                                              || x.NombreBocaDeDestino.Contains(filtro)
                                                              || x.Localidad.Descripcion.Contains(filtro)
                                                              || x.Provincia.Descripcion.Contains(filtro)
                                                              || x.Proveedor.Cuil.Contains(filtro));
            }
            return Listar<BocaDestino, BocaDestinoDto>(expresionFiltro, paginacion);
        }

        public IList<BocaDestinoDto> ListarBocasDestino()
        {
            return Listar<BocaDestino, BocaDestinoDto>(null, 50);
        }

        public IList<ProveedorDto> ListarProveedoresConBocaDestino(string filtro)
        {
            return
                Listar<Proveedor, ProveedorDto>(
                    x =>
                    x.Activo && (x.Descripcion.Contains(filtro) || x.Cuil.Contains(filtro)) && x.BocasDestino.Any() &&
                    x.Activo, 20);
        }

        public ProveedorDto ObtenerProveedorConBocaDestino(string filtro)
        {
            Proveedor entidad;
            try
            {
                entidad =
                    repositorio.Obtener<Proveedor>(
                        f =>
                        f.Activo && (f.Descripcion.Contains(filtro) || f.Cuil.Contains(filtro)) && f.BocasDestino.Any());
            }
            catch
            {
                entidad = null;
            }
            return entidad == null ? null : conversor.Convertir<Proveedor, ProveedorDto>(entidad);
        }

        public BocaDestinoDto ObtenerBocaDestino(int id)
        {
            return Obtener<BocaDestino, BocaDestinoDto>(id);
        }

        public IList<CaladoPorCaracteristicaDto> ListarCaladoPorCaracteristicas(int caladoId)
        {
            Expression<Func<CaladoPorCaracteristica, bool>> expresionFiltro = x => x.Calado.Id == caladoId;
            return Listar<CaladoPorCaracteristica, CaladoPorCaracteristicaDto>(expresionFiltro);
        }

        public IList<CalleDto> ObtenerCallesDeCallesPorRecorridoSegunMaterial(int materialId, int calleId, TipoCalidad calidadCamion, bool esSojaEPA, bool esSojaIMPO)
        {
            Expression<Func<CallePorRecorrido, bool>> expresionFiltro = x => (esSojaIMPO ? x.CargaDeCupo.Material.Id == materialId : x.Recorrido.Material.Id == materialId)
                && x.Calle.Id != calleId
                && x.Calle.TipoCalle == TipoCalle.PostCalado
                && !x.Calle.Deshabilitada
                && x.Calle.TipoCalidad != TipoCalidad.Otros
                && (esSojaIMPO || x.Recorrido.CaracteristicasAnalizadasList.FirstOrDefault().Calidad == calidadCamion)
                && x.FechaEgreso == null
                && (esSojaEPA ? x.Recorrido.Establecimiento.EPA : esSojaIMPO ? x.CargaDeCupo.TitularCartaPorteCodigoSap == Constantes.ValoresPorDefecto.CodigoSapTPR : (x.Recorrido.Establecimiento == null || !x.Recorrido.Establecimiento.EPA));

            var calles = repositorio.Listar<CallePorRecorrido, Calle>(cpr => cpr.Calle, expresionFiltro);
            return conversor.ConvertirList<Calle, CalleDto>(calles.ToList());
        }

        public ListaPaginada<CaladoPorCaracteristicaDto> ListarPaginadoCaladoPorCaracteristica(int caladoId,
                                                                                               Paginacion paginacion)
        {
            var calado = repositorio.Obtener<Calado>(caladoId);
            Expression<Func<CaladoPorCaracteristica, bool>> expresionFiltro =
                x => calado.CaladosPorCaracteristica.Contains(x);

            return Listar<CaladoPorCaracteristica, CaladoPorCaracteristicaDto>(expresionFiltro, paginacion);
        }

        public RolDto ObtenerRol(int id)
        {
            return Obtener<Rol, RolDto>(id);
        }

        public IList<RolDto> ListarRoles()
        {
            return Listar<Rol, RolDto>();
        }

        public ListaPaginada<RolDto> ListarPaginadoRoles(string filtro, Paginacion paginacion)
        {
            Expression<Func<Rol, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = x => x.Descripcion.Contains(filtro);
            }
            return Listar<Rol, RolDto>(expresionFiltro, paginacion);
        }

        public IList<RecorridoDto> ListarRecorridosPorDocumento(string tipoDoc, string numeroDoc)
        {
            var tipo = (TipoDocumentoIngreso)Enum.Parse(typeof(TipoDocumentoIngreso), tipoDoc);
            Expression<Func<Recorrido, bool>> expresionFiltro =
                x => x.TipoDocumentoIngreso == tipo && x.NumeroDocumentoIngreso == numeroDoc;
            return Listar<Recorrido, RecorridoDto>(expresionFiltro);
        }

        public DatosRecorridoDto RecorridoPorTarjetaDeAcceso(string tarjetaDeAcceso)
        {
            return repositorio.ObtenerProyeccion<Recorrido, DatosRecorridoDto>(
                x =>
                x.TarjetaDeAcceso == tarjetaDeAcceso && !x.Terminado,
                x => new DatosRecorridoDto
                {
                    InstanciaWorkflow = x.InstanciaWorkflow,
                    NumeroDocumentoIngreso = x.NumeroDocumentoIngreso,
                    Patente = x.Patente,
                    TarjetaDeAcceso = x.TarjetaDeAcceso,
                    WorkflowDefinicionId = x.WorkflowDefinicion.Id,
                    CentroCodigoSap = x.Centro.CodigoSAP,
                    WorkflowId = x.Workflow.Id,
                    AdvertirCaladoEnPlanta = x.CorrespondeCaladoEnPlanta && x.CaladoEnPlanta == null,
                    Id = x.Id,
                    TipoVehiculo = x.TipoVehiculo,
                    CartaDePorte = x.NumeroDocumentoIngreso,
                    Entregador = x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte &&
                    x.Vehiculo.CartaPorte.Entregador != null && x.Vehiculo.CartaPorte.Entregador.RazonSocial.ToUpper() != "SIN ENTREGA",
                    Material = x.Material.Descripcion,
                    PesoBrutoOrigen = x.PesoBrutoOrigen,
                    PesoTaraOrigen = x.PesoTaraOrigen,
                    PesoNetoOrigen = x.PesoBrutoOrigen - x.PesoTaraOrigen,
                    PesoBruto = x.PesoBruto,
                    PesoTara = x.PesoTara,
                    Calle = x.Calle.Nombre,
                    TipoDocumento = x.TipoDocumentoIngreso,
                    TipoComercial = x.TipoComercial.Descripcion
                });
        }

        public RecorridoDto RecorridoSinPesosPorNumeroDeDocumento(TipoDocumentoIngreso tipo, string numeroDoc)
        {
            Expression<Func<Recorrido, bool>> expresionFiltro =
                x => x.TipoDocumentoIngreso == tipo && x.NumeroDocumentoIngreso == numeroDoc && !x.Rechazado;
            var recorridos = Listar<Recorrido, RecorridoDto>(expresionFiltro);
            if (recorridos.Count == 0)
            {
                throw new Exception(Textos.Error_NoResultados);
            }

            if (recorridos.Any(x => x.PesoBruto != null || x.PesoNeto != null || x.PesoTara != null))
            {
                throw new Exception(Textos.Error_NoModificacion);
            }

            var instancias = recorridos.Select(x => x.InstanciaWorkflow);

            var caladosIniciados =
                repositorio.Contar<Calado>(x => instancias.Contains(x.WorkflowInstanceId) && x.FechaCreacion != null);
            if (caladosIniciados > 0)
            {
                throw new Exception(Textos.Error_NoModificacion);
            }

            return recorridos.LastOrDefault();
        }

        public RecorridoDto ObtenerRecorridoPorDocumentoPatenteYCentro(string tipoDoc, string numeroDoc, string patente,
                                                                       int centroId)
        {
            var tipo = (TipoDocumentoIngreso)Enum.Parse(typeof(TipoDocumentoIngreso), tipoDoc);
            Expression<Func<Recorrido, bool>> expresionFiltro =
                x =>
                x.Centro.Id == centroId && x.TipoDocumentoIngreso == tipo && x.NumeroDocumentoIngreso == numeroDoc &&
                (string.IsNullOrEmpty(patente) || x.Patente == patente) && !x.Rechazado;

            // En el caso de los trenes puede haber más de un recorrido que cumpla.
            // En ese caso el controller maneja el error informando que se debe in gresar la patente
            return ObtenerPrimero<Recorrido, RecorridoDto>(expresionFiltro);
        }

        public ListaPaginada<RecorridoDto> ListarPaginadoRecorridosPorDocumentoPatenteYCentro(TipoDocumentoIngreso tipoDoc,
                                                                                              string numeroDoc,
                                                                                              string patente,
                                                                                              string numeroTarjeta,
                                                                                              int centroId,
                                                                                              Paginacion paginacion)
        {
            Expression<Func<Recorrido, bool>> expresionFiltro =
                x =>
                x.Centro.Id == centroId &&
                (string.IsNullOrEmpty(numeroDoc) || (x.NumeroDocumentoIngreso == numeroDoc || x.NumeroDocumentoIngresoLegal == numeroDoc)) &&
                (!string.IsNullOrEmpty(numeroTarjeta) || x.TipoDocumentoIngreso == tipoDoc) &&
                (string.IsNullOrEmpty(patente) || x.Patente == patente) &&
                (string.IsNullOrEmpty(numeroTarjeta) || (x.TarjetaDeAcceso == numeroTarjeta && !x.Terminado));
            return Listar<Recorrido, RecorridoDto>(expresionFiltro, paginacion);
        }

        public IList<RecorridoDto> ListarRecorridosPorDocumentoYPatente(string tipoDoc, string numeroDoc, string patente)
        {
            var tipo = (TipoDocumentoIngreso)Enum.Parse(typeof(TipoDocumentoIngreso), tipoDoc);
            Expression<Func<Recorrido, bool>> expresionFiltro =
                x => x.TipoDocumentoIngreso == tipo && x.NumeroDocumentoIngreso == numeroDoc && x.Patente == patente;
            return Listar<Recorrido, RecorridoDto>(expresionFiltro);
        }

        public PermisoDto ObtenerPermiso(int id)
        {
            return Obtener<Permiso, PermisoDto>(id);
        }

        public IList<PermisoDto> ListarPermisos()
        {
            return Listar<Permiso, PermisoDto>();
        }

        public ListaPaginada<PermisoDto> ListarPaginadoPermisos(string filtro, Paginacion paginacion)
        {
            Expression<Func<Permiso, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = x => x.Descripcion.Contains(filtro);
            }
            return Listar<Permiso, PermisoDto>(expresionFiltro, paginacion);
        }

        public IList<PermisoDto> ListarPermisosPorUsuario(string nombreUsuario)
        {
            return
                conversor.ConvertirList<Permiso, PermisoDto>(
                    repositorio.ListarConsulta(new PermisosPorUsuarioConsulta(nombreUsuario)));
        }

        public IList<string> ListarPermisosDeActividadPorUsuario(string nombreUsuario)
        {
            return repositorio.ListarConsulta(new PermisosDeActividadPorUsuarioConsulta(nombreUsuario));
        }

        public bool UsuarioTienePermisoParaActividad(string nombreUsuario, string actividad)
        {
            return
                repositorio.ObtenerConsultaEscalar(new UsuarioTienePermisoParaActividadConsulta(nombreUsuario, actividad));
        }

        public IList<PermisoDto> ListarPermisosPorActividad(string actividad)
        {
            Expression<Func<Permiso, bool>> expresionFiltro = x => x.ActividadWorkflow == actividad;
            return Listar<Permiso, PermisoDto>(expresionFiltro);
        }

        public TicketAccesoAfipDto ObtenerTicketAccesoAfip()
        {
            return Listar<TicketAccesoAfip, TicketAccesoAfipDto>(null).LastOrDefault();
        }

        public ListaPaginada<ProveedorDto> ListarPaginadoProveedores(string filtro, Paginacion paginacion)
        {
            Expression<Func<Proveedor, bool>> expresionFiltro;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro = x => (x.CodigoSap.Contains(filtro) || x.RazonSocial.Contains(filtro)) && x.Activo;
            }
            else
            {
                expresionFiltro = x => x.Activo;
            }
            return Listar<Proveedor, ProveedorDto>(expresionFiltro, paginacion);
        }

        public UsuarioDto ObtenerUsuario(int id)
        {
            return Obtener<Usuario, UsuarioDto>(id);
        }

        public AnalisisObligatorioDto ObtenerAnalisisObligatorio(int id)
        {
            return Obtener<AnalisisObligatorio, AnalisisObligatorioDto>(id);
        }

        public UsuarioMatriculaDto ObtenerUsuarioMatricula(string usuario)
        {
            return Obtener<Usuario, UsuarioMatriculaDto>(x => x.NombreUsuario.Equals(usuario));
        }

        public UsuarioDto ObtenerUsuarioId(string usuario)
        {
            return Obtener<Usuario, UsuarioDto>(x => x.NombreUsuario.Equals(usuario));
        }

        public IList<UsuarioDto> ListarUsuarios()
        {
            return Listar<Usuario, UsuarioDto>();
        }

        public ListaPaginada<UsuarioDto> ListarPaginadoUsuarios(string filtro, Paginacion paginacion)
        {
            Expression<Func<Usuario, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro =
                    x =>
                    x.Nombre.Contains(filtro) || x.Apellido.Contains(filtro) || x.NombreUsuario.Contains(filtro) ||
                    x.Email.Contains(filtro);
            }
            return Listar<Usuario, UsuarioDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<AnalisisObligatorioDto> ListarPaginadoAnalisisObligatorios(string filtro, Paginacion paginacion)
        {
            Expression<Func<AnalisisObligatorio, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro =
                    x =>
                    x.Material.Descripcion.Contains(filtro);
            }
            return Listar<AnalisisObligatorio, AnalisisObligatorioDto>(expresionFiltro, paginacion);
        }

        public SuplenciaDto ObtenerSuplencia(int id)
        {
            return Obtener<Suplencia, SuplenciaDto>(id);
        }

        public ListaPaginada<SuplenciaDto> ListarPaginadoSuplencias(string filtro, Paginacion paginacion)
        {
            Expression<Func<Suplencia, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                DateTime filtroDate;
                if (!DateTime.TryParse(filtro, out filtroDate))
                {
                    filtroDate = DateTime.MaxValue;
                }
                expresionFiltro = (x => ((x.UsuarioASuplantar.NombreUsuario.Contains(filtro))
                                         || (x.UsuarioSuplente.NombreUsuario.Contains(filtro))
                                         || (x.FechaDesde == filtroDate)
                                         || (x.FechaHasta == filtroDate)));
            }

            return Listar<Suplencia, SuplenciaDto>(expresionFiltro, paginacion);
        }

        public ExcepcionEnvioCamaraDto ObtenerExcepcionEnvioCamara(int id)
        {
            var retorno = Obtener<ExcepcionEnvioCamara, ExcepcionEnvioCamaraDto>(id);

            retorno.CaracteristicasDeCalidadId = repositorio.Listar(x => x.CaracteristicaMaterial.Id, (ExcepcionEnvioCamara x) =>
                                                                        x.Material.Id == retorno.MaterialId
                                                                        && x.TipoComercial.Id == retorno.TipoComercialId
                                                                        && x.Proveedor.Id == retorno.ProveedorId
                                                                        && x.Entregador.Id == retorno.EntregadorId).ToList();

            return retorno;
        }

        public List<ExcepcionEnvioCamaraDto> ListarExcepcionEnvioCamara(int material, int tipoComercial, int proveedor, int entregador)
        {
            return Listar<ExcepcionEnvioCamara, ExcepcionEnvioCamaraDto>(x => x.Material.Id == material
                                                                            && x.TipoComercial.Id == tipoComercial
                                                                            && x.Proveedor.Id == proveedor
                                                                            && x.Entregador.Id == entregador).ToList();
        }

        public DocumentoDeImpresionDto ObtenerImpresiones(int id)
        {
            return Obtener<DocumentoDeImpresion, DocumentoDeImpresionDto>(id);
        }

        public ListaPaginada<DocumentoDeImpresionDto> ListarPaginadoDocumentoDeImpresion(Paginacion paginacion,
                                                                                         string filtro)
        {
            Expression<Func<DocumentoDeImpresion, bool>> expresionFiltro = null;
            if (filtro != null)
            {
                expresionFiltro = (x => x.Descripcion.Contains(filtro));
            }

            return Listar<DocumentoDeImpresion, DocumentoDeImpresionDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<DocumentoDeImpresionPorCentroDto> ListarPaginadoDocumentoDeImpresionPorCentro(
            Paginacion paginacion, string filtro, int centroId)
        {
            return Listar<DocumentoDeImpresionPorCentro, DocumentoDeImpresionPorCentroDto>(x => (String.IsNullOrEmpty(filtro) || (!String.IsNullOrEmpty(filtro) && x.DocumentoDeImpresion.Descripcion.Contains(filtro))) && centroId == x.Centro.Id, paginacion);
        }

        public IList<DocumentoDeImpresionPorCentroDto> ListarDocumentoDeImpresion(int documentoImpresion,
                                                                                  int? formatoImpresion)
        {
            return
                Listar<DocumentoDeImpresionPorCentro, DocumentoDeImpresionPorCentroDto>(
                    f =>
                    f.DocumentoDeImpresion.Id.Equals(documentoImpresion) &&
                    f.FormatoDeImpresion.Id.Equals(formatoImpresion));
        }

        public ListaPaginada<LoteListaDto> ListarPaginadoLote(BuscarLoteDto filtro, Paginacion paginacion)
        {
            Expression<Func<Lote, bool>> expresionFiltro;
            filtro.FechaDesde = filtro.FechaDesde ?? new DateTime(1970, 1, 1);
            filtro.FechaHasta = filtro.FechaHasta ?? DateTime.MaxValue;
            if (filtro.LoteId > 0)
            {
                expresionFiltro = (x => x.Id == filtro.LoteId);
            }
            else if (filtro.NroLote != null)
            {
                expresionFiltro = (x => x.NumeroDeLote == filtro.NroLote);
            }
            else
            {
                expresionFiltro =
                (x =>
                 x.Fecha <= filtro.FechaHasta && x.Fecha >= filtro.FechaDesde &&
                 (x.Camara.Id.Equals(filtro.CamaraId) || filtro.CamaraId == 0) && x.Muestras.Any(q => q.Centro.Id == filtro.CentroId));
            }

            return Listar<Lote, LoteListaDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<LoteBiotecnologiaListaDto> ListarPaginadoBiotecnologiaLote(BuscarLoteBiotecnologiaDto filtro, Paginacion paginacion)
        {
            Expression<Func<LoteBiotecnologia, bool>> expresionFiltro;
            filtro.FechaDesde = filtro.FechaDesde ?? new DateTime(1970, 1, 1);
            filtro.FechaHasta = filtro.FechaHasta ?? DateTime.MaxValue;
            if (filtro.LoteId > 0)
            {
                expresionFiltro = (x => x.Id == filtro.LoteId);
            }
            else if (filtro.NroLote != null)
            {
                expresionFiltro = (x => x.NumeroDeLote == filtro.NroLote);
            }
            else
            {
                expresionFiltro =
                (x =>
                 x.Fecha <= filtro.FechaHasta && x.Fecha >= filtro.FechaDesde &&
                 (x.Camara.Id.Equals(filtro.CamaraId) || filtro.CamaraId == 0)
                 && ((x.Material.Id.Equals(filtro.MaterialId)) || filtro.MaterialId == 0) && x.Centro.Id == filtro.CentroId);
            }

            return Listar<LoteBiotecnologia, LoteBiotecnologiaListaDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<LoteAuditoriaListaDto> ListarPaginadoAuditoriaLote(BuscarLoteAuditoriaDto filtro, Paginacion paginacion)
        {
            Expression<Func<LoteAuditoria, bool>> expresionFiltro;
            filtro.FechaDesde = filtro.FechaDesde ?? new DateTime(1970, 1, 1);
            filtro.FechaHasta = filtro.FechaHasta ?? DateTime.MaxValue;
            if (filtro.LoteId > 0)
            {
                expresionFiltro = (x => x.Id == filtro.LoteId);
            }
            else if (filtro.NroLote != null)
            {
                expresionFiltro = (x => x.NumeroDeLote == filtro.NroLote);
            }
            else
            {
                expresionFiltro =
                (x =>
                 x.Fecha <= filtro.FechaHasta && x.Fecha >= filtro.FechaDesde &&
                 (x.Camara.Id.Equals(filtro.CamaraId) || filtro.CamaraId == 0)
                 && ((x.Material.Id.Equals(filtro.MaterialId)) || filtro.MaterialId == 0) && x.Centro.Id == filtro.CentroId);
            }

            return Listar<LoteAuditoria, LoteAuditoriaListaDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<ArchivoDeMovimientosDto> ListarPaginadoArchivoDeMovimiento(BuscarArchivoDeMovimientoDto filtro, Paginacion paginacion)
        {
            Expression<Func<ArchivoDeMovimientos, bool>> expresionFiltro;
            filtro.FechaDesde = filtro.FechaDesde ?? new DateTime(1970, 1, 1);
            filtro.FechaHasta = filtro.FechaHasta ?? DateTime.MaxValue;
            if (filtro.MaterialId > 0)
            {
                expresionFiltro = (x => x.Material.Id == filtro.MaterialId);
            }
            else if (filtro.NumeroDocumentoIngreso != null)
            {
                expresionFiltro = (x => x.Muestras.Any(y => y.Recorrido.NumeroDocumentoIngreso == filtro.NumeroDocumentoIngreso));
            }
            else
            {
                expresionFiltro =
                (x =>
                 x.Fecha <= filtro.FechaHasta && x.Fecha >= filtro.FechaDesde
                 && ((x.Material.Id.Equals(filtro.MaterialId)) || filtro.MaterialId == 0) && x.Centro.Id == filtro.CentroId);
            }

            return Listar<ArchivoDeMovimientos, ArchivoDeMovimientosDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<ExcepcionEnvioCamaraDto> ListarExcepcionesEnvioCamara(Paginacion paginacion)
        {
            return Listar<ExcepcionEnvioCamara, ExcepcionEnvioCamaraDto>(null, paginacion);
        }

        public ListaPaginada<AnalisisPorCaracteristicaDto> ListarPaginadoAnalisisYCaladoPorCaracteristica(
            Guid workflowInstanceId, Paginacion paginacion)
        {
            var calado = repositorio.Listar<Calado>(x => x.WorkflowInstanceId == workflowInstanceId).LastOrDefault();
            var analisis = repositorio.Listar<AnalisisDeCalidad>(x => x.Calado.Id == calado.Id).LastOrDefault();

            var caladosSinAnalisis = calado?.CaladosPorCaracteristica.Where(x => x != null);
            if (analisis != null && calado.CaladosPorCaracteristica.Any(x => x.AnalisisPreliminar))
            {
                caladosSinAnalisis =
                    caladosSinAnalisis.Where(
                        x =>
                        analisis.CaracteristicasAnalizadas.All(
                            y => y.CaracteristicaDeCalidad != x.CaracteristicaDeCalidad));
            }

            var caladosDto = caladosSinAnalisis.Select(c => new AnalisisPorCaracteristica
            {
                AnalisisDeCalidad = analisis,
                CaracteristicaDeCalidad = c.CaracteristicaDeCalidad,
                DescuentoEnKg = c.DescuentoEnKg,
                DescuentoEnPorcentaje = c.DescuentoEnPorcentaje,
                Rango = c.Rango,
                Unidad = c.Unidad,
                ValorCalado = c.ValorCalado
            });
            var caracteristicas = analisis != null
                                      ? caladosDto.Concat(analisis.CaracteristicasAnalizadas).AsQueryable()
                                      : caladosDto.AsQueryable();
            return ObtenerListaPaginada<AnalisisPorCaracteristica, AnalisisPorCaracteristicaDto>(paginacion,
                                                                                                 caracteristicas);
        }

        public IList<AnalisisPorCaracteristicaDto> ListarAnalisisYCaladoPorCaracteristica(Guid workflowInstanceId)
        {
            var caracteristicas = ListarAnalisisYCaladoPorCaracteristicaEF(workflowInstanceId);
            return
                conversor.ConvertirList<AnalisisPorCaracteristica, AnalisisPorCaracteristicaDto>(
                    caracteristicas.ToList());
        }

        private IList<AnalisisPorCaracteristica> ListarAnalisisYCaladoPorCaracteristicaEF(Guid workflowInstanceId)
        {
            var calado = repositorio.Listar<Calado>(x => x.WorkflowInstanceId == workflowInstanceId).LastOrDefault();

            if (calado == null)
            {
                return new List<AnalisisPorCaracteristica>();
            }

            var analisis = repositorio.Listar<AnalisisDeCalidad>(x => x.Calado.Id == calado.Id).LastOrDefault();

            var caladosSinAnalisis = calado.CaladosPorCaracteristica.Where(x => x != null);
            if (analisis != null && calado.CaladosPorCaracteristica.Any(x => x.AnalisisPreliminar))
            {
                caladosSinAnalisis =
                    caladosSinAnalisis.Where(
                        x =>
                        analisis.CaracteristicasAnalizadas.All(
                            y => y.CaracteristicaDeCalidad != x.CaracteristicaDeCalidad));
            }

            var caladosDto = caladosSinAnalisis.Select(c => new AnalisisPorCaracteristica
            {
                AnalisisDeCalidad = analisis,
                CaracteristicaDeCalidad = c.CaracteristicaDeCalidad,
                DescuentoEnKg = c.DescuentoEnKg,
                DescuentoEnPorcentaje = c.DescuentoEnPorcentaje,
                Rango = c.Rango,
                Unidad = c.Unidad,
                ValorCalado = c.ValorCalado,
                EnviaACamara = c.EnviaACamara,
                HuboExcepcion = c.HuboExcepcion
            });
            return analisis != null
                                      ? caladosDto.Concat(analisis.CaracteristicasAnalizadas).ToList()
                                      : caladosDto.ToList();
        }

        public IList<AnalisisPorCaracteristicaDto> ListarAnalisisYCaladoPorCaracteristicaNoAceptables(
            Guid workflowInstanceId)
        {
            var caracteristicas = ListarAnalisisYCaladoPorCaracteristica(workflowInstanceId);
            return caracteristicas.Where(x => x.NoAceptarSiSeDefineUnValor && x.Valor > x.CaladoMinimo
                    && (x.ToleranciaSinAnalisis == null || (x.ToleranciaSinAnalisis != null && x.Valor > x.ToleranciaSinAnalisis))).ToList();
        }

        public IList<AnalisisPorCaracteristicaDto> ListarAnalisisYCaladoPorCaracteristicaConAdvertencia(
            Guid workflowInstanceId)
        {
            var caracteristicas = ListarAnalisisYCaladoPorCaracteristica(workflowInstanceId);
            return caracteristicas.Where(x => x.Valor > x.CaladoMinimo
                    && x.ToleranciaSinMensaje != null && x.Valor > x.ToleranciaSinMensaje).ToList();
        }

        public IList<AjusteDeCalidadDto> ListarCaracteristicasParaAjustesDeCalidad(int caladoId)
        {
            var calado = repositorio.Obtener<Calado>(caladoId);
            var analisis = repositorio.Listar<AnalisisDeCalidad>(x => x.Calado.Id == calado.Id).LastOrDefault();
            var caladosSinAnalisis = calado.CaladosPorCaracteristica.Where(x => x != null);
            if (analisis != null && calado.CaladosPorCaracteristica.Any(x => x.AnalisisPreliminar))
            {
                caladosSinAnalisis =
                    caladosSinAnalisis.Where(
                        x =>
                        analisis.CaracteristicasAnalizadas.All(
                            y => y.CaracteristicaDeCalidad != x.CaracteristicaDeCalidad));
            }

            var caladosDto = caladosSinAnalisis.Select(c => new AjusteDeCalidadDto
            {
                Caracteristica = c.CaracteristicaDeCalidad.CaracteristicaDeCalidadMaestro.Descripcion,
                CaracteristicaId = c.CaracteristicaDeCalidad.Id,
                Id = c.Id,
                EsModificable = c.CaracteristicaDeCalidad.EsModificable,
                Rango = c.Rango,
                Material = c.CaracteristicaDeCalidad.MaterialPorCentro.Material.Descripcion,
                ValorOriginal = c.ValorCalado
            });
            if (analisis != null)
            {
                var caracteristicas =
                    caladosDto.Concat(analisis.CaracteristicasAnalizadas.Select(s => new AjusteDeCalidadDto
                    {
                        Caracteristica = s.CaracteristicaDeCalidad.CaracteristicaDeCalidadMaestro.Descripcion,
                        CaracteristicaId = s.CaracteristicaDeCalidad.Id,
                        Id = s.Id,
                        EsModificable = s.CaracteristicaDeCalidad.EsModificable,
                        Rango = s.Rango,
                        Material = s.CaracteristicaDeCalidad.MaterialPorCentro.Material.Descripcion,
                        ValorOriginal = s.ValorAnalisis ?? s.ValorCalado,
                        EsAnalisis = true
                    }));
                return caracteristicas.ToList();
            }
            return caladosDto.ToList();
        }

        public IList<CartaPorteDto> ListarCartasDePortePorCentroYFecha(int centroId, DateTime fechaInicio,
                                                                       DateTime fechaFin, TipoDeWorkflow tipoDeWorkflow)
        {
            fechaFin = fechaFin.FinDelDia();
            fechaInicio = fechaInicio.InicioDelDia();
            var cp = repositorio.Listar<Recorrido, CartaPorte>(r => r.Vehiculo.CartaPorte,
                                                               r =>
                                                               r.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte &&
                                                               r.Workflow.TipoDeWorkflow == tipoDeWorkflow &&
                                                               r.Centro.Id == centroId && r.Terminado && !r.Rechazado &&
                                                               r.Vehiculo.CartaPorte.FechaEmision >= fechaInicio &&
                                                               r.Vehiculo.CartaPorte.FechaEmision <= fechaFin).ToList();
            return conversor.ConvertirList<CartaPorte, CartaPorteDto>(cp);
        }

        public IList<OnccaEmitidasDto> ListarArchivoOncca(List<int> centros, DateTime fechaInicio, DateTime fechaFin,
                                                          TipoDeWorkflow tipoDeWorkflow)
        {
            fechaFin = fechaFin.FinDelDia();
            fechaInicio = fechaInicio.InicioDelDia();
            return repositorio.ListarConsulta(new ListarArchivoOncca(centros, fechaInicio, fechaFin, tipoDeWorkflow));
        }

        public IList<ListadoCamionesDto> ListarCartasDePortePorCentroFechaTiposComercialesYMaterial(List<int> centros,
                                                                                                    DateTime fechaInicio,
                                                                                                    DateTime fechaFin,
                                                                                                    List<int>
                                                                                                        tiposComerciales,
                                                                                                    List<int> materiales,
                                                                                                    bool
                                                                                                        incluirRechazados)
        {
            fechaFin = fechaFin.FinDelDia();
            fechaInicio = fechaInicio.InicioDelDia();
            return
                repositorio.ListarConsulta(new ListarListadoCamiones(centros, fechaInicio, fechaFin, tiposComerciales,
                                                                     materiales, incluirRechazados));
        }

        public IList<AnalisisPorCaracteristicaDto> ListarAnalisisYCaladoPorId(int caladoId)
        {
            var calado = repositorio.Obtener<Calado>(caladoId);
            var analisis = repositorio.Listar<AnalisisDeCalidad>(x => x.Calado.Id == calado.Id).LastOrDefault();

            var caladosSinAnalisis = calado.CaladosPorCaracteristica.Where(x => x != null);
            if (analisis != null && calado.CaladosPorCaracteristica.Any(x => x.AnalisisPreliminar))
            {
                caladosSinAnalisis =
                    caladosSinAnalisis.Where(
                        x =>
                        analisis.CaracteristicasAnalizadas.All(
                            y => y.CaracteristicaDeCalidad != x.CaracteristicaDeCalidad));
            }

            var caladosDto = caladosSinAnalisis.Select(c => new AnalisisPorCaracteristica
            {
                AnalisisDeCalidad = analisis,
                CaracteristicaDeCalidad = c.CaracteristicaDeCalidad,
                DescuentoEnKg = c.DescuentoEnKg,
                DescuentoEnPorcentaje = c.DescuentoEnPorcentaje,
                Rango = c.Rango,
                Unidad = c.Unidad,
                ValorCalado = c.ValorCalado
            });
            var caracteristicas = analisis != null
                                      ? caladosDto.Concat(analisis.CaracteristicasAnalizadas).AsQueryable()
                                      : caladosDto.AsQueryable();
            return
                conversor.ConvertirList<AnalisisPorCaracteristica, AnalisisPorCaracteristicaDto>(
                    caracteristicas.ToList());
        }

        public AnalisisDeCalidadDto ObtenerAnalisisDeCalidadPorInstanceId(Guid workflowInstanceId)
        {
            var analisis =
                repositorio.ObtenerProyeccion<Recorrido, AnalisisDeCalidad>(
                    x => x.InstanciaWorkflow == workflowInstanceId, x => x.AnalisisDeCalidad);
            return analisis != null ? conversor.Convertir<AnalisisDeCalidad, AnalisisDeCalidadDto>(analisis) : null;
        }

        public AnalisisDeCalidadDto ObtenerAnalisisDeCalidadPorCaladoId(int caladoId)
        {
            return ObtenerUltimo<AnalisisDeCalidad, AnalisisDeCalidadDto>(x => x.Calado.Id == caladoId, x => x.Id);
        }

        public CaracteristicasAnalizadasDto ObtenerCaracteristicasAnalizadasPorRecorridoId(int recorridoId)
        {
            return Obtener<CaracteristicasAnalizadas, CaracteristicasAnalizadasDto>(x => x.Recorrido.Id == recorridoId);
        }

        public CaracteristicasAnalizadasDto ObtenerCaracteristicasAnalizadasPorInstanceId(Guid instanceId)
        {
            return Obtener<CaracteristicasAnalizadas, CaracteristicasAnalizadasDto>(x => x.Recorrido.InstanciaWorkflow == instanceId);
        }

        public CartaPorteDto ObtenerCartaPortePorInstanceId(Guid instanceId)
        {
            var cartaporte =
                repositorio.ObtenerProyeccion<Recorrido, CartaPorte>(
                    r => r.InstanciaWorkflow == instanceId && r.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte, x => x.Vehiculo.CartaPorte);
            return cartaporte != null ? conversor.Convertir<CartaPorte, CartaPorteDto>(cartaporte) : null;
        }

        public IList<DescuentoDto> ListarDescuentos(int caracteristica)
        {
            return Listar<Descuento, DescuentoDto>(x => x.CaracteristicaDeCalidad.Id == caracteristica);
        }

        public ListaPaginada<TransaccionSAPDto> ListarPaginadoTransaccionesSAPPorCentro(string filtro, Paginacion paginacion, int centroId)
        {
            Expression<Func<TransaccionSAP, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                int filtroInt;
                if (!int.TryParse(filtro, out filtroInt))
                {
                    filtroInt = int.MaxValue;
                }
                var filtroEnum = ParsearFuncionSap(filtro);
                expresionFiltro = (x => (x.CentroOrigen.Id == centroId) && ((x.DescripcionCorta.Contains(filtro))
                                                                            || (x.Material.Id == filtroInt)
                                                                            || (x.Material.Descripcion.Contains(filtro))
                                                                            || (x.Material.CodigoSAP.Contains(filtro))
                                                                            || (x.TipoComercial.Descripcion.Contains(filtro))
                                                                            || (filtroEnum.Any(y => y == x.FuncionSAP))
                                                                            || (x.TipoComercial.Id == filtroInt)
                                                                            || (x.CentroOrigen.Descripcion.Contains(filtro))));
            }
            else
            {
                expresionFiltro = (x => (x.CentroOrigen.Id == centroId));
            }

            return Listar<TransaccionSAP, TransaccionSAPDto>(expresionFiltro, paginacion);
        }

        private IList<FuncionSAP> ParsearFuncionSap(string valor)
        {
            var funcionesSap = Enum.GetValues(typeof(FuncionSAP)).Cast<FuncionSAP>();
            var funciones = new List<FuncionSAP>();
            foreach (var funcion in funcionesSap)
            {
                if (funcion.DisplayEnum().ToLowerInvariant().Contains(valor.ToLowerInvariant()))
                {
                    funciones.Add(funcion);
                }
            }
            return funciones;
        }

        public ListaPaginada<TransaccionSAPDto> ListarPaginadoTransaccionesSAPFiltradas(int material, int centro,
                                                                                        int? tipoComercial,
                                                                                        FuncionSAP funcionSap,
                                                                                        Paginacion paginacion)
        {
            return Listar<TransaccionSAP, TransaccionSAPDto>(x =>
                                                             (x.Material.Id == material)
                                                             && (x.TipoComercial.Id == tipoComercial)
                                                             && (x.CentroOrigen.Id == centro)
                                                             && (x.FuncionSAP == funcionSap), paginacion);
        }

        public IList<AlmacenDto> ListarAlmacenesPorMaterial(int centroId, int materialId)
        {
            return Listar<Almacen, AlmacenDto>(x => x.Centro.Id == centroId && x.Materiales.Any(y => y.Id == materialId));
        }

        /// <summary>
        /// Hace la consulta al repositorio y mapea las entidades a DTOs
        /// </summary>
        /// <typeparam name="TEntidad"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="expresionFiltro"></param>
        /// <param name="paginacion"></param>
        /// <returns></returns>
        private ListaPaginada<TDto> Listar<TEntidad, TDto>(Expression<Func<TEntidad, bool>> expresionFiltro,
                                                           Paginacion paginacion) where TEntidad : class
        {
            return conversor.ConvertirListaPaginada<TEntidad, TDto>(repositorio.Listar(expresionFiltro, paginacion));
        }

        private IList<TDto> Listar<TEntidad, TDto>() where TEntidad : class
        {
            return conversor.ConvertirList<TEntidad, TDto>(repositorio.Listar<TEntidad>());
        }

        private IList<TDto> Listar<TEntidad, TDto>(Expression<Func<TEntidad, bool>> expresionFiltro)
            where TEntidad : class
        {
            return conversor.ConvertirList<TEntidad, TDto>(repositorio.Listar(expresionFiltro));
        }

        private IList<TDto> Listar<TEntidad, TDto>(Expression<Func<TEntidad, bool>> expresionFiltro, int maxResultados)
            where TEntidad : class
        {
            return conversor.ConvertirList<TEntidad, TDto>(repositorio.Listar(expresionFiltro, maxResultados));
        }

        /// <summary>
        /// Busca una entidad por Id en el repositorio y la devuelve mapeada como DTO
        /// </summary>
        /// <typeparam name="TEntidad"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        private TDto Obtener<TEntidad, TDto>(int id) where TEntidad : class
        {
            return conversor.Convertir<TEntidad, TDto>(repositorio.Obtener<TEntidad>(id));
        }

        private TDto Obtener<TEntidad, TDto>(Expression<Func<TEntidad, bool>> expresionFiltro) where TEntidad : class
        {
            return conversor.Convertir<TEntidad, TDto>(repositorio.Obtener(expresionFiltro));
        }

        private TDto ObtenerUltimo<TEntidad, TDto>(Expression<Func<TEntidad, bool>> expresionFiltro,
                                                   Expression<Func<TEntidad, int>> expresionId) where TEntidad : class
        {
            return conversor.Convertir<TEntidad, TDto>(repositorio.ObtenerMayor(expresionFiltro, expresionId));
        }

        private TDto ObtenerPrimero<TEntidad, TDto>(Expression<Func<TEntidad, bool>> expresionFiltro)
            where TEntidad : class
        {
            return conversor.Convertir<TEntidad, TDto>(repositorio.ObtenerPrimero(expresionFiltro));
        }

        private ListaPaginada<TDto> ObtenerListaPaginada<TEntidad, TDto>(Paginacion paginacion,
                                                                         IQueryable<TEntidad> lista)
        {
            if (paginacion.OrdenarPor != null && lista != null)
            {
                var selectorOrden = Expresiones.PropiedadValueType<TEntidad>(paginacion.OrdenarPor);
                lista = paginacion.DireccionOrden == DirOrden.Asc
                            ? lista.OrderBy(selectorOrden)
                            : lista.OrderByDescending(selectorOrden);
            }
            var itemsTotales = 0;
            if (lista != null)
            {
                itemsTotales = lista.Count();
                lista =
                    lista.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);
            }
            var listaPaginada = new ListaPaginada<TEntidad>(lista.ToList(), paginacion.Pagina,
                                                            paginacion.ItemsPorPagina, itemsTotales);
            return conversor.ConvertirListaPaginada<TEntidad, TDto>(listaPaginada);
        }

        public CartaPorteDto ObtenerCartaPorte(int id)
        {
            return Obtener<CartaPorte, CartaPorteDto>(id);
        }

        public CartaPorteDto ObtenerCartaPortePorCentroYNumero(string numero, int centroId)
        {
            var cp =
                repositorio.ObtenerMayor<Recorrido, DateTime, CartaPorte>(
                    x =>
                    x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.NumeroDocumentoIngreso == numero &&
                    x.Centro.Id == centroId, x => x.FechaInicio, x => x.Vehiculo.CartaPorte);
            return conversor.Convertir<CartaPorte, CartaPorteDto>(cp);
        }

        public CartaPorteResponseDto ObtenerCartaPorteAReutilizarPorNumero(string numero, int centroId,
                                                                           string workflowCodigo)
        {
            var response = new CartaPorteResponseDto();
            try
            {
                if (!String.IsNullOrEmpty(numero))
                {
                    var workflow = repositorio.Obtener<Workflow>(x => x.Codigo == workflowCodigo);
                    var cp =
                        repositorio.ObtenerMayor<Recorrido, DateTime, CartaPorte>(
                            x =>
                            x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte &&
                            x.NumeroDocumentoIngreso == numero && x.Centro.Id == centroId &&
                            x.Workflow.TipoDeWorkflow == workflow.TipoDeWorkflow, x => x.FechaInicio,
                            x => x.Vehiculo.CartaPorte);
                    if (cp == null)
                    {
                        log.Debug("No se encontró la carta de porte {0}", numero);
                        response.Error = Textos.CartaPorte_Inexistente;
                        response.CodigoDeError = 1;
                    }
                    else if (
                        !repositorio.Existe<MaterialPorWorkflow>(
                            x =>
                            x.Material.Id == cp.Material.Id && x.Centro.Id == centroId &&
                            x.Workflow.Id == workflow.Id))
                    {
                        log.Debug("No se encontró el material {0} para el centro {0}", cp.Material.Id, centroId);
                        response.Error = String.Format(Textos.CartaPorte_MaterialInexistente,
                                                       cp.Material.Descripcion);
                        response.CodigoDeError = 2;
                    }
                    else
                    {
                        response.CartaPorte = conversor.Convertir<CartaPorte, CartaPorteDto>(cp);
                        response.CartaPorte.TipoDeWorkflow = workflow.TipoDeWorkflow;
                    }
                }
                return response;
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener la carta de porte {0}", numero);
                throw;
            }
        }

        public CartaPorteResponseDto ObtenerCartaPorteRedespachoPorNumero(string numero, int centroId,
                                                                          string workflowCodigo, int tipoVehiculo, bool cpe, bool consultactg)
        {
            var response = new CartaPorteResponseDto();
            try
            {
                var workflow = repositorio.Obtener<Workflow>(x => x.Codigo == workflowCodigo);

                if (tipoVehiculo == (int)TipoVehiculo.Tren && cpe && !consultactg)
                {
                    var listaVagones = new List<VehiculoDto>();

                    #region FerroviarioCPE

                    var numeroOperativo = Convert.ToInt64(numero);
                    var cartaPortesFerroviario = repositorio.Listar<CartaPorte>(x => x.NumeroOperativo == numeroOperativo)
                        .GroupBy(x => x.NroCartaPorte)
                        .Select(x => x.OrderBy(c => c.Id).FirstOrDefault())
                        .ToList();
                    CartaPorte cartaPorte = null;

                    foreach (var cartaPorteItem in cartaPortesFerroviario)
                    {
                        if (repositorio.Existe<Recorrido>(
                            x =>
                            x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.NumeroDocumentoIngreso == cartaPorteItem.NroCartaPorte &&
                            x.Centro.Id == centroId && x.Workflow.TipoDeWorkflow != workflow.TipoDeWorkflow && !x.Rechazado))
                        {
                            log.Debug(workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso ? "La Carta de Porte {0} no puede ser ingresada en el Centro donde egresó" : "La Carta de Porte {0} no puede ser egresada en el Centro donde ingresó ", numero);
                            continue;
                        }

                        var cp =
                        repositorio.ObtenerMayor<Recorrido, DateTime, CartaPorte>(
                            x =>
                            x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.NumeroDocumentoIngreso == cartaPorteItem.NroCartaPorte &&
                            x.Centro.Id != centroId && x.Workflow.TipoDeWorkflow != workflow.TipoDeWorkflow && !x.Rechazado &&
                            x.Terminado, x => x.FechaInicio, x => x.Vehiculo.CartaPorte);
                        if (cp is null)
                            continue;

                        if (
                        !repositorio.Existe<MaterialPorWorkflow>(
                            x =>
                            x.Material.Id == cp.Material.Id && x.Centro.Id == centroId && x.Workflow.Id == workflow.Id))
                        {
                            log.Debug("No se encontró el material {0} para el centro {0}", cp.Material.Id, centroId);
                            continue;
                        }

                        cartaPorte = cartaPorteItem;
                        var vehiculoItem = cartaPorte.Vehiculos.FirstOrDefault();
                        if (!(vehiculoItem is null))
                        {
                            listaVagones.Add(
                                new VehiculoDto
                                {
                                    NumCTG = cartaPorte?.NroCartaPorte,
                                    Sucural = cartaPorte?.Sucursal?.ToString("D5"),
                                    NumOrden = cartaPorte?.CTG,
                                    Patente = vehiculoItem?.Patente,
                                    PesoBrutoOrigen = vehiculoItem?.PesoBrutoOrigen,
                                    PesoTaraOrigen = vehiculoItem?.PesoTaraOrigen,
                                    PesoNetoOrigen = Convert.ToInt32(vehiculoItem?.PesoBrutoOrigen - vehiculoItem?.PesoTaraOrigen),
                                    PatenteAcoplado = string.Empty,
                                    TipoVehiculo = TipoVehiculo.Tren
                                }
                            );
                        }
                    }

                    if (cartaPortesFerroviario.Any())
                    {
                        response.CartaPorte = conversor.Convertir<CartaPorte, CartaPorteDto>(cartaPorte);
                        response.CartaPorte.TipoDeWorkflow = workflow.TipoDeWorkflow;
                        response.CartaPorte.Vehiculos = listaVagones;
                        response.EsRedespacho = true;
                        var tiempo = configuracion.AppSettings["TiempoDeDemoraExportaciones"];
                        if (tiempo != null)
                        {
                            var demora = (DateTime.Now - cartaPorte.FechaEmision);
                            response.EstaDemorado = demora > TimeSpan.FromMinutes(Convert.ToInt64(tiempo));
                            response.Error = string.Format(Textos.Error_TiempoDeDemoraExportaciones, demora.ToString("h'h 'm'm 's's'"));
                        }
                    }
                    else
                    {
                        log.Debug("No se encontró la carta de porte en redespacho {0}", numero);
                        return ObtenerCartaPorteAReutilizarPorNumero(numero, centroId, workflowCodigo);
                    }

                    #endregion FerroviarioCPE
                }
                else
                {
                    if (
                    repositorio.Existe<Recorrido>(
                        x =>
                        x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.NumeroDocumentoIngreso == numero &&
                        x.Centro.Id == centroId && x.Workflow.TipoDeWorkflow != workflow.TipoDeWorkflow && !x.Rechazado))
                    {
                        if (workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso)
                        {
                            log.Debug("La carta de porte {0} egresó en el centro actual, no se puede ingresar ", numero);
                            response.Error = String.Format(Textos.CartaPorte_IngresoCentroActual, numero);
                        }
                        else
                        {
                            log.Debug("La carta de porte {0} se ingresó en el centro actual, no se puede egresar ", numero);
                            response.Error = String.Format(Textos.CartaPorte_EgresoCentroActual, numero);
                        }
                        response.CodigoDeError = 4;
                        return response;
                    }

                    var cp =
                        repositorio.ObtenerMayor<Recorrido, DateTime, CartaPorte>(
                            x =>
                            x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.NumeroDocumentoIngreso == numero &&
                            x.Centro.Id != centroId && x.Workflow.TipoDeWorkflow != workflow.TipoDeWorkflow && !x.Rechazado &&
                            x.Terminado, x => x.FechaInicio, x => x.Vehiculo.CartaPorte);

                    if (cp == null)
                    {
                        log.Debug("No se encontró la carta de porte en redespacho {0}", numero);
                        return ObtenerCartaPorteAReutilizarPorNumero(numero, centroId, workflowCodigo);
                    }
                    else if (
                        !repositorio.Existe<MaterialPorWorkflow>(
                            x =>
                            x.Material.Id == cp.Material.Id && x.Centro.Id == centroId && x.Workflow.Id == workflow.Id))
                    {
                        log.Debug("No se encontró el material {0} para el centro {0}", cp.Material.Id, centroId);
                        response.Error = String.Format(Textos.CartaPorte_MaterialInexistente, cp.Material.Descripcion);
                        response.CodigoDeError = 2;
                    }
                    else
                    {
                        response.CartaPorte = conversor.Convertir<CartaPorte, CartaPorteDto>(cp);
                        response.CartaPorte.TipoDeWorkflow = workflow.TipoDeWorkflow;
                        response.EsRedespacho = true;
                        var tiempo = configuracion.AppSettings["TiempoDeDemoraExportaciones"];
                        if (tiempo != null)
                        {
                            var demora = (DateTime.Now - cp.FechaEmision);
                            response.EstaDemorado = demora > TimeSpan.FromMinutes(Convert.ToInt64(tiempo));
                            response.Error = string.Format(Textos.Error_TiempoDeDemoraExportaciones, demora.ToString("h'h 'm'm 's's'"));
                        }

                        //Tren
                        if (response.CartaPorte.TipoVehiculo == TipoVehiculo.Tren)
                        {
                            foreach (var vehiculo in response.CartaPorte.Vehiculos)
                            {
                                vehiculo.NumCTG = response?.CartaPorte?.NroCartaPorte;
                                vehiculo.Sucural = response?.CartaPorte?.Sucursal?.ToString("D5");
                                vehiculo.NumOrden = response?.CartaPorte?.CTG;
                                vehiculo.Patente = vehiculo?.Patente;
                                vehiculo.PesoBrutoOrigen = vehiculo?.PesoBrutoOrigen;
                                vehiculo.PesoTaraOrigen = vehiculo?.PesoTaraOrigen;
                                vehiculo.PesoNetoOrigen = Convert.ToInt32(vehiculo?.PesoBrutoOrigen - vehiculo?.PesoTaraOrigen);
                                vehiculo.PatenteAcoplado = string.Empty;
                                vehiculo.TipoVehiculo = TipoVehiculo.Tren;
                            }
                        }
                    }
                }

                return response;
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener la carta de porte {0}", numero);
                throw;
            }
        }

        public CartaPorteDto ObtenerCartaPorteVacia(int centroId, string workflowCodigo,
                                                    string destinatarioCodigoSap = "", string titularCodigoSap = "", string centroDestino = "", string rtteComercial = "")
        {
            try
            {
                var workflow = repositorio.Obtener<Workflow>(x => x.Codigo == workflowCodigo);

                var centro = repositorio.Obtener<Centro>(centroId);
                Proveedor destinatario = null;
                Proveedor titular = null;
                Centro destino = null;
                Proveedor rtte = null;
                if (!String.IsNullOrEmpty(destinatarioCodigoSap))
                {
                    destinatario = repositorio.Obtener<Proveedor>(x => x.CodigoSap == destinatarioCodigoSap);
                }
                if (!String.IsNullOrEmpty(titularCodigoSap) && destinatarioCodigoSap == titularCodigoSap)
                {
                    titular = destinatario;
                }
                else if (!String.IsNullOrEmpty(titularCodigoSap))
                {
                    titular = repositorio.Obtener<Proveedor>(x => x.CodigoSap == titularCodigoSap);
                }
                if (!String.IsNullOrEmpty(rtteComercial) && destinatarioCodigoSap == rtteComercial)
                {
                    rtte = destinatario;
                }
                else if (!String.IsNullOrEmpty(rtteComercial))
                {
                    rtte = repositorio.Obtener<Proveedor>(x => x.CodigoSap == rtteComercial);
                }
                if (!String.IsNullOrEmpty(centroDestino))
                {
                    destino = repositorio.Obtener<Centro>(x => x.CodigoSAP == centroDestino);
                }

                var cpVacia = new CartaPorte
                {
                    FechaEmision = DateTime.Now,
                    FechaCP = workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? DateTime.Now : DateTime.MinValue,
                    TitularCartaPorte = titular,
                    Destinatario = destinatario,
                    Procedencia = workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? centro.Localidad : null,
                    CodEstab = workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? centro.CodigoEstablecimiento : null,
                    CentroDestino = workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso ? centro : destino,
                    RtteComercial = rtte
                };
                var cpVaciaDto = conversor.Convertir<CartaPorte, CartaPorteDto>(cpVacia);
                cpVaciaDto.TipoDeWorkflow = workflow.TipoDeWorkflow;
                cpVaciaDto.TomarFotoEnMesa = centro.TomarFotoEnMesa;
                cpVaciaDto.LeerCPDeFoto = centro.LeerCPDeFoto;
                return cpVaciaDto;
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo generar" +
                             " la nueva carta de porte");
                throw;
            }
        }

        public CartaPorteDto ObtenerCartaPorteVaciaFason(int centroId, string workflowCodigo,
                                                         string destinatarioCodigoSap = "", string titularCodigoSap = "")
        {
            try
            {
                var workflow = repositorio.Obtener<Workflow>(x => x.Codigo == workflowCodigo);

                var centro = repositorio.Obtener<Centro>(centroId);
                Cliente destinatario = null;
                Proveedor titular = null;
                if (!String.IsNullOrEmpty(destinatarioCodigoSap))
                {
                    destinatario = repositorio.Obtener<Cliente>(x => x.CodigoSap == destinatarioCodigoSap);
                }
                if (!String.IsNullOrEmpty(titularCodigoSap))
                {
                    titular = repositorio.Obtener<Proveedor>(x => x.CodigoSap == titularCodigoSap);
                }

                var cpVacia = new CartaPorte
                {
                    FechaEmision = DateTime.Now,
                    TitularCartaPorte = titular,
                    DestinatarioCliente = destinatario,
                    Procedencia = workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? centro.Localidad : null,
                    CodEstab =
                            workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? centro.CodigoEstablecimiento : null,
                    CentroDestino = workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso ? centro : null,
                };
                var cpVaciaDto = conversor.Convertir<CartaPorte, CartaPorteDto>(cpVacia);
                cpVaciaDto.TipoDeWorkflow = workflow.TipoDeWorkflow;
                return cpVaciaDto;
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo generar la nueva carta de porte");
                throw;
            }
        }

        public CartaPorteValidaResponseDto NumeroCartaPorteValido(string numero, int centroId, string workflowCodigo, bool CPE = false)
        {
            var response = new CartaPorteValidaResponseDto { Valida = true };
            if (
                repositorio.Existe<Recorrido>(
                    x =>
                    x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.NumeroDocumentoIngreso == numero &&
                    (!x.Rechazado || !x.Terminado)))
            {
                log.Debug(CPE ? "El Nº de CTG {0} ya fue ingresado " : "La carta de porte {0} ya fue ingresada ", numero);
                response.Valida = false;
                response.Error = String.Format(CPE ? Textos.CTG_Ingresada_CPE : Textos.CartaPorte_Ingresada, numero);
                response.CodigoDeError = 3;
            }
            return response;
        }

        public CartaPorteValidaResponseDto NumeroCartaPorteValidoRedespacho(string numero, int centroId,
                                                                            string workflowCodigo)
        {
            var workflow = repositorio.Obtener<Workflow>(x => x.Codigo == workflowCodigo);
            var response = new CartaPorteValidaResponseDto { Valida = true };
            if (
                repositorio.Existe<Recorrido>(
                    x =>
                    x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte &&
                    x.NumeroDocumentoIngreso == numero &&
                    x.Workflow.TipoDeWorkflow == workflow.TipoDeWorkflow &&
                    x.Centro.Id == centroId &&
                    !(x.Rechazado && x.Terminado)))
            {
                log.Debug("La carta de porte {0} ya fue ingresada ", numero);
                response.Error = String.Format(Textos.CartaPorte_Ingresada, numero);
                response.CodigoDeError = 3;
                response.Valida = false;
                return response;
            }

            if (
                repositorio.Existe<Recorrido>(
                    x =>
                    x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte &&
                    x.NumeroDocumentoIngreso == numero &&
                    x.Centro.Id == centroId &&
                    x.Workflow.TipoDeWorkflow != workflow.TipoDeWorkflow &&
                    !x.Rechazado))
            {
                if (workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso)
                {
                    log.Debug("La carta de porte {0} egresó en el centro actual, no se puede ingresar ", numero);
                    response.Error = String.Format(Textos.CartaPorte_IngresoCentroActual, numero);
                }
                else
                {
                    log.Debug("La carta de porte {0} se ingresó en el centro actual, no se puede egresar ", numero);
                    response.Error = String.Format(Textos.CartaPorte_EgresoCentroActual, numero);
                }

                response.CodigoDeError = 4;
                response.Valida = false;
                return response;
            }

            return response;
        }

        public bool ProcedenciaYCodigoValido(int centroId, string codEstablecimiento, int localidadId)
        {
            var centro = repositorio.Obtener<Centro>(centroId);
            return centro.CodigoEstablecimiento == codEstablecimiento && centro.Localidad.Id == localidadId;
        }

        public OrdenDeDescargaDto ObtenerOrdenDeDescarga(int id)
        {
            return Obtener<OrdenDeDescarga, OrdenDeDescargaDto>(id);
        }

        public RemitoDto ObtenerRemito(int id)
        {
            return Obtener<Remito, RemitoDto>(id);
        }

        public RemitoDto ObtenerRemitoPorNroRemito(string numero)
        {
            return ObtenerPrimero<Remito, RemitoDto>(x => x.OrdenRemito == numero);
        }

        public RemitoDto ObtenerRemitoPorOrdenDeDescarga(string numero)
        {
            return ObtenerPrimero<Remito, RemitoDto>(x => x.OrdenDeDescarga == numero);
        }

        public OrdenDeDescargaFasonDto ObtenerOrdenDeDescargaFason(int id)
        {
            return Obtener<OrdenDeDescargaFason, OrdenDeDescargaFasonDto>(id);
        }

        public OrdenDeDescargaDto ObtenerOrdenDeDescargaPorNumeroDeOrden(string numero)
        {
            return ObtenerPrimero<OrdenDeDescarga, OrdenDeDescargaDto>(x => x.Numero == numero);
        }

        public OrdenEntrePlantasDto ObtenerOrdenEntrePlantas(int id)
        {
            return Obtener<OrdenEntrePlantas, OrdenEntrePlantasDto>(id);
        }

        public OrdenEntrePlantasDto ObtenerOrdenEntrePlantasPorNumeroDeOrden(string numero)
        {
            return ObtenerPrimero<OrdenEntrePlantas, OrdenEntrePlantasDto>(x => x.Numero == numero);
        }

        public OrdenEntrePlantasDto ObtenerOrdenEntrePlantasPorInstanceId(Guid id)
        {
            return ObtenerPrimero<OrdenEntrePlantas, OrdenEntrePlantasDto>(x => x.Recorrido.InstanciaWorkflow == id);
        }

        public HojaDeRutaDto ObtenerHojaDeRuta(int id)
        {
            return Obtener<HojaDeRuta, HojaDeRutaDto>(id);
        }

        public RemitoDto ObtenerRemitoPorInstanceId(Guid id)
        {
            var numeroDocumento =
                repositorio.ObtenerProyeccion<Recorrido, string>(
                    r => r.InstanciaWorkflow == id && r.TipoDocumentoIngreso == TipoDocumentoIngreso.Remito,
                    r => r.NumeroDocumentoIngreso);

            return numeroDocumento != null
                       ? ObtenerPrimero<Remito, RemitoDto>(c => c.OrdenDeDescarga == numeroDocumento)
                       : null;
        }

        public OrdenDeDescargaFasonDto ObtenerOrdenDeDescargaFasonPorNumeroDeOrdenYCliente(string numero, int clienteId)
        {
            return
                ObtenerPrimero<OrdenDeDescargaFason, OrdenDeDescargaFasonDto>(
                    x => x.Numero == numero && x.Cliente.Id == clienteId);
        }

        public OrdenDeDescargaFasonDto ObtenerOrdenDeDescargaFasonPorNumeroRemito(string numeroRemito)
        {
            return
                ObtenerPrimero<OrdenDeDescargaFason, OrdenDeDescargaFasonDto>(
                    x => x.NumeroRemito == numeroRemito && !x.Recorrido.Rechazado);
        }

        public HojaDeRutaDto ObtenerHojaDeRutaPorNumero(string numero)
        {
            return ObtenerPrimero<HojaDeRuta, HojaDeRutaDto>(x => x.Numero == numero);
        }

        public OrdenDeDescargaFasonDto ObtenerOrdenDeDescargaFasonPorNumeroDeOrden(string numero)
        {
            return ObtenerPrimero<OrdenDeDescargaFason, OrdenDeDescargaFasonDto>(x => x.Numero == numero);
        }

        public IList<WorkflowInfoDto> ListarWorkflowsPorUsuarioYCentro(string nombreUsuario, int centroId)
        {
            return repositorio.ListarConsulta(new WorkflowsPorUsuarioYCentroConsulta(nombreUsuario, centroId));
        }

        public ListaPaginada<EntregadorDto> ListarEntregadores(string filtro, Paginacion paginacion)
        {
            Expression<Func<Entregador, bool>> expresionFiltro;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    (x.DescripcionCorta.Contains(filtro) || x.Cuil.Contains(filtro) || x.RazonSocial.Contains(filtro) ||
                     x.Tratamiento.Contains(filtro) || x.TipoEntregador.Contains(filtro)) && x.Activo;
            }
            else
            {
                expresionFiltro = x => x.Activo;
            }
            return Listar<Entregador, EntregadorDto>(expresionFiltro, paginacion);
        }

        public IList<EntregadorDto> BuscarEntregadores(string criteria)
        {
            return
                Listar<Entregador, EntregadorDto>(f => f.Activo && (f.RazonSocial.Contains(criteria) || f.Cuil.Contains(criteria)), 20);
        }

        public EntregadorDto BuscarEntregador(string criteria)
        {
            return
                Obtener<Entregador, EntregadorDto>(
                    f => f.Activo && (f.RazonSocial.Contains(criteria) || f.Cuil.Contains(criteria)));
        }

        public EntregadorDto ObtenerEntregador(int id)
        {
            return Obtener<Entregador, EntregadorDto>(id);
        }

        public ListaPaginada<TransmisionASapDto> ListarTransmisionesASap(FiltroPanelDeTransaccionesSapDto filtro, Paginacion paginacion)
        {
            SetearFiltro(filtro);
            var consulta = new TransmisionASapConsulta(filtro, paginacion);
            return repositorio.ListarConsultaPaginada(consulta);
        }

        public ListaPaginada<TransmisionBajaCtgDefinitivaDto> ListarRetransmisionCtgDefinitiva(FiltroPanelDeBajaCtgDefinitivaDto filtro, Paginacion paginacion)
        {
            SetearFiltro(filtro);
            var consulta = new BajaCtgDefinitivaConsulta(filtro, paginacion);
            return repositorio.ListarConsultaPaginada(consulta);
        }

        public ListaPaginada<EnvioUrenportDto> ListarEnvioUrenport(FiltroPanelDeBajaCtgDefinitivaDto filtro, Paginacion paginacion)
        {
            SetearFiltro(filtro);
            return Listar<EnvioUrenport, EnvioUrenportDto>(x => x.Recorrido.Centro.Id == filtro.CentroId &&
                (filtro.EstadoTransmisionASap == null || x.Estado == filtro.EstadoTransmisionASap) &&
                x.Recorrido.FechaInicio >= filtro.FechaDesde && x.Recorrido.FechaInicio <= filtro.FechaHasta &&
                (filtro.NumeroDocumentoIngreso == "" || x.NumeroDocumentoIngreso == filtro.NumeroDocumentoIngreso) &&
                (filtro.TipoDocumentoIngreso == null || x.Recorrido.TipoDocumentoIngreso == filtro.TipoDocumentoIngreso) &&
                (filtro.Patente == "" || x.Recorrido.Patente == filtro.Patente)
                , paginacion);
        }

        private static void SetearFiltro(FiltroPanelDeTransaccionesSapDto filtro)
        {
            TimeSpan timeHasta;
            filtro.HoraHasta = filtro.HoraHasta ?? "23:59:59";
            TimeSpan.TryParse(filtro.HoraHasta, out timeHasta);
            TimeSpan timeDesde;
            filtro.HoraDesde = filtro.HoraDesde ?? "00:00:00";
            TimeSpan.TryParse(filtro.HoraDesde, out timeDesde);
            filtro.FechaHasta = filtro.FechaHasta.AddMinutes(timeHasta.Minutes);
            filtro.FechaHasta = filtro.FechaHasta.AddHours(timeHasta.Hours);
            filtro.FechaDesde = filtro.FechaDesde.AddMinutes(timeDesde.Minutes);
            filtro.FechaDesde = filtro.FechaDesde.AddHours(timeDesde.Hours);
            filtro.NumeroDocumentoIngreso = filtro.NumeroDocumentoIngreso ?? string.Empty;
            filtro.Patente = filtro.Patente ?? string.Empty;
        }

        public IList<BajaCTGRetransmisionDto> ObtenerBajasCtgDefinitivas(int[] ids)
        {
            return Listar<BajaCTG, BajaCTGRetransmisionDto>(x => ids.Contains(x.Id)).ToList();
        }

        public IList<EnvioUrenportDto> ObtenerEnvioUrenports(int[] ids)
        {
            return Listar<EnvioUrenport, EnvioUrenportDto>(x => ids.Contains(x.Id)).ToList();
        }

        public TransmisionASapDto ObtenerTransmisionASap(int id)
        {
            return conversor.Convertir<TransmisionASap, TransmisionASapDto>(repositorio.ObtenerConsultaEscalar(new ObtenerTransmisionASap(id)));
        }

        public IList<TransmisionesCupoEnErrorDto> ListarCuposEnErrorASapPorFiltro(FiltroPanelDeTransaccionesSapDto filtro)
        {
            SetearFiltro(filtro);
            var lista = repositorio.ListarConsulta(new TransmisionesCuposErrorConsulta(filtro));
            return
                lista.Select(
                    x =>
                    new TransmisionesCupoEnErrorDto
                    {
                        InstanciaWorkflow = x.InstanciaWorkflow,
                        Z2200 = conversor.Convertir<InformarCupoTransmisionASap, Z_SDMF_Z2200N>(x)
                    }).ToList();
        }

        public IList<int> CaladoPorCaracteristicaQueSeEnvianACamara(int caladoId)
        {
            return repositorio.Listar<CaladoPorCaracteristica, int>(
                x => x.CaracteristicaDeCalidad.Id, x => x.Calado.Id == caladoId && x.DescuentoEnPorcentaje > 0);
        }

        public ListaPaginada<MaterialPorWorkflowDto> ListarPaginadoMaterialPorWorkflow(string filtro, Paginacion paginacion,
                                                                                       int centroId)
        {
            Expression<Func<MaterialPorWorkflow, bool>> expresion =
                x => x.Centro.Id == centroId && x.Workflow.Centro.Id == centroId && x.Material.Activo;

            if (!string.IsNullOrEmpty(filtro))
            {
                expresion = (x => (x.Material.Descripcion.Contains(filtro)
                                         || x.Workflow.Descripcion.Contains(filtro)) && x.Material.Activo &&
                                        x.Centro.Id == centroId);
            }
            return Listar<MaterialPorWorkflow, MaterialPorWorkflowDto>(expresion, paginacion);
        }

        public ListaPaginada<CaracteristicaDeCalidadPorWorkflowDto> ListarPaginadoCaracteristicaDeCalidadPorWorkflow(string filtro, Paginacion paginacion, int centroId)
        {
            Expression<Func<CaracteristicaDeCalidadPorWorkflow, bool>> expresion =
                x => x.CaracteristicaDeCalidad.MaterialPorCentro.Centro.Id == centroId && x.CaracteristicaDeCalidad.MaterialPorCentro.Material.Activo;

            if (!string.IsNullOrEmpty(filtro))
            {
                expresion = (x => (x.CaracteristicaDeCalidad.MaterialPorCentro.Material.Descripcion.Contains(filtro)
                                   || x.Workflow.Descripcion.Contains(filtro)) && x.CaracteristicaDeCalidad.MaterialPorCentro.Material.Activo &&
                                  x.CaracteristicaDeCalidad.MaterialPorCentro.Centro.Id == centroId);
            }
            var relaciones = repositorio.Listar((CaracteristicaDeCalidadPorWorkflow x) => new { MaterialId = x.CaracteristicaDeCalidad.MaterialPorCentro.Material.Id, WorkflowId = x.Workflow.Id, CaracteristicaDeCalidadId = x.CaracteristicaDeCalidad.Id, Id = x.Id }, expresion);
            var relacionesstr = relaciones.GroupBy(x => new { x.MaterialId, x.WorkflowId }).Select(x => x.Key.MaterialId.ToString() + "|" + x.Key.WorkflowId.ToString()).ToList();
            Expression<Func<MaterialPorWorkflow, bool>> expresionMf = y => relacionesstr.Contains(SqlFunctions.StringConvert((double)y.Material.Id).Trim() + "|" + SqlFunctions.StringConvert((double)y.Workflow.Id).Trim());

            var retorno = Listar<MaterialPorWorkflow, CaracteristicaDeCalidadPorWorkflowDto>(expresionMf, paginacion);
            foreach (var item in retorno.Items)
            {
                var relacionesItem = relaciones.Where(x => x.MaterialId == item.MaterialId && x.WorkflowId == item.WorkflowId);
                item.Id = relacionesItem.First().Id;
                item.CaracteristicasDeCalidadId = relacionesItem.Select(x => x.CaracteristicaDeCalidadId).ToList();
            }
            return retorno;
        }

        public MaterialPorWorkflowDto ObtenerMaterialPorWorkflow(int id)
        {
            return Obtener<MaterialPorWorkflow, MaterialPorWorkflowDto>(id);
        }

        public CaracteristicaDeCalidadPorWorkflowDto ObtenerCaracteristicaDeCalidadPorWorkflow(int id)
        {
            var caracteristica = Obtener<CaracteristicaDeCalidadPorWorkflow, CaracteristicaDeCalidadPorWorkflowDto>(id);
            caracteristica.CaracteristicasDeCalidadId = repositorio.Listar(x => x.CaracteristicaDeCalidad.Id, (CaracteristicaDeCalidadPorWorkflow x) =>
                 x.Workflow.Id == caracteristica.WorkflowId && x.CaracteristicaDeCalidad.MaterialPorCentro.Material.Id ==
                 caracteristica.MaterialId).ToList();
            return caracteristica;
        }

        public LoteDto ObtenerLoteParaArchivo(int id)
        {
            var lote = repositorio.ObtenerProyeccion<Lote, LoteDto>(x => x.Id == id, x => new LoteDto
            {
                CamaraDesc = x.Camara.Descripcion,
                CamaraEmail = x.Camara.Email,
                CamaraFormatoDeArchivo = x.Camara.FormatoDeArchivo.Value,
                CamaraId = x.Camara.Id,
                Fecha = x.Fecha,
                Id = x.Id,
                NumeroDeLote = x.NumeroDeLote
            });

            lote.Muestras = repositorio.ListarConsulta(new ListarMuestraEnvioACamaraParaArchivoConsulta(id, firmaProvider.ObtenerFirmaSinLogo().CodigoSAP));

            return lote;
        }

        public IList<LoteDto> ListarLotesPorWorkflow(Guid workflowInstance)
        {
            return Listar<Lote, LoteDto>(f => f.Muestras.Any(m => m.Calado.WorkflowInstanceId == workflowInstance));
        }

        public IList<MaterialPorWorkflowDto> ListarMaterialesPorWorkflowYVinedo(int workflowId, int centroId,
                                                                                int vinedoId)
        {
            var cosecha = DateTime.Today.Year.ToString(CultureInfo.InvariantCulture);
            var variedadesPorVinedoId = repositorio.Listar<VariedadPorVinedo, int>(x => x.Variedad.Id,
                                                                                   x =>
                                                                                   x.Vinedo.Id == vinedoId &&
                                                                                   x.Cosecha == cosecha);

            return Listar<MaterialPorWorkflow, MaterialPorWorkflowDto>(
                f => f.Workflow.Id.Equals(workflowId) && f.Centro.Id.Equals(centroId) && f.Material.Activo
                     && variedadesPorVinedoId.Contains(f.Material.Variedad.Id));
        }

        public IList<MaterialPorWorkflowDto> ListarMaterialesPorWorkflow(int workflowId, int centroId)
        {
            var materiales = Listar<MaterialPorWorkflow, MaterialPorWorkflowDto>(
                f => f.Workflow.Id.Equals(workflowId) && f.Centro.Id.Equals(centroId) && f.Material.Activo && f.Cliente == null);
            var materialesId = materiales.Select(x => x.MaterialId).ToList();
            var materialesPorCentro =
                repositorio.Listar<MaterialPorCentro>(
                    x => materialesId.Contains(x.Material.Id) && x.Centro.Id == centroId);

            foreach (var material in materiales)
            {
                material.RequiereTecnologia =
                    materialesPorCentro.Any(x => x.Material.Id == material.MaterialId && x.RequiereTecnologia);
            }
            return materiales;
        }

        public IList<MaterialPorWorkflowDto> ListarMaterialesPorWorkflowCliente(int workflowId, int centroId, int clienteId)
        {
            var materiales = Listar<MaterialPorWorkflow, MaterialPorWorkflowDto>(
                f => f.Workflow.Id.Equals(workflowId) && f.Centro.Id.Equals(centroId) && f.Material.Activo && f.Cliente.Id.Equals(clienteId));

            if (materiales.Count == 0)
            {
                materiales = Listar<MaterialPorWorkflow, MaterialPorWorkflowDto>(
                f => f.Workflow.Id.Equals(workflowId) && f.Centro.Id.Equals(centroId) && f.Material.Activo && f.Cliente == null);
            }

            return materiales;
        }

        public IList<CategoriaDto> ListarCategorias()
        {
            return Listar<Categoria, CategoriaDto>(x => true, 50);
        }

        public IList<TipoBinDto> ListarMaterialesBin(int workflowId, int centroId, ClaseBin? claseBin)
        {
            Expression<Func<MaterialPorWorkflow, bool>> filtro;
            if (claseBin != null)
            {
                filtro = f => f.Workflow.Id == workflowId && f.Centro.Id == centroId && f.Material.Activo
                              && f.Material.UsaBinPallet && f.Material.Clase == claseBin;
            }
            else
            {
                filtro = f => f.Workflow.Id == workflowId && f.Centro.Id == centroId && f.Material.Activo
                              && f.Material.UsaBinPallet;
            }

            return Listar<MaterialPorWorkflow, TipoBinDto>(filtro);
        }

        public IList<MaterialDto> ListarMaterialesBinPallet()
        {
            return Listar<Material, MaterialDto>(x => x.UsaBinPallet);
        }

        public IList<MaterialPorWorkflowDto> BuscarMaterialesPorWorkflow(int workflowId, int centroId, string filtro)
        {
            return
                Listar<MaterialPorWorkflow, MaterialPorWorkflowDto>(
                    f =>
                    f.Workflow.Id.Equals(workflowId) && f.Centro.Id.Equals(centroId) &&
                    f.Material.Descripcion.Contains(filtro) && f.Material.Activo);
        }

        public MuestraEnvioACamaraDto ObtenerMuestraEnvioACamaraPorNumero(int centroId, string nroMuestra)
        {
            return repositorio.ListarConsulta(new ListarMuestraEnvioACamaraConsulta(centroId, nroMuestra, listarRechazadosYNoTerminados: true)).LastOrDefault();
        }

        public MuestraEnvioACamaraYRecorridoDto ObtenerMuestraEnvioACamaraYRecorridoPorNumero(string nroMuestra, int centroId, bool incluirPreLote = false)
        {
            var muestra = repositorio.ListarConsulta(new ListarMuestraEnvioACamaraConsulta(centroId, nroMuestra, listarRechazadosYNoTerminados: true, incluirPreLote: incluirPreLote)).LastOrDefault();
            var recorrido = muestra == null ? null :
                repositorio.ObtenerProyeccion((Recorrido x) => x.InstanciaWorkflow == muestra.WorkflowInstanceId,
                                              x =>
                                              new
                                              {
                                                  x.Terminado,
                                                  x.Rechazado,
                                                  x.Patente
                                              });
            return new MuestraEnvioACamaraYRecorridoDto
            {
                MuestraEnvioACamara = muestra,
                Patente = recorrido == null ? "" : recorrido.Patente,
                Terminado = recorrido != null && recorrido.Terminado,
                Rechazado = recorrido != null && recorrido.Rechazado
            };
        }

        public string ObtenerNumeroMuestraEnvioACamara(int muestraId)
        {
            return repositorio.ObtenerProyeccion<MuestraEnvioACamara, string>(x => x.Id == muestraId, x => x.NroMuestra);
        }

        public string ObtenerNumeroLote(int loteid)
        {
            return repositorio.ObtenerProyeccion<Lote, string>(x => x.Id == loteid, x => x.NumeroDeLote);
        }

        public string ObtenerNumeroLoteBiotecnologia(int loteId)
        {
            return repositorio.ObtenerProyeccion<LoteBiotecnologia, string>(x => x.Id == loteId, x => x.NumeroDeLote);
        }

        public string ObtenerNumeroLoteAuditoria(int loteId)
        {
            return repositorio.ObtenerProyeccion<LoteAuditoria, string>(x => x.Id == loteId, x => x.NumeroDeLote);
        }

        public string ObtenerArchivoDeMovimientos(int loteId)
        {
            return repositorio.ObtenerProyeccion<ArchivoDeMovimientos, string>(x => x.Id == loteId, x => x.NumeroDeArchivo);
        }

        public MuestraEnvioACamaraDto ObtenerMuestraEnvioACamaraPorCalado(int caladoId)
        {
            return Obtener<MuestraEnvioACamara, MuestraEnvioACamaraDto>(x => x.Calado.Id == caladoId);
        }

        public IList<ControlRecorridoDto> ListarControlRecorridoServiciosSap(Guid workflowId)
        {
            return
                Listar<ControlRecorrido, ControlRecorridoDto>(
                    x => x.WorkflowInstanceId == workflowId && x.Actividad.Contains("ServicioSap"));
        }

        public ControlRecorridoDto ObtenerControlRecorrido(Guid workflowId, string actividad)
        {
            var control = repositorio.ObtenerMayor<ControlRecorrido, int>(
                x => x.WorkflowInstanceId == workflowId && x.Actividad == actividad, x => x.Id);
            return conversor.Convertir<ControlRecorrido, ControlRecorridoDto>(control);
        }

        public IEnumerable<string> ObtenerGruposPorUsuario(string nombreUsuario)
        {
            var permisos = ListarPermisosPorUsuario(nombreUsuario);
            return permisos.Where(x => x.TipoPermiso == TipoPermiso.Grupo).Select(x => x.Descripcion);
        }

        public NotificacionesDto ObtenerNotificaciones(string grupos, bool listarSobre, bool mostrarAlerta, bool contar)
        {
            var notificaciones = new NotificacionesDto();
            if (listarSobre)
            {
                notificaciones.NotificacionesSobre = ListarNotificacionesPorGrupos(grupos.Split(','), 10);
            }
            if (mostrarAlerta)
            {
                notificaciones.AlertasNoLeidas = ListarAlertasNoLeidas(grupos.Split(','));
            }
            if (contar)
            {
                notificaciones.Cantidad = ContarNotificacionesNoLeidasPorGrupos(grupos.Split(','));
            }

            return notificaciones;
        }

        private IList<NotificacionDto> ListarAlertasNoLeidas(IEnumerable<string> grupos)
        {
            var lista =
                Listar<Notificacion, NotificacionDto>(
                    x => grupos.Contains(x.Grupo) && x.TipoAlerta != TipoAlerta.Sobre && !x.Leido, 30);
            return lista.OrderBy(x => x.TipoAlerta).ThenBy(x => x.Id).ToList();
        }

        private IList<NotificacionDto> ListarNotificacionesPorGrupos(IEnumerable<string> grupos, int cantidad)
        {
            //Uso el listar paginado para poder ordenar de forma invertida
            var lista = Listar<Notificacion, NotificacionDto>(x => grupos.Contains(x.Grupo),
                                                              new Paginacion("Id", DirOrden.Desc, 1, cantidad));

            foreach (var t in lista)
            {
                t.HoraServidor = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            }
            return lista.Items;
        }

        private int ContarNotificacionesNoLeidasPorGrupos(IEnumerable<string> grupos)
        {
            return repositorio.Contar<Notificacion>(x => grupos.Contains(x.Grupo) && x.Leido == false);
        }

        public IList<MuestraEnvioACamaraDto> ListarMuestraEnvioACamaraSinLote(int centroId, bool incluirPreLote)
        {
            return repositorio.ListarConsulta(new ListarMuestraEnvioACamaraConsulta(centroId, soloPendientes: true, incluirPreLote: incluirPreLote));
        }

        public IList<MuestraEnvioACamaraBiotecnoligiaDto> ListarMuestraEnvioACamaraBiotecnologiaSinLote(int materialId, int camaraId, int centroId)
        {
            var muestras = repositorio.ListarConsulta(new ListarMuestraEnvioACamaraIntactaPendientesConsulta(materialId, camaraId, centroId));
            return muestras;
        }

        public List<MovimientoDeTercerosListaDto> ListarMovimientoDeTercerosSinArchivo(int materialId, TipoDeWorkflow tipoDeWorkflow, int centroId)
        {
            var muestras = repositorio.ListarConsulta(new ListarMovimientoDeTercerosPendientesConsulta(materialId, tipoDeWorkflow, centroId));
            return muestras;
        }

        public ListaPaginada<ConversionMaterialDto> ListarPaginadoConversionMaterial(int? camaraId, Paginacion paginacion)
        {
            if (camaraId != null)
            {
                return Listar<ConversionMaterial, ConversionMaterialDto>(x => x.Camara.Id == camaraId, paginacion);
            }
            return new ListaPaginada<ConversionMaterialDto>(new List<ConversionMaterialDto>(), 1,
                                                            paginacion.ItemsPorPagina, 0);
        }

        public ListaPaginada<ConversionCentroDto> ListarPaginadoConversionCentro(int? camaraId,
                                                                                     Paginacion paginacion)
        {
            if (camaraId != null)
            {
                return Listar<ConversionCentro, ConversionCentroDto>(x => x.Camara.Id == camaraId, paginacion);
            }
            return new ListaPaginada<ConversionCentroDto>(new List<ConversionCentroDto>(), 1,
                                                            paginacion.ItemsPorPagina, 0);
        }

        public ConversionMaterialDto ObtenerConversionMaterial(int camaraId, int materialId)
        {
            return
                ObtenerPrimero<ConversionMaterial, ConversionMaterialDto>(
                    x => x.Camara.Id.Equals(camaraId) && x.Material.Id.Equals(materialId));
        }

        public ConversionCentroDto ObtenerConversionCentro(int camaraId, int centroId)
        {
            return
                ObtenerPrimero<ConversionCentro, ConversionCentroDto>(
                    x => x.Camara.Id.Equals(camaraId) && x.Centro.Id.Equals(centroId));
        }

        public ListaPaginada<ConversionProcedenciaDto> ListarPaginadoConversionProcedencia(Paginacion paginacion,
                                                                                           int? camaraId)
        {
            if (camaraId != null)
            {
                return Listar<ConversionProcedencia, ConversionProcedenciaDto>(x => x.Camara.Id == camaraId.Value,
                                                                               paginacion);
            }
            return new ListaPaginada<ConversionProcedenciaDto>(new List<ConversionProcedenciaDto>(), 1,
                                                               paginacion.ItemsPorPagina, 0);
        }

        public ListaPaginada<ConversionCaracteristicaDto> ListarPaginadoConversionCaracteristica(Paginacion paginacion,
                                                                                                 int? camaraId,
                                                                                                 int? materialId)
        {
            int filtro;
            int.TryParse(camaraId.ToString(), out filtro);
            if (camaraId != null && materialId != null)
            {
                return
                    Listar<ConversionCaracteristica, ConversionCaracteristicaDto>(
                        x => x.Camara.Id == camaraId && x.Material.Id == materialId, paginacion);
            }
            if (camaraId != null)
            {
                return Listar<ConversionCaracteristica, ConversionCaracteristicaDto>(x => x.Camara.Id == camaraId,
                                                                                     paginacion);
            }
            if (materialId != null)
            {
                return Listar<ConversionCaracteristica, ConversionCaracteristicaDto>(x => x.Material.Id == materialId,
                                                                                     paginacion);
            }
            return new ListaPaginada<ConversionCaracteristicaDto>(new List<ConversionCaracteristicaDto>(), 1,
                                                                  paginacion.ItemsPorPagina, 0);
        }

        public ListaPaginada<ConversionGrupoDto> ListarPaginadoConversionGrupo(Paginacion paginacion, int? camaraId)
        {
            if (camaraId != null)
            {
                return Listar<ConversionGrupo, ConversionGrupoDto>(x => x.Camara.Id == camaraId, paginacion);
            }
            return new ListaPaginada<ConversionGrupoDto>(new List<ConversionGrupoDto>(), 1, paginacion.ItemsPorPagina, 0);
        }

        public ConversionGrupoDto ObtenerConversionGrupo(int camaraId, int materialId)
        {
            return
                ObtenerPrimero<ConversionGrupo, ConversionGrupoDto>(
                    f => f.Camara.Id == camaraId && f.Material.Id == materialId);
        }

        public ConversionCaracteristicaDto ObtenerConversionCaracteristica(int camaraId, int caracteristicaId)
        {
            return
                ObtenerPrimero<ConversionCaracteristica, ConversionCaracteristicaDto>(
                    f => f.Caracteristica.Id == caracteristicaId && f.Camara.Id == camaraId);
        }

        public IList<CalleDto> ListarCalles(int centroId)
        {
            return Listar<Calle, CalleDto>(x => x.CentroId.Equals(centroId) && x.TipoCalle == TipoCalle.PlayaInterna && !x.Deshabilitada);
        }

        public IList<PuestosDeCargaDescargaDto> ListarHidraulicas(int centroId, bool esSustentable)
        {
            return
                Listar<PuestosDeCargaDescarga, PuestosDeCargaDescargaDto>(
                    x => x.Centro.Id == centroId && x.EsSojaSustentable == esSustentable);
        }

        public CalleDto ObtenerCalle(int id)
        {
            return Obtener<Calle, CalleDto>(id);
        }

        public string ObtenerCalleNombre(int id)
        {
            var calle = repositorio.ObtenerProyeccion<CallePorRecorrido, string>(x => x.Id == id, x => x.Calle.Nombre);
            return calle ?? "";
        }

        public PuestosDeCargaDescargaDto ObtenerHidraulica(int id)
        {
            return Obtener<PuestosDeCargaDescarga, PuestosDeCargaDescargaDto>(id);
        }

        public string ObtenerHidraulicaNombre(int id)
        {
            var hidraulica = repositorio.Obtener<PuestosDeCargaDescarga>(id);
            return hidraulica != null ? hidraulica.Nombre : "";
        }

        public RomaneoDto ObtenerRomaneo(int id)
        {
            return Obtener<Romaneo, RomaneoDto>(id);
        }

        public Guid ObtenerInstanceIdPorPatente(string patente) // TODO: Revisar si se usa, ya que el método fallará por el SingleOrDefault
        {
            return Obtener<Recorrido, RecorridoDto>(x => x.Patente == patente).InstanciaWorkflow;
        }

        public RomaneoDto ObtenerRomaneoProveedor(int id)
        {
            var romaneo = Obtener<Romaneo, RomaneoDto>(id);
            if (romaneo != null)
            {
                var documento = ObtenerOrdenDeDescargaPorInstanceId(romaneo.WorkflowInstanceId);
                if (documento == null)
                {
                    return null;
                }
                if (documento.ProveedorId != romaneo.ProveedorId)
                {
                    romaneo.ProveedorDescripcion = "distintos^-" + romaneo.ProveedorDescripcion;
                }
            }
            return romaneo;
        }

        public RomaneoDto ObtenerUltimoRomaneoPorGuid(Guid guid)
        {
            var romaneo =
                Listar<Romaneo, RomaneoDto>(r => r.WorkflowInstanceId == guid && r.Estado != EstadoRomaneo.Finalizado)
                    .LastOrDefault();
            var documento = ObtenerOrdenDeDescargaPorInstanceId(guid);
            if (romaneo == null || documento == null)
            {
                return null;
            }
            if (documento.ProveedorId != romaneo.ProveedorId)
            {
                romaneo.ProveedorDescripcion = "distintos^-" + romaneo.ProveedorDescripcion;
            }
            return romaneo;
        }

        public IList<RomaneoDto> ObtenerRomaneosPorGuid(Guid guid)
        {
            return Listar<Romaneo, RomaneoDto>(r => r.WorkflowInstanceId == guid);
        }

        public RomaneoDto ObtenerRomaneoPorNroPedido(string nroPedido)
        {
            return Listar<Romaneo, RomaneoDto>(r => r.NroPedido == nroPedido).LastOrDefault();
        }

        public IList<RomaneoItemDto> ObtenerItemRomaneosPorRomaneoId(int romaneoId)
        {
            return Listar<RomaneoItem, RomaneoItemDto>(r => r.Romaneo.Id == romaneoId);
        }

        public ProveedorDto ObtenerProveedor(int id)
        {
            return Obtener<Proveedor, ProveedorDto>(id);
        }

        public ProveedorDto ObtenerProveedorPorCuit(string cuit, TiposProveedor tipo)
        {
            return Obtener<Proveedor, ProveedorDto>(x => x.Cuil == cuit
                                                         && (!tipo.PR || x.PR) && (!tipo.AM || x.AM) &&
                                                         (!tipo.CM || x.CM) && x.Activo);
        }

        public ProveedorDto ObtenerProveedorPorCodigoSap(string codigoSap)
        {
            return Obtener<Proveedor, ProveedorDto>(x => x.CodigoSap == codigoSap);
        }

        public ControlDeBalanzaDto ObtenerControlDeBalanza(int id)
        {
            return Obtener<ControlDeBalanza, ControlDeBalanzaDto>(x => x.Id == id);
        }

        public ControlDeBalanzaDto ObtenerControlDeBalanzaPorRecorrido(int recorridoId)
        {
            return ObtenerUltimo<ControlDeBalanza, ControlDeBalanzaDto>(x => x.Recorrido.Id == recorridoId, y => y.Id);
        }

        public IList<ControlDeBalanzaPesadaDto> ListarControlDeBalanza(DateTime desde, DateTime hasta)
        {
            return
                Listar<ControlDeBalanzaPesada, ControlDeBalanzaPesadaDto>(x => x.Fecha >= desde && x.Fecha <= hasta)
                    .OrderBy(o => o.Fecha)
                    .ToList();
        }

        public DocumentoDeImpresionPorCentroDto ObtenerDocumentoDeImpresionPorCentro(int id)
        {
            return Obtener<DocumentoDeImpresionPorCentro, DocumentoDeImpresionPorCentroDto>(id);
        }

        public string ObtenerProveedorPorNroPedidoEnRomaneo(string numero, Guid instanceId)
        {
            var romaneo =
                Listar<Romaneo, RomaneoDto>(x => x.NroPedido == numero && x.WorkflowInstanceId == instanceId)
                    .LastOrDefault();
            if (romaneo == null)
            {
                return "";
            }

            var documento = ObtenerOrdenDeDescargaPorInstanceId(romaneo.WorkflowInstanceId);
            if (documento == null)
            {
                return romaneo.ProveedorDescripcion;
            }
            return documento.ProveedorId != romaneo.ProveedorId
                       ? "distintos^-" + romaneo.ProveedorDescripcion
                       : romaneo.ProveedorDescripcion;
        }

        public string ObtenerProveedorPorNroPedidoEnDescargaUnidad(string numero, Guid instanceId)
        {
            var descargaUnidadDto =
                Listar<DescargaUnidad, DescargaUnidadDto>(
                    x => x.NroPedido == numero && x.WorkflowInstanceId == instanceId)
                    .LastOrDefault();
            if (descargaUnidadDto == null)
            {
                return "";
            }

            var documento = ObtenerOrdenDeDescargaPorInstanceId(descargaUnidadDto.WorkflowInstanceId);
            if (documento == null)
            {
                return descargaUnidadDto.ProveedorDescripcion;
            }
            return documento.ProveedorId != descargaUnidadDto.ProveedorId
                       ? "distintos^-" + descargaUnidadDto.ProveedorDescripcion
                       : descargaUnidadDto.ProveedorDescripcion;
        }

        public OrdenDeDescargaDto ObtenerOrdenDeDescargaPorInstanceId(Guid instanceId)
        {
            var numeroDocumento =
                repositorio.ObtenerProyeccion<Recorrido, string>(
                    r =>
                    r.InstanciaWorkflow == instanceId && r.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenDeDescarga,
                    r => r.NumeroDocumentoIngreso);
            if (numeroDocumento != null)
            {
                return
                    ObtenerPrimero<OrdenDeDescarga, OrdenDeDescargaDto>(c => c.Numero == numeroDocumento);
            }
            return null;
        }

        public OrdenDeDescargaFasonDto ObtenerOrdenDeDescargaFasonPorInstanceId(Guid instanceId)
        {
            var numeroDocumento =
                repositorio.ObtenerProyeccion<Recorrido, string>(
                    r =>
                    r.InstanciaWorkflow == instanceId &&
                    r.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenDeDescargaFason,
                    r => r.NumeroDocumentoIngreso);
            if (numeroDocumento != null)
            {
                return
                    ObtenerPrimero<OrdenDeDescargaFason, OrdenDeDescargaFasonDto>(c => c.Numero == numeroDocumento);
            }
            return null;
        }

        public OrdenDeDescargaDto ObtenerOrdenDeDescargaPorNumero(string numero)
        {
            return ObtenerPrimero<OrdenDeDescarga, OrdenDeDescargaDto>(c => c.Numero == numero);
        }

        public OrdenDeDescargaFasonDto ObtenerOrdenDeDescargaFasonPorNumero(string numero)
        {
            return ObtenerPrimero<OrdenDeDescargaFason, OrdenDeDescargaFasonDto>(c => c.Numero == numero);
        }

        public OrdenCargaFasDto ObtenerOrdenCargaFas(int id)
        {
            return Obtener<OrdenCargaFas, OrdenCargaFasDto>(id);
        }

        public OrdenCargaFasDto ObtenerOrdenCargaFasPorInstanceId(Guid instanceId)
        {
            var orden = Obtener<OrdenCargaFas, OrdenCargaFasDto>(c => c.Recorrido.InstanciaWorkflow == instanceId);
            var cartaPorteDerivadoGranario = repositorio.Obtener<CartaPorteDerivadoGranario>(x => x.Recorrido.InstanciaWorkflow == instanceId);
            if (cartaPorteDerivadoGranario != null)
            {
                orden.NumeroCTG = cartaPorteDerivadoGranario.NroCTG;
                orden.NumeroCPE = string.Concat(cartaPorteDerivadoGranario.Sucursal, "-", cartaPorteDerivadoGranario.NroOrden);
            }
            return orden;
        }

        public decimal ObtenerPesoNetoRomaneo(Guid instanceId)
        {
            return repositorio.ObtenerConsultaEscalar(new PesoNetoRomaneoConsulta(instanceId));
        }

        public ListaPaginada<ImpresionDto> ListarImpresiones(TipoDocumentoIngreso? tipo, string numeroDocumentoIngreso,
                                                             string patente, TipoImpresion? tipoImpresion,
                                                             Paginacion paginacion)
        {
            try
            {
                return
                repositorio.ListarConsultaPaginada(new ListarImpresiones(tipo, numeroDocumentoIngreso, patente, tipoImpresion,
                                                                         paginacion));
            }
            catch (Exception e)
            {
                log.Error(e, "Error ListarImpresiones DocumentoConsultado : {0}", numeroDocumentoIngreso);
                throw e;
            }
        }

        public VehiculoDto ObtenerVehiculoPorGuid(Guid instanceId)
        {
            var vehiculo = repositorio.ObtenerProyeccion<Recorrido, Vehiculo>(x => x.InstanciaWorkflow == instanceId, x => x.Vehiculo);

            return vehiculo != null ? conversor.Convertir<Vehiculo, VehiculoDto>(vehiculo) : null;
        }

        public IList<FormatoDePapelDto> ListarFormatoDePapel()
        {
            return Listar<FormatoDePapel, FormatoDePapelDto>();
        }

        public IList<ImpresoraDto> ListarImpresoras(int centroId)
        {
            return Listar<Impresora, ImpresoraDto>(x => x.Centro.Id == centroId);
        }

        public ListaPaginada<ImpresoraDto> ListarPaginadoImpresoras(int centroId, string filtro, Paginacion paginacion)
        {
            Expression<Func<Impresora, bool>> expresion = x => x.Centro.Id == centroId;
            if (filtro != null)
            {
                expresion = (x => ((x.Descripcion.Contains(filtro))
                                         || (x.Direccion.Contains(filtro))
                                        && (x.Centro.Id == centroId)));
            }
            else
            {
                expresion = x => x.Centro.Id == centroId;
            }
            return Listar<Impresora, ImpresoraDto>(expresion, paginacion);
        }

        public ListaPaginada<FormatoDeImpresionDto> ListarPaginadoFormatosDeImpresion(string filtro,
                                                                                      Paginacion paginacion)
        {
            Expression<Func<FormatoDeImpresion, bool>> expresionFiltro = null;
            if (filtro != null)
            {
                expresionFiltro = (x => x.Descripcion.Contains(filtro));
            }

            return Listar<FormatoDeImpresion, FormatoDeImpresionDto>(expresionFiltro, paginacion);
        }

        public IList<FormatoDeImpresionDto> ListarFormatosDeImpresion()
        {
            return Listar<FormatoDeImpresion, FormatoDeImpresionDto>();
        }

        public IList<DocumentoDeImpresionDto> ListarDocumentosDeImpresion()
        {
            return Listar<DocumentoDeImpresion, DocumentoDeImpresionDto>();
        }

        public IList<FormatoDePapelDto> ListarFormatosDePapel()
        {
            return Listar<FormatoDePapel, FormatoDePapelDto>();
        }

        public FormatoDeImpresionDto ObtenerFormatoDeImpresion(int id)
        {
            return Obtener<FormatoDeImpresion, FormatoDeImpresionDto>(id);
        }

        public IList<CampoDto> ListarCampos()
        {
            return Listar<Campo, CampoDto>();
        }

        public IList<LetraDto> ListarLetras()
        {
            return Listar<Letra, LetraDto>();
        }

        public IList<FormatoDeCampoDto> ListarFormatosDeCampo(int id)
        {
            return Listar<FormatoDeCampo, FormatoDeCampoDto>(x => x.FormatoDeImpresion.Id == id);
        }

        public ImpresoraDto ObtenerImpresora(int id)
        {
            return Obtener<Impresora, ImpresoraDto>(id);
        }

        public DocumentoDeImpresionPorCentroDto ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(string codigo, int centroId, int puestoDeTrabajoId)
        {
            DocumentoDeImpresionPorCentroDto documento = null;
            if (puestoDeTrabajoId != 0)
            {
                documento = Obtener<DocumentoDeImpresionPorCentro, DocumentoDeImpresionPorCentroDto>(x => x.DocumentoDeImpresion.Codigo == codigo && x.Centro.Id == centroId && x.PuestoDeTrabajo.Id == puestoDeTrabajoId);
            }
            return documento ?? Obtener<DocumentoDeImpresionPorCentro, DocumentoDeImpresionPorCentroDto>(x => x.DocumentoDeImpresion.Codigo == codigo && x.Centro.Id == centroId && x.PuestoDeTrabajo == null);
        }

        public bool ExisteTransaccionSAP(int materialId, int centroId, FuncionSAP funcion, Guid instanceId)
        {
            var tipoComercialId =
                repositorio.Obtener<Recorrido>(r => r.InstanciaWorkflow == instanceId).TipoComercial.Id;

            return repositorio.Existe<TransaccionSAP>(x =>
                                                      x.Material.Id == materialId
                                                      && x.TipoComercial.Id == tipoComercialId
                                                      && x.CentroOrigen.Id == centroId
                                                      && x.FuncionSAP == funcion);
        }

        public bool ValidacionCtgEsAutomatica(int centroId)
        {
            var centro = repositorio.Obtener<Centro>(centroId);
            return centro == null || !centro.SolicitaConfirmarCTG;
        }

        public bool ValidacionCtgEsManual(int centroId)
        {
            var centro = repositorio.Obtener<Centro>(centroId);
            return centro == null || !centro.EncolaBajaCtgAutomatico;
        }

        public DescargaUnidadDto ObtenerUltimaDescargaUnidadPorGuid(Guid guid)
        {
            var descargaUnidad =
                Listar<DescargaUnidad, DescargaUnidadDto>(
                    r => r.WorkflowInstanceId == guid && r.Estado != EstadoDescargaUnidad.Finalizado)
                    .LastOrDefault();
            var documento = ObtenerOrdenDeDescargaPorInstanceId(guid);
            if (descargaUnidad == null || documento == null)
            {
                return null;
            }
            if (documento.ProveedorId != descargaUnidad.ProveedorId)
            {
                descargaUnidad.ProveedorDescripcion = "distintos^-" + descargaUnidad.ProveedorDescripcion;
            }
            return descargaUnidad;
        }

        public DescargaUnidadDto ObtenerDescargaUnidadPorNroPedido(string nroPedido)
        {
            DescargaUnidadDto descargaUnidad =
                Listar<DescargaUnidad, DescargaUnidadDto>(r => r.NroPedido == nroPedido).LastOrDefault();
            return descargaUnidad;
        }

        public DescargaUnidadDto ObtenerDescargaUnidadProveedor(int id)
        {
            var descargaUnidad = Obtener<DescargaUnidad, DescargaUnidadDto>(id);
            var documento = ObtenerOrdenDeDescargaPorInstanceId(descargaUnidad.WorkflowInstanceId);
            if (descargaUnidad == null || documento == null)
            {
                return null;
            }
            if (documento.ProveedorId != descargaUnidad.ProveedorId)
            {
                descargaUnidad.ProveedorDescripcion = "distintos^-" + descargaUnidad.ProveedorDescripcion;
            }
            return descargaUnidad;
        }

        public IList<DescargaUnidadDto> ObtenerDescargaUnidadPorGuid(Guid guid)
        {
            return Listar<DescargaUnidad, DescargaUnidadDto>(r => r.WorkflowInstanceId == guid);
        }

        public DescargaUnidadDto ObtenerDescargaUnidad(int id)
        {
            return Obtener<DescargaUnidad, DescargaUnidadDto>(id);
        }

        public decimal ObtenerPesoNetoDescargaUnidad(Guid instanceId)
        {
            return repositorio.ObtenerConsultaEscalar(new PesoNetoDescargaUnidadConsulta(instanceId));
        }

        public string ObtenerNumeroOrdenDeCargaContenedorGenerado(int centroId)
        {
            var codigoCentro = ObteneCodigoSapPorCentroId(centroId) ?? "";
            var numero = repositorio.ObtenerNumeroOrdenDeCargaContenedorGenerado();
            return codigoCentro + "-" + numero.ToString(CultureInfo.InvariantCulture).PadLeft(8, '0');
        }

        public OrdenDeCargaContenedorDto ObtenerOrdenDeCargaContenedorPorNumeroDeOrden(string numero)
        {
            return
                ObtenerPrimero<OrdenDeCargaContenedor, OrdenDeCargaContenedorDto>(
                    x => x.NroOrdenDeCargaContenedor == numero);
        }

        public OrdenDeCargaContenedorDto ObtenerOrdenDeCargaContenedor(int id)
        {
            return Obtener<OrdenDeCargaContenedor, OrdenDeCargaContenedorDto>(id);
        }

        public OrdenDeCargaContenedorDto ObtenerOrdenDeCargaContenedorPorInstanceId(Guid instanceId)
        {
            return
                ObtenerPrimero<OrdenDeCargaContenedor, OrdenDeCargaContenedorDto>(
                    x => x.Recorrido.InstanciaWorkflow == instanceId);
        }

        public ListaPaginada<TaraContenedorDto> ListarPaginadoTaraContenedor(string filtro, Paginacion paginacion)
        {
            Expression<Func<TaraContenedor, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro = x => x.Descripcion.Contains(filtro) || x.CodigoContenedor.Contains(filtro);
            }

            return Listar<TaraContenedor, TaraContenedorDto>(expresionFiltro, paginacion);
        }

        public TaraContenedorDto ObtenerTaraContenedor(int id)
        {
            return Obtener<TaraContenedor, TaraContenedorDto>(id);
        }

        public IList<TaraContenedorDto> ListarTaraContenedores()
        {
            return Listar<TaraContenedor, TaraContenedorDto>();
        }

        public ListaPaginada<CalleDto> ListarPaginadoCalle(int centroId, Paginacion paginacion)
        {
            Expression<Func<Calle, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(centroId.ToString(CultureInfo.InvariantCulture)))
            {
                expresionFiltro = x => x.CentroId.Equals(centroId);
            }

            return Listar<Calle, CalleDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<TarjetaBloqueadaDto> ListarPaginadoTarjetasBloqueadas(string filtro, Paginacion paginacion,
                                                                                   int centroId)
        {
            Expression<Func<TarjetaBloqueada, bool>> expresionFiltro;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = (x => x.Numero.Contains(filtro) && x.Centro.Id == centroId);
            }
            else
            {
                expresionFiltro = x => x.Centro.Id == centroId;
            }
            return Listar<TarjetaBloqueada, TarjetaBloqueadaDto>(expresionFiltro, paginacion);
        }

        public TarjetaBloqueadaDto ObtenerTarjetaBloqueada(int id)
        {
            return Obtener<TarjetaBloqueada, TarjetaBloqueadaDto>(id);
        }

        public ListaPaginada<TarjetaRangoDto> ListarPaginadoTarjetasRango(string filtro, Paginacion paginacion,
                                                                          int centroId)
        {
            Expression<Func<TarjetaRango, bool>> expresionFiltro;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro =
                    x =>
                    (x.Codigo.Contains(filtro) ||
                     (String.Compare(filtro, x.RangoDesde, StringComparison.Ordinal) >= 0 &&
                      String.Compare(filtro, x.RangoHasta, StringComparison.Ordinal) <= 0)) && x.Centro.Id == centroId;
            }
            else
            {
                expresionFiltro = x => x.Centro.Id == centroId;
            }
            return Listar<TarjetaRango, TarjetaRangoDto>(expresionFiltro, paginacion);
        }

        public TarjetaRangoDto ObtenerTarjetaRango(int id)
        {
            return Obtener<TarjetaRango, TarjetaRangoDto>(id);
        }

        public ListaPaginada<TarjetaSupervisorDto> ListarPaginadoTarjetasSupervisor(string filtro, Paginacion paginacion,
                                                                                    int centroId)
        {
            Expression<Func<TarjetaSupervisor, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = (x => x.Numero.Contains(filtro) && x.Centro.Id == centroId);
            }
            else
            {
                expresionFiltro = x => x.Centro.Id == centroId;
            }
            return Listar<TarjetaSupervisor, TarjetaSupervisorDto>(expresionFiltro, paginacion);
        }

        public TarjetaSupervisorDto ObtenerTarjetaSupervisor(int id)
        {
            return Obtener<TarjetaSupervisor, TarjetaSupervisorDto>(id);
        }

        public ReciboMunicipalDto ObtenerReciboMunicipal(int id)
        {
            return Obtener<ReciboMunicipal, ReciboMunicipalDto>(id);
        }

        public ListaPaginada<PuestosDeCargaDescargaDto> ListarPaginadoHidraulica(int centroId, Paginacion paginacion)
        {
            Expression<Func<PuestosDeCargaDescarga, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(centroId.ToString()))
            {
                expresionFiltro = x => x.Centro.Id == centroId;
            }

            return Listar<PuestosDeCargaDescarga, PuestosDeCargaDescargaDto>(expresionFiltro, paginacion);
        }

        public IList<LectorDto> ListarLectores()
        {
            return Listar<Lector, LectorDto>();
        }

        public PuestoDeTrabajoDto ObtenerPuestoDeTrabajo(int id)
        {
            return Obtener<PuestoDeTrabajo, PuestoDeTrabajoDto>(id);
        }

        public string ObtenerNumGaritaEntrada(int id)
        {
            var nombrePuesto = repositorio.ObtenerProyeccion<PuestoDeTrabajo, string>(x => x.Id == id, x => x.NombrePuesto);
            if (string.IsNullOrEmpty(nombrePuesto))
            {
                return "1";
            }
            else
            {
                var numeroPuesto = string.Join("", nombrePuesto.ToCharArray().Where(Char.IsDigit));
                return (numeroPuesto != "") ? numeroPuesto : "1";
            }
        }

        public PuestoDeTrabajoDto ObtenerPuestoDeTrabajoPorNombrePc(string nombrePc, int centroId)
        {
            return
                ObtenerPrimero<PuestoDeTrabajo, PuestoDeTrabajoDto>(
                    x => x.NombrePc == nombrePc && x.Centro.Id == centroId);
        }

        public bool RedireccionarAListaAutomatizada(string nombrePc, int centroId)
        {
            return repositorio.Existe<PuestoDeTrabajo>(x => x.NombrePc == nombrePc && x.Centro.Id == centroId && !x.InvisibleEnListaDeTareas);
        }

        public bool RedireccionarABalanzaAutomatizada(string nombrePc, int centroId)
        {
            return repositorio.Existe<PuestoDeTrabajo>(x => x.NombrePc == nombrePc && x.Centro.Id == centroId && x.AutomatizadoFull && x.Balanza != null);
        }

        public IList<VagonDto> ListarVagonesEnPesada(int centroId)
        {
            return repositorio.ListarConsulta(new ListarVagonesEnPesada(centroId));
        }

        public PuestoDeTrabajoDto ObtenerPuestoDeTrabajoPorDispositivo(string codigoDispositivo)
        {
            return Obtener<PuestoDeTrabajo, PuestoDeTrabajoDto>(x => x.Lector == codigoDispositivo);
        }

        public IList<PuestoDeTrabajoDto> ListarPuestosDeTrabajo()
        {
            return Listar<PuestoDeTrabajo, PuestoDeTrabajoDto>();
        }

        public ListaPaginada<PuestoDeTrabajoDto> ListarPaginadoPuestosDeTrabajo(string filtro, int centroId,
                                                                                Paginacion paginacion)
        {
            Expression<Func<PuestoDeTrabajo, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = x => (x.NombrePuesto.Contains(filtro) || x.NombrePc.Contains(filtro)
                                        || x.Lector.Contains(filtro) || x.Entrada.Contains(filtro)) &&
                                       x.Centro.Id == centroId;
            }
            else
            {
                expresionFiltro = x => x.Centro.Id == centroId;
            }

            return Listar<PuestoDeTrabajo, PuestoDeTrabajoDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<PuestoDeTrabajoContingenciaDto> ListarPaginadoPuestosDeTrabajoContingencia(string filtro, int centroId,
                                                                                Paginacion paginacion)
        {
            Expression<Func<PuestoDeTrabajo, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = x => (x.NombrePuesto.Contains(filtro) || x.NombrePc.Contains(filtro)
                                        || x.Lector.Contains(filtro) || x.Entrada.Contains(filtro)) &&
                                       x.Centro.Id == centroId;
            }
            else
            {
                expresionFiltro = x => x.Centro.Id == centroId;
            }

            return Listar<PuestoDeTrabajo, PuestoDeTrabajoContingenciaDto>(expresionFiltro, paginacion);
        }

        public int ObtenerNumeroAleatorio()
        {
            return repositorio.ObtenerNumeroAleatorio();
        }

        public AlmacenDto ObtenerAlmacenPredeterminado(int centroId, int materialId)
        {
            return
                conversor.Convertir<Almacen, AlmacenDto>(
                    repositorio.ObtenerMayor<MaterialPorCentro, int, Almacen>(
                        x => x.Centro.Id == centroId && x.Material.Id == materialId, x => x.Id,
                        x => x.AlmacenPredeterminado));
        }

        public IList<ListadoDeCalidadesDto> ListarListadoDeCalidades(List<int> centros, DateTime fechaInicio,
                                                                     DateTime fechaFin, List<int> tiposComerciales,
                                                                     int material)
        {
            log.Debug("Inicio - ListarListadoDeCalidades");
            fechaFin = fechaFin.FinDelDia();
            fechaInicio = fechaInicio.InicioDelDia();
            return
                repositorio.ListarConsulta(new ListarListadoDeCalidades(centros, fechaInicio, fechaFin, tiposComerciales,
                                                                        material));
        }

        public IList<MuestraDeHumedadDto> ListarListadoDeMuestrasDeHumedad(int? centro, int? humedimetro,
                                                                           DateTime fechaDesde,
                                                                           DateTime fechaHasta)
        {
            log.Debug("Inicio - ListarListadoMuestrasDeHumedad");

            return repositorio.ListarConsulta(new ListarMuestrasDeHumedad(centro, humedimetro, fechaDesde, fechaHasta));
        }

        public ListaPaginada<ControlDeTiempoDto> ListarPaginadoControlesDeTiempo(Paginacion paginacion, int centroId)
        {
            return Listar<ControlDeTiempo, ControlDeTiempoDto>(x => x.Workflow.Centro.Id == centroId, paginacion);
        }

        public AutorizacionTiempoEnTransitoDto ObtenerAutorizacionTiempoEnTransito(Guid instanceId, string codigoControl, string nombreUsuario)
        {
            var recorrido = repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == instanceId);
            var controlDeTiempo = repositorio.Obtener<ControlDeTiempo>(x => x.CodigoControl == codigoControl && x.Workflow.Id == recorrido.Workflow.Id);
            AutorizacionTiempoEnTransitoDto control;
            if (controlDeTiempo != null)
            {
                var logActividadHasta = repositorio.ObtenerMayor<LogActividad, int>(
                                    x => x.WorkflowInstanceId == instanceId && x.ActividadXaml == controlDeTiempo.ActividadHasta, x => x.Id);
                var logActividadDesde = repositorio.ObtenerMayor<LogActividad, int>(
                                    x => x.WorkflowInstanceId == instanceId && x.ActividadXaml == controlDeTiempo.ActividadDesde, x => x.Id);
                var tiempoEnTransito = (logActividadDesde != null && logActividadHasta != null)
                                            ? logActividadHasta.Fecha - logActividadDesde.Fecha : TimeSpan.Zero;
                control = new AutorizacionTiempoEnTransitoDto
                {
                    WorkflowInstanceId = instanceId,
                    NombreUsuario = nombreUsuario,
                    Actividad = Textos.Actividad_AutorizarTiempoEnTransito,
                    Actividad1 = Textos.ResourceManager.GetString("Act" + controlDeTiempo.ActividadDesde) ?? controlDeTiempo.ActividadDesde,
                    Actividad2 = Textos.ResourceManager.GetString("Act" + controlDeTiempo.ActividadHasta) ?? controlDeTiempo.ActividadHasta,
                    DocumentoIngreso = recorrido.TipoDocumentoIngreso.DisplayEnum(),
                    Material = recorrido.Material != null ? recorrido.Material.Descripcion : string.Empty,
                    NroDocumentoIngreso = recorrido.NumeroDocumentoIngreso,
                    Patente = recorrido.Patente,
                    TiempoEnTransito = tiempoEnTransito.Formatted(),
                    TiempoAceptado = TimeSpan.FromMinutes(controlDeTiempo.TiempoMaximo).Formatted(),
                    FechaActividad1 = logActividadDesde != null ? logActividadDesde.Fecha.FormattedTime() : string.Empty,
                    FechaActividad2 = logActividadHasta != null ? logActividadHasta.Fecha.FormattedTime() : string.Empty,
                    DiferenciaTiempo = (tiempoEnTransito - TimeSpan.FromMinutes(controlDeTiempo.TiempoMaximo)).Formatted()
                };
                if (logActividadDesde == null || logActividadHasta == null)
                {
                    control.Error = string.Format(Textos.Error_ControlDeTiempoActividades, recorrido.DatosProximaActividad, recorrido.Workflow.Descripcion);
                }
                else if (tiempoEnTransito < TimeSpan.Zero)
                {
                    control.Error = string.Format(Textos.Error_TiempoEntreActividades, recorrido.DatosProximaActividad);
                }
            }
            else
            {
                control = new AutorizacionTiempoEnTransitoDto
                {
                    WorkflowInstanceId = instanceId,
                    NombreUsuario = nombreUsuario,
                    Actividad = Textos.Actividad_AutorizarTiempoEnTransito,
                    Actividad1 = Textos.NoDisponible,
                    Actividad2 = Textos.NoDisponible,
                    DocumentoIngreso = recorrido.TipoDocumentoIngreso.DisplayEnum(),
                    Material = recorrido.Material != null ? recorrido.Material.Descripcion : Textos.NoDisponible,
                    NroDocumentoIngreso = recorrido.NumeroDocumentoIngreso,
                    Patente = recorrido.Patente,
                    TiempoEnTransito = Textos.NoDisponible,
                    TiempoAceptado = Textos.NoDisponible,
                    FechaActividad1 = Textos.NoDisponible,
                    FechaActividad2 = Textos.NoDisponible,
                    DiferenciaTiempo = Textos.NoDisponible,
                    Error = string.Format(Textos.Error_ControlDeTiempoInexistente, recorrido.DatosProximaActividad, recorrido.Workflow.Descripcion)
                };
            }
            return control;
        }

        public ControlDeTiempoDto ObtenerControlDeTiempoPorCodigoControlPorGuid(string codigoControl, Guid instanceId)
        {
            var workflwoid = repositorio.ObtenerProyeccion<Recorrido, int>(x => x.InstanciaWorkflow == instanceId, x => x.Workflow.Id);
            return Obtener<ControlDeTiempo, ControlDeTiempoDto>(
                    x => x.CodigoControl == codigoControl && x.Workflow.Id == workflwoid);
        }

        public LogActividadDto ObtenerUltimoLogActividad(Guid instanceId, string actividad)
        {
            var logActividad = repositorio.ObtenerMayor<LogActividad, int>(
                x => x.WorkflowInstanceId == instanceId && x.ActividadXaml == actividad,
                x => x.Id);
            return conversor.Convertir<LogActividad, LogActividadDto>(logActividad);
        }

        public LogActividadDto ObtenerUltimoLog(Guid instanceId)
        {
            var logActividad = repositorio.ObtenerMayor<LogActividad, int>(
                x => x.WorkflowInstanceId == instanceId && !x.Actividad.Contains("Peso Tomado"),
                x => x.Id);
            return conversor.Convertir<LogActividad, LogActividadDto>(logActividad);
        }

        public ControlDeTiempoDto ObtenerControlDeTiempo(int id)
        {
            return Obtener<ControlDeTiempo, ControlDeTiempoDto>(id);
        }

        public ActividadPorDispositivoDto ObtenerActividadPorDispositivo(int id)
        {
            return Obtener<ActividadPorDispositivo, ActividadPorDispositivoDto>(id);
        }

        public ListaPaginada<ActividadPorDispositivoDto> ListarPaginadoActividadesPorBarreraSemaforo(int puestoId,
                                                                                                     Paginacion
                                                                                                         paginacion)
        {
            return Listar<ActividadPorDispositivo, ActividadPorDispositivoDto>(x => x.PuestoDeTrabajo.Id == puestoId,
                                                                               paginacion);
        }

        public bool EsTarjetaBloqueada(string numero, int centroId)
        {
            return repositorio.Existe<TarjetaBloqueada>(x => x.Numero == numero && x.Centro.Id == centroId);
        }

        public bool EsTarjetaEnRangoValido(string numero, int centroId)
        {
            var codigo = numero.Substring(0, 5);
            var rango = numero.Substring(numero.Length - Math.Min(5, numero.Length));
            var date = DateTime.Today;
            return
                repositorio.Existe<TarjetaRango>(
                    x =>
                    x.Codigo == codigo && String.Compare(rango, x.RangoDesde, StringComparison.Ordinal) >= 0 &&
                    String.Compare(rango, x.RangoHasta, StringComparison.Ordinal) <= 0 && x.Centro.Id == centroId &&
                    date >= x.ValidoDesde && date <= x.ValidoHasta);
        }

        public string ObtenerTarjetaRFIDAsignada(TipoDocumentoIngreso tipoDocumentoIngreso, string nroDocumentoIngreso)
        {
            var tarjetaDeAcceso =
                repositorio.ObtenerProyeccion<Recorrido, string>(
                    x =>
                    x.TipoDocumentoIngreso == tipoDocumentoIngreso &&
                    x.NumeroDocumentoIngreso.Equals(nroDocumentoIngreso) && !x.Terminado, x => x.TarjetaDeAcceso);
            return !string.IsNullOrEmpty(tarjetaDeAcceso) ? tarjetaDeAcceso : null;
        }

        public Guid ObtenerInstanceIdPorTipoYNumero(TipoDocumentoIngreso tipoDocumentoIngreso,
                                                    string nroDocumentoIngreso)
        {
            var recorrido =
                repositorio.Obtener<Recorrido>(
                    x =>
                    x.TipoDocumentoIngreso == tipoDocumentoIngreso &&
                    x.NumeroDocumentoIngreso.Equals(nroDocumentoIngreso) && !x.Terminado);
            return recorrido != null ? recorrido.InstanciaWorkflow : new Guid();
        }

        public ListaPaginada<MotivoQuiebreBarreraDto> ListarPaginadoMotivoQuiebreBarrera(int puestoId, int centroId,
                                                                                         Paginacion paginacion)
        {
            Expression<Func<MotivoQuiebreBarrera, bool>> expresionFiltro;
            if (puestoId != 0)
            {
                expresionFiltro = x => x.PuestoTrabajo.Id == puestoId && x.Motivo == null;
            }
            else
            {
                expresionFiltro = x => x.PuestoTrabajo.Centro.Id == centroId && x.Motivo == null;
            }

            return Listar<MotivoQuiebreBarrera, MotivoQuiebreBarreraDto>(expresionFiltro, paginacion);
        }

        public MotivoQuiebreBarreraDto ObtenerMotivoQuiebreBarrera(int id)
        {
            return Obtener<MotivoQuiebreBarrera, MotivoQuiebreBarreraDto>(id);
        }

        public IList<PuestoDeTrabajoDto> ListarPuestosDeTrabajoPorCentro(int centroId)
        {
            return Listar<PuestoDeTrabajo, PuestoDeTrabajoDto>(x => x.Centro.Id == centroId);
        }

        public IList<PuestoDeTrabajoDto> ListarPuestosDeTrabajoPorNombrePc(string nombrePc, int centroId)
        {
            return Listar<PuestoDeTrabajo, PuestoDeTrabajoDto>(x => x.NombrePc == nombrePc && x.Centro.Id == centroId);
        }

        public IList<string> ListarCamarasPorNombrePc(string nombrePc, int centroId)
        {
            var camaras = repositorio.Listar<VideoCamara, string>(x => x.Codigo, x => x.PuestoDeTrabajo.NombrePc == nombrePc && x.PuestoDeTrabajo.Centro.Id == centroId);

            return camaras.Any() ? servicioOrquestador.ObtenerUrlPorCamara(camaras.ToArray()).ToList() : new List<string>();
        }

        private int? ObtenerMaterialIdPorInstanceIds(List<Material> materiales)
        {
            if (materiales.Count == 0)
            {
                return null;
            }
            if (materiales.All(x => x == null))
            {
                return null; //Si todos los recorridos no tienen material, retorno null para listar todos los almacenes
            }
            if (materiales.Any(x => x == null))
            {
                return 0;
                //Si alguno es null, significa que hay alguno distinto, retorno 0 para no listar ningun almacen
            }

            return materiales.GroupBy(x => x.Id).Count() == 1 ? materiales?.FirstOrDefault().Id : 0;
            //Si todos los materiales tienen el mismo id, lo retorno, sino 0
        }

        public AsignacionDto ObtenerAsignacionDePuestoComando(string instanceId)
        {
            var asignacion = new AsignacionDto();
            asignacion.InstanceIds = instanceId;
            if (asignacion.InstanceIdsList.Count == 0)
            {
                return asignacion;
            }

            var recorridos =
                repositorio.Listar<Recorrido>(x => asignacion.InstanceIdsList.Contains(x.InstanciaWorkflow))
                           .OrderBy(x => asignacion.InstanceIdsList.IndexOf(x.InstanciaWorkflow));
            if (recorridos.Any(x => x.Establecimiento == null) && recorridos.Any(x => x.Establecimiento != null))
            {
                asignacion.SustentableMixto = true;
            }
            asignacion.SonSustentables = recorridos.Any(x => x.Establecimiento != null);
            asignacion.SonSojaEPA = recorridos.Any(x => x.Establecimiento != null && x.Establecimiento.EPA == true);

            var materiales = recorridos.Select(x => x.Material).ToList();
            foreach (var recorrido in recorridos)
            {
                asignacion.TipoVehiculo = recorrido.TipoVehiculo;

                if (asignacion.AlmacenId == 0 && recorrido.Almacen != null)
                {
                    asignacion.AlmacenId = recorrido.Almacen.Id;
                }
                if (!asignacion.BalanzaBrutoId.HasValue && recorrido.BalanzaBruto != null)
                {
                    asignacion.BalanzaBrutoId = recorrido.BalanzaBruto.Id;
                }
                if (!asignacion.BalanzaTaraId.HasValue && recorrido.BalanzaTara != null)
                {
                    asignacion.BalanzaTaraId = recorrido.BalanzaTara.Id;
                }
                if (asignacion.CalleId == 0 && recorrido.Calle != null)
                {
                    asignacion.CalleId = recorrido.Calle.Id;
                }
                if (asignacion.HidraulicasId == null && recorrido.PuestosDeCargaDescargas != null &&
                    recorrido.PuestosDeCargaDescargas.Count > 0)
                {
                    asignacion.HidraulicasId = recorrido.PuestosDeCargaDescargas.Select(x => x.Id).ToArray();
                }
                asignacion.CorrespondeCaladoEnPlanta = recorrido.CorrespondeCaladoEnPlanta;
            }

            var materialId = ObtenerMaterialIdPorInstanceIds(materiales);
            asignacion.MaterialId = materialId;
            if (materialId.HasValue && asignacion.AlmacenId == 0 && recorridos.Any())
            {
                var almacenPredeterminado = ObtenerAlmacenPredeterminado(recorridos.First().Centro.Id, materialId.Value);
                asignacion.AlmacenId = almacenPredeterminado != null ? almacenPredeterminado.Id : 0;
            }

            return asignacion;
        }

        public IList<CalidadMaterialDto> ListarCalidadesPorCentro(int centroId)
        {
            return Listar<CalidadMaterial, CalidadMaterialDto>(x => x.MaterialPorCentro.Centro.Id == centroId);
        }

        public bool BalanzasObligatoriasEnPuestoComando(int centroId)
        {
            return repositorio.ObtenerProyeccion<Centro, bool>(x => x.Id == centroId, x => x.AsignaBalanzaEnComando);
        }

        public DatosDeWorkflowsDto ListarWorkFlows(Paginacion paginacion, FiltroListaDeWorkflowsDto filtro)
        {
            try
            {
                var datosDeWorkflows = new DatosDeWorkflowsDto
                {
                    Calidades = new List<CalidadMaterialDto>(),
                };
                log.Debug("ListarWorkFlows INICIO");
                var recorridos = repositorio.ListarConsultaPaginada(new ListarWorkFlowsConsulta(filtro, paginacion));

                log.Debug("ListarWorkFlows CalidadMaterial");
                datosDeWorkflows.Calidades = repositorio.Listar<CalidadMaterial, CalidadMaterialDto>(
                    y => new CalidadMaterialDto { Descripcion = y.Descripcion },
                    x => x.MaterialPorCentro.Centro.Id == filtro.CentroId).GroupBy(x => x.Descripcion)
                                                        .Select(grp => grp.First())
                                                        .ToList();

                datosDeWorkflows.Workflows = recorridos;
                log.Debug("ListarWorkFlows MuestraPuestoComandoDto");
                var caladosId = datosDeWorkflows.Workflows.Select(x => x.CaladoId).ToList();
                var muestras = repositorio.Listar(y =>
                    new MuestraPuestoComandoDto
                    {
                        CaladoId = y.Calado.Id,
                        EsHumedad = y.CaracteristicasDeCalidad.Any(x => x.EsHumedad),
                        EsGranosVerdes = y.CaracteristicasDeCalidad.Any(x => x.EsGranosVerdes),
                        EsGranosDañados = y.CaracteristicasDeCalidad.Any(x => x.EsGranosDañados),
                        EsCuerposExtranos = y.CaracteristicasDeCalidad.Any(x => x.EsCuerposExtranos)
                    }, (MuestraEnvioACamara x) => caladosId.Contains(x.Calado.Id) && x.CaracteristicasDeCalidad.Any(y => y.EsHumedad || y.EsGranosVerdes || y.EsGranosDañados || y.EsCuerposExtranos));
                log.Debug("ListarWorkFlows muestras");
                foreach (var muestra in muestras)
                {
                    var instanciaWorkflow = datosDeWorkflows.Workflows.FirstOrDefault(x => x.CaladoId == muestra.CaladoId);
                    if (instanciaWorkflow != null)
                    {
                        instanciaWorkflow.EsGranosDañados = muestra.EsGranosDañados || instanciaWorkflow.EsGranosDañados;
                        instanciaWorkflow.EsGranosVerdes = muestra.EsGranosVerdes || instanciaWorkflow.EsGranosVerdes;
                        instanciaWorkflow.EsCuerposExtranos = muestra.EsCuerposExtranos || instanciaWorkflow.EsCuerposExtranos;
                        instanciaWorkflow.EsHumedad = muestra.EsHumedad || instanciaWorkflow.EsHumedad;
                    }
                }
                log.Debug("ListarWorkFlows ConfiguracionDeTabla");

                var configTablas =
                    repositorio.Listar<ConfiguracionDeTabla>(
                        x => (!filtro.MaterialId.HasValue || x.Material.Id == filtro.MaterialId) && x.Centro.Id == filtro.CentroId && x.Usuario.NombreUsuario == filtro.NombreUsuario);
                if (configTablas.Any())
                {
                    foreach (var workflow in datosDeWorkflows.Workflows.Where(x => configTablas.Any(y => y.Material.Id == x.MaterialId)))
                    {
                        workflow.CaracteristicasDeCalidad = new Dictionary<string, string>();
                        var recorrido = repositorio.ObtenerProyeccion((Recorrido x) => x.InstanciaWorkflow == workflow.Id,
                            x => new { x.Calado.CaladosPorCaracteristica, x.AnalisisDeCalidad.CaracteristicasAnalizadas });

                        if (recorrido != null && recorrido.CaladosPorCaracteristica != null)
                        {
                            foreach (var caracteristica in configTablas.First(x => x.Material.Id == workflow.MaterialId).CaracteristicasDeCalidad)
                            {
                                var caladoPorCaracteristica = recorrido.CaladosPorCaracteristica.FirstOrDefault(x => x.CaracteristicaDeCalidad.Id == caracteristica.Id && x.ValorCalado.HasValue);
                                if (caladoPorCaracteristica != null)
                                {
                                    workflow.CaracteristicasDeCalidad[caracteristica.CaracteristicaDeCalidadMaestro.Descripcion] = caladoPorCaracteristica.ValorCalado.ToString();
                                }
                                if (recorrido.CaracteristicasAnalizadas != null)
                                {
                                    var analisisPorCaracteristica = recorrido.CaracteristicasAnalizadas.FirstOrDefault(x => x.CaracteristicaDeCalidad.Id == caracteristica.Id && x.ValorAnalisis.HasValue);
                                    if (analisisPorCaracteristica != null)
                                    {
                                        workflow.CaracteristicasDeCalidad[caracteristica.CaracteristicaDeCalidadMaestro.Descripcion] = analisisPorCaracteristica.ValorAnalisis.ToString();
                                    }
                                }
                            }
                        }
                    }
                }

                log.Debug("ListarWorkFlows Workflows");
                datosDeWorkflows.WorkflowsCentro = repositorio.Listar<Workflow, WorkflowDto>(
                    x => new WorkflowDto { Id = x.Id, Codigo = x.Codigo, Descripcion = x.Descripcion },
                    w => w.Centro.Id == filtro.CentroId && w.Activo);
                log.Debug("ListarWorkFlows FIN");
                return datosDeWorkflows;
            }
            catch (Exception e)
            {
                log.Error(e, "Error al ejecutar ListarWorkFlows");
                throw e;
            }
        }

        public CalidadMaterialDto ObtenerCalidadMaterial(int id)
        {
            return Obtener<CalidadMaterial, CalidadMaterialDto>(id);
        }

        public CaracteristicaDeCalidadDto ObtenerCaracteristicaDeCalidadPorMaterial(int materialId)
        {
            return Obtener<CaracteristicaDeCalidad, CaracteristicaDeCalidadDto>(x => x.MaterialPorCentro.Material.Id == materialId);
        }

        public CalidadMaterialDto ObtenerCalidadMaterialPorHumedadEInstanceId(decimal valorHumedad, bool piedeAnalisis,
                                                                              Guid instancieId)
        {
            var recorrido = repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == instancieId);
            return
                Obtener<CalidadMaterial, CalidadMaterialDto>(
                    x =>
                    x.TieneAnalisis == piedeAnalisis && x.ValorDesde < valorHumedad && valorHumedad <= x.ValorHasta &&
                    x.MaterialPorCentro.Material.Id == recorrido.Material.Id &&
                    x.MaterialPorCentro.Centro.Id == recorrido.Centro.Id);
        }

        public bool WorkflowEstaAsignado(Guid instanceId)
        {
            var recorrido = repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == instanceId);
            return recorrido != null && recorrido.Almacen != null;
        }

        public IList<string> ListarCaracteristicaConfiguracionDeTabla(int centroId, int materialId, string usuario)
        {
            return repositorio.ListarConsulta(new ListarCaracteristicaConfiguracionDeTabla(materialId, centroId, usuario));
        }

        public IList<CaracteristicaConfiguracionDeTablaDto> ListarCaracteristicasDeCalidadPorConfiguracion(int centroId, int materialId, string nombreUsuario)
        {
            if (!repositorio.Existe<MaterialPorCentro>(x => x.Material.Id == materialId && x.Centro.Id == centroId))
            {
                return new List<CaracteristicaConfiguracionDeTablaDto>();
            }
            var caracteristicas = repositorio.Listar<CaracteristicaDeCalidad, CaracteristicaConfiguracionDeTablaDto>(
                x =>
                new CaracteristicaConfiguracionDeTablaDto
                {
                    CaracteristicaDeCalidadDesc = x.CaracteristicaDeCalidadMaestro.Descripcion,
                    CaracteristicaDeCalidadId = x.Id,
                    Visible = false
                },
                x =>
                x.MaterialPorCentro.Centro.Id == centroId && x.MaterialPorCentro.Material.Id == materialId &&
                !x.EsHumedad);

            var configuracion =
                repositorio.Obtener<ConfiguracionDeTabla>(x => x.Material.Id == materialId && x.Centro.Id == centroId && x.Usuario.NombreUsuario == nombreUsuario);
            if (configuracion != null)
            {
                var caracteristicasVisibles = configuracion.CaracteristicasDeCalidad.Select(x => x.Id);
                foreach (
                    var caracteristica in
                        caracteristicas.Where(
                            caracteristica =>
                            caracteristicasVisibles.Any(x => x == caracteristica.CaracteristicaDeCalidadId)))
                {
                    caracteristica.Visible = true;
                }
            }

            return caracteristicas;
        }

        public ListaPaginada<AjusteDeStockDto> ListarPaginadoAjusteDeStock(string filtro, Paginacion paginacion,
                                                                           int centroId)
        {
            DateTime? filtroFecha = null;
            var fecha = new DateTime();
            if (DateTime.TryParseExact(filtro, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                filtroFecha = fecha;
            }
            Expression<Func<AjusteDeStock, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = x => x.Centro.Id == centroId && (x.Material.Descripcion.Contains(filtro)
                                                                   || x.NumeroDocumentoIngreso.Contains(filtro) ||
                                                                   x.TipoComprobanteOncca.Descripcion.Contains(filtro) || (filtroFecha.HasValue && filtroFecha.Value == x.Fecha));
            }
            else
            {
                expresionFiltro = x => x.Centro.Id == centroId;
            }

            return Listar<AjusteDeStock, AjusteDeStockDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<AjusteStockBinesDto> ListarPaginadoAjusteYStockBines(string filtro, Paginacion paginacion,
                                                                                   int centroId)
        {
            Expression<Func<AjusteStockBines, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro =
                    x =>
                    (x.Proveedor.Descripcion.Contains(filtro) || x.Proveedor.Cuil.Contains(filtro) ||
                     x.Centro.Descripcion.Contains(filtro) || x.Centro.NumeroINV.Contains(filtro) ||
                     x.VinedoPropio.Descripcion.Contains(filtro) || x.VinedoPropio.NumeroINV.Contains(filtro));
            }
            else
            {
                expresionFiltro = x => true;
            }

            return Listar<AjusteStockBines, AjusteStockBinesDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<MovimientoDeBinesDto> ListarPaginadoMovimientoDeBines(string filtro, Paginacion paginacion,
                                                                                   int centroId)
        {
            Expression<Func<MovimientoDeBines, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro =
                    x =>
                    (x.Proveedor.Descripcion.Contains(filtro) || x.Proveedor.Cuil.Contains(filtro) ||
                     x.Centro.Descripcion.Contains(filtro) || x.Centro.NumeroINV.Contains(filtro) ||
                     x.VinedoPropio.Descripcion.Contains(filtro) || x.VinedoPropio.NumeroINV.Contains(filtro));
            }
            else
            {
                expresionFiltro = x => true;
            }

            return Listar<MovimientoDeBines, MovimientoDeBinesDto>(expresionFiltro, paginacion);
        }

        public AjusteDeStockDto ObtenerAjusteDeStock(int id)
        {
            return Obtener<AjusteDeStock, AjusteDeStockDto>(id);
        }

        public AjusteStockBinesDto ObtenerAjusteYStockBines(int id)
        {
            return Obtener<AjusteStockBines, AjusteStockBinesDto>(id);
        }

        private ValidarProximaAccionDto ValidarProximaActividad(DatosRecorridoDto recorrido, string proximaActividad,
                                                                IList<PuestoDeTrabajoDto> puestos,
                                                                ValidarProximaAccionDto validarProximaAccion,
                                                                string nombreUsuario, bool puestoSinPatente)
        {
            log.Debug("Validando proxima actividad");

            if (recorrido == null || recorrido.SinRecorrido)
            {
                validarProximaAccion.MensajeError = Textos.Recorrido_InexistentePorPatente;
                return validarProximaAccion;
            }

            var instanceId = recorrido.InstanciaWorkflow;
            var puesto = puestos.Where(x => x.Lectura == recorrido.TarjetaDeAcceso).Select(x => x.Id).ToList();
            log.Debug("Instancia WF: {0} Puestos: {1}", instanceId, puesto.ToArray());

            log.Debug("Proxima actividad de wf {0}: {1}", instanceId, proximaActividad);
            if (!String.IsNullOrEmpty(nombreUsuario))
            {
                if (
                    !repositorio.ObtenerConsultaEscalar(new UsuarioTienePermisoParaActividadConsulta(nombreUsuario,
                                                                                                     proximaActividad)))
                {
                    log.Debug("El usuario '{0}' no tiene permisos para ejecutar la actividad {1}", nombreUsuario,
                              proximaActividad);
                    validarProximaAccion.MensajeError = Textos.Permiso_NoTienePermiso;
                    return validarProximaAccion;
                }
            }
            else if (!puestoSinPatente)
            {
                log.Debug("En un puesto con patente el usuario vino vacío.");
                validarProximaAccion.MensajeError = Textos.Permiso_NoTienePermiso;
                return validarProximaAccion;
            }

            var puestosValidos = repositorio.Listar<ActividadPorDispositivo, int>(x => x.PuestoDeTrabajo.Id,
                                                                                  x =>
                                                                                  puesto.Contains(x.PuestoDeTrabajo.Id) &&
                                                                                  x.Actividad == proximaActividad &&
                                                                                  x.Workflow.Id == recorrido.WorkflowId);

            if (puestosValidos == null || !puestosValidos.Any())
            {
                log.Debug("El usuario no tiene puestos válidos para la actividad");
                validarProximaAccion.MensajeError = Textos.ActividadInvalidaParaElPuesto;
                return validarProximaAccion;
            }
            if (puestosValidos.Count > 1)
            {
                log.Debug("Hay más de un puesto válido para ejecutar la actividad");
                var lectura = repositorio.ObtenerMayor<LecturaDeTarjeta, int>(
                    x => puesto.Contains(x.PuestoDeTrabajo.Id), x => x.Id);
                puestosValidos.Clear();
                puestosValidos.Add(lectura.PuestoDeTrabajo.Id);
            }

            validarProximaAccion.ProximaActividad = proximaActividad;
            validarProximaAccion.Valida = true;
            validarProximaAccion.InstanceId = instanceId;
            validarProximaAccion.PuestoDeTrabajoId = puestosValidos.FirstOrDefault();
            validarProximaAccion.WorkflowDefinicionId = recorrido.WorkflowDefinicionId;
            validarProximaAccion.Patente = recorrido.Patente;
            validarProximaAccion.NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso;
            validarProximaAccion.CodigoSapCentro = recorrido.CentroCodigoSap;
            return validarProximaAccion;
        }

        public DatosRecorridoDto ObtenerDatosRecorridoActivo(string patente, IList<string> lecturasTarjetaDeAcceso)
        {
            return repositorio.ObtenerProyeccion<Recorrido, DatosRecorridoDto>(
                x =>
                (patente == null || patente == x.Patente) && x.TarjetaDeAcceso != null && !x.Terminado &&
                lecturasTarjetaDeAcceso.Contains(x.TarjetaDeAcceso),
                x => new DatosRecorridoDto
                {
                    InstanciaWorkflow = x.InstanciaWorkflow,
                    NumeroDocumentoIngreso = x.NumeroDocumentoIngreso,
                    Patente = x.Patente,
                    TarjetaDeAcceso = x.TarjetaDeAcceso,
                    WorkflowDefinicionId = x.WorkflowDefinicion.Id,
                    CentroCodigoSap = x.Centro.CodigoSAP,
                    WorkflowId = x.Workflow.Id,
                    AdvertirCaladoEnPlanta = x.CorrespondeCaladoEnPlanta && x.CaladoEnPlanta == null,
                    Id = x.Id,
                    TipoVehiculo = x.TipoVehiculo,
                    CartaDePorte = x.NumeroDocumentoIngreso,
                    Entregador = x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte &&
                    x.Vehiculo.CartaPorte.Entregador != null && x.Vehiculo.CartaPorte.Entregador.RazonSocial.ToUpper() != "SIN ENTREGA",
                    Material = x.Material.Descripcion,
                    PesoBrutoOrigen = x.PesoBrutoOrigen,
                    PesoTaraOrigen = x.PesoTaraOrigen,
                    PesoNetoOrigen = x.PesoBrutoOrigen - x.PesoTaraOrigen,
                    PesoBruto = x.PesoBruto,
                    PesoTara = x.PesoTara,
                    Calle = x.Calle.Nombre,
                    TipoDocumento = x.TipoDocumentoIngreso,
                    TipoComercial = x.TipoComercial.Descripcion,
                    TipoDeWorkflow = x.Workflow.TipoDeWorkflow
                });
        }

        public DatosRecorridoDto ObtenerDatosRecorridoActivoSinTarjeta(string patente, string lecturasTarjetaDeAcceso)
        {
            return repositorio.ObtenerProyeccion<Recorrido, DatosRecorridoDto>(
                x =>
                (patente == null || patente == x.Patente) && !x.Terminado &&
                (lecturasTarjetaDeAcceso == null || lecturasTarjetaDeAcceso.Contains(x.TarjetaDeAcceso)),
                x => new DatosRecorridoDto
                {
                    InstanciaWorkflow = x.InstanciaWorkflow,
                    NumeroDocumentoIngreso = x.NumeroDocumentoIngreso,
                    Patente = x.Patente,
                    TarjetaDeAcceso = x.TarjetaDeAcceso,
                    WorkflowDefinicionId = x.WorkflowDefinicion.Id,
                    CentroCodigoSap = x.Centro.CodigoSAP,
                    WorkflowId = x.Workflow.Id,
                    AdvertirCaladoEnPlanta = x.CorrespondeCaladoEnPlanta && x.CaladoEnPlanta == null,
                    Id = x.Id,
                    TipoVehiculo = x.TipoVehiculo,
                    CartaDePorte = x.NumeroDocumentoIngreso,
                    Entregador = x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte &&
                    x.Vehiculo.CartaPorte.Entregador != null && x.Vehiculo.CartaPorte.Entregador.RazonSocial.ToUpper() != "SIN ENTREGA",
                    Material = x.Material.Descripcion,
                    PesoBrutoOrigen = x.PesoBrutoOrigen,
                    PesoTaraOrigen = x.PesoTaraOrigen,
                    PesoNetoOrigen = x.PesoBrutoOrigen - x.PesoTaraOrigen,
                    PesoBruto = x.PesoBruto,
                    PesoTara = x.PesoTara,
                    Calle = x.Calle.Nombre,
                    TipoComercial = x.TipoComercial.Descripcion,
                    TipoDocumento = x.TipoDocumentoIngreso,
                });
        }

        public DatosRecorridoDto ObtenerDatosRecorridoActivoPorWorkflow(Guid workflow)
        {
            return repositorio.ObtenerProyeccion<Recorrido, DatosRecorridoDto>(
                x => x.InstanciaWorkflow == workflow,
                x => new DatosRecorridoDto
                {
                    InstanciaWorkflow = x.InstanciaWorkflow,
                    NumeroDocumentoIngreso = x.NumeroDocumentoIngreso,
                    Patente = x.Patente,
                    TarjetaDeAcceso = x.TarjetaDeAcceso,
                    WorkflowDefinicionId = x.WorkflowDefinicion.Id,
                    CentroCodigoSap = x.Centro.CodigoSAP,
                    WorkflowId = x.Workflow.Id,
                    AdvertirCaladoEnPlanta = x.CorrespondeCaladoEnPlanta && x.CaladoEnPlanta == null,
                    Id = x.Id,
                    CartaDePorte = x.NumeroDocumentoIngreso,
                    Entregador = x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte ? x.Vehiculo.CartaPorte.Entregador != null : false,
                    Material = x.Material.Descripcion,
                    PesoBrutoOrigen = x.PesoBrutoOrigen,
                    PesoTaraOrigen = x.PesoTaraOrigen,
                    PesoBruto = x.PesoBruto,
                    PesoTara = x.PesoTara,
                    PesoNetoOrigen = x.PesoBrutoOrigen - x.PesoTaraOrigen,
                    TipoDeWorkflow = x.Workflow.TipoDeWorkflow,
                    Calle = x.Calle.Nombre,
                    PasoPorContingenciaPesosExc = x.PasoPorContingenciaPesoExc
                });
        }

        public ValidarProximaAccionPorPuestoDto ValidarProximaActividadPorPuesto(DatosRecorridoDto recorrido,
                                                                                 string proximaActividad,
                                                                                 IList<PuestoDeTrabajoDto> puestos,
                                                                                 string nombreUsuario)
        {
            var validarProximaAccion = new ValidarProximaAccionPorPuestoDto();

            ValidarProximaActividad(recorrido, proximaActividad, puestos, validarProximaAccion, nombreUsuario, false);

            if (validarProximaAccion.PuestoDeTrabajoId != 0)
            {
                var puestoDeTrabajo = repositorio.Obtener<PuestoDeTrabajo>(validarProximaAccion.PuestoDeTrabajoId);
                validarProximaAccion.Entrada = puestoDeTrabajo.Entradas();

                validarProximaAccion.VideoCamaras = !puestoDeTrabajo.FotoAlMarcarTarjeta ? conversor.ConvertirList<VideoCamara, VideoCamaraDto>(puestoDeTrabajo.VideoCamaras.ToList()).ToList() : new List<VideoCamaraDto>();
            }
            return validarProximaAccion;
        }

        public ValidarProximaAccionDto ValidarProximaActividadPorPuestoSinPatente(DatosRecorridoDto recorrido,
                                                                                  string proximaActividad,
                                                                                  IList<PuestoDeTrabajoDto> puestos)
        {
            var validarProximaAccion = new ValidarProximaAccionDto();
            return ValidarProximaActividad(recorrido, proximaActividad, puestos, validarProximaAccion, null, true);
        }

        public IList<ListadoDePesadasDto> ListarListadoDePesadas(List<int> centros, DateTime fechaInicio,
                                                                 DateTime fechaFin, List<int> tiposComerciales,
                                                                 List<int> materiales, bool incluirRechazados)
        {
            fechaFin = fechaFin.FinDelDia();
            fechaInicio = fechaInicio.InicioDelDia();
            return
                repositorio.ListarConsulta(new ListarListadoDePesadas(centros, fechaInicio, fechaFin, tiposComerciales,
                                                                      materiales, incluirRechazados));
        }

        public ListaPaginada<CiuAnuladoDto> ListarPaginadoCiuAnulados(Paginacion paginacion)
        {
            return Listar<CiuAnulado, CiuAnuladoDto>(null, paginacion);
        }

        public bool EsCiuAnulado(string numeroCiu)
        {
            return repositorio.Existe<CiuAnulado>(x => x.Numero == numeroCiu);
        }

        public VerificarKilosDeclaradosPorFincaDto VerificarKilosDeclaradosPorVinedo(int vinedoId, int materialId,
                                                                                     string cosecha)
        {
            var resultado = new VerificarKilosDeclaradosPorFincaDto();
            try
            {
                log.Debug("Iniciando VerificarKilosDeclaradosPorVinedo vinedo {0}, materialId{1}, cosecha{2}", vinedoId,
                          materialId, cosecha);
                var material = repositorio.Obtener<Material>(materialId);
                if (material == null)
                {
                    log.Error("Error material no encontrado id {0}", materialId);
                    resultado.Error = true;
                    resultado.MensajeError = String.Format(Textos.Error_Requerido, "material");
                    resultado.ExcedeKilosARecibir = true;
                    return resultado;
                }
                if (material.Variedad == null)
                {
                    log.Error("Error el material seleccionado no posee variedad");
                    resultado.Error = true;
                    resultado.MensajeError = String.Format(Textos.Error_Requerido, "variedad");
                    resultado.ExcedeKilosARecibir = true;
                    return resultado;
                }
                var variedadId = material.Variedad.Id;
                var vinedo =
                    repositorio.Obtener<VariedadPorVinedo>(
                        x => x.Vinedo.Id == vinedoId && x.Variedad.Id == variedadId && x.Cosecha == cosecha);

                if (vinedo == null)
                {
                    log.Error("Error variedad por vinedo no encontrado-- vinedo {0}, variedadId{1}, cosecha{2}",
                              vinedoId, variedadId, cosecha);
                    resultado.Error = true;
                    resultado.ExcedeKilosARecibir = true;
                    resultado.MensajeError = String.Format(Textos.Error_Requerido, "variedadPorVinedo");
                    return resultado;
                }

                var kgRecibidos =
                    repositorio.ObtenerConsultaEscalar(new KgRecibidosPorFincaViñedoAño(vinedoId, variedadId, cosecha));
                var kgARecibir = Convert.ToDecimal(vinedo.TopeHectarea * vinedo.Hectareas) - kgRecibidos;
                log.Debug("Procesando - KgARecibir {0},avisoDecorte{1}, kgRecibidos {2}", kgARecibir, vinedo.AvisoCorte,
                          kgRecibidos);

                if (kgARecibir < 0)
                {
                    resultado.ExcedeKilosARecibir = true;
                }
                else if (kgARecibir <= vinedo.AvisoCorte)
                {
                    resultado.AvisoDeCorte = true;
                }

                resultado.Vinedo = vinedo.Vinedo.Descripcion;
                resultado.Variedad = vinedo.Variedad.NumeroINV;
                resultado.KilosARecibir = kgARecibir;
                log.Debug("Fin - Excede: {0},avisoDecorte: {1}, kilosARecibir: {2}, error: {3}",
                          resultado.ExcedeKilosARecibir, resultado.AvisoDeCorte, resultado.KilosARecibir,
                          resultado.Error);
            }
            catch (Exception e)
            {
                resultado.Error = true;
                resultado.MensajeError = String.Format(Textos.Error_Requerido + ": " + e.Message, "viñedo");
            }
            return resultado;
        }

        public decimal ObtenerKilosRecibidosPorVinedo(int vinedoId, int variedadId, string cosecha)
        {
            return repositorio.ObtenerConsultaEscalar(new KgRecibidosPorFincaViñedoAño(vinedoId, variedadId, cosecha));
        }

        public VerificarKilosDeclaradosPorFincaDto ObtenerKilosARecibirPorVinedo(int vinedoId, int variedadId,
                                                                                 string cosecha)
        {
            log.Debug("Iniciando ObtenerKilosARecibirPorVinedo vinedo {0}, variedadId{1}, cosecha{2}", vinedoId,
                      variedadId, cosecha);
            var resultado = new VerificarKilosDeclaradosPorFincaDto();
            var recibidos =
                repositorio.ObtenerConsultaEscalar(new KgRecibidosPorFincaViñedoAño(vinedoId, variedadId, cosecha));
            var variedadPorVinedo =
                repositorio.Obtener<VariedadPorVinedo>(
                    x => x.Variedad.Id == variedadId && x.Vinedo.Id == vinedoId && x.Cosecha == cosecha);
            if (variedadPorVinedo == null)
            {
                log.Error("Error variedad por vinedo no encontrado-- vinedo {0}, variedadId{1}, cosecha{2}", vinedoId,
                          variedadId, cosecha);
                resultado.Error = true;
                resultado.MensajeError = Textos.Error_VariedadPorVinedo;
                return resultado;
            }
            resultado.KilosARecibir = Convert.ToDecimal(variedadPorVinedo.TopeHectarea * variedadPorVinedo.Hectareas) -
                                      recibidos;
            log.Debug("Iniciando - KgARecibir {0}, kgRecibidos {1}", resultado.KilosARecibir, recibidos);
            return resultado;
        }

        public RemitoBodegaUvaDto ObtenerRemitoBodegaUvaPorGuid(Guid guid)
        {
            return Obtener<RemitoBodegaUva, RemitoBodegaUvaDto>(x => x.Recorrido.InstanciaWorkflow == guid);
        }

        public int ObtenerRemitoBodegaUvaIdPorGuid(Guid guid)
        {
            return repositorio.ObtenerProyeccion<RemitoBodegaUva, int>(x => x.Recorrido.InstanciaWorkflow == guid,
                                                                       x => x.Id);
        }

        public IList<VariedadDto> ListarVariedades()
        {
            return Listar<Variedad, VariedadDto>();
        }

        public RemitoBodegaUvaDto ObtenerRemitoBodegaUva(int id)
        {
            return Obtener<RemitoBodegaUva, RemitoBodegaUvaDto>(id);
        }

        public RemitoBodegaUvaDto ObtenerRemitoBodegaUvaPorInstanceId(Guid instanceId)
        {
            return ObtenerPrimero<RemitoBodegaUva, RemitoBodegaUvaDto>(x => x.Recorrido.InstanciaWorkflow == instanceId);
        }

        public RemitoBodegaVinoDto ObtenerRemitoBodegaVino(int id)
        {
            return Obtener<RemitoBodegaVino, RemitoBodegaVinoDto>(id);
        }

        public RemitoBodegaVinoDto ObtenerRemitoBodegaVinoPorInstanceId(Guid instanceId)
        {
            return
                ObtenerPrimero<RemitoBodegaVino, RemitoBodegaVinoDto>(x => x.Recorrido.InstanciaWorkflow == instanceId);
        }

        public HojaDeRutaDto ObtenerHojaDeRutaPorInstanceId(Guid instanceId)
        {
            return ObtenerPrimero<HojaDeRuta, HojaDeRutaDto>(x => x.Recorrido.InstanciaWorkflow == instanceId);
        }

        public IList<CuartelDto> FiltrarCuartelesPorVinedo(int vinedoId)
        {
            return Listar<Cuartel, CuartelDto>(x => x.VinedoPropio.Id == vinedoId && x.Activo);
        }

        public IList<DescargaDeBinesDto> ObtenerDescargasDeBinesPorRemitoBodegaUva(int remitoBodegaUvaId)
        {
            return Listar<DescargaDeBines, DescargaDeBinesDto>(x => x.RemitoBodegaUva.Id == remitoBodegaUvaId);
        }

        public ListaPaginada<VinedoTercerosDto> ListarVinedoTerceros(string filtro, Paginacion paginacion)
        {
            Expression<Func<VinedoTerceros, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    x.Descripcion.Contains(filtro);
            }

            return Listar<VinedoTerceros, VinedoTercerosDto>(expresionFiltro, paginacion);
        }

        public DistribucionDeAlmacenesDto ObtenerDistribucionDeAlmacenes(Guid guid)
        {
            var distribucion = Obtener<DistribucionDeAlmacenes, DistribucionDeAlmacenesDto>(x => x.Recorrido.InstanciaWorkflow == guid);
            if (distribucion == null)
            {
                return null;
            }
            var almacenes = Listar<DistribucionDeAlmacen, DistribucionDeAlmacenDto>(x => x.DistribucionDeAlmacenes.Recorrido.InstanciaWorkflow == guid);
            distribucion.DistribucionesDeAlmacenes = almacenes != null ? almacenes.ToList() : null;
            return distribucion;
        }

        public IList<MaterialPorWorkflowDto> ListarMaterialesPorWorkflowVinedoYCodigoSAP(int workflowId, int centroId,
                                                                                         int vinedoId,
                                                                                         List<string>
                                                                                             materialesCodigoSap)
        {
            var variedadesPorVinedoId =
                repositorio.Obtener<Vinedo>(vinedoId)
                           .VariedadesPorVinedo.Where(
                               x => x.Cosecha == DateTime.Today.Year.ToString(CultureInfo.InvariantCulture))
                           .Select(x => x.Variedad.Id)
                           .ToList();

            return
                Listar<MaterialPorWorkflow, MaterialPorWorkflowDto>(
                    f => f.Workflow.Id.Equals(workflowId) && f.Centro.Id.Equals(centroId) && f.Material.Activo
                         && variedadesPorVinedoId.Contains(f.Material.Variedad.Id) &&
                         ((materialesCodigoSap.Count <= 1 && String.IsNullOrEmpty(materialesCodigoSap.FirstOrDefault())) ||
                          materialesCodigoSap.Contains(f.Material.CodigoSAP)))
                as List<MaterialPorWorkflowDto>;
        }

        public IList<MaterialPorWorkflowDto> ListarMaterialesPorWorkflowYCodigoSAP(int workflowId, int centroId,
                                                                                   List<string> materialesCodigoSap)
        {
            return
                Listar<MaterialPorWorkflow, MaterialPorWorkflowDto>(
                    f => f.Workflow.Id.Equals(workflowId) && f.Centro.Id.Equals(centroId) && f.Material.Activo
                         && materialesCodigoSap.Contains(f.Material.CodigoSAP) && f.Cliente == null)
                as List<MaterialPorWorkflowDto>;
        }

        public decimal? ObtenerTenorAzucarinoNumerico(Guid guid)
        {
            var calado = repositorio.ObtenerProyeccion<Recorrido, Calado>(x => x.InstanciaWorkflow == guid,
                                                                          x => x.Calado);
            if (calado == null)
            {
                return null;
            }
            var analisis = repositorio.Obtener<AnalisisDeCalidad>(x => x.Calado.Id == calado.Id);
            if (analisis != null)
            {
                var caracteristicaAzucarina =
                    analisis.CaracteristicasAnalizadas.FirstOrDefault(x => x.CaracteristicaDeCalidad.EsTenorAzucarino);
                if (caracteristicaAzucarina != null)
                {
                    return caracteristicaAzucarina.ValorAnalisis;
                }
            }
            var caractAzucarina =
                calado.CaladosPorCaracteristica.FirstOrDefault(x => x.CaracteristicaDeCalidad.EsTenorAzucarino);
            if (caractAzucarina != null)
            {
                return caractAzucarina.ValorCalado;
            }
            return null;
        }

        public string ObtenerTenorAzucarino(Guid guid)
        {
            var tenor = ObtenerTenorAzucarinoNumerico(guid);
            return tenor == null ? null : ((int)tenor.Value).ToString(CultureInfo.InvariantCulture);
        }

        public ListaPaginada<AsignacionDeRecorridoDto> ListarPaginadoAsignacionesDeRecorrido(int centroId, string filtro,
                                                                                             Paginacion paginacion)
        {
            Expression<Func<AsignacionDeRecorrido, bool>> expresionFiltro;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = (x => (x.Calidad.Descripcion.Contains(filtro) && x.Centro.Id == centroId));
            }
            else
            {
                expresionFiltro = (x => x.Centro.Id == centroId);
            }
            return Listar<AsignacionDeRecorrido, AsignacionDeRecorridoDto>(expresionFiltro, paginacion);
        }

        public AsignacionDeRecorridoDto ObtenerAsignacionDeRecorrido(int id)
        {
            return Obtener<AsignacionDeRecorrido, AsignacionDeRecorridoDto>(id);
        }

        public IList<MaterialPorCentroDto> ListarMaterialesPorCentro(int centroId)
        {
            return Listar<MaterialPorCentro, MaterialPorCentroDto>(m => m.Material.Activo && m.Centro.Id == centroId);
        }

        public IList<CalidadMaterialDto> ListarCalidadesPorMaterialyCentro(int materialPorCentroId)
        {
            return Listar<CalidadMaterial, CalidadMaterialDto>(x => x.MaterialPorCentro.Id == materialPorCentroId);
        }

        public HojaDeRutaYerbateraDto ObtenerHojaDeRutaYerbatera(int id)
        {
            return Obtener<HojaDeRutaYerbatera, HojaDeRutaYerbateraDto>(id);
        }

        public HojaDeRutaYerbateraDto ObtenerHojaDeRutaYerbateraPorInstanceId(Guid id)
        {
            return Obtener<HojaDeRutaYerbatera, HojaDeRutaYerbateraDto>(x => x.Recorrido.InstanciaWorkflow == id);
        }

        public HojaDeRutaYerbateraDto ObtenerHojaDeRutaYerbateraVacia(int centroId, string codigoWorkflow,
                                                                      string codigoDestinatario, string codigoProveedor)
        {
            try
            {
                var workflow = repositorio.Obtener<Workflow>(x => x.Codigo == codigoWorkflow);
                var centro = repositorio.Obtener<Centro>(centroId);
                Proveedor destinatario = null;
                Proveedor proveedor = null;

                if (!String.IsNullOrEmpty(codigoDestinatario))
                {
                    destinatario = repositorio.Obtener<Proveedor>(x => x.CodigoSap == codigoDestinatario);
                }
                if (!String.IsNullOrEmpty(codigoProveedor) && codigoDestinatario == codigoProveedor)
                {
                    proveedor = destinatario;
                }
                else if (!String.IsNullOrEmpty(codigoProveedor))
                {
                    proveedor = repositorio.Obtener<Proveedor>(x => x.CodigoSap == codigoProveedor);
                }

                var hVacia = new HojaDeRutaYerbatera
                {
                    FechaCarga = DateTime.Now,
                    FechaVencimiento = DateTime.Now,
                    FechaEmision = DateTime.Now,
                    Destinatario = destinatario,
                    CentroDestino = workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso ? centro : null,
                    Procedencia = workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? centro.Localidad : null,
                    Proveedor = workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? proveedor : null,
                };
                var hVaciaDto = conversor.Convertir<HojaDeRutaYerbatera, HojaDeRutaYerbateraDto>(hVacia);
                hVaciaDto.TipoDeWorkflow = workflow.TipoDeWorkflow;
                return hVaciaDto;
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo generar la nueva carta de porte");
                throw;
            }
        }

        public HojaDeRutaYerbateraValidaResponseDto NumeroHojaDeRutaYerbateraValido(string numero, int centroId,
                                                                                    string workflow)
        {
            var response = new HojaDeRutaYerbateraValidaResponseDto { Valida = true };
            if (
                repositorio.Existe<Recorrido>(
                    x =>
                    x.TipoDocumentoIngreso == TipoDocumentoIngreso.HojaDeRutaYerbatera &&
                    x.NumeroDocumentoIngreso == numero && (!x.Rechazado || !x.Terminado)))
            {
                log.Debug("La hoja de ruta {0} ya fue ingresada ", numero);
                response.Valida = false;
                response.Error = String.Format(Textos.HojaDeRutaYerbatera_Ingresada, numero);
                response.CodigoDeError = 3;
            }
            return response;
        }

        public IList<ArchivoINVFilaDto> ListarArchivoINV(DateTime fechaDesde, DateTime fechaHasta, int centroId)
        {
            log.Debug("Inicio - ListarArchivoINV");
            fechaHasta = fechaHasta.FinDelDia();
            fechaDesde = fechaDesde.InicioDelDia();
            var remitos = repositorio.Listar<RemitoBodegaUva>(r => r.Recorrido.Terminado &&
                                                                   r.Recorrido.FechaEgreso >= fechaDesde &&
                                                                   r.Recorrido.FechaEgreso <= fechaHasta &&
                                                                   !r.Recorrido.Rechazado && r.Recorrido.Centro.Id == centroId
                ).ToList();
            log.Debug("Listo los recorridos a mostrar en el archivo INV cantidad: {0}", remitos.Count);
            var centro = repositorio.Obtener<Centro>(centroId);
            var ciusAnulados = repositorio.Listar<CiuAnulado>(r => r.Fecha >= fechaDesde &&
                                                                   r.Fecha <= fechaHasta);

            log.Debug("Listo los cius anulados a mostrar en el archivo INV cantidad: {0}", ciusAnulados.Count);
            var resultado = new List<ArchivoINVFilaDto>();

            foreach (var ciu in ciusAnulados)
            {
                log.Debug("Inicio mapeo para el ciu: {0}", ciu.Numero);
                var dto = conversor.Convertir<CiuAnulado, ArchivoINVFilaDto>(ciu);
                resultado.Add(dto);
            }

            foreach (var remito in remitos)
            {
                log.Debug("Inicio mapeo para el recorrido: {0}, tipode documento {1}",
                          remito.Recorrido.InstanciaWorkflow, remito.Recorrido.TipoDocumentoIngreso);
                var dto = conversor.Convertir<RemitoBodegaUva, ArchivoINVFilaDto>(remito);
                if (resultado.Exists(x => x.NumeroCiu == dto.NumeroCiu))
                {
                    log.Debug("Ya existe un dto con el numero de ciu {0} en el resultado", dto.NumeroCiu);
                    continue;
                }

                var analisis = repositorio.Obtener<AnalisisDeCalidad>(x => x.Calado.Id == remito.Recorrido.Calado.Id);
                var ancar = analisis != null
                                ? analisis.CaracteristicasAnalizadas.FirstOrDefault(
                                    x => x.CaracteristicaDeCalidad.EsTenorAzucarino)
                                : null;
                var observacion =
                    repositorio.Obtener<Observacion>(x => x.WorkflowInstanceId == remito.Recorrido.InstanciaWorkflow);

                var calcar =
                    remito.Recorrido.Calado.CaladosPorCaracteristica.FirstOrDefault(
                        x => x.CaracteristicaDeCalidad.EsTenorAzucarino);
                dto.TenorAzucarino = ancar != null && ancar.ValorAnalisis.HasValue
                                         ? Convert.ToInt32(ancar.ValorAnalisis).ToString()
                                         : (calcar != null && calcar.ValorCalado.HasValue
                                                ? Convert.ToInt32(calcar.ValorCalado).ToString()
                                                : "0");
                dto.Observaciones = observacion != null ? observacion.Observaciones : null;
                log.Debug("Se mapeo Recorrido a ArchivoINVFilaDto");

                log.Debug("Se cargo el recorrido con Numero de Ciu: {0} en la el resultado", dto.NumeroCiu);
                resultado.Add(dto);
            }

            resultado = resultado.OrderBy(x => x.FechaEgreso).ToList();
            var i = 1;
            foreach (var dto in resultado)
            {
                dto.Numero = (i++).ToString(CultureInfo.InvariantCulture);
                dto.NumeroINVBodega = centro.NumeroINV;
                dto.RazonSocialBodega = centro.RazonSocial;
                dto.CuitBodega = centro.Cuit;
                dto.IBBBodega = centro.IngresosBrutos;
            }
            log.Debug("FIN - ListarArchivoINV");
            return resultado;
        }

        public AsignacionDeRecorridoDto BuscarAsignacionDeRecorridoPorInstanceId(Guid instanceId)
        {
            AsignacionDeRecorridoDto asignacion = null;
            var recorrido = repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == instanceId);
            var today = DateTime.Today;
            if (recorrido != null && recorrido.Calado != null)
            {
                var idMaterialCentro = repositorio.ObtenerProyeccion<MaterialPorCentro, int>(
                    x => x.Centro.Id == recorrido.Centro.Id && x.Material.Id == recorrido.Material.Id,
                    x => x.Id);
                var calidadMaterialId = recorrido.Calado.CalidadMaterial != null
                                            ? recorrido.Calado.CalidadMaterial.Id
                                            : 0;
                asignacion = ObtenerPrimero<AsignacionDeRecorrido, AsignacionDeRecorridoDto>(
                    x => x.Centro.Id == recorrido.Centro.Id &&
                         x.MaterialPorCentro.Id == idMaterialCentro &&
                         x.Calidad.Id == calidadMaterialId &&
                         today >= x.FechaDesde &&
                         today <= x.FechaHasta);
            }
            else
            {
                var idMaterialCentro = repositorio.ObtenerProyeccion<MaterialPorCentro, int>(
                    x => x.Centro.Id == recorrido.Centro.Id && x.Material.Id == recorrido.Material.Id,
                    x => x.Id);
                asignacion = ObtenerPrimero<AsignacionDeRecorrido, AsignacionDeRecorridoDto>(
                    x => x.Centro.Id == recorrido.Centro.Id &&
                         x.MaterialPorCentro.Id == idMaterialCentro &&
                         x.Workflow.Id == recorrido.Workflow.Id &&
                         today >= x.FechaDesde &&
                         today <= x.FechaHasta);
            }
            return asignacion;
        }

        public bool EsActividadAutomatica(Guid guid, string nombreActividad, string codigo)
        {
            var automatica = repositorio.Existe<ActividadPorDispositivo>(x =>
                                                                         x.Actividad == nombreActividad &&
                                                                         x.Workflow.Codigo == codigo &&
                                                                         x.PuestoDeTrabajo.Automatico);

            // Si es un vagón, la actividad debe ser manual
            return automatica && !repositorio.Existe<Recorrido>(x =>
                                                                x.InstanciaWorkflow == guid &&
                                                                x.TipoVehiculo == TipoVehiculo.Tren);
        }

        public ListaPaginada<ActividadConCargaAutomaticaDto> ListarPaginadoActividadesConCargaAutomatica(int centroId,
                                                                                                         string filtro,
                                                                                                         Paginacion
                                                                                                             paginacion)
        {
            return
                Listar<ActividadConCargaAutomatica, ActividadConCargaAutomaticaDto>(
                    x =>
                    x.Workflow.Centro.Id == centroId &&
                    (x.Actividad.Contains(filtro) || x.Workflow.Descripcion.Contains(filtro)), paginacion);
        }

        public ActividadConCargaAutomaticaDto ObtenerActividadConCargaAutomatica(int id)
        {
            return Obtener<ActividadConCargaAutomatica, ActividadConCargaAutomaticaDto>(id);
        }

        public bool ActividadEsEjecutable(string workflowCodigo, string actividad, string nombreUsuario)
        {
            return repositorio.Existe<ActividadConCargaAutomatica>(
                x => x.Workflow.Codigo == workflowCodigo && x.Actividad == actividad)
                   &&
                   repositorio.ObtenerConsultaEscalar(new UsuarioTienePermisoParaActividadConsulta(nombreUsuario,
                                                                                                   actividad));
        }

        public IList<string> ListarPermisosDeActividad()
        {
            return repositorio.Listar<Permiso, string>(x => x.ActividadWorkflow,
                                                       x => x.TipoPermiso == TipoPermiso.Actividad);
        }

        public int LeerPesoMaximo(Guid guid, TipoDeWorkflow tipoDeWorkflow)
        {
            var recorrido = repositorio.ObtenerProyeccion((Recorrido x) => x.InstanciaWorkflow == guid, x => new { x.TipoVehiculo, CentroId = x.Centro.Id });
            var pesoMaximoPorTipoVechiculo = repositorio.Obtener<PesoMaximoPorTipoVehiculo>(x => x.TipoVehiculo == recorrido.TipoVehiculo && x.Centro.Id == recorrido.CentroId);

            if (pesoMaximoPorTipoVechiculo == null)
            {
                return 0;
            }
            return tipoDeWorkflow == TipoDeWorkflow.Egreso ? pesoMaximoPorTipoVechiculo.PesoMaxEgreso : pesoMaximoPorTipoVechiculo.PesoMaxIngreso;
        }

        public int LeerToleranciaRechazo(Guid guid)
        {
            var asignacion = ObtenerAsignacionDePuestoComando(guid.ToString("D"));
            if (asignacion == null)
            {
                return 0;
            }
            var balanza = repositorio.Obtener<Balanza>(asignacion.BalanzaBrutoId ?? 0);
            if (balanza == null)
            {
                return 0;
            }
            return balanza.ToleranciaRechazo ?? 0;
        }

        public int ObtenerToleranciaRomaneo(Guid guid)
        {
            var asignacion = ObtenerAsignacionDePuestoComando(guid.ToString("D"));
            if (asignacion == null)
            {
                return 0;
            }
            var balanza = repositorio.Obtener<Balanza>(asignacion.BalanzaBrutoId ?? 0);
            if (balanza == null)
            {
                return 0;
            }
            return balanza.ToleranciaRechazo != null ? balanza.ToleranciaRechazo.Value : 0;
        }

        public int ObtenerToleranciaOrigen(Guid guid)
        {
            var asignacion = ObtenerAsignacionDePuestoComando(guid.ToString("D"));
            if (asignacion == null)
            {
                return 0;
            }
            var balanza = repositorio.Obtener<Balanza>(asignacion.BalanzaBrutoId ?? 0);
            if (balanza == null)
            {
                return 0;
            }
            return balanza.ToleranciaOrigen != null ? balanza.ToleranciaOrigen.Value : 0;
        }

        public bool VerificarCorrespondeControlDeBalanza(Guid guid)
        {
            return repositorio.Existe<Recorrido>(x => x.InstanciaWorkflow == guid && x.ControlBalanza);
        }

        public bool VerificarCorrespondeDescarga(Guid guid)
        {
            var recorrido = repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == guid);
            if (recorrido == null)
            {
                return false;
            }
            var material =
                repositorio.ObtenerPrimero<MaterialPorCentro>(
                    x => x.Centro.Id == recorrido.Centro.Id && x.Material.Id == recorrido.Material.Id);
            return material != null && material.CorrespondeDescarga;
        }

        public BalanzasDto ObtenerBalanzasPorGuid(Guid guid)
        {
            var balanzaTara = repositorio.ObtenerProyeccion<Recorrido, Balanza>(x => x.InstanciaWorkflow == guid,
                                                                                x => x.BalanzaTara);
            var balanzaBruto = repositorio.ObtenerProyeccion<Recorrido, Balanza>(x => x.InstanciaWorkflow == guid,
                                                                                 x => x.BalanzaBruto);
            return new BalanzasDto
            {
                BalanzaBruto = balanzaBruto != null ? conversor.Convertir<Balanza, BalanzaDto>(balanzaBruto) : null,
                BalanzaTara = balanzaTara != null ? conversor.Convertir<Balanza, BalanzaDto>(balanzaTara) : null
            };
        }

        public Formulario239RecorridoMaterialCentroDto ObtenerRecorridoMaterialCentroImpresionFormulario239(Guid guid)
        {
            var recorrido = repositorio.ObtenerProyeccion<Recorrido, Formulario239RecorridoMaterialCentroDto>(x => x.InstanciaWorkflow == guid, x =>
                new Formulario239RecorridoMaterialCentroDto
                {
                    CentroDescripcion = x.Centro.Descripcion,
                    CentroDireccion = x.Centro.Direccion,
                    CentroLocalidadDesc = x.Centro.Localidad.Descripcion,
                    CentroProvinciaDesc = x.Centro.Provincia.Descripcion,
                    MaterialCodigoSAP = x.Material.CodigoSAP,
                    MaterialDescripcion = x.Material.Descripcion,
                    TipoDocumentoIngreso = x.TipoDocumentoIngreso,
                    CTG = x.Vehiculo != null ? x.Vehiculo.CartaPorte.CTG : ""
                });
            return recorrido;
        }

        public ImpresionReciboMunicipalRecorridoDto ObtenerRecorridoImpresionReciboMunicipal(Guid guid)
        {
            return ObtenerImpresionReciboMunicipal(x => x.InstanciaWorkflow == guid);
        }

        public ImpresionReciboMunicipalRecorridoDto ObtenerRecorridoImpresionReciboMunicipalPorTarjeta(string numTarjeta)
        {
            return ObtenerImpresionReciboMunicipal(x => x.TarjetaDeAcceso == numTarjeta && !x.Terminado);
        }

        private ImpresionReciboMunicipalRecorridoDto ObtenerImpresionReciboMunicipal(Expression<Func<Recorrido, bool>> condicion)
        {
            var recorrido = repositorio.ObtenerMayor<Recorrido, int>(condicion, x => x.Id);
            var hoy = DateTime.Now;
            var reciboMunicipal = repositorio.ObtenerMayor<ReciboMunicipal, int>(x => x.Centro.Id == recorrido.Centro.Id && hoy >= x.FechaActivacion &&
            x.TipoVehiculo == recorrido.TipoVehiculo, x => x.Id);

            if (reciboMunicipal == null)
            {
                reciboMunicipal = repositorio.ObtenerMayor<ReciboMunicipal, int>(x => x.Centro.Id == recorrido.Centro.Id && hoy >= x.FechaActivacion &&
            x.TipoVehiculo == null, x => x.Id);
            }
            var pagoConMercadoPago = repositorio.Existe<PagoConMercadoPago>(x => x.Recorrido.Id == recorrido.Id && !string.IsNullOrEmpty(x.MercadoPagoId) && !x.Devuelto);

            return new ImpresionReciboMunicipalRecorridoDto
            {
                DocumentoInternoSap = recorrido != null ? recorrido.DocumentoInternoSap : null,
                NumeroDocumentoIngreso = recorrido != null ? recorrido.NumeroDocumentoIngreso : null,
                Ordenanza = reciboMunicipal != null ? reciboMunicipal.Ordenanza : string.Empty,
                Monto =
                        reciboMunicipal != null
                            ? reciboMunicipal.Monto.ToString(CultureInfo.InvariantCulture)
                            : string.Empty,
                TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso,
                TipoVehiculo = recorrido.TipoVehiculo,
                Patente = recorrido.Patente,
                RecorridoId = recorrido.Id,
                NombreTransportista = recorrido.Transportista.RazonSocial,
                PagoConMercadoPago = pagoConMercadoPago,
                MedioDePago = recorrido.Transportista.MedioDePago,
                Material = AutoMapper.Mapper.Map<Material, MaterialDto>(recorrido.Material)
                //Material = material
            };
        }

        public ImpresionCartaPorteUrenportRecorridoDto ObtenerRecorridoCartaPorteUrenport(int cartaporteId)
        {
            var cp = repositorio.ObtenerProyeccion((CartaPorte x) => x.Id == cartaporteId, x => new { Ruta = x.FotoRutaDestino, Numero = x.NroCartaPorte });
            string ruta = null;
            if (string.IsNullOrEmpty(cp.Ruta))
            {
                ruta = repositorio.ObtenerProyeccion<CartaPorte, string>(x => x.NroCartaPorte == cp.Numero && x.FotoRutaDestino != null, x => x.FotoRutaDestino);
            }
            return new ImpresionCartaPorteUrenportRecorridoDto
            {
                FotoRutaDestino = cp.Ruta ?? ruta
            };
        }

        public EstablecimientoDto ObtenerEstablecimiento(int id)
        {
            return Obtener<Establecimiento, EstablecimientoDto>(id);
        }

        public ListaPaginada<EstablecimientoDto> ListarPaginadoEstablecimientos(string filtro, Paginacion paginacion)
        {
            Expression<Func<Establecimiento, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = x => x.Proveedor.Activo && ((x.Proveedor.Descripcion.Contains(filtro)
                                                               || x.Proveedor.Cuil.Contains(filtro)) ||
                                                              x.NombreDeEstablecimiento.Contains(filtro) ||
                                                              x.CodigoDeEstablecimiento.Contains(filtro));
            }
            return Listar<Establecimiento, EstablecimientoDto>(expresionFiltro, paginacion);
        }

        public bool EsProveedorSustentable(int proveedorId)
        {
            return repositorio.Existe<Establecimiento>(x => x.Proveedor.Id == proveedorId && !x.Anulado);
        }

        public AsignacionDeEstablecimientoDto ObtenerAsignacionDeEstablecimiento(Guid instanceId)
        {
            var proveedor =
                repositorio.ObtenerProyeccion<Recorrido, Proveedor>(
                    x => x.InstanciaWorkflow == instanceId && x.Vehiculo != null,
                    x => x.Vehiculo.CartaPorte.TitularCartaPorte);
            var recorrido = ObtenerDatosDeInstanciaPorGuid(instanceId);
            return new AsignacionDeEstablecimientoDto
            {
                Proveedor = proveedor != null ? proveedor.Cuil + " - " + proveedor.Descripcion : "",
                InstanceId = instanceId,
                WorkflowDefinicionId = recorrido.WorkflowDefinicionId,
                Workflow = recorrido.WorkflowCodigo,
                RecorridoId = recorrido.Id,
                TipoVehiculo = recorrido.TipoVehiculo,
                Establecimientos =
                        proveedor != null
                            ? Listar<Establecimiento, EstablecimientoDto>(x => x.Proveedor.Id == proveedor.Id && !x.Anulado)
                            : new List<EstablecimientoDto>()
            };
        }

        public IList<DatosInstanciaWorkflowDto> ListarDatosDeWorkflows(List<Guid> instanceIds)
        {
            var datosAnalisis = repositorio.Listar(x => new { x.CaracteristicasNoCorrenspodenEspecial, x.Recorrido.InstanciaWorkflow }, (CaracteristicasAnalizadas x) => instanceIds.Contains(x.Recorrido.InstanciaWorkflow));
            var caladoPorCaracteristica = repositorio.Listar<CaladoPorCaracteristica>(y => instanceIds.Contains(y.Calado.WorkflowInstanceId));
            var caladosPorCaracteristicaConProteina = caladoPorCaracteristica.Where(x => x.CaracteristicaDeCalidad.DescripcionCorta.ToUpper() == "PROTEINA");
            var datos = repositorio.Listar<Recorrido, DatosInstanciaWorkflowDto>(
                    x =>
                    new DatosInstanciaWorkflowDto
                    {
                        RecorridoId = x.Id,
                        Cuit = x.Transportista.Cuit,
                        Id = x.InstanciaWorkflow,
                        Material = x.Material.Descripcion,
                        MaterialCodigoSap = x.Material.CodigoSAP,
                        MaterialId = x.Material.Id,
                        Patente = x.Patente,
                        Transportista = x.Transportista.RazonSocial,
                        TransportistaId = x.Transportista.Id,
                        Rechazado = x.Rechazado,
                        Workflow = x.Workflow.Descripcion,
                        ChoferDNI = x.Chofer.NumeroDeDocumento,
                        ChoferNombre = x.Chofer.Nombre + " " + x.Chofer.Apellido,
                        NumeroDeTarjeta = x.TarjetaDeAcceso,
                        Sustentable = x.Establecimiento != null,
                        EsEspecial = x.Vehiculo != null && x.Vehiculo.CartaPorte.TrigoEspecial != null && x.Vehiculo.CartaPorte.TrigoEspecial.Value,
                        Entregador = x.Vehiculo != null && x.Vehiculo.CartaPorte.Entregador != null ? x.Vehiculo.CartaPorte.Entregador.RazonSocial : "",
                        Cosecha = x.Vehiculo != null ? x.Vehiculo.CartaPorte.Cosecha : "",
                        LocalidadId = x.Vehiculo != null ? x.Vehiculo.CartaPorte.Procedencia.Id : 0,
                        ProvinciaId = x.Vehiculo != null ? x.Vehiculo.CartaPorte.Procedencia.Provincia.Id : 0,
                        VehiculoDemorado = x.VehiculoDemorado || x.EstablecimientoDemorado,
                        LlegoEnHorario = x.LlegoEnHorario,
                        Proteina = "",
                        AlmacenDestino = x.Almacen.DescripcionCorta != null ? x.Almacen.DescripcionCorta : "",
                        DiferenciaPesoNeto = x.PesoTara.HasValue && x.PesoBruto.HasValue ? x.PesoBruto - x.PesoTara - (x.PesoBrutoOrigen - x.PesoTaraOrigen) : null,
                        SojaEPA = x.Establecimiento != null ? x.Establecimiento.EPA : false
                    }, x => instanceIds.Contains(x.InstanciaWorkflow),
                    instanceIds.Count); ;
            foreach (var dato in datos)
            {
                dato.Proteina = caladosPorCaracteristicaConProteina.Where(x => x.Calado.WorkflowInstanceId == dato.Id).FirstOrDefault() != null ? caladosPorCaracteristicaConProteina?.Where(x => x.Calado.WorkflowInstanceId == dato.Id).FirstOrDefault().ValorCalado.ToString() : "";
            }

            foreach (var datoAnalisis in datosAnalisis)
            {
                var dato = datos.FirstOrDefault(x => x.Id == datoAnalisis.InstanciaWorkflow);
                if (dato != null)
                {
                    dato.CaracteristicasNoCorrenspodenEspecial = datoAnalisis.CaracteristicasNoCorrenspodenEspecial;
                }
            }

            var reglasDeAnalisisObligatorioActivas = ListarReglaDeAnalisisObligatorioActivas();
            foreach (DatosInstanciaWorkflowDto dato in datos)
            {
                dato.AnalisisObligatorio = reglasDeAnalisisObligatorioActivas.Any(x =>
                   ((x.LocalidadId == dato.LocalidadId) || (x.LocalidadId == 0 && x.ProvinciaId == dato.ProvinciaId)) &&
                    x.MaterialId == dato.MaterialId &&
                    x.Cosecha == dato.Cosecha);
            }

            return datos;
        }

        public DatosInstanciaWorkflowPuertoDto ListarDatosDeWorkflowsPuerto(List<Guid> instanceIds)
        {
            return new DatosInstanciaWorkflowPuertoDto
            {
                LineUps = conversor.ConvertirList<LineUp, LineUpDto>(repositorio.Listar<LineUp>(x => instanceIds.Contains(x.Recorrido.InstanciaWorkflow))),
                Embarques = conversor.ConvertirList<Embarque, EmbarqueDto>(repositorio.Listar<Embarque>(x => instanceIds.Contains(x.Recorrido.InstanciaWorkflow)))
            };
        }

        public IList<InstanciaWorkflowDto> ListarDatosDeWorkflowsPendientes(int centroId, int cantidad)
        {
            try
            {
                return repositorio.ListarConsulta<InstanciaWorkflowDto>(
                    new ListarDatosDeWorkflowsPendientesConsulta(centroId, cantidad));
            }
            catch (Exception e)
            {
                log.Error(e, "Error al listar camiones pendientes");
            }
            return new List<InstanciaWorkflowDto>();
        }

        public TimeSpan? ObtenerTiempoEntreActividades(Guid instanceId, string actividadDesde, string actividadHasta)
        {
            TimeSpan? tiempo = null;
            var fechaDesde = repositorio.ObtenerMayor<LogActividad, int, DateTime>(
                x => x.WorkflowInstanceId == instanceId && x.ActividadXaml == actividadDesde,
                x => x.Id,
                x => x.Fecha);
            var fechaHasta = repositorio.ObtenerMayor<LogActividad, int, DateTime>(
                x => x.WorkflowInstanceId == instanceId && x.ActividadXaml == actividadHasta,
                x => x.Id,
                x => x.Fecha);

            if (fechaDesde != DateTime.MinValue && fechaHasta != DateTime.MinValue)
            {
                tiempo = fechaHasta - fechaDesde;
            }
            return tiempo;
        }

        public IList<int> ObtenerPuestosIdPorPC(string nombrePc)
        {
            return repositorio.Listar<PuestoDeTrabajo, int>(x => x.Id, x => x.NombrePc == nombrePc);
        }

        public bool VerificarPuestodeCargaDescarga(Guid instanceId, int puestoDeTrabajoId)
        {
            // Si el recorrido tiene asignado puestos de carga/desgarga: validar que alguno esté asignado al puesto de trabajo
            // Si no tiene puesto de carga/descarga asociado: puedo descargar en cualquier lugar
            return repositorio.Existe<Recorrido>(r =>
                                                 r.InstanciaWorkflow == instanceId &&
                                                 (!r.PuestosDeCargaDescargas.Any() ||
                                                  r.PuestosDeCargaDescargas.Any(
                                                      p => p.PuestoDeTrabajo.Id == puestoDeTrabajoId))
                );
        }

        public bool MaterialImprimeReciboMunicipal(Guid instanceId)
        {
            try
            {
                var materialCentro = repositorio.ObtenerProyeccion((Recorrido x) => x.InstanciaWorkflow == instanceId, x => new { MaterialId = x.Material.Id, CentroId = x.Centro.Id });

                var retorno = repositorio.ObtenerProyeccion<MaterialPorCentro, bool>(
                    x => x.Material.Id == materialCentro.MaterialId && x.Centro.Id == materialCentro.CentroId,
                    x => x.ImprimeReciboMunicipal);
                log.Debug("Material {0} ImprimeReciboMunicipal {1}", instanceId, retorno);
                return retorno;
            }
            catch (Exception e)
            {
                log.Error(e, "Fallo MaterialImprimeReciboMunicipal {0}", instanceId);
                throw new Exception("Fallo MaterialImprimeReciboMunicipal", e);
            }
        }

        public bool? RecorridoPagaTicketMunicipal(Guid instanceId)
        {
            var recorridoPagaTicketMunicipal =
                    repositorio.ObtenerProyeccion<Recorrido, bool?>(x => x.InstanciaWorkflow == instanceId,
                                                  x => x.PagaTicketMunicipal);
            return recorridoPagaTicketMunicipal;
        }

        public IList<TecnologiaDto> ListarTecnologias()
        {
            return Listar<Tecnologia, TecnologiaDto>(x => true, 50);
        }

        public IList<EmpresaDto> ListarEmpresas()
        {
            return Listar<Empresa, EmpresaDto>(x => true, 50);
        }

        public ListaPaginada<EmpresaDto> ListarPaginadoEmpresa(string filtro, Paginacion paginacion)
        {
            return Listar<Empresa, EmpresaDto>(x => filtro == null || x.Nombre.Contains(filtro), paginacion);
        }

        public ListaPaginada<TecnologiaDto> ListarPaginadoTecnologia(string filtro, Paginacion paginacion)
        {
            return Listar<Tecnologia, TecnologiaDto>(x => filtro == null || x.Nombre.Contains(filtro), paginacion);
        }

        public ListaPaginada<ProveedorExcluidoIntactaDto> ListarPaginadoProveedorExcluidoIntacta(string filtro,
                                                                                                 Paginacion paginacion)
        {
            return
                Listar<ProveedorExcluidoIntacta, ProveedorExcluidoIntactaDto>(
                    x => filtro == null || x.Proveedor.RazonSocial.Contains(filtro), paginacion);
        }

        public EmpresaDto ObtenerEmpresa(int id)
        {
            return Obtener<Empresa, EmpresaDto>(id);
        }

        public TecnologiaDto ObtenerTecnologia(int id)
        {
            return Obtener<Tecnologia, TecnologiaDto>(id);
        }

        public bool RequiereTecnologia(Guid instanceId)
        {
            var recorrido = repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == instanceId);
            return
                repositorio.ObtenerProyeccion<MaterialPorCentro, bool>(
                    x => x.Material.Id == recorrido.Material.Id && x.Centro.Id == recorrido.Centro.Id,
                    x => x.RequiereTecnologia);
        }

        public bool CorrespondeRegistrarCartadePorteTren(Guid instanceId)
        {
            var cartaPorteId = repositorio.ObtenerProyeccion<Recorrido, int>(x => x.InstanciaWorkflow == instanceId,
                                                                             x => x.Vehiculo.CartaPorte.Id);
            var instancesIds =
                repositorio.Listar<Recorrido, Guid>(x => x.InstanciaWorkflow,
                                                    x => x.Vehiculo.CartaPorte.Id == cartaPorteId).ToList();

            return
                repositorio.ObtenerMenor<LogActividad, int, Guid>(
                    x =>
                    instancesIds.Contains(x.WorkflowInstanceId) &&
                    x.ActividadXaml == "ServicioMonsantoRegistrarCartadePorte", x => x.Id, x => x.WorkflowInstanceId) ==
                instanceId;
        }

        public int ObtenerCantidadVagones(Guid instanceId)
        {
            var cartaPorteId = repositorio.ObtenerProyeccion<Recorrido, int>(x => x.InstanciaWorkflow == instanceId,
                                                                             x => x.Vehiculo.CartaPorte.Id);
            return repositorio.Contar<Vehiculo>(x => x.CartaPorte.Id == cartaPorteId);
        }

        public IList<Guid> ObtenerGuidVagones(Guid instanceId)
        {
            var cartaPorteId = repositorio.ObtenerProyeccion<Recorrido, int>(x => x.InstanciaWorkflow == instanceId,
                                                                             x => x.Vehiculo.CartaPorte.Id);
            return repositorio.Listar<Recorrido, Guid>(x => x.InstanciaWorkflow, x => x.Vehiculo.CartaPorte.Id == cartaPorteId);
        }

        public decimal PorcentajeMuestraAuditoria(Guid instanceId)
        {
            decimal porcentaje = 0;
            var caladoYAnalisis = ListarAnalisisYCaladoPorCaracteristicaEF(instanceId);
            if (caladoYAnalisis != null)
            {
                porcentaje = calculadora.CalcularPorcentajeMuestraAuditoria(caladoYAnalisis, instanceId);
            }
            return porcentaje;
        }

        public IList<AnalisisVagonDto> ObtenerAnalisisPorVagones(Guid instanceId)
        {
            var cartaPorteId = repositorio.ObtenerProyeccion<Recorrido, int>(x => x.InstanciaWorkflow == instanceId,
                                                                             x => x.Vehiculo.CartaPorte.Id);
            var recorridos = repositorio.Listar<Recorrido>(x => x.Vehiculo.CartaPorte.Id == cartaPorteId);

            return
                recorridos.Select(
                    x =>
                    new AnalisisVagonDto
                    {
                        AnalisisDeCalidad =
                                conversor.Convertir<AnalisisDeCalidad, AnalisisDeCalidadDto>(x.AnalisisDeCalidad),
                        Calado = conversor.Convertir<Calado, CaladoDto>(x.Calado),
                        NumeroVagon = x.Vehiculo.NumeroVehiculo,
                        PesoNeto = x.PesoBruto ?? 0 - x.PesoTara ?? 0
                    }).ToList();
        }

        public bool CorrespondeRegistrarMuestreoYPesajeTren(Guid instanceId)
        {
            var cartaPorteId = repositorio.ObtenerProyeccion<Recorrido, int>(x => x.InstanciaWorkflow == instanceId,
                                                                             x => x.Vehiculo.CartaPorte.Id);

            var instancesIds =
                repositorio.Listar<Recorrido, Guid>(x => x.InstanciaWorkflow,
                                                    x => x.Vehiculo.CartaPorte.Id == cartaPorteId).ToList();
            var cantidadDeVagones = instancesIds.Count();
            return
                repositorio.Contar<LogActividad>(
                    x =>
                    instancesIds.Contains(x.WorkflowInstanceId) &&
                    x.ActividadXaml == "ServicioMonsantoRegistrarMuestreoYPesaje") == cantidadDeVagones
                &&
                repositorio.ObtenerMayor<LogActividad, int, Guid>(
                    x =>
                    instancesIds.Contains(x.WorkflowInstanceId) &&
                    x.ActividadXaml == "ServicioMonsantoRegistrarMuestreoYPesaje", x => x.Id, x => x.WorkflowInstanceId) ==
                instanceId;
        }

        public CartaDePorteRegistradaServicioMonsantoDto ObtenerCartaDePorteRegistradaServicioMonsanto(Guid instanceId,
                                                                                                       TipoVehiculo tipo)
        {
            if (tipo != TipoVehiculo.Tren)
            {
                return
                    Obtener<CartaDePorteRegistradaServicioMonsanto, CartaDePorteRegistradaServicioMonsantoDto>(
                        x => x.Recorrido.InstanciaWorkflow == instanceId);
            }
            var cartaPorteId = repositorio.ObtenerProyeccion<Recorrido, int>(x => x.InstanciaWorkflow == instanceId,
                                                                             x => x.Vehiculo.CartaPorte.Id);
            var instancesIds =
                repositorio.Listar<Recorrido, Guid>(x => x.InstanciaWorkflow,
                                                    x => x.Vehiculo.CartaPorte.Id == cartaPorteId).ToList();
            var registro =
                repositorio.ObtenerMayor<CartaDePorteRegistradaServicioMonsanto, int>(
                    x => instancesIds.Contains(x.Recorrido.InstanciaWorkflow), x => x.Id);
            return
                conversor.Convertir<CartaDePorteRegistradaServicioMonsanto, CartaDePorteRegistradaServicioMonsantoDto>(
                    registro);
        }

        public Guid ObtenerRecorridoInstanceIdPorRecorridoId(int id)
        {
            return repositorio.ObtenerProyeccion<Recorrido, Guid>(x => x.Id == id, x => x.InstanciaWorkflow);
        }

        public bool TienePermiso(string nombreUsuario, PermisosScato permiso)
        {
            return repositorio.ObtenerConsultaEscalar(new PermisoPorUsuarioyCodigo(nombreUsuario, permiso));
        }

        public bool CartaPorteTieneEntregador(int id)
        {
            return repositorio.ObtenerProyeccion<CartaPorte, bool>(x => x.Id == id, x => x.Entregador.CodigoSAPCondicionFiscal != "999999");
        }

        public InfoCaladoDto ObtenerInformacionCartaPorte(int recorridoId)
        {
            var cartaPorte = repositorio.ObtenerProyeccion<Recorrido, CartaPorte>(x => x.Id == recorridoId && x.Vehiculo != null, x => x.Vehiculo.CartaPorte);
            return conversor.Convertir<CartaPorte, InfoCaladoDto>(cartaPorte) ?? new InfoCaladoDto();
        }

        public HumedimetroDto ObtenerHumedimetroPorNombrePc(int centroId, string nombrePc)
        {
            return Obtener<Humedimetro, HumedimetroDto>(x => x.Centro.Id == centroId && x.PuestoDeTrabajo == nombrePc);
        }

        public bool EsProveedorExcluidoIntacta(int id)
        {
            return repositorio.Existe<ProveedorExcluidoIntacta>(p => p.Proveedor.Id == id);
        }

        public LoteBiotecnologiaDto ObtenerLoteBiotecnologiaParaArchivo(int loteId)
        {
            var lotedto = repositorio.ObtenerProyeccion<LoteBiotecnologia, LoteBiotecnologiaDto>(x => x.Id == loteId, lote => new LoteBiotecnologiaDto
            {
                CamaraFormatoDeArchivo = lote.Camara.FormatoDeArchivo ?? CamaraFormatoDeArchivo.NoEspecificado,
                CamaraId = lote.Camara.Id,
                CentroId = lote.Centro.Id,
                CentroDesc = lote.Centro.Descripcion,
                CamaraEmail = lote.Camara.Email,
                NumeroDeLote = lote.NumeroDeLote,
                Id = lote.Id
            });
            lotedto.Muestras = repositorio.ListarConsulta(new ListarMuestraEnvioACamaraIntactaParaArchivoConsulta(loteId, firmaProvider.ObtenerFirmaSinLogo().CodigoSAP));
            return lotedto;
        }

        public LoteAuditoriaDto ObtenerLoteAuditoriaParaArchivo(int loteId)
        {
            var lotedto = repositorio.ObtenerProyeccion<LoteAuditoria, LoteAuditoriaDto>(x => x.Id == loteId, lote => new LoteAuditoriaDto
            {
                CamaraFormatoDeArchivo = lote.Camara.FormatoDeArchivo ?? CamaraFormatoDeArchivo.NoEspecificado,
                CamaraId = lote.Camara.Id,
                CentroId = lote.Centro.Id,
                CentroDesc = lote.Centro.Descripcion,
                CamaraEmail = lote.Camara.Email,
                NumeroDeLote = lote.NumeroDeLote,
                Id = lote.Id
            });
            lotedto.Muestras = repositorio.ListarConsulta(new ListarMuestraEnvioACamaraAuditoriaParaArchivoConsulta(loteId, firmaProvider.ObtenerFirmaSinLogo().CodigoSAP));
            return lotedto;
        }

        public bool ExistenMuestraEnvioACamaraAuditoriaPendientes(int materialId, int centroId)
        {
            return
                repositorio.ObtenerConsultaEscalar(new ExistenMuestraEnvioACamaraAuditoriaPendientesConsulta(materialId, centroId));
        }

        public bool ExistenMuestraEnvioACamaraIntactaPendientes(int materialId, int centroId)
        {
            return
                repositorio.ObtenerConsultaEscalar(new ExistenMuestraEnvioACamaraIntactaPendientesConsulta(materialId, centroId));
        }

        public bool ExistenMovimientosDeTercerosPendientes(int materialId, TipoDeWorkflow tipoDeWorkflow, int centroId)
        {
            return
                repositorio.ObtenerConsultaEscalar(new ExistenMovimientosDeTercerosPendientesConsulta(materialId, tipoDeWorkflow, centroId));
        }

        public LoteBiotecnologiaDto ObtenerLoteBiotecnologiaParaImpresion(int loteId)
        {
            var lotedto = repositorio.ObtenerProyeccion<LoteBiotecnologia, LoteBiotecnologiaDto>(x => x.Id == loteId, lote => new LoteBiotecnologiaDto
            {
                CamaraFormatoDeArchivo = lote.Camara.FormatoDeArchivo ?? CamaraFormatoDeArchivo.NoEspecificado,
                CamaraId = lote.Camara.Id,
                CentroId = lote.Centro.Id,
                NumeroDeLote = lote.NumeroDeLote,
                Id = lote.Id
            });
            lotedto.Muestras = repositorio.ListarConsultaPaginada(new ListarMuestraEnvioACamaraIntactaParaImpresionConsulta(loteId)).Items;
            return lotedto;
        }

        public LoteAuditoriaDto ObtenerLoteAuditoriaParaImpresion(int loteId)
        {
            var lotedto = repositorio.ObtenerProyeccion<LoteAuditoria, LoteAuditoriaDto>(x => x.Id == loteId, lote => new LoteAuditoriaDto
            {
                CamaraFormatoDeArchivo = lote.Camara.FormatoDeArchivo ?? CamaraFormatoDeArchivo.NoEspecificado,
                CamaraId = lote.Camara.Id,
                CentroId = lote.Centro.Id,
                NumeroDeLote = lote.NumeroDeLote,
                Id = lote.Id
            });
            lotedto.Muestras = repositorio.ListarConsultaPaginada(new ListarMuestraEnvioACamaraAuditoriaParaImpresionConsulta(loteId)).Items;
            return lotedto;
        }

        public ListaPaginada<MuestraEnvioACamaraBiotecnoligiaDto> ListarMuestrasPorLoteBiotecnologia(int loteId, Paginacion paginacion)
        {
            return repositorio.ListarConsultaPaginada(new ListarMuestraEnvioACamaraIntactaParaImpresionConsulta(loteId, paginacion));
        }

        public ListaPaginada<MuestraEnvioACamaraAuditoriaDto> ListarMuestrasPorLoteAuditoria(int loteId, Paginacion paginacion)
        {
            return repositorio.ListarConsultaPaginada(new ListarMuestraEnvioACamaraAuditoriaParaImpresionConsulta(loteId, paginacion));
        }

        public ListaPaginada<MovimientoDeTercerosListaDto> ListarMuestrasPorArchivoDeMovimientos(int loteId, Paginacion paginacion)
        {
            return repositorio.ListarConsultaPaginada(new ListarMovimientoDeTercerosParaImpresionConsulta(loteId, paginacion));
        }

        public ImpresionDto ObtenerUltimaImpresionPorInstanceIdYCodigo(Guid id, string codigo)
        {
            var impresion = repositorio.Obtener<Dominio.Entidades.Impresion>(x => x.WorkflowId == id && x.Codigo == codigo && !x.Eliminada);
            if (impresion == null)
            {
                return null;
            }
            return conversor.Convertir<Dominio.Entidades.Impresion, ImpresionDto>(impresion);
        }

        public IList<ZonaDto> ListarZonas()
        {
            return Listar<Zona, ZonaDto>();
        }

        public IList<SubZonaDto> ListarSubZonasPorZona(int zonaId)
        {
            return Listar<SubZona, SubZonaDto>(x => x.Zona.Id == zonaId);
        }

        public TransmisionSapAModificarDto ObtenerTransmisionASapPorIdyFuncionSap(int id)
        {
            var transmison = repositorio.ObtenerConsultaEscalar(new ObtenerTransmisionASap(id));
            var transmisionDto = new TransmisionSapAModificarDto { Id = id, Campos = transmison.ObtenerPropiedades(), EstadoTransmision = transmison.Estado };
            return transmisionDto;
        }

        public IList<int> ObtenerTransmisionesCuposIdsConError()
        {
            return repositorio.Listar<TransmisionASap, int>(x => x.Id, x => x.FuncionSap == FuncionSAP.InformarCupo && x.Estado == EstadoTransmisionASap.Error);
        }

        public PanelServerAppPoolDto ObtenerEstadoServidor(string nombre)
        {
            var resultado = new PanelServerAppPoolDto { ServidorNombre = nombre };
            try
            {
                //Servidor
                var memoriaDisponibleDisco = new PerformanceCounter("LogicalDisk", "Free Megabytes", "_Total");
                var ramUtilizada = new PerformanceCounter("Memory", "Available MBytes", "");
                var cargaProcesador = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                var sesiones = new PerformanceCounter("Server", "Server Sessions", "");
                var cantidadDeConexionesEstablecidas = new PerformanceCounter("TCPv4", "Connections Established");
                resultado.DiscoDisponible = (int)memoriaDisponibleDisco.NextValue() / 1024;
                resultado.Sesiones = (int)sesiones.NextValue();
                resultado.MemoriaDisponible = (int)ramUtilizada.NextValue();
                CounterSample c1 = cargaProcesador.NextSample();
                System.Threading.Thread.Sleep(100);
                CounterSample c2 = cargaProcesador.NextSample();

                var cargaFinal = (int)CounterSample.Calculate(c1, c2);
                resultado.CargaProcesador = cargaFinal;
                resultado.CantidadDeConexiones = (int)cantidadDeConexionesEstablecidas.NextValue();
                //AppPools
                resultado.AppPools = ListarAppPools();
            }
            catch (Exception e)
            {
                resultado.Error = String.Format("Falla en el acceso al Servidor {0}: " + e.Message, nombre);
            }

            return resultado;
        }

        public IEnumerable<AppPoolDto> ListarAppPools()
        {
            var serverManager = new ServerManager();
            var applicationPoolCollection = serverManager.ApplicationPools;
            var resultado = new List<AppPoolDto>();

            foreach (var applicationPool in applicationPoolCollection)
            {
                try
                {
                    resultado.Add(new AppPoolDto
                    {
                        Estado = applicationPool.State.ToString(),
                        Nombre = applicationPool.Name,
                        Procesos = applicationPool.WorkerProcesses.Count,
                        TiempoReciclado =
                                applicationPool.Recycling.PeriodicRestart.Time.TotalMinutes.ToString(
                                    CultureInfo.InvariantCulture)
                    });
                    ApplicationPoolRecycling log = applicationPool.Recycling;
                }
                catch (Exception e)
                {
                    resultado.Add(new AppPoolDto { Error = String.Format("Falla al intentar acceder al AppPool {0}: " + e.Message, applicationPool.Name) });
                }
            }
            return resultado;
        }

        public void IniciarAppPool(string nombre)
        {
            var serverManager = new ServerManager();
            var applicationPool = serverManager.ApplicationPools[nombre];

            if (applicationPool.State == ObjectState.Stopped)
            {
                applicationPool.Start();
            }
        }

        public void DetenerAppPool(string nombre)
        {
            var serverManager = new ServerManager();
            var applicationPool = serverManager.ApplicationPools[nombre];

            if (applicationPool.State == ObjectState.Started)
            {
                applicationPool.Stop();
            }
        }

        public void ReciclarAppPool(string nombre)
        {
            var serverManager = new ServerManager();
            var applicationPool = serverManager.ApplicationPools[nombre];
            var estadoServidor = ObtenerEstadoServidor(nombre);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            repositorio.Listar<TipoDocumentoIdentidad>();
            stopwatch.Stop();

            log.Debug("{0} -> Estado del servidor: Memoria: {1}mb | Procesador: {2}% | Espacio en Disco: {3}gb | Tiempo de respuesta de la DB: {4}",
                        nombre,
                        estadoServidor.MemoriaDisponible,
                        estadoServidor.CargaProcesador,
                        estadoServidor.DiscoDisponible,
                        stopwatch.Elapsed
                        );
            if (applicationPool.State == ObjectState.Started)
            {
                applicationPool.Recycle();
            }
        }

        public void ConfigurarTiempoReciclado(string nombre, double tiempo)
        {
            var serverManager = new ServerManager();
            var applicationPool = serverManager.ApplicationPools[nombre];

            applicationPool.Recycling.PeriodicRestart.Time = TimeSpan.FromMinutes(tiempo);

            serverManager.CommitChanges();
        }

        public List<EventLogEntry> ObtenerLogEventos(string serverNombre, int idEvento, DateTime fechaDesde, DateTime fechaHasta)
        {
            var eventLog = new EventLog("System");
            if (serverNombre != "localhost")
            {
                eventLog = new EventLog("System", serverNombre);
            }

            var collection = eventLog.Entries;

            return (from object eventoLog in collection
                    let even = (EventLogEntry)eventoLog
                    where (even.EventID == idEvento) && (fechaDesde <= even.TimeGenerated) && (even.TimeGenerated <= fechaHasta)
                    select eventoLog).Cast<EventLogEntry>().ToList();
        }

        public DatosDeWorkflowsDto ListarRecorridosEnPlayaExternaPorCentro(FiltroListaDeWorkflowsDto filtro, Paginacion paginacion)
        {
            var datosDeWorkflows = new DatosDeWorkflowsDto
            {
                Calidades = new List<CalidadMaterialDto>(),
            };

            datosDeWorkflows.Workflows = repositorio.ListarConsultaPaginada(new ListarRecorridosEnPlayaExterna(filtro, paginacion));

            datosDeWorkflows.WorkflowsCentro = Listar<Workflow, WorkflowDto>(w => w.Centro.Id == filtro.CentroId && w.Activo);
            return datosDeWorkflows;
        }

        public ListaPaginada<InstanciaWorkflowDto> ListarRecorridosRechazados(FiltroListaDeWorkflowsDto filtro, Paginacion paginacion)
        {
            return repositorio.ListarConsultaPaginada(new ListarRecorridosRechazados(filtro, paginacion));
        }

        public List<ControlRecorridoLogActividadConsultaDto> ConsultaControlRecorridoLogActividad(Guid instanceId)
        {
            return repositorio.ListarConsulta(new ControlRecorridoLogActividadConsulta(instanceId));
        }

        public MovimientoDeBinesDto ObtenerMovimientoDeBines(int id)
        {
            return Obtener<MovimientoDeBines, MovimientoDeBinesDto>(id);
        }

        public int ObtenerStock(TipoStockBines tipo, DateTime fecha, int id, int materialId)
        {
            return repositorio.ObtenerConsultaEscalar(new ObtenerStock(tipo, fecha, id, materialId));
        }

        public int LibroMovimientosExistenciaGranosCalcularHojas(LibroMovimientosExistenciaGranosDto dto, int centroId)
        {
            return repositorio.ObtenerConsultaEscalar(new ObtenerLibroMovimientosExistenciaGranosCalcularHojas(centroId, dto.MaterialId, dto.MaterialDescripcionCorta, dto.FechaDesde, dto.FechaHasta));
        }

        public FirmaDto ObtenerFirma()
        {
            return ObtenerPrimero<Firma, FirmaDto>(x => true);
        }

        public bool LibroOnccaEsPorDescripcionCorta(int id)
        {
            return repositorio.ObtenerProyeccion<Centro, bool>(x => x.Id == id, x => x.LibroOnccaPorDescripcionCorta);
        }

        public string BuscarDescripcionCortaMaterial(int centroId, string criteria)
        {
            Material material;
            try
            {
                material =
                    repositorio.ObtenerPrimero<MaterialPorCentro>(
                        f => f.Material.Activo && (f.Material.DescripcionCorta.Contains(criteria)) && f.Centro.Id == centroId)
                               .Material;
            }
            catch
            {
                material = null;
            }
            return material == null ? null : material.DescripcionCorta;
        }

        public IList<string> BuscarDescripcionMaterialesPorCentro(int centroId, string filtro)
        {
            return repositorio.ListarDistintos<MaterialPorCentro, string>(x => x.Material.DescripcionCorta,
                                                                 m =>
                                                                 m.Material.Activo && m.Centro.Id == centroId &&
                                                                 m.Material.DescripcionCorta.Contains(filtro), 20).Where(x => x != null).ToList();
        }

        public List<ServicioDto> ListarEstadoDeServicios()
        {
            var clientSection = (WebConfigurationManager.GetSection("system.serviceModel/client") as ClientSection);
            var model = new List<ServicioDto>();
            foreach (ChannelEndpointElement cee in clientSection.Endpoints.Cast<ChannelEndpointElement>().Where(cee => !cee.Address.AbsoluteUri.StartsWith("net.msmq")))
            {
                model.Add(new ServicioDto
                {
                    Nombre = cee.Name,
                    Url = cee.Address.AbsoluteUri
                });
            }
            return model;
        }

        public DateTime? ObtenerUltimaPesadaFechaPorGuid(Guid id)
        {
            var recorrido = repositorio.Obtener<Recorrido>(r => r.InstanciaWorkflow == id);
            var ultimaFecha = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso ? recorrido.PesoTaraFecha : recorrido.PesoBrutoFecha;
            return ultimaFecha;
        }

        public int ObtenerProveedorIdPorRecorrido(Guid workflowInstanceId)
        {
            var recorrido = repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == workflowInstanceId);
            if (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte)
            {
                return recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Id;
            }
            if (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.Remito)
            {
                var remito = repositorio.Obtener<Remito>(x => x.Recorrido.Id == recorrido.Id);
                if (remito != null && remito.ProveedorOrigen != null)
                {
                    return remito.ProveedorOrigen.Id;
                }
            }
            return 0;
        }

        public bool MuestraConjuntoFueUtilizada(Guid workflowInstanceId, int muestraConj)
        {
            return repositorio.ObtenerConsultaEscalar(new ExisteMuestraConjuntoConsulta(ObtenerProveedorIdPorRecorrido(workflowInstanceId), muestraConj));
        }

        public DestinatarioCTGDto ObtenerDestinatarioCTGporGuid(Guid workflowInstanceId)
        {
            DestinatarioCTG destinatarioCTG;
            try
            {
                destinatarioCTG = repositorio.Obtener<DestinatarioCTG>(x => x.Recorrido.InstanciaWorkflow == workflowInstanceId);
            }
            catch
            {
                destinatarioCTG = null;
            }
            return destinatarioCTG == null ? null : conversor.Convertir<DestinatarioCTG, DestinatarioCTGDto>(destinatarioCTG);
        }

        public ArchivoDeMovimientosDto ObtenerMovimientosDeTercerosParaArchivo(int archivoId)
        {
            var lotedto = repositorio.ObtenerProyeccion<ArchivoDeMovimientos, ArchivoDeMovimientosDto>(x => x.Id == archivoId, lote => new ArchivoDeMovimientosDto
            {
                NumeroDeArchivo = lote.NumeroDeArchivo,
                TipoDeWorkflow = lote.TipoDeWorkflow,
                Id = lote.Id
            });
            lotedto.Movimientos = lotedto.TipoDeWorkflow == TipoDeWorkflow.Ingreso ? repositorio.ListarConsulta(new ListarArchivoDeMovimientosIngresosParaArchivoConsulta(archivoId)) : repositorio.ListarConsulta(new ListarArchivoDeMovimientosEgresosParaArchivoConsulta(archivoId));
            return lotedto;
        }

        public IList<FirmaDto> ListarFirmas()
        {
            return repositorio.Listar<Firma, FirmaDto>(x => new FirmaDto { RazonSocial = x.RazonSocial, Id = x.Id });
        }

        public IngresoDeDatosDeExportacionDto ObtenerIngresoDeDatosDeExportacionPorRecorrido(int id)
        {
            var datos = repositorio.ObtenerMayor<IngresoDeDatosDeExportacion, int>(x => x.Recorrido.Id == id, x => x.Id);
            if (datos != null)
            {
                return conversor.Convertir<IngresoDeDatosDeExportacion, IngresoDeDatosDeExportacionDto>(datos);
            }
            return null;
        }

        public List<string> ListarCuitfirmas()
        {
            return repositorio.Listar<Firma, string>(x => x.Cuit, x => true).ToList();
        }

        public IList<ColaImpresionDto> ConsultarColaImpresion(int id, string servidor)
        {
            var lista = new List<ColaImpresionDto>();
            try
            {
                var impresora = repositorio.Obtener<Impresora>(id);
                var nombreImpresora = impresora.Direccion.Split('\\')[3];
                var nombreServidor = impresora.Direccion.Split('\\')[2];
                var appServer = servidor.Split('/')[2];
                var servidorImpresion = new PrintServer(@"\\" + nombreServidor);
                var cola = new PrintQueue(servidorImpresion, nombreImpresora);
                PrintJobInfoCollection jobs = cola.GetPrintJobInfoCollection();
                foreach (PrintSystemJobInfo job in jobs)
                {
                    var colaImpresion = new ColaImpresionDto
                    {
                        Documento = job.Name,
                        Estado = job.JobStatus.ToString(),
                        Fecha = job.TimeJobSubmitted,
                        Paginas = job.NumberOfPages,
                        Tamanio = job.JobSize / 1024,
                        Servidor = appServer,
                        ServidorUrl = servidor,
                        JobId = job.JobIdentifier,
                        ImpresoraId = id
                    };
                    lista.Add(colaImpresion);
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Hubo un error al obtener la cola de impresion");
                throw;
            }
            return lista;
        }

        public void AccionJobImpresion(int impresoraId, int jobId, AccionColaImpresion opcion)
        {
            try
            {
                var impresora = repositorio.Obtener<Impresora>(impresoraId);
                var nombreImpresora = impresora.Direccion.Split('\\')[3];
                var nombreServidor = impresora.Direccion.Split('\\')[2];
                var servidorImpresion = new PrintServer(@"\\" + nombreServidor);

                var cola = new PrintQueue(servidorImpresion, nombreImpresora);
                var job = cola.GetJob(jobId);
                switch (opcion)
                {
                    case AccionColaImpresion.Resumir:
                        {
                            job.Resume();
                        }
                        break;

                    case AccionColaImpresion.Pausar:
                        {
                            job.Pause();
                        }
                        break;

                    case AccionColaImpresion.Reiniciar:
                        {
                            job.Restart();
                        }
                        break;

                    case AccionColaImpresion.Cancelar:
                        {
                            job.Cancel();
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Hubo un error al " + opcion + " el job de la cola de impresión");
                throw;
            }
        }

        public decimal? ObtenerPesoNetoConDescuento(Guid instanceId)
        {
            var recorrido = repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == instanceId);
            return CalcularPesoNetoConDescuento(recorrido);
        }

        private decimal? CalcularPesoNetoConDescuento(Recorrido recorrido)
        {
            var calado = recorrido.Calado;
            var pesoNeto = recorrido.PesoBruto - recorrido.PesoTara;
            if (calado != null)
            {
                var caladosPorCaracteristica = calado.CaladosPorCaracteristica;
                var analisis = recorrido.AnalisisDeCalidad;
                var descuentos = caladosPorCaracteristica.ToDictionary<CaladoPorCaracteristica, string, decimal?>(x => x.CaracteristicaDeCalidad.CodigoSAP, x => x.DescuentoEnKg);
                if (analisis != null)
                {
                    foreach (var valorAnalisis in analisis.CaracteristicasAnalizadas)
                    {
                        descuentos[valorAnalisis.CaracteristicaDeCalidad.CodigoSAP] = valorAnalisis.DescuentoEnKg;
                    }
                }
                return pesoNeto - descuentos.Sum(keyValue => keyValue.Value);
            }
            else
            {
                return pesoNeto;
            }
        }

        public decimal? ObtenerPesoNetoSinDescuento(Guid instanceId)
        {
            return repositorio.ObtenerProyeccion<Recorrido, decimal?>(x => x.InstanciaWorkflow == instanceId, x => (x.PesoBruto ?? 0) - (x.PesoTara ?? 0));
        }

        public Dominio.Dto.CamaraDto ObtenerCamaraDeExcepcionDescuento(Guid instanceId, int materialId, int centroId)
        {
            var recorrido = repositorio.ObtenerProyeccion((Recorrido x) => x.InstanciaWorkflow == instanceId && x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.Vehiculo.CartaPorte.Corredor != null
                , x => new
                {
                    CorredorId = x.Vehiculo.CartaPorte.Corredor.Id,
                    CaracteristicasAnalizadas = x.AnalisisDeCalidad.CaracteristicasAnalizadas.Select(y => y.CaracteristicaDeCalidad.Id),
                    HayAnalisis = x.AnalisisDeCalidad != null,
                    HayCalado = x.Calado != null,
                    CaracteristicasCaladas = x.Calado.CaladosPorCaracteristica.Select(y => y.CaracteristicaDeCalidad.Id),
                });
            Camara camara = null;
            if (recorrido != null)
            {
                log.Debug("Recorrido encontrado");
                var fecha = DateTime.Now;
                var corredorId = recorrido.CorredorId;

                var caracteristicasAnalizadas = recorrido.HayAnalisis ? recorrido.CaracteristicasAnalizadas.ToList() : new List<int>();
                var caracteristicasCaladas = recorrido.HayCalado ? recorrido.CaracteristicasCaladas.ToList() : new List<int>();
                var caracteristicasMergeadas = caracteristicasAnalizadas.Concat(caracteristicasCaladas).ToList();

                log.Debug("Corredor {0} caracteristicas {1} material{2} centro{3}", corredorId, caracteristicasMergeadas.ToXml(), materialId, centroId);
                camara = repositorio.Listar<ExcepcionAlDescuento, Camara>(
                x => x.Camara, x => (fecha >= x.FechaDesde && fecha <= x.FechaHasta) &&
                    x.Proveedor.Id == corredorId && caracteristicasMergeadas.Contains(x.CaracteristicaDeCalidad.Id)).FirstOrDefault();
                log.Debug("resultado: " + (camara != null ? camara.Descripcion : "null"));
            }
            return camara == null ? null : conversor.Convertir<Camara, Dominio.Dto.CamaraDto>(camara);
        }

        public List<CaracteristicaDeCalidadDto> ListarCaracteristicasDeCalidadPorMaterialSinExceptuadas(Guid instanceId, int materialId, int centroId)
        {
            var corredorId = repositorio.ObtenerProyeccion<Recorrido, int?>(x => x.InstanciaWorkflow == instanceId && x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.Vehiculo.CartaPorte.Corredor != null, x => x.Vehiculo.CartaPorte.Corredor.Id);
            var caracteristicasExceptuadas = new List<int>();
            var fecha = DateTime.Now;
            if (corredorId.HasValue)
            {
                caracteristicasExceptuadas = repositorio.Listar<ExcepcionAlDescuento, int>(
                x => x.CaracteristicaDeCalidad.Id, x => (fecha >= x.FechaDesde && fecha <= x.FechaHasta) &&
                    x.Proveedor.Id == corredorId.Value && x.CaracteristicaDeCalidad.MaterialPorCentro.Material.Id == materialId &&
                    x.CaracteristicaDeCalidad.MaterialPorCentro.Centro.Id == centroId).ToList();
            }
            return Listar<CaracteristicaDeCalidad, CaracteristicaDeCalidadDto>(
                    x => x.MaterialPorCentro.Material.Id == materialId && x.MaterialPorCentro.Centro.Id == centroId && !caracteristicasExceptuadas.Contains(x.Id) && !x.NoObservableEnCalado).ToList();
        }

        public IList<KmPorProveedorDto> ListarKmPorProveedorYCentro(int clienteId, int centroId)
        {
            return Listar<KmPorProveedor, KmPorProveedorDto>(x => x.Cliente.Id == clienteId && x.Centro.Id == centroId);
        }

        public Resultado ActualizarBajaCTGDefinitivaManual(Guid id)
        {
            var resultado = new Resultado();
            try
            {
                var bajaCtg = repositorio.Obtener<BajaCTG>(x => x.WorkflowId == id);
                if (bajaCtg.CodigoDeBajaDefinitivo == null)
                {
                    bajaCtg.CodigoDeBajaDefinitivo = "Manual";
                }
                if (bajaCtg.CodigoDeBaja == null)
                {
                    bajaCtg.CodigoDeBaja = "Manual";
                }
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                resultado.Error("Hubo un error", e.Message);
            }
            return resultado;
        }

        public bool ExisteOrdenCargaFas(string orden)
        {
            return repositorio.Existe<OrdenCargaFas>(x => x.NumeroOrden == orden);
        }

        public UltimoEstadoDto ObtenerEstadoUltimoRecorrido(Guid id)
        {
            var patente = repositorio.ObtenerProyeccion<Recorrido, string>(x => x.InstanciaWorkflow == id,
                f => f.Patente);
            var recorridoTerminado = repositorio.ObtenerMayor<Recorrido, int>(x => x.Patente == patente && x.Terminado, f => f.Id);
            if (recorridoTerminado != null && recorridoTerminado.Rechazado)
            {
                var controlRecorrido = repositorio.Listar<ControlRecorrido>(x => x.WorkflowInstanceId == recorridoTerminado.InstanciaWorkflow).ToList();

                var mensajes = controlRecorrido.Select(x => x.Mensaje).ToList();
                var selectMotivo = repositorio.ObtenerMayor<Motivo, int>(x => mensajes.Contains(x.Descripcion), f => f.Id);

                UltimoEstadoDto ultimoEstado;
                if (selectMotivo == null)
                {
                    if (controlRecorrido.Any(f => f.Mensaje == Textos.TransportistaInhabilitado))
                    {
                        ultimoEstado = new UltimoEstadoDto
                        {
                            Descripcion = Textos.TransportistaInhabilitado,
                            DescripcionCorta = "",
                            fecha = recorridoTerminado.FechaEgreso.Value
                        };
                        return ultimoEstado;
                    }
                    if (controlRecorrido.Any(f => f.Mensaje == Textos.Rechazado) && controlRecorrido.Any(f => f.Comentario == Textos.RechazoBajaCTGComentario))
                    {
                        ultimoEstado = new UltimoEstadoDto
                        {
                            Descripcion = Textos.RechazoBajaCTG,
                            DescripcionCorta = "",
                            fecha = recorridoTerminado.FechaEgreso.Value
                        };
                        return ultimoEstado;
                    }
                    if (controlRecorrido.Any(f => f.ActividadXaml == Textos.Actividad_AutorizarTiempoEnTransito && !f.Decision))
                    {
                        ultimoEstado = new UltimoEstadoDto
                        {
                            Descripcion = Textos.MotivoTiempoEnTransito,
                            DescripcionCorta = "",
                            fecha = recorridoTerminado.FechaEgreso.Value
                        };
                        return ultimoEstado;
                    }
                    if (controlRecorrido.Any(f => f.Actividad == Textos.VerificacionSalidaFlete && f.Comentario.Contains("Compliance responde:")))
                    {
                        ultimoEstado = new UltimoEstadoDto
                        {
                            Descripcion = Textos.RechazoPorServicioCompliance,
                            DescripcionCorta = "",
                            fecha = recorridoTerminado.FechaEgreso.Value
                        };
                        return ultimoEstado;
                    }
                    if (
                        controlRecorrido.Any(
                            f =>
                                f.Actividad == Textos.Actividad_ExistePedidoDeTraslado &&
                                f.Comentario == Textos.PreguntaExistePedido && f.Decision == false))
                    {
                        ultimoEstado = new UltimoEstadoDto
                        {
                            Descripcion = Textos.ExistePedidoDeTraslado_Titulo,
                            DescripcionCorta = "",
                            fecha = recorridoTerminado.FechaEgreso.Value
                        };
                        return ultimoEstado;
                    }
                }
                else
                {
                    var comentario = controlRecorrido.First(x => x.Mensaje == selectMotivo.Descripcion).Comentario;
                    ultimoEstado = new UltimoEstadoDto
                    {
                        Descripcion = string.Format("{0}, Comentario: {1}", selectMotivo.Descripcion, comentario),
                        DescripcionCorta = selectMotivo.DescripcionCorta,
                        fecha = recorridoTerminado.FechaEgreso.Value
                    };
                    return ultimoEstado;
                }
            }
            return null;
        }

        public GraficoCamionesHoraDto ObtenerGraficoDeCamionesPorHora(GraficoCamionesHoraDto model, int centroId)
        {
            model = repositorio.ObtenerConsultaEscalar(new CantidadCamionesHoraConsulta(model, centroId));
            return model;
        }

        public GraficoCamionesDiaDto ObtenerGraficoDeCamionesPorDia(GraficoCamionesDiaDto model, int centroId)
        {
            model = repositorio.ObtenerConsultaEscalar(new CantidadCamionesDiaConsulta(model, centroId));
            return model;
        }

        public GraficoEficienciaHidraulicasDto ObtenerGraficoDeEficienciaHidraulicas(GraficoEficienciaHidraulicasDto model, int centroId, string codigoHidraulicaVagon)
        {
            model = repositorio.ObtenerConsultaEscalar(new EficienciaHidraulicasConsulta(model, centroId, codigoHidraulicaVagon));
            return model;
        }

        public GraficoToneladasRangoDeDiasDto ObtenerGraficoToneladasPorRangoDeDias(GraficoToneladasRangoDeDiasDto model, int centroId)
        {
            model = repositorio.ObtenerConsultaEscalar(new ToneladasPorRangoDeDiasConsulta(model, centroId));
            return model;
        }

        public ListaPaginada<PesoMaximoPorTipoVehiculoDto> ListarPaginadoPesoMaximoPorTipoVehiculo(Paginacion paginacion, int centroId)
        {
            return Listar<PesoMaximoPorTipoVehiculo, PesoMaximoPorTipoVehiculoDto>(x => x.Centro.Id == centroId, paginacion);
        }

        public PesoMaximoPorTipoVehiculoDto ObtenerPesoMaximoPorTipoVehiculo(int id)
        {
            return Obtener<PesoMaximoPorTipoVehiculo, PesoMaximoPorTipoVehiculoDto>(id);
        }

        public IList<PesoMaximoPorTipoVehiculoDto> ListarPesoMaximoPorTipoVehiculoPorCentro(int centroId)
        {
            return Listar<PesoMaximoPorTipoVehiculo, PesoMaximoPorTipoVehiculoDto>(x => x.Centro.Id == centroId && x.Activo);
        }

        public int? ObtenerPesoNetoMaximo(TipoVehiculo tipoVehiculo, int centroId)
        {
            return repositorio.ObtenerProyeccion<PesoMaximoPorTipoVehiculo, int?>(x => x.TipoVehiculo == tipoVehiculo && x.Centro.Id == centroId, x => x.PesoNetoMaxPlanta);
        }

        public StockDeEstablecimientoDto ObtenerStockDeEstablecimiento(int id)
        {
            var entidad = Obtener<StockDeEstablecimiento, StockDeEstablecimientoDto>(id);
            if (entidad != null)
            {
                entidad.StockUtilizado = StockEPAutilizado(entidad.CodigoEstablecimiento, entidad.Cosecha);
            }
            return entidad;
        }

        public ListaPaginada<StockDeEstablecimientoDto> ListarPaginadoStockDeEstablecimientos(string filtro, Paginacion paginacion)
        {
            Expression<Func<StockDeEstablecimiento, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = (x => (x.CodigoEstablecimiento.Contains(filtro)));
            }

            var stocksDeEstablecimiento = Listar<StockDeEstablecimiento, StockDeEstablecimientoDto>(expresionFiltro, paginacion);

            foreach (var stock in stocksDeEstablecimiento)
            {
                stock.StockUtilizado = StockEPAutilizado(stock.CodigoEstablecimiento, stock.Cosecha);
            }
            return stocksDeEstablecimiento;
        }

        public decimal StockEPAutilizado(string codigoEstablecimiento, string cosecha)
        {
            var pesosNeto = repositorio.Sumar<RegistroStockEPA>(x => x.PesoNeto, x => x.CodigoEstablecimiento == codigoEstablecimiento && x.Cosecha == cosecha);
            pesosNeto += repositorio.Sumar<RegistroStockOtrosPuertos>(x => x.PesoNeto, x => x.CodigoEstablecimiento == codigoEstablecimiento && x.Cosecha == cosecha);
            return pesosNeto;
        }

        public MailAvisoStockDto ObtenerDatosMailAvisoStock(Guid workflowId)
        {
            log.Debug("Obteniendo Datos Envío Mail Por Stock EPA para camión: {0}", workflowId);
            var dto = new MailAvisoStockDto();
            var establecimientoYCosecha = repositorio.ObtenerProyeccion((Recorrido x) => x.InstanciaWorkflow == workflowId, x => new { x.Establecimiento, x.Vehiculo.CartaPorte.Cosecha, materialId = x.Material.Id, centroId = x.Centro.Id, pesoBrutoOrigen = x.PesoBrutoOrigen, pesoTaraOrigen = x.PesoTaraOrigen });
            if (establecimientoYCosecha.Establecimiento == null)
            {
                dto.Error = Textos.Establecimiento_NoAsignado;
                log.Debug("No se asignó establecimiento al camion: {0}", workflowId);
                return dto;
            }
            dto.CodigoDeEstablecimiento = establecimientoYCosecha.Establecimiento.CodigoDeEstablecimiento;
            dto.NombreDeEstablecimiento = establecimientoYCosecha.Establecimiento.NombreDeEstablecimiento;
            dto.Cosecha = establecimientoYCosecha.Cosecha;

            var epaStockPorCorte =
                repositorio.ObtenerProyeccion<MaterialPorCentro, int?>(
                    x =>
                        x.Centro.Id == establecimientoYCosecha.centroId &&
                        x.Material.Id == establecimientoYCosecha.materialId, x => x.EpaStockPorCorte);
            if (epaStockPorCorte == null)
            {
                dto.Error = Textos.EpaStockNoConfigurado;
                log.Debug("El campo EPAStockPorCorte para el material id {0} y el centro id {1} no está configurado", establecimientoYCosecha.materialId, establecimientoYCosecha.centroId);
                return dto;
            }

            var stockDeclarado = repositorio.Obtener<StockDeEstablecimiento>(x => x.CodigoEstablecimiento == dto.CodigoDeEstablecimiento && x.Cosecha == dto.Cosecha);
            if (stockDeclarado == null)
            {
                dto.Error = Textos.StockDeEstablecimientoNoConfigurado;
                log.Debug("Stock de establecimiento {0} de código {1} no configurado", establecimientoYCosecha.Establecimiento.NombreDeEstablecimiento, establecimientoYCosecha.Establecimiento.CodigoDeEstablecimiento);
                return dto;
            }
            dto.StockDeclarado = stockDeclarado.StockDeclarado;
            var stockUtilizado = StockEPAutilizado(dto.CodigoDeEstablecimiento, dto.Cosecha);
            var pesoNetoOrigen = establecimientoYCosecha.pesoBrutoOrigen - establecimientoYCosecha.pesoTaraOrigen;
            dto.StockUtilizado = stockUtilizado + pesoNetoOrigen.Value;
            if (dto.StockDisponible <= epaStockPorCorte)
            {
                log.Debug("Se envia mail porque ({0} - {1} <= {2})", dto.StockDisponible, pesoNetoOrigen, epaStockPorCorte);
                dto.SeEnviaMail = true;

                dto.Localidad = establecimientoYCosecha.Establecimiento.Localidad.Descripcion;
                dto.Provincia = establecimientoYCosecha.Establecimiento.Provincia.Descripcion;
                dto.RazonesSociales = repositorio.Listar<Establecimiento, string>(x => x.Proveedor.RazonSocial, x => x.CodigoDeEstablecimiento == dto.CodigoDeEstablecimiento).ToList();
                dto.Intermediarios = repositorio.ListarDistintos<RegistroStockEPA, string>(x => x.Recorrido.Vehiculo.CartaPorte.Intermediario.RazonSocial, x => x.CodigoEstablecimiento == dto.CodigoDeEstablecimiento && x.Cosecha == dto.Cosecha, 50);
                dto.Remitentes = repositorio.ListarDistintos<RegistroStockEPA, string>(x => x.Recorrido.Vehiculo.CartaPorte.RtteComercial.RazonSocial, x => x.CodigoEstablecimiento == dto.CodigoDeEstablecimiento && x.Cosecha == dto.Cosecha, 50);
                dto.Corredores = repositorio.ListarDistintos<RegistroStockEPA, string>(x => x.Recorrido.Vehiculo.CartaPorte.Corredor.RazonSocial, x => x.CodigoEstablecimiento == dto.CodigoDeEstablecimiento && x.Cosecha == dto.Cosecha, 50);
            }
            if (!dto.SeEnviaMail)
            {
                log.Debug("No se envia mail porque ({0} - {1} > {2})", dto.StockDisponible, pesoNetoOrigen, epaStockPorCorte);
            }
            return dto;
        }

        public Resultado ValidarStockEstablecimiento(int establecimientoId, Guid instanceId)
        {
            var resultado = new Resultado();
            try
            {
                var hoy = DateTime.Now.Date;
                if (!ValidaStockEPA(instanceId))
                {
                    return resultado;
                }
                var codigoEstablecimiento = repositorio.ObtenerProyeccion<Establecimiento, string>(x => x.Id == establecimientoId, x => x.CodigoDeEstablecimiento);
                var cartaPorte = ObtenerCartaPortePorInstanceId(instanceId);
                var netoOrigen = repositorio.ObtenerProyeccion<Recorrido, int?>(x => x.InstanciaWorkflow == instanceId, x => x.PesoBrutoOrigen - x.PesoTaraOrigen);
                var stockEstablecimiento = repositorio.Obtener<StockDeEstablecimiento>(x => x.CodigoEstablecimiento == codigoEstablecimiento && x.Cosecha == cartaPorte.Cosecha);
                if (stockEstablecimiento == null)
                {
                    resultado.Error("", Textos.AsignacionEstablecimientoError_SinStockDeEstablecimiento);
                }
                else if (hoy < stockEstablecimiento.FechaDesde.Date || hoy > stockEstablecimiento.FechaHasta.Date)
                {
                    resultado.Error("", Textos.AsignacionEstablecimientoError_FueraVigencia);
                }
                else if (netoOrigen.HasValue && StockEPAutilizado(stockEstablecimiento.CodigoEstablecimiento, stockEstablecimiento.Cosecha) + netoOrigen.Value > stockEstablecimiento.StockDeclarado - stockEstablecimiento.StockReservado + (stockEstablecimiento.Tolerancia ?? 0))
                {
                    resultado.Error("", Textos.AsignacionEstablecimientoError_LimiteStockSuperado);
                }
                else if (!netoOrigen.HasValue)
                {
                    resultado.Error("", Textos.AsignacionEstablecimientoError_SinNetoOrigen);
                }
                else
                {
                    stockEstablecimiento.StockReservado = stockEstablecimiento.StockReservado + netoOrigen.Value;
                    repositorio.GuardarCambios();
                }
                return resultado;
            }
            catch (Exception e)
            {
                log.Error(e, $"Error ValidarStockEstablecimiento {instanceId} , codigo establecimiento {establecimientoId}");
                resultado.Error("", Textos.Error_ActualizarGenerico);
                return resultado;
            }
        }

        public bool DescuentaPesoDescontado(string cosecha)
        {
            return repositorio.ObtenerProyeccion<Cosecha, bool>(x => x.Descripcion == cosecha, x => x.EpaPesoDescontado);
        }

        public bool EsRecorridoSustentable(Guid instanceId)
        {
            return repositorio.ObtenerProyeccion<Recorrido, bool>(x => x.InstanciaWorkflow == instanceId, f => f.Establecimiento != null && !f.Establecimiento.EPA);
        }

        public bool ValidaStockEPA(Guid instanceId)
        {
            return repositorio.ObtenerProyeccion<Recorrido, bool>(x => x.InstanciaWorkflow == instanceId, f => f.TipoComercial.ValidaStockEPA);
        }

        public ListaPaginada<CosechaDto> ListarPaginadoCosechas(string filtro, Paginacion paginacion)
        {
            Expression<Func<Cosecha, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = x => x.Descripcion.Contains(filtro);
            }
            return Listar<Cosecha, CosechaDto>(expresionFiltro, paginacion);
        }

        public CosechaDto ObtenerCosecha(int id)
        {
            return Obtener<Cosecha, CosechaDto>(id);
        }

        public CentroDto ObtenerCentroPorCodigoSap(string codigo)
        {
            return Obtener<Centro, CentroDto>(x => x.CodigoSAP == codigo);
        }

        public MaterialIdYDescripcionDto ObtenerMaterialIdYDescripcionPorCodigoSap(string codigo)
        {
            string codigoTrim = codigo.TrimStart('0');
            var datos = repositorio.ObtenerProyeccion<Material, MaterialIdYDescripcionDto>(x => x.CodigoSAP == codigoTrim,
                x => new MaterialIdYDescripcionDto()
                {
                    MaterialId = x.Id,
                    Descripcion = x.Descripcion
                });
            return datos;
        }

        public PesoVagonesDto ObtenerPesoNetoTren(int cartaporteId)
        {
            var vagones = repositorio.Listar<Recorrido>(
                x => x.Vehiculo.CartaPorte.Id == cartaporteId);

            if (vagones.All(x => x.PesoTara.HasValue && x.PesoBruto.HasValue))
            {
                var aaa = new PesoVagonesDto
                {
                    PesoNeto = vagones.Sum(x => x.PesoBruto.Value - x.PesoTara.Value),
                    PesoBruto = vagones.Sum(x => x.PesoBruto.Value),
                    PesoTara = vagones.Sum(x => x.PesoTara.Value),
                    PesoDescuento = vagones.Sum(x => CalcularPesoNetoConDescuento(x) ?? 0),
                    Vagones = vagones.Select(x => new RecorridoDto
                    {
                        PesoBruto = x.PesoBruto,
                        PesoTara = x.PesoTara,
                        Patente = x.Patente,
                    }).ToList()
                };
                return aaa;
            }
            return null;
        }

        public decimal TotalKilosDescuentos(CaladoDto calado, AnalisisDeCalidadDto analisis, int pesoNeto)
        {
            return calculadora.TotalKilosDescuento(calado, analisis, pesoNeto);
        }

        private FotosDto ListarFotosGenerico(string actividad, Expression<Func<Recorrido, bool>> filtro)
        {
            var datosCamion = repositorio.Listar(x => new { x.Centro.CodigoSAP, x.NumeroDocumentoIngreso, x.Patente, Material = x.Material.Descripcion, x.FechaInicio, x.TipoVehiculo, x.Id, CentroId = x.Centro.Id }, filtro).LastOrDefault();
            if (datosCamion == null) return new FotosDto();
            var fileName = FotoCamionHelper.GenerarNombreBusqueda(datosCamion.CodigoSAP, datosCamion.NumeroDocumentoIngreso, datosCamion.Patente, actividad, datosCamion.TipoVehiculo);
            var centro = repositorio.Obtener<Centro>(datosCamion.CentroId);
            var retorno = new FotosDto { Fotos = new List<FotoDto>(), Material = datosCamion.Material, NumeroDocumentoIngreso = datosCamion.NumeroDocumentoIngreso, Patente = datosCamion.Patente, FechaInicio = datosCamion.FechaInicio, RecorridoId = datosCamion.Id };

            return ListarFotos(fileName, retorno, fotosPath: centro?.FotosPath);
        }

        public FotosDto ListarFotosCamion(Guid instanciaWorkflow, string actividad)
        {
            return ListarFotosGenerico(actividad, (Recorrido x) => x.InstanciaWorkflow == instanciaWorkflow);
        }

        public FotosDto ListarFotosCamionPorCargaDeCupo(int id)
        {
            var cargaDeCupo = repositorio.Obtener<CargaDeCupo>(id);
            var retorno = new FotosDto();
            ObtenerFoto(retorno, cargaDeCupo.FotoRutaDestino, cargaDeCupo.Fecha, "Garita");
            ObtenerFoto(retorno, cargaDeCupo.FotoCamionRutaDestino, cargaDeCupo.Fecha, "Garita");
            return retorno;
        }

        public FotosDto ObtenerFotoPorPath(string path)
        {
            var retorno = new FotosDto();
            ObtenerFoto(retorno, path, DateTime.Now, string.Empty);
            return retorno;
        }

        public FotosDto ListarFotosCamionPorTarjeta(string tarjeta, string actividad)
        {
            return ListarFotosGenerico(actividad, (Recorrido x) => x.TarjetaDeAcceso == tarjeta && !x.Terminado);
        }

        public FotosDto ListarFotosCamionPorRecorrido(int id, string actividad)
        {
            return ListarFotosGenerico(actividad, (Recorrido x) => x.Id == id);
        }

        public void RecuperarFotosTemporalesPorTarjeta(Guid instanceId)
        {
            log.Debug($"RecuperarFotosTemporalesPorTarjeta {instanceId}");
            try
            {
                var datos = repositorio.ObtenerProyeccion((Recorrido x) => x.InstanciaWorkflow == instanceId, x => new { x.TarjetaDeAcceso, x.Patente, x.NumeroDocumentoIngreso, x.Centro.CodigoSAP, x.Id });
                var fotosTemp = BuscarFotos(datos.TarjetaDeAcceso, new string[] { "temp" });
                log.Debug($"RecuperarFotosTemporalesPorTarjeta {instanceId} Encontradas: {fotosTemp.Count}");

                if (fotosTemp.Any())
                {
                    var carpetaDelDia = DateTime.Now.ToString("yyyyMMdd");
                    foreach (FileData foundFile in fotosTemp)
                    {
                        string rutaDestino = string.Empty;
                        try
                        {
                            rutaDestino = foundFile.Path.Replace("temp", carpetaDelDia);
                            rutaDestino = rutaDestino.Replace(datos.TarjetaDeAcceso, FotoCamionHelper.GenerarNombre(datos.CodigoSAP, datos.NumeroDocumentoIngreso, datos.Patente));

                            if (!Directory.Exists(Path.GetDirectoryName(rutaDestino)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(rutaDestino));
                            }
                            if (!File.Exists(rutaDestino))
                            {
                                File.Move(foundFile.Path, rutaDestino);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex, $"RecuperarFotosTemporalesPorTarjeta {instanceId} : {foundFile.Path} => {rutaDestino}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, $"RecuperarFotosTemporalesPorTarjeta {instanceId}");
            }
        }

        public FotosDto ListarFotosQuiebre(int id)
        {
            var quiebre = repositorio.Obtener<MotivoQuiebreBarrera>(id);
            if (quiebre != null && !string.IsNullOrEmpty(quiebre.FileName))
            {
                return ListarFotos(quiebre.FileName, new FotosDto() { FechaInicio = quiebre.Fecha });
            }
            return new FotosDto();
        }

        public string GuardarArchivoFotoDocumentoExterno(string fotoMesaDigitalizacion, string extension, string numeroDocumento)
        {
            try
            {
                var path = configuracion.AppSettings["FotosPath"];
                var directorio = "documentosExternos";
                var subpath = DateTime.Now.ToString("yyyyMMdd");
                var foto = Convert.FromBase64String(fotoMesaDigitalizacion);
                var rutaDestino = path + (path.EndsWith("\\") ? "" : "\\") + directorio + "\\" + subpath + "\\" + FotoCamionHelper.GenerarNombreTemporal(numeroDocumento, DateTime.Now) + "." + extension.Split('.').Last();

                if (!Directory.Exists(Path.GetDirectoryName(rutaDestino)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(rutaDestino));
                }
                using (var outputStream = File.OpenWrite(rutaDestino))
                {
                    outputStream.Write(foto, 0, foto.Length);
                }
                return rutaDestino;
            }
            catch (Exception ex)
            {
                log.Error(ex, "");
            }
            return null;
        }

        public void EliminarArchivoFotoDocumentoExterno(string ruta)
        {
            try
            {
                if (Directory.Exists(Path.GetDirectoryName(ruta)))
                {
                    File.Delete(Path.GetDirectoryName(ruta));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "");
            }
        }

        private byte[] ReadAllBytes(string fileName)
        {
            byte[] buffer = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
            }
            return buffer;
        }

        public byte[] ObtenerUltimaFotoPorTarjeta(string tarjeta, int puestoDeTrabajoId)
        {
            try
            {
                var directorios = repositorio.Listar<VideoCamara, string>(x => x.Directorio, x => x.PuestoDeTrabajo.Id == puestoDeTrabajoId).ToList();

                var datosCamion = repositorio.Listar(x => new { x.Centro.CodigoSAP, x.NumeroDocumentoIngreso, x.Patente, Material = x.Material.Descripcion, x.FechaInicio }, (Recorrido x) => x.TarjetaDeAcceso == tarjeta && !x.Terminado).LastOrDefault();
                if (datosCamion == null) return new byte[0];
                var fileName = datosCamion.CodigoSAP + "-" + datosCamion.NumeroDocumentoIngreso + "-" + datosCamion.Patente + "-";

                var foundFile = BuscarFotos(fileName, datosCamion.FechaInicio, false, directorios).OrderByDescending(x => x.CreationTime).FirstOrDefault();

                if (foundFile != null)
                {
                    return this.ReadAllBytes(foundFile.Path);
                }
                return new byte[0];
            }
            catch (Exception e)
            {
                log.Error(e, "Error al ObtenerUltimaFotoPorTarjeta " + tarjeta);
                return new byte[0];
            }
        }

        public byte[] ObtenerFotoPorQuiebreDeBarrera(string fileName, DateTime fecha)
        {
            try
            {
                var foundFile = BuscarFotos(fileName, fecha, true).FirstOrDefault();

                if (foundFile != null)
                {
                    return this.ReadAllBytes(foundFile.Path);
                }
                return new byte[0];
            }
            catch (Exception e)
            {
                log.Error(e, "Error al foto de quiebre " + fileName);
                return new byte[0];
            }
        }

        public int ObtenerCantidadCamionesRechazados(int centroId)
        {
            return repositorio.Contar<Recorrido>(x => !x.Terminado && x.Rechazado && centroId == x.Centro.Id);
        }

        private FotosDto ListarFotos(string fileName, FotosDto retorno, bool obtenerPrimera = false, string fotosPath = null)
        {
            try
            {
                foreach (FileData foundFile in BuscarFotos(fileName, retorno.FechaInicio, obtenerPrimera, fotosPath: fotosPath).OrderByDescending(x => x.CreationTime))
                {
                    var nombre = foundFile.Name.Split('.')[0].Split('-');
                    var actividad = nombre.Count() >= 4 ? nombre[3] : string.Empty;
                    ObtenerFoto(retorno, foundFile.Path, foundFile.CreationTime, actividad);
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Error al ListarFotos " + fileName);
            }

            return retorno;
        }

        private static void ObtenerFoto(FotosDto retorno, string path, DateTime fecha, string actividad)
        {
            if (string.IsNullOrEmpty(path)) return;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = null;
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);

                using (Image image = Image.FromStream(fs))
                {
                    var documentcontentsSmall = image.ResizeImage(50, 28);

                    retorno.Fotos.Add(new FotoDto
                    {
                        Actividad = actividad,
                        Foto = buffer,
                        FotoChica = documentcontentsSmall,
                        Fecha = fecha.ToString("dd/MM/yyyy HH:mm"),
                        Extension = Path.GetExtension(path)
                    });
                }
            }
        }

        private static void ObtenerArchivo(FotosDto retorno, string path, DateTime fecha, string actividad)
        {
            if (string.IsNullOrEmpty(path)) return;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = null;
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);

                retorno.Fotos.Add(new FotoDto
                {
                    Actividad = actividad,
                    Foto = buffer,
                    FotoChica = null,
                    Fecha = fecha.ToString("dd/MM/yyyy HH:mm"),
                    Extension = Path.GetExtension(path)
                });
            }
        }

        private List<FileData> BuscarFotos(string fileName, DateTime fechaInicio, bool obtenerPrimera = false, List<string> directorios = null, string fotosPath = null)
        {
            return BuscarFotos(fileName, new string[] { fechaInicio.ToString("yyyyMMdd"), fechaInicio.AddDays(1).ToString("yyyyMMdd"), fechaInicio.AddDays(-1).ToString("yyyyMMdd") }, obtenerPrimera, directorios, fotosPath);
        }

        private List<FileData> BuscarFotos(string fileName, string[] subpaths, bool obtenerPrimera = false, List<string> directorios = null, string fotosPath = null)
        {
            log.Debug("Inicio BuscarFotos" + fileName);
            var path = !string.IsNullOrEmpty(fotosPath) ? fotosPath : configuracion.AppSettings["FotosPath"];
            var resultado = new List<FileData>();
            try
            {
                foreach (var directoriosBusqueda in Directory.GetDirectories(path))
                {
                    if (directorios != null && directorios.Any() && !directorios.Any(t => new Uri(t).LocalPath == new Uri(directoriosBusqueda).LocalPath))
                    {
                        continue;
                    }

                    //Modificacion Multiples Paths - Mejora en Acopios (Pergamino)
                    if (!string.IsNullOrEmpty(fotosPath))
                    {
                        foreach (var carpeta in Directory.GetDirectories(directoriosBusqueda))
                        {
                            foreach (var subpath in subpaths)
                            {
                                var pathBusqueda = carpeta + "\\" + subpath + "\\";
                                //log.Debug("Ruta-File 1:" + pathBusqueda);
                                if (Directory.Exists(pathBusqueda))
                                {
                                    var filesInDir = FastDirectoryEnumerator.GetFiles(pathBusqueda, fileName + "*.*", SearchOption.AllDirectories);
                                    if (filesInDir.Any() && obtenerPrimera)
                                    {
                                        return new List<FileData> { filesInDir.First() };
                                    }
                                    resultado.AddRange(filesInDir);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var subpath in subpaths)
                        {
                            var pathBusqueda = directoriosBusqueda + "\\" + subpath + "\\";
                            //log.Debug("Ruta-File 2:" + pathBusqueda);
                            if (Directory.Exists(pathBusqueda))
                            {
                                var filesInDir = FastDirectoryEnumerator.GetFiles(pathBusqueda, fileName + "*.*", SearchOption.AllDirectories);
                                if (filesInDir.Any() && obtenerPrimera)
                                {
                                    return new List<FileData> { filesInDir.First() };
                                }
                                resultado.AddRange(filesInDir);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e, $"Error al buscar fotos {fileName} {string.Join("", subpaths)} {obtenerPrimera}");
            }
            log.Debug("Fin BuscarFotos" + fileName);
            return resultado;
        }

        public List<EstadoMaterialDto> ListarEstadoPlanta(int centroId, bool mostrarIngresos, bool esGrano)
        {
            return Listar<EstadoMaterial, EstadoMaterialDto>(
                x => x.CentroId == centroId &&
                x.EsIngreso == mostrarIngresos &&
                x.EsGrano == esGrano).ToList();
        }

        public List<CupoMobileDto> ListarEstadoCupos(int centroId)
        {
            return Listar<CupoMobile, CupoMobileDto>(x => x.CentroId == centroId).ToList();
        }

        public ListaPaginada<ExcepcionEnvioCamaraDto> ListarPaginadoExcepcionEnvioCamara(string filtro, Paginacion paginacion)
        {
            return repositorio.ListarConsultaPaginada(new ListarPaginadoExcepcionEnvioCamara(filtro, paginacion));
        }

        public bool MaterialEnviaASapAlmacenPredeterminado(Guid guid)
        {
            var dato = repositorio.ObtenerProyeccion((Recorrido x) => x.InstanciaWorkflow == guid, x => new { Workflow = x.Workflow.Id, Material = x.Material.Id });
            return repositorio.Existe<MaterialPorWorkflow>(x => x.Material.Id == dato.Material && x.Workflow.Id == dato.Workflow && x.EnviaASapAlmacenPredeterminado);
        }

        public ValidarCupoDto ValidarCupo(string cupo, int centroId, string numeroCartaPorte)
        {
            if (cupo == configuracion.AppSettings["CupoDefault"])
            {
                return new ValidarCupoDto
                {
                    Valido = true
                };
            }

            var cargaDeCupo = repositorio.ObtenerMayor<CargaDeCupo, int>(x => x.Cupo == cupo &&
                                                                x.SinCupo == false &&
                                                                x.Centro.Id == centroId, x => x.Id);
            return new ValidarCupoDto
            {
                MensajeError = cargaDeCupo == null ? Textos.Error_ValidarCupoNoExiste :
                               cargaDeCupo.Recorrido != null && !cargaDeCupo.Recorrido.Rechazado ? Textos.Error_ValidarCupoYaAsignado : null,
                Valido = cargaDeCupo != null,
                Reingresado = cargaDeCupo != null && cargaDeCupo.Recorrido != null && cargaDeCupo.Recorrido.Rechazado ? conversor.Convertir<CargaDeCupo, CargaDeCupoDto>(cargaDeCupo) : null,
                YaAsignado = cargaDeCupo != null && cargaDeCupo.Recorrido != null && !cargaDeCupo.Recorrido.Rechazado
            };
        }

        public string[] ObtenerCodigoDeCentroPorId(int centroId)
        {
            var centro = repositorio.Obtener<Centro>(centroId);
            return (centro.CodigoSAP + ',' + centro.CodigoSAPEspecial).Split(',').Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }

        public IList<MotivoHumedadManualDto> ListarMotivosHumedad()
        {
            var precintos = repositorio.Listar<MotivoHumedadManual>();
            return conversor.ConvertirList<MotivoHumedadManual, MotivoHumedadManualDto>(precintos);
        }

        public ListaPaginada<RangosDeRedondeoDto> ListarPaginadoRangosDeRedondeo(string filtro, Paginacion paginacion)
        {
            Expression<Func<RangosDeRedondeo, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = x => x.MaterialPorCentro.Material.Descripcion.Contains(filtro);
            }
            return Listar<RangosDeRedondeo, RangosDeRedondeoDto>(expresionFiltro, paginacion);
        }

        public IList<RangosDeRedondeoDto> ListarRangosDeRedondeoPorMaterial(int filtro)
        {
            return Listar<RangosDeRedondeo, RangosDeRedondeoDto>(x => x.MaterialPorCentro.Material.Id == filtro);
        }

        public RangosDeRedondeoDto ObtenerRangosDeRedondeo(int id)
        {
            return Obtener<RangosDeRedondeo, RangosDeRedondeoDto>(id);
        }

        public BalanzaDto ObtenerBalanzaPorPuestoDeTrabajo(int puestoDeTrabajoId, TipoVehiculo tipoVehiculo)
        {
            var tipo = tipoVehiculo == TipoVehiculo.Tren ? tipoVehiculo : TipoVehiculo.Camión;
            var lector = repositorio.ObtenerProyeccion<PuestoDeTrabajo, string>(x => x.Id == puestoDeTrabajoId, x => x.Lector);
            var pcs = repositorio.Listar<PuestoDeTrabajo, string>(x => x.NombrePc, x => x.Lector == lector);
            var balanza = repositorio.ObtenerPrimero<Balanza>(x => pcs.Contains(x.PuestoDeTrabajo) && x.TipoVehiculo == tipo);
            return conversor.Convertir<Balanza, BalanzaDto>(balanza);
        }

        public BalanzaDto ObtenerBalanzaAsociadaAPuestoAutomatico(int puestoDeTrabajoId)
        {
            var balanza = repositorio.ObtenerProyeccion<PuestoDeTrabajo, Balanza>(x => x.Id == puestoDeTrabajoId, x => x.Balanza);

            return conversor.Convertir<Balanza, BalanzaDto>(balanza);
        }

        public int ObtenerIdBalanzaAutomaticaPorPuestoDeTrabajo(int puestoDeTrabajoId)
        {
            var balanza = repositorio.ObtenerProyeccion<PuestoDeTrabajo, int>(x => x.Id == puestoDeTrabajoId, x => x.Balanza.Id);
            return balanza;
        }

        public RecorridoDto ObtenerRecorridoPesadaExportacion(Guid id)
        {
            return repositorio.ObtenerProyeccion<Recorrido, RecorridoDto>(x => x.InstanciaWorkflow == id, x =>
                new RecorridoDto
                {
                    InstanciaWorkflow = x.InstanciaWorkflow,
                    PesoTara = x.PesoTara,
                    PesoBruto = x.PesoBruto,
                    Id = x.Id,
                    WorkflowDefinicionId = x.WorkflowDefinicion.Id,
                });
        }

        public List<string> ObtenerUsuariosAutorizarTiempoEnTransito()
        {
            return repositorio.Listar<Usuario, string>(x => x.Email,
                                                       x => x.AvisoAutorizarTiempoEnTransito == true).ToList();
        }

        public List<string> ObtenerUsuariosQuiebreApertura()
        {
            return repositorio.Listar<Usuario, string>(x => x.Email,
                                                       x => x.AvisoQuiebreApertura == true).ToList();
        }

        public List<string> ObtenerUsuariosQuiebreCierre()
        {
            return repositorio.Listar<Usuario, string>(x => x.Email,
                                                       x => x.AvisoQuiebreCierre == true).ToList();
        }

        public List<string> ObtenerUsuarioEntregaHexano()
        {
            return repositorio.Listar<Usuario, string>(x => x.Email,
                                                       x => x.AvisoEntregaHexano == true).ToList();
        }

        public List<string> ObtenerUsuariosReasignacionDeTarjeta()
        {
            return repositorio.Listar<Usuario, string>(x => x.Email,
                                           x => x.ReasignacionDeTarjeta == true).ToList();
        }

        public List<string> ObtenerUsuariosContingencia()
        {
            return repositorio.Listar<Usuario, string>(x => x.Email,
                                           x => x.AvisoContingencia == true).ToList();
        }

        public List<string> ObtenerUsuariosLineUp()
        {
            return repositorio.Listar<Usuario, string>(x => x.Email,
                                                       x => x.AvisoLineUp == true).ToList();
        }

        public List<string> ObtenerUsuariosPlanoDeCarga()
        {
            return repositorio.Listar<Usuario, string>(x => x.Email,
                                                       x => x.AvisoPlanoDeCarga == true).ToList();
        }

        public bool ObtenerCodigoEstablecimientoEsDeMolinos(string codigoDeEstablecimiento)
        {
            return repositorio.Existe<Centro>(x => x.CodigoEstablecimiento == codigoDeEstablecimiento);
        }

        public bool ValoresNoCorrespondenAEspecial(Guid workflowInstanceId, int materialId, int centroId)
        {
            var caracteristicasDelCamion = ListarAnalisisYCaladoPorCaracteristica(workflowInstanceId);
            var caracteristicasAComparar = ListarCaracteristicasDeCalidadPorMaterial(materialId, centroId).Where(x => x.ValorEspecialMaximo != null && x.ValorEspecialMinimo != null);

            return caracteristicasDelCamion.Any(x => caracteristicasAComparar.Any(y => x.CaracteristicaId == y.Id && (y.ValorEspecialMinimo > x.Valor || y.ValorEspecialMaximo < x.Valor)));
        }

        public CargaDeCupoDto ObtenerCupoPorRecorrido(int recorridoId)
        {
            return Obtener<CargaDeCupo, CargaDeCupoDto>(x => x.Recorrido.Id == recorridoId);
        }

        public CargaDeCupoDto ObtenerCupoPorId(int id)
        {
            return Obtener<CargaDeCupo, CargaDeCupoDto>(x => x.Id == id);
        }

        public BalanzaPuertoDto ObtenerBalanzaPuerto(int id)
        {
            return Obtener<BalanzaPuerto, BalanzaPuertoDto>(id);
        }

        public IList<BalanzaPuertoDto> ListarBalanzasPuerto()
        {
            return Listar<BalanzaPuerto, BalanzaPuertoDto>();
        }

        public IList<string> ListarBalanzasPuertoReales()
        {
            return repositorio.Listar<BalanzaPuerto, string>(x => x.CodigoBalanza, x => !x.Administrativa);
        }

        public ListaPaginada<BalanzaPuertoDto> ListarBalanzasPuertoPaginado(string filtro, Paginacion paginacion)
        {
            Expression<Func<BalanzaPuerto, bool>> expresionFiltro = null;
            if (filtro != null)
            {
                expresionFiltro = x => x.CodigoBalanza.Contains(filtro) || x.CodigoDispositivo.Contains(filtro);
            }
            var lista = Listar<BalanzaPuerto, BalanzaPuertoDto>(expresionFiltro, paginacion);
            foreach (var balanza in lista)
            {
                var ultimo = repositorio.ObtenerMayor<RegistroBalanzaPuerto, int>(x => x.NumeroBalanza == balanza.CodigoBalanza, x => x.Id);
                balanza.UltimoIdInsertado = ultimo != null ? ultimo.Id : 0;
            }
            return lista;
        }

        public CargaDto ObtenerCarga(int id, string numeroBalanza)
        {
            return Obtener<Carga, CargaDto>(x => x.Id == id && x.NumeroBalanza == numeroBalanza);
        }

        public ListaPaginada<CargaDto> ListarPaginadoCargas(CargaFiltroDto filtro, Paginacion paginacion)
        {
            return repositorio.ListarConsultaPaginada(new ListarPaginadoCargas(filtro, paginacion));
        }

        public IList<CargaDto> ListarCargasSinPaginado(CargaFiltroDto filtro)
        {
            Expression<Func<Carga, bool>> expresionFiltro = null;
            if (filtro != null)
            {
                expresionFiltro = x => (!filtro.Id.HasValue || filtro.Id == x.Id) &&
                                       (string.IsNullOrEmpty(filtro.NumeroBalanza) || filtro.NumeroBalanza == x.NumeroBalanza) &&
                                       (!filtro.IdMaterial.HasValue || filtro.IdMaterial == x.Material.Id) &&
                                       (!filtro.IdVapor.HasValue || filtro.IdVapor == x.Vapor.Id) &&
                                       (!filtro.IdExportador.HasValue || filtro.IdExportador == x.Exportador.Id) &&
                                       (!filtro.IdDestino.HasValue || filtro.IdDestino == x.Destino.Id) &&
                                       (!filtro.IdBodega.HasValue || filtro.IdBodega == x.Bodega.Id);
            }
            return Listar<Carga, CargaDto>(expresionFiltro);
        }

        public ListaPaginada<CargaDto> ListarEmbarquePorBuques(CargaFiltroDto filtro, Paginacion paginacion)
        {
            return repositorio.ListarConsultaPaginada(new ListarPaginadoEmbarques(filtro, paginacion));
        }

        public ListaPaginada<BalanzadaDto> ListarPaginadoBalanzadas(int id, int? idFin, string numeroBalanza, bool? enviado, Paginacion paginacion)
        {
            return Listar<Balanzada, BalanzadaDto>(x => x.CargaInicial.Id == id && x.CargaInicial.NumeroBalanza == numeroBalanza && (enviado == null || x.EnviadoASap == enviado), paginacion);
        }

        public ListaPaginada<VaporDto> ListarVapores(Paginacion paginacion, string filtro)
        {
            Expression<Func<Vapor, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    x.Nombre.Contains(filtro);
            }
            return Listar<Vapor, VaporDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<BodegaDto> ListarBodegas(Paginacion paginacion, string filtro)
        {
            Expression<Func<Bodega, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    x.Nombre.Contains(filtro);
            }
            return Listar<Bodega, BodegaDto>(expresionFiltro, paginacion);
        }

        public IList<MaterialPuertoDto> ListaMaterialesPuerto()
        {
            return Listar<MaterialPuerto, MaterialPuertoDto>();
        }

        public IList<MaterialPuertoDto> ListaMaterialesPuertoConDescripcionCorta()
        {
            return Listar<MaterialPuerto, MaterialPuertoDto>(x => x.DescripcionCorta != null);
        }

        public IList<AgenciaMaritimaPuertoDto> ListarAgenciasMaritimas()
        {
            return Listar<AgenciaMaritimaPuerto, AgenciaMaritimaPuertoDto>();
        }

        public IList<CoordinadorPuertoDto> ListarCoordinadores()
        {
            return Listar<CoordinadorPuerto, CoordinadorPuertoDto>();
        }

        public ListaPaginada<MaterialPuertoDto> ListarMaterialesPuerto(Paginacion paginacion, string filtro)
        {
            Expression<Func<MaterialPuerto, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    x.Descripcion.Contains(filtro);
            }

            return Listar<MaterialPuerto, MaterialPuertoDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<DestinoDto> ListarDestinos(Paginacion paginacion, string filtro)
        {
            Expression<Func<Destino, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    x.Nombre.Contains(filtro);
            }
            return Listar<Destino, DestinoDto>(expresionFiltro, paginacion);
        }

        public IList<DestinoDto> ListarTodosDestinos()
        {
            return Listar<Destino, DestinoDto>().ToList();
        }

        public IList<AgenciaControlPrivadoDto> ListarAgenciasControlPrivado()
        {
            return Listar<AgenciaControlPrivado, AgenciaControlPrivadoDto>().ToList();
        }

        public IList<AgenteControlPrivadoDto> ListarAgentesControlPrivado()
        {
            return Listar<AgenteControlPrivado, AgenteControlPrivadoDto>().ToList();
        }

        public IList<EstibaDto> ListarEstibas()
        {
            return Listar<Estiba, EstibaDto>().ToList();
        }

        public PlanoDeCargaDto ObtenerPlanoDeCarga(int planoDeCargaId)
        {
            var planoDeCargaDto = Obtener<PlanoDeCarga, PlanoDeCargaDto>(x => x.Id == planoDeCargaId);
            if (File.Exists(planoDeCargaDto.FilePathPlano))
            {
                MemoryStream ms = new MemoryStream();
                using (FileStream file = new FileStream(planoDeCargaDto.FilePathPlano, FileMode.Open, FileAccess.Read))
                {
                    planoDeCargaDto.PlanoDeCargaArchivoPlanoNombre = Path.GetFileName(file.Name);
                    file.CopyTo(ms);
                    planoDeCargaDto.FilePathPlano = Convert.ToBase64String(ms.ToArray());
                }
            }

            if (File.Exists(planoDeCargaDto.FilePathSecuencia))
            {
                MemoryStream ms = new MemoryStream();
                using (FileStream file = new FileStream(planoDeCargaDto.FilePathSecuencia, FileMode.Open, FileAccess.Read))
                {
                    planoDeCargaDto.PlanoDeCargaArchivoSecuenciaNombre = Path.GetFileName(file.Name);
                    file.CopyTo(ms);
                    planoDeCargaDto.FilePathSecuencia = Convert.ToBase64String(ms.ToArray());
                }
            }
            return planoDeCargaDto;
        }

        public ListaPaginada<ExportadorDto> ListarExportadores(Paginacion paginacion, string filtro)
        {
            Expression<Func<Exportador, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    x.Nombre.Contains(filtro);
            }
            return Listar<Exportador, ExportadorDto>(expresionFiltro, paginacion);
        }

        public IList<ExportadorDto> ListaExportadores()
        {
            return Listar<Exportador, ExportadorDto>();
        }

        public BalanzadaDto ObtenerBalanzada(int id, string numeroBalanza)
        {
            return Obtener<Balanzada, BalanzadaDto>(x => x.Id == id && x.NumeroBalanza == numeroBalanza);
        }

        public IEnumerable<int> ListarBalanzadasFaltantesPorRango(int id, int idFin, string numeroBalanza)
        {
            if (idFin != 0)
            {
                var listaEnteraBalanzadas = repositorio.Listar<Balanzada, int>(x => x.Id, x => x.CargaInicial.Id == id && x.CargaInicial.NumeroBalanza == numeroBalanza).OrderBy(x => x).ToList();
                var listaDeErrores = repositorio.Listar<RegistroBalanzaPuerto, int>(x => x.Id, x => x.Id > id && x.Id < idFin && x.Tipo != "balanzada");
                return Enumerable.Range(id + 1, (idFin - id - 1)).Except(listaEnteraBalanzadas).Except(listaDeErrores);
            }
            return Enumerable.Range(0, 0);
        }

        public ListaPaginada<ReportePesadaDto> ListarPaginadoCargasPorFecha(DateTime fechaInicio, DateTime FechaFin, Paginacion paginacion)
        {
            Expression<Func<Balanzada, bool>> expresionFiltro = null;

            expresionFiltro = x => (x.Fecha >= fechaInicio && x.Fecha <= FechaFin);

            return Listar<Balanzada, ReportePesadaDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<ReportePesadaDto> ListarConsultaDeCargasPorFecha(DateTime fechaInicio, DateTime fechaFin, string tipo, Paginacion paginacion)
        {
            var consulta = new ListarReporteCargasPuerto(fechaInicio, fechaFin, tipo, paginacion);
            return repositorio.ListarConsultaPaginada(consulta);
        }

        public ReportePesadaDto ListarPesadasOnline(string codigoBalanza)
        {
            return repositorio.ObtenerConsultaEscalar(new ObtenerDatosEmbarcandoPuerto(codigoBalanza));
        }

        public ListaPaginada<ReportePesadaSeisHorasDto> ListarReporteDePesadasPorTurno(DateTime fechaInicio, DateTime fechaFin, Paginacion paginacion, int? ExportadorId, int? MaterialId)
        {
            var consulta = new ListarReportePesadaSeisHoras(fechaInicio, fechaFin, paginacion, ExportadorId, MaterialId);
            return repositorio.ListarConsultaPaginada(consulta);
        }

        public ListaPaginada<CentroDto> ListarPaginadoCentrosConEstacionesMeteorologicas(Paginacion paginacion)
        {
            return Listar<Centro, CentroDto>(x => x.CodigoEstacionMeteorologica != null, paginacion);
        }

        public VaporDto ObtenerVapor(int id)
        {
            return Obtener<Vapor, VaporDto>(x => x.Id == id);
        }

        public BodegaDto ObtenerBodega(int id)
        {
            return Obtener<Bodega, BodegaDto>(x => x.Id == id);
        }

        public ExportadorDto ObtenerExportador(int id)
        {
            return Obtener<Exportador, ExportadorDto>(x => x.Id == id);
        }

        public DestinoDto ObtenerDestino(int id)
        {
            return Obtener<Destino, DestinoDto>(x => x.Id == id);
        }

        public MaterialPuertoDto ObtenerMaterialPuerto(int id)
        {
            return Obtener<MaterialPuerto, MaterialPuertoDto>(x => x.Id == id);
        }

        public IList<VaporDto> BuscarVapores(string criteria)
        {
            return Listar<Vapor, VaporDto>(f => f.Nombre.Contains(criteria), 20);
        }

        public VaporDto BuscarVapor(string criteria)
        {
            return Obtener<Vapor, VaporDto>(f => f.Nombre.Contains(criteria));
        }

        public IList<BodegaDto> BuscarBodegas(string criteria)
        {
            return Listar<Bodega, BodegaDto>(f => f.Nombre.Contains(criteria), 20);
        }

        public BodegaDto BuscarBodega(string criteria)
        {
            return Obtener<Bodega, BodegaDto>(f => f.Nombre.Contains(criteria));
        }

        public IList<ExportadorDto> BuscarExportadores(string criteria)
        {
            return Listar<Exportador, ExportadorDto>(f => f.Nombre.Contains(criteria), 20);
        }

        public ExportadorDto BuscarExportador(string criteria)
        {
            return Obtener<Exportador, ExportadorDto>(f => f.Nombre.Contains(criteria));
        }

        public IList<DestinoDto> BuscarDestinos(string criteria)
        {
            return Listar<Destino, DestinoDto>(f => f.Nombre.Contains(criteria), 20);
        }

        public DestinoDto BuscarDestino(string criteria)
        {
            return Obtener<Destino, DestinoDto>(f => f.Nombre.Contains(criteria));
        }

        public IList<MaterialPuertoDto> BuscarMaterialesPuerto(string criteria)
        {
            return Listar<MaterialPuerto, MaterialPuertoDto>(f => f.Descripcion.Contains(criteria), 20);
        }

        public MaterialPuertoDto BuscarMaterialPuerto(string criteria)
        {
            return Obtener<MaterialPuerto, MaterialPuertoDto>(f => f.Descripcion.Contains(criteria));
        }

        public IList<AlmacenDto> BuscarAlmacenesPuerto(string criteria)
        {
            return Listar<Almacen, AlmacenDto>(f => f.Descripcion.Contains(criteria), 20);
        }

        public AlmacenDto BuscarAlmacenPuerto(string criteria)
        {
            return Obtener<Almacen, AlmacenDto>(f => f.Descripcion.Contains(criteria));
        }

        public int TotalEmbarcado(int cargaInicial_id, string cargaInicial_numeroBalanza)
        {
            return repositorio.Listar<Balanzada>(x => x.CargaInicial_Id == cargaInicial_id && x.NumeroBalanza == cargaInicial_numeroBalanza).Sum(x => x.PesoNeto);
        }

        public IList<BalanzadaDto> ObtenerBalanzadasParaEnviarASAP(int cargaInicial_Id, string cargaInicial_NumeroBalanza)
        {
            return Listar<Balanzada, BalanzadaDto>(f => f.CargaInicial_Id == cargaInicial_Id && f.CargaInicial_NumeroBalanza == cargaInicial_NumeroBalanza && !f.EnviadoASap);
        }

        public List<MotivoQuiebreBarreraDto> ObtenerUltimosMovimientosDispositivo(string codigoDispositivo)
        {
            return Listar<MotivoQuiebreBarrera, MotivoQuiebreBarreraDto>(x => x.PuestoTrabajo.SensorQuiebre == codigoDispositivo, new Paginacion("Id", DirOrden.Desc, 1, 3)).Items.ToList();
        }

        public OtroRecorridoDelChoferDto ObtenerOtroRecorridoDelChofer(int choferId)
        {
            return ObtenerPrimero<Recorrido, OtroRecorridoDelChoferDto>(x => x.Chofer.Id == choferId && !x.Terminado && x.TipoVehiculo != TipoVehiculo.Tren);
        }

        public bool CupoConsumido(string cupoParametro, int centroId)
        {
            return repositorio.Existe<CargaDeCupo>(x => x.Cupo == cupoParametro && !x.SinCupo && (!x.Recorrido.Rechazado || (x.Recorrido.Rechazado && !x.Recorrido.Terminado)) && x.Centro.Id == centroId);
        }

        public bool CupoTransmitido(string cupoParamatro, string nroCartaPorte)
        {
            return repositorio.Existe<InformarCupoTransmisionASap>(x => (x.CodigoCupo == cupoParamatro && x.NumeroCartaPorte == nroCartaPorte));
        }

        public NirsDto ObtenerNirs(int id)
        {
            return Obtener<Nirs, NirsDto>(id);
        }

        public IList<NirsDto> ListarNirs()
        {
            return Listar<Nirs, NirsDto>();
        }

        public IList<NirsDto> ListarNirsPorCentro(int centroId)
        {
            return Listar<Nirs, NirsDto>(x => x.Centro.Id == centroId);
        }

        public ListaPaginada<NirsDto> ListarPaginadoNirs(string filtro, int centroId, Paginacion paginacion)
        {
            Expression<Func<Nirs, bool>> expresionFiltro;
            if (!string.IsNullOrEmpty(filtro))
            {
                int filtroInt;
                if (!int.TryParse(filtro, out filtroInt))
                {
                    filtroInt = int.MaxValue;
                }

                expresionFiltro = (x => ((x.Id == filtroInt)
                                         || ((x.Descripcion.Contains(filtro))
                                         || (x.DescripcionCorta.Contains(filtro)))
                                         && (x.Centro.Id == centroId)));
            }
            else
            {
                expresionFiltro = x => x.Centro.Id == centroId;
            }

            return Listar<Nirs, NirsDto>(expresionFiltro, paginacion);
        }

        public NirsDto ObtenerNirsPorNombrePc(int centroId, string nombrePc)
        {
            return Obtener<Nirs, NirsDto>(x => x.Centro.Id == centroId && x.PuestoDeTrabajo == nombrePc);
        }

        public string ObtenerMaterialNirsCodigoProducto(int id)
        {
            return Obtener<Material, MaterialDto>(x => x.Id == id).NirsCodigoProducto;
        }

        public IList<FotosDto> ListarHistoricoDeFotosPorPatente(int centroId, string patente)
        {
            var listaRetorno = new List<FotosDto>();
            var datosCamion = repositorio.Listar(x => new { x.Centro.CodigoSAP, x.NumeroDocumentoIngreso, x.Patente, Material = x.Material.Descripcion, x.FechaInicio, x.DatosProximaActividad, x.TipoDocumentoIngreso, x.Id }, (Recorrido x) => x.Patente == patente);
            foreach (var datoCamion in datosCamion)
            {
                var retorno = new FotosDto { Fotos = new List<FotoDto>(), Material = datoCamion.Material, NumeroDocumentoIngreso = datoCamion.NumeroDocumentoIngreso, Patente = datoCamion.Patente, FechaInicio = datoCamion.FechaInicio, TipoDocumento = datoCamion.TipoDocumentoIngreso.ToString(), RecorridoId = datoCamion.Id };
                listaRetorno.Add(retorno);
            }

            return listaRetorno.OrderByDescending(x => x.FechaInicio).ToList();
        }

        public ReasignacionDeTarjetaDto ObtenerReasignacionDeTarjeta(int id)
        {
            return Obtener<ReasignacionDeTarjeta, ReasignacionDeTarjetaDto>(x => x.Id == id);
        }

        public double CaladosPorHora(DateTime fecha, DateTime hasta)
        {
            TimeSpan diff = hasta - fecha;
            var hours = diff.TotalHours;
            return Math.Round(repositorio.Contar<Calado>(x => x.FechaCreacion >= fecha && x.FechaCreacion <= hasta) / (double)hours, 2);
        }

        public RecorridoDto UltimoCalado(DateTime fecha, DateTime turno, int centroId)
        {
            var recorrido = repositorio.ObtenerMayor<Recorrido, DateTime>(x => x.Centro.Id == centroId && x.Calado.FechaCreacion >= fecha && x.Calado.FechaCreacion <= turno && x.Calado.FechaCreacion.HasValue, x => x.Calado.FechaCreacion.Value);
            return conversor.Convertir<Recorrido, RecorridoDto>(recorrido);
        }

        public IList<string> ObtenerTodasBalanzaPuertoAdministrativa()
        {
            return repositorio.Listar<BalanzaPuerto, string>(x => x.CodigoBalanza, c => c.Administrativa);
        }

        public int ObtenerProximoIdEmbarqueLiquido(string codigoBalanza)
        {
            var ultimoregistro = repositorio.ObtenerMayor<RegistroBalanzaPuerto, int>(x => x.NumeroBalanza == codigoBalanza, x => x.Id);
            return (ultimoregistro != null) ? ultimoregistro.Id + 1 : 0;
        }

        public int obtenerIdBalanzadaSiguiente(int CargaInicial_Id, string NumeroBalanza)
        {
            var ultimoregistro = repositorio.ObtenerMayor<Balanzada, int>(x => x.CargaInicial_NumeroBalanza == NumeroBalanza && x.CargaInicial_Id == CargaInicial_Id, x => x.Id);
            return (ultimoregistro != null) ? ultimoregistro.Id + 1 : CargaInicial_Id + 1;
        }

        public ListaPaginada<ReportePesadaDto> ListarCargasHistoricas(DateTime desde, DateTime hasta, Paginacion paginacion)
        {
            return repositorio.Listar<Carga, ReportePesadaDto>(x =>
                    new ReportePesadaDto
                    {
                        IdCarga = x.Id,
                        Bodega = x.Bodega.Nombre,
                        Commodity = x.Material.Descripcion,
                        Destino = x.Destino.Nombre,
                        Exportador = x.Exportador.Nombre,
                        Fecha = x.Fecha,
                        NumeroBalanza = x.NumeroBalanza,
                        PesoProgramado = x.PesoProgramado,
                        TotalEmbarcado = (x.CargaOpuesta != null) ? x.CargaOpuesta.ToneladasAW : 0,
                        Vapor = x.Vapor.Nombre
                    }, x => x.Fecha <= hasta && x.Fecha >= desde && x.Tipo == "inicio" && x.Exportador.Nombre != "", paginacion);
        }

        public ListaPaginada<ReportePesadaDto> ListarCargasOnline(DateTime desde, DateTime hasta, Paginacion paginacion)
        {
            var cargas = repositorio.Listar<Carga, ReportePesadaDto>(x =>
                    new ReportePesadaDto
                    {
                        IdCarga = x.Id,
                        Bodega = x.Bodega.Nombre,
                        Commodity = x.Material.Descripcion,
                        Destino = x.Destino.Nombre,
                        Exportador = x.Exportador.Nombre,
                        Fecha = x.Fecha,
                        NumeroBalanza = x.NumeroBalanza,
                        PesoProgramado = x.PesoProgramado,
                        TotalEmbarcado = (x.CargaOpuesta != null) ? x.CargaOpuesta.ToneladasAW : 0,
                        Vapor = x.Vapor.Nombre
                    }, x => x.Fecha <= hasta && x.Fecha >= desde && x.Tipo == "inicio", paginacion);

            foreach (ReportePesadaDto carga in cargas)
            {
                if (carga.TotalEmbarcado == 0)
                {
                    carga.TotalEmbarcado = (int)repositorio.Sumar<Balanzada>(x => x.PesoNeto, x => x.CargaInicial_NumeroBalanza == carga.NumeroBalanza && x.CargaInicial_Id == carga.IdCarga);
                }
            }
            return cargas;
        }

        public ListaPaginada<ReciboMunicipalDto> ListarPaginadoReciboMunicipal(string filtro, Paginacion paginacion, int centroId)
        {
            DateTime filtroDate;
            if (!DateTime.TryParse(filtro, out filtroDate))
            {
                filtroDate = DateTime.MaxValue;
            }
            Expression<Func<ReciboMunicipal, bool>> expresionFiltro = x => true;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro = x => ((x.FechaActivacion == filtroDate));
            }
            else
            {
                expresionFiltro = x => x.Centro.Id == centroId;
            }

            return Listar<ReciboMunicipal, ReciboMunicipalDto>(expresionFiltro, paginacion);
        }

        public UltimoEstadoDto ObtenerEstadoRecorrido(Guid id)
        {
            var patente = repositorio.ObtenerProyeccion<Recorrido, string>(x => x.InstanciaWorkflow == id,
                f => f.Patente);
            var recorridoTerminado = repositorio.ObtenerMayor<Recorrido, int>(x => x.Patente == patente, f => f.Id);
            var controlRecorrido = repositorio.Listar<ControlRecorrido>(x => x.WorkflowInstanceId == recorridoTerminado.InstanciaWorkflow).ToList();
            if (recorridoTerminado != null && recorridoTerminado.Rechazado)
            {
                var mensajes = controlRecorrido.Select(x => x.Mensaje).ToList();
                var selectMotivo = repositorio.ObtenerMayor<Motivo, int>(x => mensajes.Contains(x.Descripcion), f => f.Id);

                UltimoEstadoDto ultimoEstado;
                if (selectMotivo == null)
                {
                    if (controlRecorrido.Any(f => f.Mensaje == Textos.TransportistaInhabilitado))
                    {
                        ultimoEstado = new UltimoEstadoDto
                        {
                            Descripcion = Textos.TransportistaInhabilitado,
                            DescripcionCorta = "",
                        };
                        return ultimoEstado;
                    }
                    if (controlRecorrido.Any(f => f.Mensaje == Textos.Rechazado) && controlRecorrido.Any(f => f.Comentario == Textos.RechazoBajaCTGComentario))
                    {
                        ultimoEstado = new UltimoEstadoDto
                        {
                            Descripcion = Textos.RechazoBajaCTG,
                            DescripcionCorta = "",
                        };
                        return ultimoEstado;
                    }
                    if (controlRecorrido.Any(f => f.ActividadXaml == Textos.Actividad_AutorizarTiempoEnTransito && !f.Decision))
                    {
                        ultimoEstado = new UltimoEstadoDto
                        {
                            Descripcion = Textos.MotivoTiempoEnTransito,
                            DescripcionCorta = "",
                        };
                        return ultimoEstado;
                    }

                    if (controlRecorrido.Any(f => f.Actividad == Textos.VerificacionSalidaFlete && f.Comentario.Contains("Compliance responde:")))
                    {
                        ultimoEstado = new UltimoEstadoDto
                        {
                            Descripcion = Textos.RechazoPorServicioCompliance,
                            DescripcionCorta = "",
                        };
                        return ultimoEstado;
                    }
                    if (
                        controlRecorrido.Any(
                            f =>
                                f.Actividad == Textos.Actividad_ExistePedidoDeTraslado &&
                                f.Comentario == Textos.PreguntaExistePedido && f.Decision == false))
                    {
                        ultimoEstado = new UltimoEstadoDto
                        {
                            Descripcion = Textos.ExistePedidoDeTraslado_Titulo,
                            DescripcionCorta = "",
                        };
                        return ultimoEstado;
                    }
                }
                else
                {
                    var control = controlRecorrido.Where(x => x.Mensaje == selectMotivo.Descripcion && x.Comentario != null && x.Comentario != "").FirstOrDefault();
                    ultimoEstado = new UltimoEstadoDto
                    {
                        Descripcion = string.Format("{0}: {1}", selectMotivo.Descripcion, control != null ? control.Comentario : ""),
                        DescripcionCorta = selectMotivo.DescripcionCorta,
                    };
                    return ultimoEstado;
                }
            }

            return null;
        }

        public UltimoEstadoDto ObtenerMotivoAutorizarRecorrido(Guid id)
        {
            var patente = repositorio.ObtenerProyeccion<Recorrido, string>(x => x.InstanciaWorkflow == id,
                f => f.Patente);
            var recorridoTerminado = repositorio.ObtenerMayor<Recorrido, int>(x => x.Patente == patente, f => f.Id);
            var controlRecorrido = repositorio.Listar<ControlRecorrido>(x => x.WorkflowInstanceId == recorridoTerminado.InstanciaWorkflow).ToList();
            if (controlRecorrido.Any(f => f.ActividadXaml == Textos.Actividad_AutorizarTiempoEnTransito && f.Decision))
            {
                var ultimoEstado = new UltimoEstadoDto
                {
                    Descripcion = repositorio.ObtenerProyeccion<AutorizarTiempoEnTransito, string>(x => x.WorkflowInstanceId == id, x => x.Comentario),
                    DescripcionCorta = "",
                };
                return ultimoEstado;
            }
            return null;
        }

        public RegistroBalanzaPuertoDto ObtenerMayorRegistro(string numeroBalanza)
        {
            return conversor.Convertir<RegistroBalanzaPuerto, RegistroBalanzaPuertoDto>(repositorio.ObtenerMayor<RegistroBalanzaPuerto, int>(x => x.NumeroBalanza == numeroBalanza, x => x.Id));
        }

        public List<String> ListarEmailsPorRoles(List<string> rols)
        {
            return repositorio.Listar<Usuario, String>(x => x.Email
            , x => x.RolesAsociados.Any(f => rols.Contains(f.Descripcion))).ToList();
        }

        public IList<AdjuntoDto> ListarAdjuntosCamion(int inhabilitacionCamionId)
        {
            return repositorio.Listar<Adjunto, AdjuntoDto>(x => new AdjuntoDto { Id = x.Id, Descripcion = x.Descripcion }, x => x.InhabilitacionCamion.Id == inhabilitacionCamionId);
        }

        public IList<AdjuntoDto> ListarAdjuntosChofer(int inhabilitacionChoferId)
        {
            return repositorio.Listar<Adjunto, AdjuntoDto>(x => new AdjuntoDto { Id = x.Id, Descripcion = x.Descripcion }, x => x.InhabilitacionChofer.Id == inhabilitacionChoferId);
        }

        public AdjuntoDto ObtenerAdjunto(int id)
        {
            return Obtener<Adjunto, AdjuntoDto>(id);
        }

        public ListaPaginada<ReglaDeAnalisisObligatorioDto> ListarReglaDeAnalisisObligatorioPaginado(Paginacion paginacion,
                                                                                           string term,
                                                                                           int centroId)
        {
            Expression<Func<ReglaDeAnalisisObligatorio, bool>> expresionFiltro =
                x =>
                x.Provincia.Descripcion.Contains(term) || x.Material.Descripcion.Contains(term) || String.IsNullOrEmpty(term) ||
                x.Localidad.Descripcion.Contains(term) && x.Cosecha.Contains(term);

            return Listar<ReglaDeAnalisisObligatorio, ReglaDeAnalisisObligatorioDto>(expresionFiltro, paginacion);
        }

        public ReglaDeAnalisisObligatorioDto ObtenerReglaDeAnalisisObligatorio(int id)
        {
            return Obtener<ReglaDeAnalisisObligatorio, ReglaDeAnalisisObligatorioDto>(id);
        }

        public IList<ReglaDeAnalisisObligatorioDto> ListarReglaDeAnalisisObligatorioActivas()
        {
            return Listar<ReglaDeAnalisisObligatorio, ReglaDeAnalisisObligatorioDto>(x => x.FechaDeVigenciaHasta > DateTime.Now && x.CantidadAnalisis > 0);
        }

        public bool VerificarCorrespondeCaladoEnPlanta(Guid instanceId)
        {
            return repositorio.ObtenerProyeccion<Recorrido, bool>(x => x.InstanciaWorkflow == instanceId, x => x.CorrespondeCaladoEnPlanta);
        }

        public ListaPaginada<RecorridoDto> ListarPaginadoRecorridosPorModificarDocumentoDeIngreso(int centroId,
                                                                                          Paginacion paginacion,
                                                                                          ModificarDocumentoDeIngresoDto model)
        {
            var listaCp = model.NumeroDocumentoIngreso.Split(';').Select(p => p.Trim()).ToList();
            Expression<Func<Recorrido, bool>> expresionFiltro =
                x =>
                x.Centro.Id == centroId &&
                (string.IsNullOrEmpty(model.NumeroDocumentoIngreso) ||
                (!string.IsNullOrEmpty(model.NumeroDocumentoIngreso) && listaCp.Contains(x.NumeroDocumentoIngreso)) ||
                (x.NumeroDocumentoIngreso == model.NumeroDocumentoIngreso || x.NumeroDocumentoIngresoLegal == model.NumeroDocumentoIngreso)) &&
                (!string.IsNullOrEmpty(model.NumeroDeTarjeta) || x.TipoDocumentoIngreso == model.TipoDocumentoIngreso) &&
                (string.IsNullOrEmpty(model.Patente) || x.Patente == model.Patente) &&
                (string.IsNullOrEmpty(model.NumeroDeTarjeta) || (x.TarjetaDeAcceso == model.NumeroDeTarjeta && !x.Terminado)) &&
                (model.TitularCartaPorteId == 0 ||
                    (x.Vehiculo.CartaPorte.RtteComercial != null && x.Vehiculo.CartaPorte.RtteComercial.Id == model.TitularCartaPorteId) ||
                    (x.Vehiculo.CartaPorte.RtteComercial == null && x.Vehiculo.CartaPorte.TitularCartaPorte.Id == model.TitularCartaPorteId)) &&
                (model.CorredorId == 0 || x.Vehiculo.CartaPorte.Corredor.Id == model.CorredorId) &&
                (model.FechaDesdeCP == null || (x.FechaInicio != null && x.FechaInicio >= model.FechaDesdeCP)) &&
                (model.FechaHastaCP == null || (x.FechaInicio != null && x.FechaInicio <= model.FechaHastaCP)) &&
                (model.FechaTaraDesdeCP == null ||
                    (x.Workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso && x.PesoTaraFecha != null && x.PesoTaraFecha >= model.FechaTaraDesdeCP) ||
                    (x.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso && x.PesoBrutoFecha != null && x.PesoBrutoFecha >= model.FechaTaraDesdeCP)
                    ) &&
                (model.FechaTaraHastaCP == null ||
                    (x.Workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso && x.PesoTaraFecha != null && x.PesoTaraFecha <= model.FechaTaraHastaCP) ||
                    (x.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso && x.PesoBrutoFecha != null && x.PesoBrutoFecha <= model.FechaTaraHastaCP)
                    ) &&
                (model.FechaVto == null || x.Vehiculo.CartaPorte.FechaVto == model.FechaVto) &&
                (model.MaterialId == 0 || x.Material.Id == model.MaterialId);
            return Listar<Recorrido, RecorridoDto>(expresionFiltro, paginacion);
        }

        public IList<AlmacenDto> ListarAlmacenesPorMaterialYCentroSustentableMixto(int centroId, int materialId)
        {
            return
                Listar<Almacen, AlmacenDto>(
                    f =>
                    f.Centro.Id == centroId && f.Materiales.Any(x => x.Id == materialId) && !f.EPA);
        }

        public IList<PuestosDeCargaDescargaDto> ListarHidraulicasPorCriterioSustentable(int centroId, bool esSustentable, bool sustentableMixta, bool excluirEspeciales = false)
        {
            return (excluirEspeciales)
                ? Listar<PuestosDeCargaDescarga, PuestosDeCargaDescargaDto>(
                    x => x.Centro.Id == centroId && (x.EsSojaSustentable == esSustentable || sustentableMixta) && (x.EsEspecial != excluirEspeciales || x.EsEspecial == null))
                : Listar<PuestosDeCargaDescarga, PuestosDeCargaDescargaDto>(
                    x => x.Centro.Id == centroId && (x.EsSojaSustentable == esSustentable || sustentableMixta));
        }

        public IList<MaterialDto> ListarMaterialesFiltroF515(int centroId)
        {
            var materiales = repositorio.ListarDistintos<Recorrido, Material>(x => x.Material, x => x.Terminado && x.Material != null && x.Centro.Id == centroId, 100);
            return conversor.ConvertirList<Material, MaterialDto>(materiales);
        }

        public bool ValidarCupoCartaPorte(string cupo, int centroId, string numeroCartaPorte)
        {
            return repositorio.Existe<CargaDeCupo>(x => x.Cupo == cupo && x.Centro.Id == centroId && x.Recorrido != null
                                                                && x.Recorrido.NumeroDocumentoIngreso != numeroCartaPorte);
        }

        public ListaPaginada<ClienteDto> ListarClientes(string filtro, Paginacion paginacion)
        {
            Expression<Func<Cliente, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    x.Descripcion.Contains(filtro) || x.Cuit.Contains(filtro) ||
                    x.CodigoSap.Contains(filtro) || x.Direccion.Contains(filtro);
            }

            return Listar<Cliente, ClienteDto>(expresionFiltro, paginacion);
        }

        public FotosDto ObtenerFotoCPDeCartasDePortePorrecorrido(List<int> id)
        {
            var resultado = new FotosDto();
            foreach (var r in id)
            {
                try
                {
                    var rutaFoto = repositorio.ObtenerProyeccion((Recorrido x) => x.Id == r, x => new { x.Vehiculo.CartaPorte.FotoRutaDestino, x.Patente, x.NumeroDocumentoIngreso, x.FechaInicio, x.Id });
                    if (!string.IsNullOrEmpty(rutaFoto.FotoRutaDestino))
                    {
                        ObtenerFoto(resultado, rutaFoto.FotoRutaDestino, rutaFoto.FechaInicio, rutaFoto.NumeroDocumentoIngreso);
                    }
                    if (id.Count == 1)
                    {
                        resultado.RecorridoId = rutaFoto.Id;
                        resultado.NumeroDocumentoIngreso = rutaFoto.NumeroDocumentoIngreso;
                        resultado.Patente = rutaFoto.Patente;
                    }
                }
                catch (Exception e)
                {
                    log.Error(e, "Error al ObtenerCartaDePortePorrecorrido " + id);
                }
            }
            return resultado;
        }

        public FotosDto ObtenerFotoCPDeCartaDePortePorrecorrido(Guid id)
        {
            var resultado = new FotosDto();
            try
            {
                var rutaFoto = repositorio.ObtenerProyeccion((Recorrido x) => x.InstanciaWorkflow == id, x => new { x.Vehiculo.CartaPorte.FotoRutaDestino, x.Patente, x.NumeroDocumentoIngreso, x.FechaInicio, x.Id });
                resultado.Patente = rutaFoto.Patente;
                resultado.NumeroDocumentoIngreso = rutaFoto.NumeroDocumentoIngreso;
                if (!string.IsNullOrEmpty(rutaFoto.FotoRutaDestino))
                {
                    ObtenerFoto(resultado, rutaFoto.FotoRutaDestino, rutaFoto.FechaInicio, rutaFoto.NumeroDocumentoIngreso);
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Error al ObtenerCartaDePortePorrecorrido " + id);
            }
            return resultado;
        }

        public CartaPorteDto ObtenerCartaDePortePorrecorrido(int id)
        {
            try
            {
                var rutaFoto = repositorio.ObtenerProyeccion<Recorrido, CartaPorte>(x => x.Id == id, x => x.Vehiculo.CartaPorte);
                if (rutaFoto != null)
                {
                    return conversor.Convertir<CartaPorte, CartaPorteDto>(rutaFoto);
                }
                return null;
            }
            catch (Exception e)
            {
                log.Error(e, "Error al ObtenerCartaDePortePorrecorrido " + id);
                return null;
            }
        }

        public List<string> ObtenerUsuariosAutorizarTiempoEnTransitoConfirmado()
        {
            return repositorio.Listar<Usuario, string>(x => x.Email,
                                                       x => x.AvisoAutorizarTiempoEnTransitoConfirmado == true).ToList();
        }

        public List<string> ObtenerUsuariosAutorizarTiempoEnTransitoRechazado()
        {
            return repositorio.Listar<Usuario, string>(x => x.Email,
                                                       x => x.AvisoAutorizarTiempoEnTransitoRechazado == true).ToList();
        }

        public List<HistoricoInhabilitacionChoferDto> ListarHistoricoInhabilitacionChofer(int id)
        {
            return Listar<HistoricoInhabilitacionChofer, HistoricoInhabilitacionChoferDto>(x => x.InhabilitacionChofer.Id == id).OrderBy(x => x.Fecha).ToList();
        }

        public List<HistoricoInhabilitacionCamionDto> ListarHistoricoInhabilitacionCamion(int id)
        {
            return Listar<HistoricoInhabilitacionCamion, HistoricoInhabilitacionCamionDto>(x => x.InhabilitacionCamion.Id == id).OrderBy(x => x.Fecha).ToList();
        }

        public List<AutorizacionCamionDto> ListarAutorizacionCamionPorInhabilitacion(int id)
        {
            return Listar<AutorizacionCamion, AutorizacionCamionDto>(x => x.InhabilitacionCamion.Id == id).OrderBy(x => x.Fecha).ToList();
        }

        public List<AutorizacionChoferDto> ListarAutorizacionChoferPorInhabilitacion(int id)
        {
            return Listar<AutorizacionChofer, AutorizacionChoferDto>(x => x.InhabilitacionChofer.Id == id).OrderBy(x => x.Fecha).ToList();
        }

        public DocumentoExternoDto ObtenerAdjuntoDocumentoExterno(int id)
        {
            var documento = Obtener<DocumentoExterno, DocumentoExternoDto>(id);
            if (documento != null)
            {
                var retorno = new FotosDto();
                if (documento.ArchivoExtension.ToLower() == "pdf")
                {
                    ObtenerArchivo(retorno, documento.ArchivoRutaDestino, DateTime.Now, string.Empty);
                }
                else
                {
                    ObtenerFoto(retorno, documento.ArchivoRutaDestino, DateTime.Now, string.Empty);
                }

                if (retorno.Fotos.Any())
                {
                    documento.Archivo = retorno.Fotos.First().Foto;
                }
            }

            return documento;
        }

        public string ObtenerTarjetaPorCargaDeCupo(Guid instanceId)
        {
            var nroCp = repositorio.ObtenerProyeccion((Recorrido x) => x.InstanciaWorkflow == instanceId, x => new { x.NumeroDocumentoIngreso, Cupo = x.Vehiculo != null ? x.Vehiculo.CartaPorte.Cupo : null, x.Patente });
            var carga = repositorio.ObtenerMayor<CargaDeCupo, int>(x => x.NumeroCartaPorte == nroCp.NumeroDocumentoIngreso || (x.Cupo != "MOL1111/11111111" && x.Cupo == nroCp.Cupo) || (x.Patente == nroCp.Patente && x.Recorrido == null), x => x.Id);
            return carga != null ? carga.Numero : null;
        }

        public bool ValidarLimiteDeCreditoVentaEnSAP(int centroId)
        {
            return repositorio.ObtenerProyeccion<Centro, bool>(x => x.Id == centroId, x => x.ValidarLimiteDeCreditoVentaEnSAP);
        }

        public bool TomaFotoEnMesa(int centroId)
        {
            return repositorio.ObtenerProyeccion<Centro, bool>(x => x.Id == centroId, x => x.TomarFotoEnMesa);
        }

        public PagoConMercadoPagoDto ObtenerPagoConMercadoPagoPorRecorridoId(int recorridoId)
        {
            var pago = repositorio.ObtenerMayor<PagoConMercadoPago, int>(x => x.Recorrido.Id == recorridoId, x => x.Id);
            return pago != null ? conversor.Convertir<PagoConMercadoPago, PagoConMercadoPagoDto>(pago) : null;
        }

        public FotosDto ObtenerFotosCartaPortePorNumero(string numero)
        {
            var retorno = new FotosDto();

            var cp = repositorio.ObtenerMayor<Recorrido, int, CartaPorteDto>(
                x => x.NumeroDocumentoIngreso == numero &&
                x.Vehiculo.CartaPorte.FotoRutaDestino != null
                , x => x.Id, x => new CartaPorteDto
                {
                    FotoRutaDestino = x.Vehiculo.CartaPorte.FotoRutaDestino,
                    FotoRutaDestinoDetalle = x.Vehiculo.CartaPorte.FotoRutaDestinoDetalle
                });

            if (cp == null)
            {
                cp = repositorio.ObtenerMayor<DocumentoExterno, int, CartaPorteDto>(
                x => x.NumeroDeDocumento == numero &&
                x.ArchivoRutaDestino != null
                , x => x.Id, x => new CartaPorteDto
                {
                    FotoRutaDestino = x.ArchivoRutaDestino
                });
            }

            if (cp != null && !string.IsNullOrEmpty(cp.FotoRutaDestino))
            {
                ObtenerFoto(retorno, cp.FotoRutaDestino, DateTime.Now, string.Empty);
            }
            if (cp != null && !string.IsNullOrEmpty(cp.FotoRutaDestinoDetalle))
            {
                ObtenerFoto(retorno, cp.FotoRutaDestinoDetalle, DateTime.Now, string.Empty);
            }
            return retorno;
        }

        public DocumentoExternoDto ObtenerDocumentoExterno(int id)
        {
            return Obtener<DocumentoExterno, DocumentoExternoDto>(id);
        }

        public bool TieneAgenteDecompras(Guid instanceId)
        {
            return repositorio.Existe<Recorrido>(x => x.InstanciaWorkflow == instanceId && x.Vehiculo.CartaPorte.AgenteCompras != null);
        }

        public List<EstadisticasCaladoDto> ObtenerEstadisticasCalado(DateTime fechaDesde, DateTime fechaHasta, int idCentro)
        {
            var datosCalado = repositorio.Listar<Recorrido, DatosCaladoDto>(
                x => new DatosCaladoDto
                {
                    Rechazado = x.Rechazado,
                    EnAnalisis = x.AnalisisDeCalidad != null,
                    Recalado = x.Calado.CicloDeCalado > 1,
                    Calador = x.Calado.Usuario,
                    Material = x.Material.Descripcion
                },
                x => x.Calado.FechaCreacion > fechaDesde && x.Calado.FechaCreacion < fechaHasta && x.Centro.Id == idCentro);

            var caladores = datosCalado.GroupBy(x => x.Calador).Select(x => x.Key).OrderBy(x => x).ToList();

            var agrupadoPorMaterial = datosCalado.GroupBy(x => x.Material);

            var resultado = new List<EstadisticasCaladoDto>();

            foreach (var datosCaladoMaterial in agrupadoPorMaterial)
            {
                var datosCaladores = this.ObtenerDatosCaladores(datosCaladoMaterial, caladores);

                resultado.Add(new EstadisticasCaladoDto
                {
                    Material = datosCaladoMaterial.Key,
                    CantidadRechazados = datosCaladoMaterial.Count(x => x.Rechazado),
                    CantidadEnAnalisis = datosCaladoMaterial.Count(x => x.EnAnalisis),
                    CantidadReclado = datosCaladoMaterial.Count(x => x.Recalado),
                    Total = datosCaladoMaterial.Count(),
                    Caladores = datosCaladores
                });
            }
            return resultado;
        }

        private List<CaladorCantidadDto> ObtenerDatosCaladores(IEnumerable<DatosCaladoDto> datosCaladoDto, List<string> caladores)
        {
            var agrupadoPorCalador = datosCaladoDto.GroupBy(x => x.Calador);
            var caladoresCantidad = new List<CaladorCantidadDto>();

            //Por cada calador existente en la lista total
            foreach (var calador in caladores.Where(x => !string.IsNullOrEmpty(x)))
            {
                var cantidad = agrupadoPorCalador.Where(x => x.Key == calador).FirstOrDefault();
                //Si aparece en la lista ingresada se obtiene cuantos hay.
                caladoresCantidad.Add(new CaladorCantidadDto
                {
                    Calador = calador,
                    Cantidad = cantidad != null ? cantidad.Count() : 0
                });
            }
            return caladoresCantidad;
        }

        public bool EsPuestoEnContingencia(int idPuesto, bool granos)
        {
            var workId = repositorio.Listar<ActividadPorDispositivo, int>(x => x.Workflow.Id, x => x.PuestoDeTrabajo.Id == idPuesto);

            return repositorio.Existe<MaterialPorWorkflow>(y => workId.Contains(y.Workflow.Id) && y.Material.EsGrano != granos);
        }

        public bool ImprimeTicketSalida(int idCentro)
        {
            return !repositorio.Existe<MaterialPorCentro>(x => x.Centro.Id == idCentro && !x.ImprimeReciboMunicipal && !x.IgnoraContingencia);
        }

        public EstadoPuertoDto ObtenerEstadoPuerto()
        {
            var estado = repositorio.ObtenerMayor<EstadoPuerto, int>(x => true, x => x.Id) ?? new EstadoPuerto();
            return conversor.Convertir<EstadoPuerto, EstadoPuertoDto>(estado);
        }

        public string ObtenerDispositivosMaestroApertura(int puestoId)
        {
            return repositorio.ObtenerProyeccion<PuestoDeTrabajo, string>(
                x => x.Id == puestoId,
                x => x.EntradaSupervisor);
        }

        public string ObtenerDispositivosMaestroCierre(int puestoId)
        {
            return repositorio.ObtenerProyeccion<PuestoDeTrabajo, string>(
                x => x.Id == puestoId,
                x => x.CierreSupervisor);
        }

        public string ObtenerCalleInterna(Guid id)
        {
            var calle = repositorio.ObtenerProyeccion<Recorrido, Calle>(x => x.InstanciaWorkflow == id, x => x.Calle);
            if (calle != null)
            {
                return calle.Nombre;
            }
            return null;
        }

        public int? ObtenerAlmacenPorRecorrido(int id)
        {
            return repositorio.ObtenerProyeccion<Recorrido, int>(x => x.Id == id, x => x.Almacen.Id);
        }

        public int ObtenerEspacioDisponible(TipoCalle tipoCalle, int materialId, TipoCalidad calidad)
        {
            return administradorDeCalles.ObtenerEspacioDisponible(tipoCalle, materialId, calidad);
        }

        public InfoPatenteDeCalleDto ObtenerInfoPatente(string patente, int calleId)
        {
            var camion = repositorio.ObtenerProyeccion<CallePorRecorrido, InfoPatenteDeCalleDto>(x => (x.Recorrido.Patente == patente || x.CargaDeCupo.Patente == patente)
             && x.Calle.Id == calleId
             && x.FechaEgreso == null, x => new InfoPatenteDeCalleDto
             {
                 Patente = x.Recorrido != null ? x.Recorrido.Patente : x.CargaDeCupo != null ? x.CargaDeCupo.Patente : string.Empty,
                 NombreChofer = x.Recorrido != null ? x.Recorrido.Chofer.Nombre + "  " + x.Recorrido.Chofer.Apellido : string.Empty,
                 CartaPorte = x.Recorrido != null ? x.Recorrido.NumeroDocumentoIngreso : x.CargaDeCupo != null ? x.CargaDeCupo.CTG : string.Empty,
                 RecorridoId = x.Recorrido != null ? x.Recorrido.Id : (int?)null,
                 CargaDeCupoId = x.CargaDeCupo != null ? x.CargaDeCupo.Id : (int?)null,
                 CalleId = x.Calle.Id,
                 InstanciaWorflow = x.Recorrido != null ? x.Recorrido.InstanciaWorkflow : (Guid?)null,
                 Rechazado = x.Recorrido != null && x.Recorrido.Rechazado,
                 TipoCalle = x.Calle.TipoCalle,
                 TipoCalidad = x.Calle.TipoCalidad,
                 MaterialId = x.Recorrido != null ? x.Recorrido.Material.Id : x.CargaDeCupo.Material.Id,
                 CaladoId = x.Recorrido != null ? x.Recorrido.Calado.Id : (int?)null,
                 Tarjeta = x.Recorrido != null ? x.Recorrido.TarjetaDeAcceso : x.CargaDeCupo != null ? x.CargaDeCupo.Numero : null,
                 WorkflowDefinicionId = x.Recorrido != null ? x.Recorrido.WorkflowDefinicion.Id : (int?)null,
                 CalidadCamion = x.Recorrido != null ? (x.Recorrido.CaracteristicasAnalizadasList.FirstOrDefault() != null ? x.Recorrido.CaracteristicasAnalizadasList.FirstOrDefault().Calidad : TipoCalidad.Desconocida) : TipoCalidad.Desconocida,
                 CalleNoGrano = x.Calle.TipoCalle == TipoCalle.NoGranos,
                 NombreWorkflow = x.Recorrido != null ? x.Recorrido.Workflow.Descripcion : string.Empty,
                 FechaIngreso = x.FechaIngeso,
                 TipoDocumento = x.Recorrido != null ? x.Recorrido.TipoDocumentoIngreso : (TipoDocumentoIngreso?)null,
                 Material = x.Recorrido != null ? x.Recorrido.Material.Descripcion : x.CargaDeCupo != null ? x.CargaDeCupo.Material.Descripcion : string.Empty,
                 TipoVehiculo = x.Recorrido != null ? x.Recorrido.TipoVehiculo : (TipoVehiculo?)null,
                 DescripcionAlmacen = x.Recorrido != null ? x.Recorrido.Almacen.Descripcion : string.Empty,
                 EsSojaEPA = x.Recorrido != null && x.Recorrido.Establecimiento != null && x.Recorrido.Establecimiento.EPA,
                 EsSojaIMPO = x.CargaDeCupo != null && x.CargaDeCupo.TitularCartaPorteCodigoSap == Constantes.ValoresPorDefecto.CodigoSapTPR
             });

            if (camion == null)
            {
                return new InfoPatenteDeCalleDto();
            }

            var actividad = repositorio.Listar<LogActividad>(x => x.WorkflowInstanceId == camion.InstanciaWorflow).OrderBy(x => x.Fecha).LastOrDefault();
            camion.Etapa = actividad != null ? actividad.Actividad : "";
            camion.Cliente = camion.TipoDocumento == TipoDocumentoIngreso.OrdenCargaFas ?
                repositorio.ObtenerProyeccion<OrdenCargaFas, string>(x => x.Recorrido.Id == camion.RecorridoId, x => x.Cliente.Descripcion) : "";
            return camion;
        }

        public PinchazosPorCaladaDto UltimoCambioPinchazo(int centroId)
        {
            var pinchazo = repositorio.ObtenerMayor<PinchazosPorCalada, DateTime>(x => x.Centro.Id == centroId, x => x.FechaModificacion);
            if (pinchazo == null)
            {
                pinchazo = new PinchazosPorCalada()
                {
                    Centro_Id = centroId,
                    TipoPinchazo = TipoPinchazo.DosYTres,
                    FechaModificacion = DateTime.MinValue
                };
            }
            return conversor.Convertir<PinchazosPorCalada, PinchazosPorCaladaDto>(pinchazo);
        }

        public List<string> ObtenerUsuariosCambioPinchazosPorCalada()
        {
            return repositorio.Listar<Usuario, string>(x => x.Email,
                                           x => x.AvisoCambioPinchazosPorCalada).ToList();
        }

        public IList<CalleDto> ListarTodasLasCalles(int centroId)
        {
            return Listar<Calle, CalleDto>(x => x.CentroId.Equals(centroId));
        }

        public IList<CallePorRecorridoDto> ListarCallePorRecorridoPorCalleId(int calleId)
        {
            return Listar<CallePorRecorrido, CallePorRecorridoDto>(x => x.Calle.Id.Equals(calleId) && x.FechaEgreso == null);
        }

        public IList<PuestosDeCargaDescargaDto> ListarHidraulicasPorCentro(int centroId)
        {
            return
                Listar<PuestosDeCargaDescarga, PuestosDeCargaDescargaDto>(
                    x => x.Centro.Id == centroId);
        }

        public WorkFlowsFiltradosDto ConsultarEstadoWorkflow(WorkFlowsFiltradosDto resultado)
        {
            return repositorio.ObtenerConsultaEscalar(new ConsultarEstadosWorkflows(resultado));
        }

        public IList<PuestoDeTrabajoDto> ListarPuestosDeBalanzasAutomaticas()
        {
            return Listar<PuestoDeTrabajo, PuestoDeTrabajoDto>(x => x.AutomatizadoFull || x.Firmware.Contains("FirmwarePuestoBalanzaAutomatico"));
        }

        public bool BalanzaEnCero(int balanzaId)
        {
            return repositorio.Existe<Balanza>(x => x.Id == balanzaId && x.EstaEnCero);
        }

        public IList<BajaCTGRetransmisionDto> ListarBajaCTGError(int centroId)
        {
            return repositorio.ObtenerConsultaEscalar(new ListarBajaCTGConErrorConsulta(centroId));
        }

        public IList<VideoCamaraDto> ObtenerCamarasPorNombrePc(string nombrePc, int centroId)
        {
            var camaras = Listar<VideoCamara, VideoCamaraDto>(x => x.PuestoDeTrabajo.NombrePc == nombrePc && x.PuestoDeTrabajo.Centro.Id == centroId);
            var codigosDeCamara = camaras.Select(c => c.Codigo).ToList();
            var lista = servicioOrquestador.ObtenerCamaras(codigosDeCamara.ToArray()).ToList();

            return lista.Where(x => codigosDeCamara.Contains(x.Codigo)).Select(x => new VideoCamaraDto
            {
                Codigo = x.Codigo,
                Directorio = x.Url,
                Posicion = camaras.Where(c => c.Codigo == x.Codigo).Select(c => c.Posicion).FirstOrDefault()
            }).ToList();
        }

        public bool EsPuestoPausado(int puestoId)
        {
            return repositorio.Existe<PuestoDeTrabajo>(x => x.Id == puestoId && x.PausaAutoFull);
        }

        public string ObtenerBodyPlanoDeCarga(int planoDeCargaId, EmbarqueDto embarque)
        {
            // CARACTERES NO IMPRIMIBLES:
            // Enter: (\n -> <br/>)
            // Tabulador: (\t -> &nbsp;&nbsp;&nbsp;&nbsp;)
            // Negrita: (\f -> <b>) (\f\f -> </b>)
            // Subrayado: (\0 -> <u>) (\0\0 -> </u>)
            var body = $"\f {embarque.NombreBuque.ToUpper()} \n";
            //body += $"CANTIDAD: {embarque.MaterialesPuertoCantidad.Sum(x => x.Cantidad)} tn. \n";
            body += "\0 CARGADOR(ES): \0\0\f\f\n";
            var cargasComerciales = Listar<CargaComercial, CargaComercialDto>(x => x.PlanoDeCarga.Id == planoDeCargaId)
                                        .GroupBy(x => x.Exportador.Nombre)
                                        .Select(group => new
                                        {
                                            Exportador = group.Key,
                                            Cantidad = group.Sum(y => y.Cantidad)
                                        }).ToList();
            foreach (var carga in cargasComerciales)
            {
                body += $"- {carga.Exportador.Trim().PadRight(30, '.')} {carga.Cantidad} tn.\n";
                body += "\t\f PRODUCTO(S): \f\f\n";

                var cargasComercialesP = Listar<CargaComercial, CargaComercialDto>(x => x.PlanoDeCarga.Id == planoDeCargaId && x.Exportador.Nombre == carga.Exportador)
                                        .GroupBy(x => x.MaterialPuerto.DescripcionCorta).Select(group => new
                                        {
                                            Producto = group.Key,
                                            Cantidad = group.Sum(y => y.Cantidad)
                                        }).ToList();
                foreach (var materiales in cargasComercialesP)
                {
                    body += $"\t {materiales.Producto.Trim().PadRight(10, '.')} {materiales.Cantidad} tn.\n";
                }
            }

            body += "\n\0\f DESTINO(S): \0\0\f\f\n";
            var planoDeCargaBodegas = Listar<PlanoDeCargaBodega, PlanoDeCargaBodegaDto>(x => x.PlanoDeCarga.Id == planoDeCargaId);
            var destinos = planoDeCargaBodegas.GroupBy(x => x.Destino == null ? "No Definido" : x.Destino.Nombre).Select(group => new
            {
                Destino = group.Key,
                Cantidad = group.Sum(y => y.Cantidad)
            }).ToList();
            foreach (var destino in destinos)
            {
                body += $"\t {destino.Destino.Trim().PadRight(10, '.')} {destino.Cantidad} tn. \n";
            }

            body += "\n\f-------------------------------------------------------------------------------------------------\f\f\n";
            body += " \fPlano de carga: \f\f\n";
            foreach (var bodega in planoDeCargaBodegas)
            {
                body += $"\t H{bodega.BodegaParcel}S - {bodega.MaterialPuerto.DescripcionCorta.Trim().PadRight(10, '.')} {bodega.Cantidad} tn. ";
                if (bodega.Destino != null)
                    body += $"{bodega.Destino.Nombre.Trim()}. \n";
                else
                    body += "No Definido.\n";
            }

            var planoDeCarga = repositorio.Obtener<PlanoDeCarga>(planoDeCargaId);

            body += $"\n\f Total a cargar: \f\f....... {planoDeCargaBodegas.Sum(x => x.Cantidad)} tn. (A un calado {planoDeCarga.CaladoSalida.ToString()} mts.).\n";
            body += "\f-------------------------------------------------------------------------------------------------\f\f\n\n";

            body += $"\fEncargado de agencia: \f\f";
            int contador = 0;
            foreach (var encargados in planoDeCarga.AgentesControlPrivado)
            {
                if (contador != 0)
                    body += " o ";

                body += $"{encargados.Nombre} {encargados.Apellido}";
                contador++;
            }
            if (contador == 0)
                body += "No Definido.";

            body += $"\n\fEncargado de estiba: \f\f";
            if (planoDeCarga.Estiba != null)
                body += $"{planoDeCarga.Estiba.Nombre} {planoDeCarga.Estiba.Apellido}.\n";
            else
                body += "No Definido.\n";

            body += $"\fControles: \f\f";
            if (planoDeCarga.AgenciaControlPrivado != null)
                body += $"{planoDeCarga.AgenciaControlPrivado.Nombre}.\n";
            else
                body += "No Definido.\n";

            if (embarque.Senasa)
                body += "\fSENASA:\f\f SI.\n";
            else
                body += "\fSENASA:\f\f NO.\n";

            if (planoDeCarga.Fumigacion)
                body += $"\fFumigación:\f\f SI. ({planoDeCarga.EmpresaFumigadora}).\n";
            else
                body += $"\fFumigación:\f\f NO.\n";

            body += $"\fAgencia: \f\f";
            if (embarque.Agencias != null)
                body += $"{embarque.Agencias.Nombre}.\n";
            else
                body += "No Definida.\n";

            if (planoDeCarga.DefensasMoviles)
                body += $"\fDefensas móviles:\f\f SI.\n";
            else
                body += $"\fDefensas móviles:\f\f NO.\n";

            body += "\n\fObservacion(es):\f\f\n";
            body += $"\t {planoDeCarga.Observaciones}";
            return body;
        }

        public IList<ATAPuertoDto> ListarATAPuerto()
        {
            return Listar<ATAPuerto, ATAPuertoDto>();
        }

        public IList<TipoDeBuquePuertoDto> ListarTipoDeBuquePuerto()
        {
            return Listar<TipoDeBuquePuerto, TipoDeBuquePuertoDto>();
        }

        public IList<UbicacionDeBuquePuertoDto> ListarUbicacionDeBuquePuerto()
        {
            var lista = Listar<UbicacionDeBuquePuerto, UbicacionDeBuquePuertoDto>();
            return lista.OrderBy(x => x.Orden).ToList();
        }

        public bool ExistePagoRealizado(string patente)
        {
            var hoy = DateTime.Now;
            return repositorio.Existe<ImpReciboMunicipal>(
                x => x.Patente == patente
                && hoy > x.FechaImpresion && hoy < (DateTime)EntityFunctions.AddDays(x.FechaImpresion, 1)
                );
        }

        public BalanzaDto ObtenerBalanzaPorPuestoDeTrabajoSinTipoVehiculo(int puestoDeTrabajoId)
        {
            return repositorio.ObtenerProyeccion<PuestoDeTrabajo, BalanzaDto>(x => x.Id == puestoDeTrabajoId,
                x => new BalanzaDto { Id = x.Balanza.Id, CodigoCabezal = x.Balanza.CodigoCabezal });
        }

        public IList<MensajeCartelLedDto> ListarMensajesCartelLed(string codigo)
        {
            return Listar<MensajeCartelLed, MensajeCartelLedDto>(x => x.Codigo == codigo && x.Habilitado).OrderBy(x => x.Orden).ToList();
        }

        public IList<SentidoManoDeEmbarqueDto> ListarSentidoManoDeEmbarques()
        {
            return Listar<SentidoManoDeEmbarque, SentidoManoDeEmbarqueDto>().OrderBy(x => x.Posicion).ToList();
        }

        public IList<CeldaManoDeEmbarqueDto> ListarCeldaManoDeEmbarques()
        {
            return Listar<CeldaManoDeEmbarque, CeldaManoDeEmbarqueDto>().OrderBy(x => x.Posicion).ToList();
        }

        public ModuloDeCargaDto ObtenerModuloDeCarga(int moduloDeCargaId)
        {
            var moduloDeCargaDto = Obtener<ModuloDeCarga, ModuloDeCargaDto>(x => x.Id == moduloDeCargaId);

            return moduloDeCargaDto;
        }

        public IList<MotivosLimpiezaDto> ListarMotivosLimpieza()
        {
            return Listar<MotivosLimpieza, MotivosLimpiezaDto>();
        }

        public ModuloDeCargaListadoDto ObtenerUltimaHabilitacionDeTanques()
        {
            var resultado = new ModuloDeCargaListadoDto();

            try
            {
                Expression<Func<ModuloDeCarga, bool>> expresionFiltro = x => x.Enviado == true && x.ModuloDeCargaHabilitacionDeTanques.Count() != 0;
                var auxModuloDeCargaListadoDto = repositorio.Listar(expresionFiltro);
                var ultimoModuloDeCargaListadoDto = conversor.ConvertirList<ModuloDeCarga, ModuloDeCargaListadoDto>(auxModuloDeCargaListadoDto)
                                            .OrderByDescending(x => x.FechaDeFinalizacion).ToList();

                if (ultimoModuloDeCargaListadoDto.Count() != 0)
                {
                    var item = ultimoModuloDeCargaListadoDto.First();
                    var moduloDeCargaHabilitacionDeTanquesDto = Obtener<ModuloDeCargaHabilitacionDeTanques, ModuloDeCargaHabilitacionDeTanquesDto>
                        (x => x.ModuloDeCarga.Id == item.Id);

                    if (moduloDeCargaHabilitacionDeTanquesDto != null)
                    {
                        resultado = item;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultado;
        }

        public IList<MotivosDeCorteDto> ListarMotivosDeCorte()
        {
            return Listar<MotivosDeCorte, MotivosDeCorteDto>();
        }

        public IList<MotivosFallasBalanzaDto> ListarMotivosFallasBalanza()
        {
            return Listar<MotivosFallasBalanza, MotivosFallasBalanzaDto>();
        }

        public IList<TurnoPuertoDto> ListarTurnoPuerto()
        {
            return Listar<TurnoPuerto, TurnoPuertoDto>().OrderBy(x => x.Orden).ToList();
        }

        public double ObtenerCantidadCubitacionDeTanques(string tk, double altura)
        {
            var cantidadCubitacionDeTanques = Obtener<CubitacionDeTanques, CubitacionDeTanquesDto>(x => x.Tk == tk && x.Altura == altura).Cantidad;

            return cantidadCubitacionDeTanques;
        }

        public decimal ObtenerDensidadPorTemperaturaDeMaterial(int materialPuertoId, int grado)
        {
            DensidadPorTemperaturaDeMaterialDto densidadPorTemperaturaDeMaterialDto = new DensidadPorTemperaturaDeMaterialDto();

            densidadPorTemperaturaDeMaterialDto = Obtener<DensidadPorTemperaturaDeMaterial, DensidadPorTemperaturaDeMaterialDto>(x => x.MaterialPuerto.Id == materialPuertoId && x.Grado == grado);

            if (densidadPorTemperaturaDeMaterialDto != null)
            {
                return densidadPorTemperaturaDeMaterialDto.Densidad;
            }

            return 0;
        }

        public int ObtenerLlenadoMinimetroPorTanque(string cm, string mm, string tanqueNum)
        {
            LlenadoMilimetroPorTanqueDto llenadoMilimetroPorTanqueDto = new LlenadoMilimetroPorTanqueDto();

            llenadoMilimetroPorTanqueDto = Obtener<LlenadoMilimetroPorTanque, LlenadoMilimetroPorTanqueDto>(x => x.Cm == cm && x.Mm == mm && x.TanqueNum == tanqueNum);

            if (llenadoMilimetroPorTanqueDto != null)
            {
                return Convert.ToInt32(llenadoMilimetroPorTanqueDto.LlenadoMm);
            }

            return 0;
        }

        public IList<BalanzadaDto> ListarBalanzadaBuque(int buque)
        {
            try
            {
                var balanzadas = new List<BalanzadaDto>();
                var balanzas = ListarBalanzasPuerto();
                IList<BalanzadaDto> balanzada = new List<BalanzadaDto>();
                foreach (var balanza in balanzas)
                {
                    CargaFiltroDto filtro = new CargaFiltroDto();
                    filtro.IdVapor = buque;
                    filtro.NumeroBalanza = balanza.CodigoBalanza;

                    var cargas = ListarCargasSinPaginado(filtro);
                    foreach (var cargaBalanza in cargas)
                    {
                        balanzada = Listar<Balanzada, BalanzadaDto>(x => x.CargaInicial.Id == cargaBalanza.Id && x.CargaInicial.NumeroBalanza == cargaBalanza.NumeroBalanza);

                        foreach (var bal in balanzada)
                            balanzadas.Add(bal);
                    }
                    //  balanzadas.Add(Obtener<Balanzada, BalanzadaDto>(x => x.CargaInicial_Id == cargaBalanza.Id));
                }

                return balanzadas;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<CargaDto> ListarCargaBalanzaPuerto()
        {
            var balanzas = ListarBalanzasPuerto();
            var cargas = new List<CargaDto>();
            IList<CargaDto> cargasBalanza = new List<CargaDto>();
            CargaFiltroDto filtro = new CargaFiltroDto();
            foreach (var balanza in balanzas)
            {
                filtro.NumeroBalanza = balanza.CodigoBalanza;
                cargasBalanza = ListarCargasSinPaginado(filtro);
                //cargasBalanza = Listar<Carga, CargaDto>(x => x.NumeroBalanza == balanza.CodigoBalanza);
                foreach (var listaCargas in cargasBalanza)
                {
                    cargas.Add(listaCargas);
                }
                //cargas.Add(Listar<Carga,CargaDto>(x=>x.NumeroBalanza == balanza.CodigoBalanza));
            }
            return cargas;
        }

        public Resultado ActualizarEstadoBuque(int Embarque_Id, int Estado)
        {
            var resultado = new Resultado();
            try
            {
                var embarque = repositorio.Obtener<Embarque>(x => x.Id == Embarque_Id);

                if (embarque != null)
                {
                    var estadoBuq = repositorio.Obtener<EstadoBuque>(x => x.Id == Estado);
                    embarque.EstadoBuque = estadoBuq;
                    repositorio.GuardarCambios();
                }
            }
            catch (Exception e)
            {
                resultado.Error("Hubo un error", e.Message);
            }
            return resultado;
        }

        public string obtenerDireccionesDeMail(string templateMail)
        {
            var direcciones = Obtener<ConfiguracionMail, ConfiguracionMailDto>(x => x.TemplateMail == templateMail).Direcciones;
            return direcciones;
        }

        public ListaPaginada<ImpEtiquetaPuertoDto> ListarEtiquetasPuerto(int usuarioId, Paginacion paginacion)
        {
            return Listar<ImpEtiquetaPuerto, ImpEtiquetaPuertoDto>(x => x.Usuario_Id == usuarioId, paginacion);
        }

        public int? ObtenerPesoNetoExportacion(Guid id)
        {
            return repositorio.ObtenerProyeccion<IngresoDeDatosDeExportacion, int?>(x => x.Recorrido.InstanciaWorkflow == id, x => x.PesoNeto);
        }

        public NotificacionAplicacionDto ObtenerNotificacionAplicacion(string usuario)
        {
            var notificacion = repositorio.ObtenerMayor<NotificacionAplicacion, DateTime>(x => x.FechaAccion != null, x => x.FechaAccion);
            if (notificacion == null)
            {
                notificacion = new NotificacionAplicacion()
                {
                    TipoAccion = " ",
                    FechaAccion = DateTime.Now
                };
            }

            return conversor.Convertir<NotificacionAplicacion, NotificacionAplicacionDto>(notificacion);
        }

        public ReporteDetalleMovimientoDto ReporteDetalleDeMovimiento(int centroId, DateTime fecha)
        {
            var detalle = new ReporteDetalleMovimientoDto();

            detalle.DsIngresoDto = repositorio.ListarConsulta(new ListarDetalleDeMovimientoIngreso(centroId, fecha));
            return detalle;
        }

        public List<MaterialPorCentroDto> ListarMaterialGranoPorCentro(int centroId, bool esGrano)
        {
            var materialesPorCentro = Listar<MaterialPorCentro, MaterialPorCentroDto>(
                    x => x.Centro.Id == centroId && x.Material.EsGrano == esGrano && x.Material.Activo).ToList();

            return materialesPorCentro;
        }

        public IList<MaterialDto> ObtenerMaterialNoGranoAsignableCalle()
        {
            return Listar<Material, MaterialDto>(x => !x.EsGrano && x.EsAsignableCalle);
        }

        public IList<WorkflowDto> ListarWorkFlowsPendientesNoGrano(int centroId)
        {
            return Listar<Workflow, WorkflowDto>(x => x.Centro.Id == centroId && x.PendienteNoGranos && x.Activo);
        }

        public MaterialPorCentroDto ObtenerMaterialPorCodigoAfip(int centroId, int material)
        {
            return Listar<MaterialPorCentro, MaterialPorCentroDto>(x => x.Material.Activo && x.Material.CodigoEspecie == material && x.Centro.Id == centroId).LastOrDefault();
        }

        public ProveedorDto ObtenerProveedorPorId(int Id)
        {
            return Obtener<Proveedor, ProveedorDto>(x => x.Id == Id);
        }

        public IList<PuestoDeTrabajoDto> ListarPuestosDeTrabajoPorCodigoLectorQR(string Codigo)
        {
            return Listar<PuestoDeTrabajo, PuestoDeTrabajoDto>(x => x.LectorQr == Codigo);
        }

        public List<ConfiguracionAutomatizacionEtapasDto> ObtenerConfiguracionAutomatizacionEtapas()
        {
            return Listar<ConfiguracionAutomatizacionEtapas, ConfiguracionAutomatizacionEtapasDto>(x => !x.Deshabilitada).ToList();
        }

        public BajaCTGDto ObtenerBajaCTG(int cartaPorteid)
        {
            return Obtener<BajaCTG, BajaCTGDto>(x => cartaPorteid == x.CartaPorte.Id);
        }

        public List<long> ObtenerCpesNoCacheadas(List<CartaPorteResumenDto> ctgs)
        {
            var cpesEnBase = this.repositorio.ListarConsulta<CartaPorteElectronicaDto>(new ListarCpeAfipCacheadas(ctgs));
            ctgs.RemoveAll(x => cpesEnBase.Any(y => y.NroCtg == x.Ctg));
            return ctgs.Select(x => x.Ctg).ToList();
        }

        public int ObtenerSequenciaNumeroCTGCartaPorteElectronica()
        {
            return repositorio.ObtenerSequenciaCPENroCTG();
        }

        public CartaPorteElectronicaDto ObtenerCartaPorteElectronicaPorCTG(string ctg)
        {
            var ctgLong = long.Parse(ctg);
            return Listar<CartaPorteElectronica, CartaPorteElectronicaDto>(x => x.NroCTG == ctgLong).FirstOrDefault();
        }

        public List<StockDeEstablecimientoDto> ListarCampaniaPorCuit(string cuit, string cosecha)
        {
            cuit = cuit.Length < 13 ? cuit.Insert(2, "-").Insert(11, "-") : cuit;
            return repositorio.ListarConsulta(new ListarCampaniaPorCuit(cuit, cosecha));
        }

        public IList<CalleDto> ObtenerCallesPorCentro(int centroId)
        {
            return Listar<Calle, CalleDto>(x => x.CentroId == centroId);
        }

        public IList<CallePorRecorridoDto> ObtenerEstadoDeCalle()
        {
            var estadoDeRecorridos = repositorio.ListarConsulta(new ListarEstadoDeCalle());

            var resultado = new List<CallePorRecorridoDto>();

            foreach (var item in estadoDeRecorridos)
            {
                var callePorRecorrido = new CallePorRecorridoDto
                {
                    Id = item.Id,
                    Calidad = (item.Calidad != null) ? (int)item.Calidad : 0,
                    MaterialId = item.RecorridoMaterialId ?? item.CargaCupoMaterialId ?? 0,
                    MaterialDesc = item.RecorridoMaterialDescripcion ?? item.CargaCupoMaterialDescripcion ?? string.Empty,
                    Patente = item.RecorridoPatente ?? item.CargaDeCupoPatente ?? item.CargaDeCupoRecorridoPatente ?? string.Empty,
                    CalleId = item.CalleId,
                    FechaIngeso = item.FechaIngreso,
                    UltimoDeLaFila = item.UltimoDeLaFila,
                    Rechazado = item.Rechazado ?? false,
                    AsignadoEnPuestoComando = item.AsignadoEnPuestoComando,
                    TipoCalle = item.TipoCalle,
                    Escalable = item.TipoVehiculo == TipoVehiculo.CamiónC
                    || item.TipoVehiculo == TipoVehiculo.CamiónD
                    || item.TipoVehiculo == TipoVehiculo.CamiónE,
                    EsSojaEPA = item.EPA ?? false,
                    EsSojaIMPO = item.RecorridoCodigoSAP != null ?
                                       item.RecorridoCodigoSAP == Constantes.ValoresPorDefecto.CodigoSapTPR :
                                       item.CargaDeCupoCodigoSAP != null ? item.CargaDeCupoCodigoSAP == Constantes.ValoresPorDefecto.CodigoSapTPR : false,
                    ColorFondo = item.EPA == true ? Constantes.ValoresPorDefecto.ColorFondoSojaEPA : (item.MaterialColorFondo ?? item.CargaCupoColorFondo),
                    ColorTexto = item.EPA == true ? Constantes.ValoresPorDefecto.ColorTextoSojaEPA : (item.MaterialColorTexto ?? item.CargaCupoColorTexto),
                    EsDemorado = item.TipoCalle == TipoCalle.NoGranos && item.EsDemorado
                };

                resultado.Add(callePorRecorrido);
            }
            return resultado;
        }

        public CalleDto ObtenerSiguienteCalle(int materialId)
        {
            var calle = repositorio.ObtenerConsultaEscalar(new ObtenerSiguienteCalle(TipoCalle.PreCalado, materialId));
            return calle != null ? conversor.Convertir<Calle, CalleDto>(calle) : null;
        }

        public CalleDto CalcularCalle(TipoCalle tipoCalle, TipoCalidad tipoCalidad, int materialId, int centroId)
        {
            var calle = administradorDeCalles.AsignarCalle(tipoCalle, materialId, tipoCalidad, centroId);
            return calle != null ? conversor.Convertir<Calle, CalleDto>(calle) : null;
        }

        public bool EsPuestoFullAutomatizado(int puestoId)
        {
            return repositorio.ObtenerProyeccion<PuestoDeTrabajo, bool>(x => x.Id == puestoId, x => x.AutomatizadoFull && !x.PausaAutoFull);
        }

        public ChoferDto ObtenerChoferPorNumeroDocumento(string numeroDocumento)
        {
            return Obtener<Chofer, ChoferDto>(x => x.NumeroDeDocumento.Equals(numeroDocumento));
        }

        public TipoComercialDto ObtenerTipoComercialPorCodigoSap(string codigoSap)
        {
            return Obtener<TipoComercial, TipoComercialDto>(x => x.CodigoSap.Equals(codigoSap));
        }

        public string ObtenerMotivoDemoraRecorrido(int id)
        {
            return repositorio.ObtenerProyeccion<Recorrido, string>(x => x.Id == id, x => x.MotivoDemora);
        }

        public bool? ObtenerSiEsCPEporWf(Guid instanceId)
        {
            return repositorio.ObtenerProyeccion<Recorrido, bool?>(x => x.InstanciaWorkflow == instanceId, x => x.Vehiculo.CartaPorte.Cpe);
        }

        public IList<RamalFerroviarioDto> ListarRamalFerroviario()
        {
            return Listar<RamalFerroviario, RamalFerroviarioDto>(x => !x.Deshabilitada);
        }

        public bool ValidarCPERedespacho(string numero, int centroId, string workflowCodigo, int tipoVehiculo, bool consultactg = false)
        {
            bool resultado = false;
            try
            {
                var workflow = repositorio.Obtener<Workflow>(x => x.Codigo == workflowCodigo);

                if (tipoVehiculo == (int)TipoVehiculo.Tren && !consultactg)
                {
                    #region FerroviarioCPE

                    var numeroOperativo = Convert.ToInt64(numero);
                    var cartaPortesFerroviario = repositorio.Listar<CartaPorte>(x => x.NumeroOperativo == numeroOperativo);

                    foreach (var cartaPorteItem in cartaPortesFerroviario)
                    {
                        var cp =
                        repositorio.ObtenerMayor<Recorrido, DateTime, CartaPorte>(
                            x =>
                            x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.NumeroDocumentoIngreso == cartaPorteItem.NroCartaPorte &&
                            x.Centro.Id != centroId && x.Workflow.TipoDeWorkflow != workflow.TipoDeWorkflow && !x.Rechazado &&
                            x.Terminado, x => x.FechaInicio, x => x.Vehiculo.CartaPorte);

                        if (cp != null)
                        {
                            resultado = true;
                            break;
                        }
                    }

                    #endregion FerroviarioCPE
                }
                else
                {
                    var cp =
                        repositorio.ObtenerMayor<Recorrido, DateTime, CartaPorte>(
                            x =>
                            x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.NumeroDocumentoIngreso == numero &&
                            x.Centro.Id != centroId && x.Workflow.TipoDeWorkflow != workflow.TipoDeWorkflow && !x.Rechazado &&
                            x.Terminado, x => x.FechaInicio, x => x.Vehiculo.CartaPorte);

                    if (cp != null)
                    {
                        resultado = true;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo validar la carta de porte : {0} si es redespacho.", numero);
                return resultado;
            }

            return resultado;
        }

        public IList<MotivoInactividadDto> ListarMotivosInactividad()
        {
            return Listar<MotivoInactividad, MotivoInactividadDto>();
        }

        public RegistroInactividadDto ObtenerRegistroInactividad(int id)
        {
            var registroInactividad = Obtener<RegistroInactividad, RegistroInactividadDto>(id);

            return registroInactividad;
        }

        public RegistroInactividadDto ObtenerUltimoRegistroInactividadPorUsuario(string usuario)
        {
            var registroInactividad = new RegistroInactividad();
            registroInactividad = repositorio.ObtenerMayor<RegistroInactividad, int>(x => x.Usuario == usuario, x => x.Id);
            return conversor.Convertir<RegistroInactividad, RegistroInactividadDto>(registroInactividad);
        }

        public ControlRecorridoDto ObtenerUltimoCaladoPorPuestoDeTrabajo(int idPuesto)
        {
            var controlRecorrido = repositorio.ObtenerMayor<ControlRecorrido, int>(x => x.PuestoDeTrabajo.Id == idPuesto && x.ActividadXaml == "Calado", x => x.Id);

            return conversor.Convertir<ControlRecorrido, ControlRecorridoDto>(controlRecorrido);
        }

        public EntidadTipoDeActividadDto ObtenerEntidadActividadPorCodigos(string codigoEntidad, string codigoTipoActividad)
        {
            return Obtener<EntidadTipoDeActividad, EntidadTipoDeActividadDto>(x => x.Entidad.Codigo == codigoEntidad && x.TipoDeActividad.Codigo == codigoTipoActividad);
        }

        public IList<EntidadTipoDeActividadDto> ListarActividadesPorEntidad(string codigoEntidad)
        {
            return Listar<EntidadTipoDeActividad, EntidadTipoDeActividadDto>(x => x.Entidad.Codigo == codigoEntidad);
        }

        public MonitorCPECacheadaResultadoDto ListarCPEsCacheadas(MonitorCPECacheadaFiltroDto filtro, Paginacion paginacion)
        {
            var consulta = new MonitorCPEsCacheadasConsulta(filtro, paginacion);
            var resultado = new MonitorCPECacheadaResultadoDto
            {
                MonitorCPECacheadaListado = repositorio.ListarConsultaPaginada(consulta),
                CamionesPendientes = filtro.CamionesPendientes,
                FechaUltimaEjecucion = filtro.FechaEjecucionCacheoCPE,
                ErrorCacheoAfipCPE = filtro.ErrorCacheoAfipCPE
            };
            return resultado;
        }

        public List<MaterialDto> ListarMaterialGranosConCodigoONCCA()
        {
            var result = Listar<Material, MaterialDto>(x => !x.CodigoONCCA.Equals(null) && x.EsGrano).ToList();
            return result;
        }

        public CartaPorteElectronicaDto ObtenerCartaPorteElectronica(int id)
        {
            var result = Obtener<CartaPorteElectronica, CartaPorteElectronicaDto>(x => x.Id == id);
            return result;
        }

        public Resultado ActualizarFechaEstadoCacheadoCPECentro(int id, string mensaje)
        {
            var resultado = new Resultado();
            var centro = repositorio.Obtener<Centro>(x => x.Id == id);

            centro.FechaEjecucionCacheoCPE = DateTime.Now;
            centro.ErrorCacheoAfipCPE = mensaje;
            repositorio.GuardarCambios();
            return resultado;
        }

        public string ObtenerDispositivoBarreraEntrada(int puestoId)
        {
            return repositorio.ObtenerProyeccion<PuestoDeTrabajo, string>(
                x => x.Id == puestoId,
                x => x.Entrada);
        }

        public string ObtenerPuestoDeLogLecturaDeTarjeta(string patente, int? analisisDeCalidadId)
        {
            var fechaCalado = repositorio.ObtenerProyeccion<AnalisisDeCalidad, DateTime?>(x => x.Id == analisisDeCalidadId,
                                                  x => x.Calado.FechaCreacion);

            var fechaFinal = fechaCalado;
            var fechaInicio = fechaCalado?.AddHours(-1);
            var result = repositorio.ObtenerMayor<LogLecturaDeTarjeta, DateTime, string>(
                x => (x.Patente == patente && (x.Fecha > fechaInicio && x.Fecha <= fechaFinal)),
                x => x.Fecha,
                x => x.PuestoDeTrabajo.NombrePuesto);
            return result;
        }

        public ConfiguracionGeneralDto ObtenerConfiguracionGeneral(string pantalla, string nombre, int? centroId = null)
        {
            return Obtener<ConfiguracionGeneral, ConfiguracionGeneralDto>(x => x.Pantalla == pantalla && x.Nombre == nombre && (centroId.HasValue ? x.CentroId == centroId : x.CentroId == null));
        }

        public List<ConfiguracionGeneralDto> ListarConfiguracionesGenerales(string pantalla, int? centroId = null)
        {
            return Listar<ConfiguracionGeneral, ConfiguracionGeneralDto>(x => x.Pantalla == pantalla && (centroId.HasValue ? x.CentroId == centroId : x.CentroId == null)).ToList();
        }

        public List<ConfiguracionGeneralDto> ListarConfiguracionesGeneralesPorNombres(string pantalla, List<string> nombres, int? centroId = null)
        {
            return Listar<ConfiguracionGeneral, ConfiguracionGeneralDto>(x => x.Pantalla == pantalla && nombres.Contains(x.Nombre) && (centroId.HasValue ? x.CentroId == centroId : x.CentroId == null)).ToList();
        }

        public ListaPaginada<VisualizacionBarreraDto> ListarPaginadoVisualizacionBarrera(int centroId, Paginacion paginacion)
        {
            Expression<Func<VisualizacionBarrera, bool>> expresionFiltro =
                x =>
                 x.CentroId == centroId;

            return Listar<VisualizacionBarrera, VisualizacionBarreraDto>(expresionFiltro, paginacion);
        }

        public IList<SensorBarreraDto> ListarSensoresBarreras(int grupoId)
        {
            return Listar<SensorBarrera, SensorBarreraDto>(x => x.VisualizacionBarrera.Id == grupoId);
        }

        public VisualizacionBarreraDto ObtenerVisualizacionBarrera(int id)
        {
            return Obtener<VisualizacionBarrera, VisualizacionBarreraDto>(id);
        }

        public IList<VisualizacionBarreraDto> ObtenerGruposBarrerasPorUsuario(string usuario, int centroId)
        {
            var usuarioDto = Obtener<Usuario, UsuarioDto>(x => x.NombreUsuario.Equals(usuario));
            if (usuarioDto != null)
            {
                var lista = (Listar<VisualizacionBarrera, VisualizacionBarreraDto>()).Where(w => !w.Deshabilitada && w.Visible && w.CentroId == centroId).ToList();
                return lista.Where(w => usuarioDto.RolesAsociados.Any(a => a.Id == w.RolId)).ToList();
            }

            return new List<VisualizacionBarreraDto>();
        }

        public int ObtenerCantidadBarrerasPorUsuario(string usuario, int centroId)
        {
            int resultado = 0;

            try
            {
                var listaGrupos = ObtenerGruposBarrerasPorUsuario(usuario, centroId);
                foreach (var grupo in listaGrupos)
                {
                    var sensores = ListarSensoresBarreras(grupo.Id);
                    resultado += sensores.Count();
                }
            }
            catch (Exception)
            {
            }

            return resultado;
        }

        public IList<SensorBarreraDto> ListarSensoresBarrerasActivos()
        {
            return Listar<SensorBarrera, SensorBarreraDto>(x => !x.VisualizacionBarrera.Deshabilitada);
        }

        public CargaDeCupoDto ObtenerCupoPorCupoSap(string cupo)
        {
            var result = repositorio.ObtenerMayor<CargaDeCupo, int>(
                x => x.Cupo == cupo,
                x => x.Id);
            return conversor.Convertir<CargaDeCupo, CargaDeCupoDto>(result);
        }

        public ConfigSensoresDto ObtenerConfiguracionSensores(int id)
        {
            return Obtener<ConfigSensores, ConfigSensoresDto>(id);
        }

        public ListaPaginada<ConfigSensoresDto> ListarPaginadoConfigSensores(string filtro, Paginacion paginacion, int centroId)
        {
            Expression<Func<ConfigSensores, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                int filtroInt;
                if (!int.TryParse(filtro, out filtroInt))
                {
                    filtroInt = int.MaxValue;
                }

                filtro = filtro.Trim();
                expresionFiltro =
                    x =>
                    x.Descripcion.Equals(filtroInt) || x.Descripcion.Contains(filtro) || x.Id.Equals(filtroInt);
            }

            return Listar<ConfigSensores, ConfigSensoresDto>(expresionFiltro, paginacion);
        }

        public IList<ConfigSensoresDto> ListarConfiguracionSensores(int centroId)
        {
            return Listar<ConfigSensores, ConfigSensoresDto>(x => x.Centro.Id == centroId);
        }

        public IList<VisualizacionBarreraDto> ObtenerGruposBarrerasPorCentro(int centroId)
        {
            return (Listar<VisualizacionBarrera, VisualizacionBarreraDto>()).Where(w => !w.Deshabilitada && w.CentroId == centroId).ToList();
        }

        public List<EficienciaCaladoValoresDto> ObtenerEficienciaCalado(DateTime desde, DateTime hasta)
        {
            return repositorio.ListarConsulta(new ListarEficienciaCalado(desde, hasta));
        }

        public List<EficienciaCaladoValoresDto> ListarCallesCalado(int centroId)
        {
            var callesCalado = Listar<Calle, CalleDto>(x => x.CentroId == centroId && x.TipoCalle == TipoCalle.Calado && !x.Deshabilitada);
            var nombresCallesCalado = callesCalado.Select(x => x.Nombre).ToList();
            var puestoDeTrabajoCalado = Listar<PuestoDeTrabajo, PuestoDeTrabajoDto>(p => nombresCallesCalado.Any(c => p.NombrePuesto.StartsWith(c)));
            return puestoDeTrabajoCalado.Select(x => new EficienciaCaladoValoresDto
            {
                Id = x.Id,
                Cantidad = 0,
                Nombre = x.NombrePuesto.Substring((x.NombrePuesto.IndexOf("-")) + 1, x.NombrePuesto.Length - (x.NombrePuesto.IndexOf("-")) - 1).Trim()
            }).ToList();
        }

        public EficienciaCaladoValoresDto ObtenerEficienciaCalle(int id, int centroId)
        {
            var configuracion = ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EficienciaCalado, Constantes.ConfiguracionGeneral.EficienciaCalado.EficienciaCalles, centroId);
            var configuracionCalles = configuracion.Valor.FromJson<List<EficienciaCaladoValoresDto>>();
            return configuracionCalles.Where(c => c.Id == id).FirstOrDefault();
        }

        public int ObtenerCantidadPendientesPorCalar(int centroId)
        {
            var currentDate = DateTime.Now;
            currentDate = currentDate.Date.Add(TimeSpan.Parse("00:00:00.000"));
            return repositorio.Contar<CallePorRecorrido>(x => x.Calle.TipoCalle == TipoCalle.PreCalado && x.Calle.CentroId == centroId && x.FechaIngeso >= currentDate && x.FechaEgreso == null);
        }

        public FotoDto ObtenerFotoSustentable(int centroId, string numeroDocumento, string actividad)
        {
            var cartaPorte = ObtenerUltimo<CartaPorte, CartaPorteDto>(x => x.NroCartaPorte == numeroDocumento && x.CentroDestino.Id == centroId, x => x.Id);
            if (cartaPorte != null && cartaPorte.FotoRutaDestino != null)
            {
                var path = cartaPorte.FotoRutaDestino;
                var index = path.IndexOf("-Mesa");
                if (index < 0)
                {
                    index = path.LastIndexOf("-");
                }
                var pathSustentable = path.Substring(0, index) + "-sustentable.jpeg";
                if (File.Exists(pathSustentable))
                {
                    var fecha = File.GetCreationTime(pathSustentable);
                    return ObtenerImagen(pathSustentable, fecha, actividad);
                }

                pathSustentable = path.Substring(0, index) + "-sustentable.png";
                if (File.Exists(pathSustentable))
                {
                    var fecha = File.GetCreationTime(pathSustentable);
                    return ObtenerImagen(pathSustentable, fecha, actividad);
                }
            }
            return null;
        }

        private FotoDto ObtenerImagen(string path, DateTime fecha, string actividad)
        {
            FotoDto result = null;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = null;
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);

                using (Image image = Image.FromStream(fs))
                {
                    var documentcontentsSmall = image.ResizeImage(50, 28);

                    result = new FotoDto
                    {
                        Actividad = actividad,
                        Foto = buffer,
                        FotoChica = documentcontentsSmall,
                        Fecha = fecha.ToString("dd/MM/yyyy HH:mm"),
                        Extension = Path.GetExtension(path)
                    };
                }
            }
            return result;
        }

        public IList<LecturaPuestoDeTrabajoDto> ObtenerLogLecturasPorTarjeta(string tarjeta)
        {
            var fecha = DateTime.Now;
            var hora = new TimeSpan(0);
            fecha = fecha.Date.Add(hora);
            return repositorio.Listar<LogLecturaDeTarjeta, LecturaPuestoDeTrabajoDto>(x => new LecturaPuestoDeTrabajoDto
            {
                PuestoDeTrabajoId = x.PuestoDeTrabajo.Id,
                Patente = x.Patente,
                PatenteLeida = x.PatenteLeida
            }, x => x.Tarjeta == tarjeta && x.Fecha >= fecha);
        }

        public IList<SensorBarreraDto> ListarSensoresBarrerasActivosPorNombreDePC(string nombrePc)
        {
            var grupoId = repositorio.ObtenerProyeccion<PuestoDeTrabajo, int>(x => x.VisualizacionBarrera != null && x.NombrePc == nombrePc, x => x.VisualizacionBarrera.Id);
            return Listar<SensorBarrera, SensorBarreraDto>(x => x.VisualizacionBarrera.Id == grupoId);
        }

        public bool AvanzaCpe(int centroId)
        {
            return repositorio.ObtenerProyeccion<Centro, bool>(x => x.Id == centroId, x => x.AvanzaCpe);
        }

        public FotoDto ObtenerFotoDescargada(int centroId, string numeroDocumento, string actividad, bool esSustentable)
        {
            FotoDto result = null;
            var cartaPorte = ObtenerUltimo<CartaPorte, CartaPorteDto>(x => x.NroCartaPorte == numeroDocumento && x.CentroDestino.Id == centroId, x => x.Id);
            if (cartaPorte != null && cartaPorte.FotoRutaDestino != null)
            {
                var path = cartaPorte.FotoRutaDestino;
                var index = path.IndexOf("-Mesa");
                if (index < 0)
                {
                    index = path.LastIndexOf("-");
                }
                var pathSustentable = esSustentable ? path.Substring(0, index) + "-descargado-sustentable.jpeg" : path.Substring(0, index) + "-descargado.jpeg";
                if (File.Exists(pathSustentable))
                {
                    var fecha = File.GetCreationTime(pathSustentable);
                    result = ObtenerImagen(pathSustentable, fecha, actividad);
                }
            }
            return result;
        }

        public bool ValidarAltaCTGRepetida(string ctg)
        {
            var altaCTG = Obtener<AltaCTG, AltaCTGDto>(x => x.CodigoCTG.Equals(ctg));
            if (altaCTG != null)
            {
                var existenRecorridos = repositorio.Listar<Recorrido>(x => x.InstanciaWorkflow == altaCTG.WorkflowId);
                if (existenRecorridos.Count > 0 && existenRecorridos.All(q => q.Rechazado == true))
                    return false;
                else
                    return true;
            }
            return false;
        }

        public bool ValidarAltaCPERepetida(string cpe, int sucursal)
        {
            var altaCTG = Obtener<AltaCTG, AltaCTGDto>(x => x.CartaPorte.Sucursal == sucursal && x.CartaPorte.CTG == cpe);
            if (altaCTG != null)
            {
                var existenRecorridos = repositorio.Listar<Recorrido>(x => x.InstanciaWorkflow == altaCTG.WorkflowId);
                if (existenRecorridos.Count > 0 && existenRecorridos.All(q => q.Rechazado == true))
                    return false;
                else
                    return true;
            }
            return false;
        }

        public MensajeCartelLedDto ObtenerMensajeCartelLedPorCodigo(string codigo)
        {
            return Obtener<MensajeCartelLed, MensajeCartelLedDto>(x => x.Codigo == codigo);
        }

        public IList<MuestraDeInaseDto> ObtenerLotesMuestrasInase()
        {
            return Listar<MuestraDeInase, MuestraDeInaseDto>(x => !x.MuestraEnviada && !x.Recorrido.Rechazado);
        }

        public IList<ConfiguracionGeneralDto> ObtenerConfiguracionMailInase(int centroId)
        {
            return Listar<ConfiguracionGeneral, ConfiguracionGeneralDto>(x => x.Pantalla == "MuestraInase" && (x.CentroId == null || x.CentroId == centroId));
        }

        public List<MuestraDeInaseDto> ObtenerMuestrasInaseParaArchivo()
        {
            return repositorio.ListarConsulta(new ListarMuestraInaseParaArchivoConsulta(firmaProvider.ObtenerFirmaSinLogo().CodigoSAP));
        }

        public Resultado ActualizarDispositivoLog(string codigo, string nombre, string valor, bool limpiarLog)
        {
            var resultado = new Resultado();
            try
            {
                log.Info("Se ejecutara el servicio ActualizarDispositivoLog " + codigo + "-" + nombre + "-" + valor);
                var logDispositivo = repositorio.Obtener<LogDispositivo>(x => x.CodigoDispositivo == codigo && x.NombreLog == nombre);
                if (logDispositivo == null)
                {
                    logDispositivo = new LogDispositivo
                    {
                        Id = -1,
                        CodigoDispositivo = codigo,
                        NombreLog = nombre,
                    };
                    repositorio.Agregar(logDispositivo);
                }
                logDispositivo.Fecha = DateTime.Now;
                var valorAnterior = logDispositivo.ValorActual;

                if (limpiarLog == true)
                {
                    logDispositivo.ValorAnterior = null;
                    logDispositivo.ValorActual = null;
                }
                else if (valorAnterior != valor)
                {
                    logDispositivo.ValorAnterior = valorAnterior;
                    logDispositivo.ValorActual = valor;
                }

                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                resultado.Error("Hubo un error", e.Message);
            }
            return resultado;
        }

        public List<string> ObtenerGruposBarreraEnUso(List<string> codigos)
        {
            return repositorio.Listar<PuestoDeTrabajo, string>(x => x.GrupoBarreraCodigo, x => codigos.Contains(x.GrupoBarreraCodigo)).ToList();
        }

        public List<LogDispositivoDto> ObtenerLogDispositivos(List<string> codigos)
        {
            var respuesta = repositorio.ListarNoTracking<LogDispositivo, LogDispositivoDto>(x => new LogDispositivoDto
            {
                Id = x.Id,
                Fecha = x.Fecha,
                CodigoDispositivo = x.CodigoDispositivo,
                NombreLog = x.NombreLog,
                ValorAnterior = x.ValorAnterior,
                ValorActual = x.ValorActual,
            }, x => codigos.Contains(x.CodigoDispositivo)).ToList();

            return respuesta;
        }

        public ListaPaginada<LoteInaseDto> ListarPaginadoLoteInase(FiltroLoteInaseDto filtro, Paginacion paginacion)
        {
            Expression<Func<LoteInase, bool>> expresionFiltro;
            filtro.FechaDesde = filtro.FechaDesde ?? new DateTime(1970, 1, 1);
            filtro.FechaHasta = filtro.FechaHasta ?? DateTime.MaxValue;
            if (filtro.LoteId > 0)
            {
                expresionFiltro = (x => x.Id == filtro.LoteId);
            }
            else if (filtro.NroLote != null)
            {
                expresionFiltro = (x => x.NumeroDeLote == filtro.NroLote);
            }
            else
            {
                expresionFiltro = (x =>
                    x.Fecha <= filtro.FechaHasta && x.Fecha >= filtro.FechaDesde
                    && x.Centro.Id == filtro.CentroId);
            }

            return Listar<LoteInase, LoteInaseDto>(expresionFiltro, paginacion);
        }

        public ListaPaginada<MuestraDeInaseDto> ListarMuestrasPorLoteInase(int loteId, Paginacion paginacion)
        {
            return repositorio.ListarConsultaPaginada(new ListarMuestraDeInaseParaImpresionConsulta(firmaProvider.ObtenerFirmaSinLogo().CodigoSAP, loteId, paginacion));
        }

        public string ObtenerNumeroLoteInase(int loteId)
        {
            return repositorio.ObtenerProyeccion<LoteInase, string>(x => x.Id == loteId, x => x.NumeroDeLote);
        }

        public LoteInaseDto ObtenerLoteInaseParaImpresion(int loteId)
        {
            var lotedto = repositorio.ObtenerProyeccion<LoteInase, LoteInaseDto>(x => x.Id == loteId, lote => new LoteInaseDto
            {
                Id = lote.Id,
                CentroId = lote.Centro.Id,
                NumeroDeLote = lote.NumeroDeLote,
            });
            lotedto.Muestras = repositorio.ListarConsulta(new ListarMuestraInaseParaArchivoConsulta(firmaProvider.ObtenerFirmaSinLogo().CodigoSAP, loteId));
            return lotedto;
        }

        public bool EsCupoReingresado(string cupo, string nroCartaPorte, int centroId)
        {
            return repositorio.Existe<CargaDeCupo>(x => x.Cupo == cupo && x.CTG == nroCartaPorte && x.Centro.Id == centroId && !x.SinCupo && x.Recorrido.Rechazado && x.Recorrido.Terminado);
        }

        public CargaDeCupoDto ObtenerCupoReingresado(string cupo, string nroCartaPorte, int centroId)
        {
            return ObtenerUltimo<CargaDeCupo, CargaDeCupoDto>(x => x.Cupo == cupo && x.CTG == nroCartaPorte && x.Centro.Id == centroId && !x.SinCupo && x.Recorrido.Rechazado && x.Recorrido.Terminado, x => x.Id);
        }

        public List<SensorBarreraDto> ListarSensoresBarrerasHidraulicasActivos()
        {
            var puestosDeTrabajo = repositorio.Listar<PuestosDeCargaDescarga, PuestoDeTrabajo>(x => x.PuestoDeTrabajo, x => x.PuestoDeTrabajo != null);
            var hidraulicas = puestosDeTrabajo.Where(x => x.VisualizacionBarrera != null).Select(x => x.VisualizacionBarrera.Id).ToList();
            var barrerasActivas = puestosDeTrabajo.Where(x => x.VisualizacionBarrera != null && hidraulicas.Contains(x.VisualizacionBarrera.Id) && !x.VisualizacionBarrera.Deshabilitada).Select(x => x.VisualizacionBarrera.Id).ToList();
            var sensores = Listar<SensorBarrera, SensorBarreraDto>(x => barrerasActivas.Contains(x.VisualizacionBarrera.Id)).ToList();
            foreach (var sensor in sensores)
            {
                sensor.PuestoDeTrabajoId = puestosDeTrabajo.FirstOrDefault(x => x.VisualizacionBarrera != null && x.VisualizacionBarrera.Id == sensor.VisualizacionBarrera.Id)?.Id;
            }
            return sensores;
        }

        public CaladoDto ObtenerCaladoPorId(int id)
        {
            return Obtener<Calado, CaladoDto>(x => x.Id == id);
        }

        public CargaDeCupoDto ObtenerCargaDeCupoPorGuid(Guid instanceId)
        {
            return Obtener<CargaDeCupo, CargaDeCupoDto>(x => x.Recorrido.InstanciaWorkflow == instanceId);
        }

        public List<LlamadoAutomaticoHidraulicaDto> ListarHidraulicasPorEstado(EstadoHidraulica estado)
        {
            return Listar<LlamadoAutomaticoHidraulica, LlamadoAutomaticoHidraulicaDto>(x => x.Estado == estado).ToList();
        }

        public ConfiguracionCalleHidraulicaDto ObtenerConfiguracionCalleHidraulicaPorSensorCamaraALPR(string codigoSensor)
        {
            return Obtener<ConfiguracionCalleHidraulica, ConfiguracionCalleHidraulicaDto>(x => x.CodigoSensorCamaraALPR == codigoSensor);
        }

        public List<LlamadoAutomaticoHidraulicaDto> ListarHidraulicasAutomatizadas()
        {
            return Listar<LlamadoAutomaticoHidraulica, LlamadoAutomaticoHidraulicaDto>().ToList();
        }

        public PuestosDeCargaDescargaDto ObtenerHidraulicaPorSensorBajada(string codigoSensorBajada)
        {
            return Obtener<PuestosDeCargaDescarga, PuestosDeCargaDescargaDto>(x => x.CodigoSensorBajada == codigoSensorBajada);
        }

        public RecorridoDto ObtenerRecorridoActivoPorPatente(string patente)
        {
            return Obtener<Recorrido, RecorridoDto>(x => x.Patente == patente && !x.Terminado);
        }

        public ConfiguracionCalleHidraulicaDto ObtenerConfiguracionCalleHidraulicaPorSensorCirculacion(string codigoSensor)
        {
            return Obtener<ConfiguracionCalleHidraulica, ConfiguracionCalleHidraulicaDto>(x => x.CodigoSensorCirculacion == codigoSensor);
        }

        public PuestosDeCargaDescargaDto ObtenerPuestoDeCargaDescargaPorPuestoId(int puestoDeTrabajoId)
        {
            var resultado = ObtenerPrimero<PuestosDeCargaDescarga, PuestosDeCargaDescargaDto>(x => x.PuestoDeTrabajo.Id == puestoDeTrabajoId);
            return resultado;
        }

        public ListaPaginada<ConfiguracionCalleHidraulicaDto> ListarPaginadoCalleHidraulica(Paginacion paginacion)
        {
            Expression<Func<ConfiguracionCalleHidraulica, bool>> expresionFiltro = null;

            return Listar<ConfiguracionCalleHidraulica, ConfiguracionCalleHidraulicaDto>(expresionFiltro, paginacion);
        }

        public ConfiguracionCalleHidraulicaDto ObtenerCalleHidraulica(int id)
        {
            return Obtener<ConfiguracionCalleHidraulica, ConfiguracionCalleHidraulicaDto>(id);
        }

        public bool ExisteConfirmacionCargaDescargaDeRecorrido(int recorridoId)
        {
            return repositorio.Existe<ConfirmacionCargaDescarga>(x => x.Recorrido.Id == recorridoId && !x.Confirmado && x.PendienteConfirmacion);
        }

        public IList<CalleDto> ListarCallesPorTipo(TipoCalle tipo)
        {
            return Listar<Calle, CalleDto>(x => x.TipoCalle == tipo);
        }

        public string ObtenerCodigoMensaje(int calleCaladoId)
        {
            return repositorio.ObtenerPrimero<MensajeCartelLedCalador>(x => x.Calle.Id == calleCaladoId).MensajeCartelLed.Codigo;
        }

        public int ObtenerOrdenCircular(string codigo)
        {
            var lista = Listar<MensajeCartelLed, MensajeCartelLedDto>(x => x.Codigo == codigo);

            return lista.Last().Orden;
        }

        public int ObtenerCantidadCamionesLlamadosEnCallePreBalanza(int callePlayaInternaId)
        {
            var callePreBalanzaPlayaInternaList = repositorio.Listar<CallePreBalanzaPlayaInterna>(x => x.CallePlayaInterna.Id == callePlayaInternaId)
                .Select(q => q.CallePreBalanzaId).ToList();

            var cantidadCamiones = repositorio.Contar<CallePorRecorrido>(q => callePreBalanzaPlayaInternaList.Contains(q.Calle.Id) && q.FechaEgreso.Equals(null));
            return cantidadCamiones;
        }

        public List<CalleDto> ListarCallesPreBalanzaPorCallePlayaInternaId(int callePlayaInternaId)
        {
            var callePreBalanzaIdList = repositorio.Listar<CallePreBalanzaPlayaInterna>(x => x.CallePlayaInterna.Id == callePlayaInternaId)
                .Select(q => q.CallePreBalanzaId).ToList();
            return Listar<Calle, CalleDto>(x => callePreBalanzaIdList.Contains(x.Id)).ToList();
        }

        public CantidadPrecaladoCircularHelper ContarCallesBloqueadas(int? calleCaladoId)
        {
            var listaPreCaladoCircular = Listar<Calle, CalleDto>(x => (x.TipoCalle == TipoCalle.PreCalado || x.TipoCalle == TipoCalle.Circular));

            var cantidadPrecaladoCircularHelper = new CantidadPrecaladoCircularHelper();
            cantidadPrecaladoCircularHelper.CantidadPrecalado = listaPreCaladoCircular.Count(x => x.TipoCalle == TipoCalle.PreCalado && x.Bloqueada && x.CalleCaladoId == calleCaladoId);
            cantidadPrecaladoCircularHelper.CantidadCircular = listaPreCaladoCircular.Count(x => x.TipoCalle == TipoCalle.Circular && x.Bloqueada && x.CalleCaladoId == calleCaladoId);
            cantidadPrecaladoCircularHelper.CantidadTotal = listaPreCaladoCircular.Count(x => (x.TipoCalle == TipoCalle.PreCalado || x.TipoCalle == TipoCalle.Circular) && x.Bloqueada && x.CalleCaladoId == calleCaladoId);

            return cantidadPrecaladoCircularHelper;
        }

        public ListaPaginada<ExcepcionAlControlProveedorDto> ListarExcepcionesAlControlProveedor(string filtro, Paginacion paginacion)
        {
            Expression<Func<ExcepcionAlControlProveedor, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.Trim();
                expresionFiltro =
                    x => x.Motivo != MotivoExcepcionAlControl.B && (x.Proveedor.RazonSocial.Contains(filtro) ||
                                                                       x.Proveedor.Cuil.Contains(filtro) ||
                                                                       x.Material.Descripcion.Contains(filtro) ||
                                                                       x.Material.DescripcionCorta.Contains(filtro) ||
                                                                       x.Material.CodigoSAP.Contains(filtro) ||
                                                                       x.Centro.Descripcion.Contains(filtro) ||
                                                                       (x.CentroDestino != null && x.CentroDestino.Descripcion.Contains(filtro)) ||
                                                                       (x.ClienteDestino != null && x.ClienteDestino.Descripcion.Contains(filtro)));
            }
            else
            {
                expresionFiltro = x => x.Motivo != MotivoExcepcionAlControl.B;
            }

            return Listar<ExcepcionAlControlProveedor, ExcepcionAlControlProveedorDto>(expresionFiltro, paginacion);
        }

        public ExcepcionAlControlProveedorDto ObtenerExcepcionAlControlProveedor(int id)
        {
            return Obtener<ExcepcionAlControlProveedor, ExcepcionAlControlProveedorDto>(id);
        }

        public bool BuscarExcepcionAlControlProveedor(int materialId, int proveedorId, int centroId, DateTime fecha, int? centroDestinoId, int? clienteDestinoId)
        {
            return repositorio.Existe<ExcepcionAlControlProveedor>(x =>
                                                          x.Material.Id == materialId &&
                                                          x.Proveedor.Id == proveedorId &&
                                                          x.Centro.Id == centroId &&
                                                          x.FechaDesde <= fecha && x.FechaHasta >= fecha &&
                                                          x.Motivo != MotivoExcepcionAlControl.B &&
                                                          ((centroDestinoId.HasValue && x.CentroDestino.Id == centroDestinoId.Value) || (clienteDestinoId.HasValue && x.ClienteDestino.Id == clienteDestinoId.Value) || (!centroDestinoId.HasValue && !clienteDestinoId.HasValue))
                                                          );
        }

        public bool ValidacionAutomaticaCtgDG(int centroId)
        {
            var centro = repositorio.Obtener<Centro>(centroId);
            return centro != null && centro.ValidarAutomaticamenteCTGDG;
        }

        public List<DomicilioDto> ListarDomicilios()
        {
            return Listar<Domicilio, DomicilioDto>().ToList();
        }

        public CartaPorteDerivadoGranarioDto ObtenerCartaPorteDerivadoGranarioPorGuid(Guid instanceId)
        {
            return Obtener<CartaPorteDerivadoGranario, CartaPorteDerivadoGranarioDto>(x => x.Recorrido.InstanciaWorkflow == instanceId);
        }

        public IList<VideoCamaraDto> ListarVideoCamarasPuesto(int idPuesto)
        {
            return Listar<VideoCamara, VideoCamaraDto>(x => x.PuestoDeTrabajo.Id == idPuesto);
        }

        public DatosDerivadoGranarioDto ObtenerDatoDerivadoGranarioPorRecorridoTipoDocumento(int recorridoId, TipoDocumentoIngreso tipoDocumentoIngreso)
        {
            DatosDerivadoGranarioDto datosDerivadosGranario = null;

            switch (tipoDocumentoIngreso)
            {
                case TipoDocumentoIngreso.OrdenDeDescargaFason:
                    datosDerivadosGranario =
                        repositorio.ObtenerProyeccion<OrdenDeDescargaFason, DatosDerivadoGranarioDto>(
                        r =>
                        r.Recorrido.Id == recorridoId &&
                        r.Recorrido.TipoDocumentoIngreso == tipoDocumentoIngreso,
                        r => new DatosDerivadoGranarioDto
                        {
                            NroCTG = r.NumeroCTG,
                            NroOrden = r.NroOrden,
                            Sucursal = r.Sucursal
                        });
                    break;

                case TipoDocumentoIngreso.OrdenCargaInternaFason:
                    datosDerivadosGranario =
                        repositorio.ObtenerProyeccion<CartaPorteDerivadoGranario, DatosDerivadoGranarioDto>(
                        r =>
                        r.Recorrido.Id == recorridoId &&
                        r.Recorrido.TipoDocumentoIngreso == tipoDocumentoIngreso,
                        r => new DatosDerivadoGranarioDto
                        {
                            NroCTG = r.NroCTG,
                            NroOrden = r.NroOrden,
                            Sucursal = r.Sucursal
                        });
                    break;
            }

            return datosDerivadosGranario;
        }

        public ClienteDto ObtenerClientePorCuit(string cuit)
        {
            return Obtener<Cliente, ClienteDto>(x => x.Cuit.Equals(cuit));
        }

        public CartaPorteDerivadoGranarioDto ObtenerCartaPorteDerivadoGranarioPorCTG(string nroCTG)
        {
            return Obtener<CartaPorteDerivadoGranario, CartaPorteDerivadoGranarioDto>(x => x.NroCTG == nroCTG);
        }

        public MaterialDto ObtenerMaterialDerivadoGranario(short codigoPadre, short codigoDreivadoGranario)
        {
            return Obtener<Material, MaterialDto>(x => x.CodigoEspecie == codigoDreivadoGranario && x.CodigoGranoPadre == codigoPadre && x.EsDerivadoGranario);
        }

        public DomicilioDto ObtenerDomicilioDG(int plantaDG)
        {
            int? domicilioId;
            try
            {
                domicilioId = Obtener<Centro, CentroDto>(x => x.PlantaDG == plantaDG)?.DomicilioId;
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo obtener el domicilio con número de planta {0}", plantaDG);
                throw;
            }
            return domicilioId != null ? Obtener<Domicilio, DomicilioDto>(x => x.Id == domicilioId) : null;
        }

        public IList<AlmacenDto> ListarAlmacenesPorMaterialYCentroEPA(int centroId, int materialId)
        {
            return Listar<Almacen, AlmacenDto>(
                f =>
                f.Centro.Id == centroId && f.Materiales.Any(x => x.Id == materialId) && f.EPA == true);
        }

        public bool EsRecorridoConEstablecimiento(Guid instanceId)
        {
            return repositorio.ObtenerProyeccion<Recorrido, bool>(x => x.InstanciaWorkflow == instanceId, f => f.Establecimiento != null);
        }

        public int? EsRecorridoSojaEPAConAlmacen(Guid instanceId)
        {
            return repositorio.ObtenerProyeccion<Recorrido, int>(x => x.InstanciaWorkflow == instanceId && x.Establecimiento != null && x.Establecimiento.EPA == true, x => x.Almacen.Id);
        }

        public bool EsRecorridoSojaEPA(Guid instanceId)
        {
            return repositorio.Existe<Recorrido>(x => x.InstanciaWorkflow == instanceId && x.Establecimiento != null && x.Establecimiento.EPA == true);
        }

        public bool RecorridoRepetidoEnElDia(string patente)
        {
            DateTime now = DateTime.Now;
            DateTime yesterday = now.AddHours(-24);
            return repositorio.Existe<Recorrido>(x => x.Patente == patente &&
            x.Terminado &&
            x.FechaEgreso != null &&
            x.FechaEgreso > yesterday &&
            x.FechaEgreso <= now);
        }

        public List<ClienteDto> ListarClientesPorCuit(string cuit)
        {
            return Listar<Cliente, ClienteDto>(x => x.Cuit.Equals(cuit) && x.Activo).ToList();
        }

        public List<ProveedorDto> ListarProveedoresPorCuit(string cuit, TiposProveedor tipo)
        {
            return Listar<Proveedor, ProveedorDto>(x => x.Cuil == cuit
                                                         && (!tipo.PR || x.PR)
                                                         && (!tipo.AM || x.AM)
                                                         && (!tipo.CM || x.CM)
                                                         && x.Activo)
                .ToList();
        }

        public bool CaladorLleno(int calleCaladoId, int limiteFilasPrecaladoLlamadas)
        {
            var caladorLleno = false;
            var cantCalles = Listar<Calle, CalleDto>(x => x.CalleCalado.Id == calleCaladoId && x.Bloqueada).Count();
            var cantCallesCirc = Listar<Calle, CalleDto>(x => x.CalleCalado.Id == calleCaladoId && x.Bloqueada && x.TipoCalle == TipoCalle.Circular).Count();

            var cantCaladores = CaladoresActivos();
            var limiteCalles = limiteFilasPrecaladoLlamadas / cantCaladores;
            if (cantCallesCirc == 1 && cantCalles == limiteCalles - 1)
            {
                caladorLleno = true;
            }

            return caladorLleno;
        }

        public int CaladoresActivos()
        {
            var cantCaladores = Listar<Calle, CalleDto>(x => x.TipoCalle == TipoCalle.Calado && !x.Deshabilitada).Count();

            return cantCaladores;
        }

        public int ObtenerCalleInicial(Guid instanciaWorkflow)
        {
            var calleRecorrido = repositorio.ObtenerProyeccion<Recorrido, CallePorRecorrido>(x => x.InstanciaWorkflow == instanciaWorkflow, x => x.CallePorRecorridos.FirstOrDefault(w => w.Recorrido.Id == x.Id && w.FechaEgreso == null));
            return calleRecorrido.Calle.Id;
        }

        public IList<AlmacenDto> ListarAlmacenesPorMaterialFiltrado(int materialId)
        {
            return Listar<Almacen, AlmacenDto>(al => al.Materiales.Any(ma => ma.Id == materialId) && al.EstadoAutomatismo == true);
        }

        public IList<AlmacenDto> ListarAlmacenesPorMaterialSinFiltro(int materialId)
        {
            return Listar<Almacen, AlmacenDto>(al => al.Materiales.Any(ma => ma.Id == materialId));
        }

        public IList<AlmacenDto> ListarAlmacenesNoGranos()
        {
            return Listar<Almacen, AlmacenDto>(al => al.Materiales.Any(ma => ma.EsGrano == false));
        }

        public bool EstaDisponibleCalle(int calleId)
        {
            var calle = ObtenerCalle(calleId);

            if (calle == null)
                return false;

            if (calle.Deshabilitada)
                return false;

            if (calle.Bloqueada)
                return false;

            if (calle.FechaLLamada != null)
                return false;

            return true;
        }

        public IList<PuntoDeCargaDto> ListarPuntoDeCarga()
        {
            return Listar<PuntoDeCarga, PuntoDeCargaDto>();
        }

        public bool ExisteCalleConEspacioParaAsignarSegunTipoCalleYMaterial(TipoCalle tipoCalle, Guid workflowInstance)
        {
            var recorrido = repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == workflowInstance);
            var calles = repositorio.Listar<Calle>(x => x.TipoCalle == tipoCalle && x.Material.Id == recorrido.Material.Id && !x.Deshabilitada && !x.Bloqueada);
            foreach (var calle in calles)
            {
                if (repositorio.Contar<CallePorRecorrido>(x => x.Calle.Id == calle.Id && x.FechaEgreso == null) < calle.CantidadDeCamiones)
                    return true;
            }
            return false;
        }

        public bool ExisteExcepcionAlControlProveedorParaCartaPorte(int materialId, int proveedorId, int centroId, DateTime fecha, int? centroDestinoId, int? proveedorDestinoId)
        {
            return repositorio.Existe<ExcepcionAlControlProveedor>(x =>
                                                          x.Material.Id == materialId &&
                                                          x.Proveedor.Id == proveedorId &&
                                                          x.Centro.Id == centroId &&
                                                          x.FechaDesde <= fecha && x.FechaHasta >= fecha &&
                                                          x.Motivo != MotivoExcepcionAlControl.B &&
                                                          ((centroDestinoId.HasValue && x.CentroDestino.Id == centroDestinoId.Value) || (proveedorDestinoId.HasValue && x.ProveedorDestino.Id == proveedorDestinoId.Value) || (!centroDestinoId.HasValue && !proveedorDestinoId.HasValue))
                                                          );
        }

        public bool ExisteExcepcionAlControlParaCartaPorte(int materialId, int transportistaId, int centroId, DateTime fecha, int? centroDestinoId, int? proveedorDestinoId)
        {
            return repositorio.Existe<ExcepcionAlControl>(x =>
                                                          x.Material.Id == materialId &&
                                                          x.Transportista.Id == transportistaId &&
                                                          x.Centro.Id == centroId &&
                                                          x.FechaDesde <= fecha && x.FechaHasta >= fecha &&
                                                          x.Motivo != MotivoExcepcionAlControl.B &&
                                                          ((centroDestinoId.HasValue && x.CentroDestino.Id == centroDestinoId.Value) || (proveedorDestinoId.HasValue && x.ProveedorDestino.Id == proveedorDestinoId.Value) || (!centroDestinoId.HasValue && !proveedorDestinoId.HasValue))
                                                          );
        }

        public int ObtenerMaterialIdPorInstanceId(Guid instanceId)
        {
            var recorrido = repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == instanceId);
            return recorrido.Material.Id;
        }

        public ListaPaginada<ComercialDto> ListarPaginadoComerciales(string filtro, Paginacion paginacion)
        {
            Expression<Func<Comercial, bool>> expresionFiltro = null;
            if (!string.IsNullOrEmpty(filtro))
            {
                expresionFiltro = x => x.CodigoSap.Contains(filtro)
                                       || x.Descripcion.Contains(filtro);
            }
            return Listar<Comercial, ComercialDto>(expresionFiltro, paginacion);
        }

        public ComercialDto ObtenerComercial(int id)
        {
            return Obtener<Comercial, ComercialDto>(id);
        }

        public List<ProveedorDto> ListarProveedores()
        {
            return Listar<Proveedor, ProveedorDto>().ToList();
        }

        public ProveedorInfoDto ObtenerProveedorInfoPorId(int id)
        {
            return repositorio.Listar<Proveedor, ProveedorInfoDto>(x => new ProveedorInfoDto
            {
                Cuil = x.Cuil,
                Descripcion = x.Descripcion,
                Id = x.Id,
                CodigoSap = x.CodigoSap,
                RazonSocial = x.RazonSocial
            }, f => f.Activo && f.Id == id, 1).First();
        }

        public IList<ProveedorDto> ObtenerCorredoresAsociadosPorEstablecimientoId(int id)
        {
            var establecimiento = repositorio.Obtener<Establecimiento>(id);
            var corredoresAsociados = establecimiento.CorredoresAsociados;
            return conversor.ConvertirList<Proveedor, ProveedorDto>(corredoresAsociados);
        }

        public List<ComercialDto> ListarComerciales()
        {
            return Listar<Comercial, ComercialDto>().ToList();
        }

        public IList<TipoVariedadPorMaterialDto> ListarTipoVariedadPorMaterial(int materialId)
        {
            return Listar<TipoVariedadPorMaterial, TipoVariedadPorMaterialDto>(x => x.MaterialId == materialId);
        }

        public IList<TipoVariedadDto> ListarTipoVariedad()
        {
            return Listar<TipoVariedad, TipoVariedadDto>();
        }

        public TipoVariedadDto[] ObtenerVariedadPorTipoMaterial(int idMaterial)
        {
            return ObtenerRelacionVariedadPorMaterial(idMaterial).ToArray();
        }

        public List<int> ListarIdCallesNoEditables()
        {
            var callesNoEditablesId = new List<int>();
            var configuracionGeneralGrano = this.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica, Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos);
            if (bool.TryParse(configuracionGeneralGrano?.Valor, out bool automatismoGeneralGrano) && automatismoGeneralGrano)
            {
                var includesGrano = new List<Expression<Func<AutomatismoGrano, object>>> { x => x.CallePreBalanza, x => x.CallePreHidraulica };
                var automatismosGrano = repositorio.Listar(includesGrano, x => x.Activo).Select(s => new { idCalleFirst = s.CallePreBalanza.Id, idCalleSecond = s.CallePreHidraulica.Id });
                callesNoEditablesId.AddRange(automatismosGrano.Select(q => q.idCalleFirst));
                callesNoEditablesId.AddRange(automatismosGrano.Select(q => q.idCalleSecond));
            }

            var configuracionGeneralNoGrano = this.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoPuerto, Constantes.ConfiguracionGeneral.LlamadoAutomatico.NoGranos);
            if (bool.TryParse(configuracionGeneralNoGrano?.Valor, out bool automatismoGeneralNoGrano) && automatismoGeneralNoGrano)
            {
                var automatismosNoGrano = Listar<AutomatismoNoGrano, AutomatismoNoGranoDto>(x => x.Activo).Select(s => new { idCalleFirst = s.CallePlanta.Id, idCalleSecond = s.CallePlayaInterna.Id });
                callesNoEditablesId.AddRange(automatismosNoGrano.Select(q => q.idCalleFirst));
                callesNoEditablesId.AddRange(automatismosNoGrano.Select(q => q.idCalleSecond));
            }

            return callesNoEditablesId.Distinct().ToList();
        }

        public InfoCalleDto ObtenerInfoCalle(int idCalle)
        {
            var calle = repositorio.Obtener<Calle>(c => c.Id == idCalle);
            var infoCalle = new InfoCalleDto(calle);
            if (calle.TipoCalle == TipoCalle.PreBalanzaGranos)
            {
                var includes = new List<Expression<Func<AutomatismoGrano, object>>> { x => x.Material, x => x.CallePreBalanza, x => x.CallePreHidraulica, x => x.TipoVariedad, x => x.Almacen, x => x.Hidraulicas };
                var automatismo = repositorio.Obtener<AutomatismoGrano>(includes, c => c.CallePreBalanzaId == idCalle);

                if (automatismo != null)
                {
                    var automatismoConvertido = conversor.Convertir<AutomatismoGrano, AutomatismoGranoDto>(automatismo);
                    var configuracionGeneralGrano = this.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica, Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos);

                    infoCalle.EsIncluidoAutomatismo = "Si";
                    infoCalle.Hidraulica = automatismoConvertido.HidraulicaDescripcion;
                    infoCalle.Variedad = string.IsNullOrEmpty(automatismoConvertido.VariedadDescripcion) ? Textos.Variedad_Estandar : automatismoConvertido.VariedadDescripcion;
                    infoCalle.Almacen = string.Empty;
                    infoCalle.PuntoDeCarga = string.Empty;
                    infoCalle.EstadoAutomatismo = automatismoConvertido.Activo && bool.TryParse(configuracionGeneralGrano?.Valor, out bool automatismoGeneralGrano) && automatismoGeneralGrano ? "Activo" : "Inactivo";
                }
            }
            if (calle.TipoCalle == TipoCalle.PlantaNoGranos)
            {
                var automatismoNoGrano = repositorio.Listar<AutomatismoNoGrano>(x => x.CallePlanta.Id == idCalle).FirstOrDefault();

                if (automatismoNoGrano != null)
                {
                    var configuracionGeneralNoGrano = this.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoPuerto, Constantes.ConfiguracionGeneral.LlamadoAutomatico.NoGranos);
                    infoCalle.EsIncluidoAutomatismo = "Si";
                    infoCalle.Almacen = automatismoNoGrano.Almacen.Descripcion;
                    infoCalle.PuntoDeCarga = automatismoNoGrano.PuntoDeCarga.Descripcion;
                    infoCalle.EstadoAutomatismo = automatismoNoGrano.Activo && bool.TryParse(configuracionGeneralNoGrano?.Valor, out bool automatismoGeneralNoGrano) && automatismoGeneralNoGrano ? "Activo" : "Inactivo";
                }
            }

            return infoCalle;
        }

        public int? ObtenerVariedadIdPorMaterial(int materialId, string codigoSAPtitularCP = null, bool esEpa = false, bool esSustentable = false)
        {
            var variedadesPorMaterial = repositorio.Listar<TipoVariedadPorMaterial>(x => x.MaterialId == materialId).Select(x => x.TipoVariedad);

            if (!string.IsNullOrEmpty(codigoSAPtitularCP) && codigoSAPtitularCP.Equals(Constantes.ValoresPorDefecto.CodigoSapTPR))
                return variedadesPorMaterial.Where(c => c.Codigo.Equals(Constantes.TipoVariedadMaterial.Importacion)).Select(x => x.Id).FirstOrDefault();

            if (esEpa)
                return variedadesPorMaterial.Where(c => c.Codigo.Equals(Constantes.TipoVariedadMaterial.EPA)).Select(x => x.Id).FirstOrDefault();

            if (esSustentable)
                return variedadesPorMaterial.Where(c => c.Codigo.Equals(Constantes.TipoVariedadMaterial.Sustentable)).Select(x => x.Id).FirstOrDefault();

            return null;
        }

        private List<TipoVariedadDto> ObtenerRelacionVariedadPorMaterial(int idMaterial)
        {
            var tipoVariedades = Listar<TipoVariedad, TipoVariedadDto>();
            var variedadesPorMaterial = Listar<TipoVariedadPorMaterial, TipoVariedadPorMaterialDto>(x => x.MaterialId == idMaterial);
            var includes = new List<Expression<Func<AutomatismoGrano, object>>> { x => x.Material, x => x.CallePreBalanza, x => x.CallePreHidraulica, x => x.TipoVariedad, x => x.Almacen, x => x.Hidraulicas };
            var automatismos = repositorio.Listar<AutomatismoGrano>(includes, x => x.MaterialId == idMaterial);

            var variedadesActivas =
                   tipoVariedades.Join(variedadesPorMaterial,
                   variedades => variedades.Id,
                   variedadMaterial => variedadMaterial.TipoVariedadId,
                   (variedades, variedadMaterial) =>
                       new TipoVariedadDto { Descripcion = variedades.Descripcion, Id = variedades.Id, Codigo = variedades.Codigo, ColorTexto = variedadMaterial.ColorTexto, ColorFondo = variedadMaterial.ColorFondo, EstaEnAutomatismo = automatismos.Any(c => c.MaterialId == variedadMaterial.MaterialId && c.TipoVariedadId == variedadMaterial.TipoVariedadId) })

                   .ToList();

            return variedadesActivas;
        }

        public IList<AutomatismoGranoDto> ListarAutomatismoGrano()
        {
            var includes = new List<Expression<Func<AutomatismoGrano, object>>> { x => x.Material, x => x.CallePreBalanza, x => x.CallePreHidraulica, x => x.TipoVariedad, x => x.Almacen, x => x.Hidraulicas };
            var lista = repositorio.Listar<AutomatismoGrano>(includes);
            var automatismos = conversor.ConvertirList<AutomatismoGrano, AutomatismoGranoDto>(lista);

            automatismos.Where(atomatismo => String.IsNullOrEmpty(atomatismo.VariedadDescripcion)).ToList().ForEach(atomatismoVariedad => atomatismoVariedad.VariedadDescripcion = Textos.Variedad_Estandar);

            return automatismos;
        }

        public IList<CalleDto> ListarCallesAutomatismoGrano(TipoCalle tipoCalle, bool esNoGranos, int idCalleActual)
        {
            List<int> listaId = new List<int>();

            var includesGrano = new List<Expression<Func<AutomatismoGrano, object>>> { x => x.Material, x => x.CallePreBalanza, x => x.CallePreHidraulica, x => x.TipoVariedad, x => x.Almacen, x => x.Hidraulicas };
            var includesNoGrano = new List<Expression<Func<AutomatismoNoGrano, object>>> { x => x.Almacen, x => x.CallePlayaInterna, x => x.PuntoDeCarga, x => x.CallePlanta };

            if (tipoCalle == TipoCalle.PreBalanzaGranos)
            {
                var lista = repositorio.Listar<AutomatismoGrano>(includesGrano);
                listaId.AddRange(lista.Select(s => s.CallePreBalanzaId).ToList());
            }
            if (tipoCalle == TipoCalle.PlayaInterna)
            {
                var lista = repositorio.Listar<AutomatismoNoGrano>(includesNoGrano);
                listaId.AddRange(lista.Select(s => s.CallePlayaInterna.Id).ToList());
                if (esNoGranos)
                {
                    var listaGranos = repositorio.Listar<AutomatismoGrano>(includesGrano);
                    listaId.AddRange(listaGranos.Select(s => s.CallePreHidraulicaId).ToList());
                }
                listaId = listaId.Distinct().ToList();
            }

            var listaCalles = repositorio.Listar<Calle>(c => c.TipoCalle == tipoCalle).ToList();

            var calleDisponibles = listaCalles.Where(c => !listaId.Contains(c.Id)).ToList();

            if (idCalleActual != 0)
            {
                calleDisponibles.Add(repositorio.Obtener<Calle>(c => c.Id == idCalleActual));
            }

            var calles = conversor.ConvertirList<Calle, CalleDto>(calleDisponibles.Distinct().ToList());

            return calles;
        }

        public bool ExisteCamionesEnCalle(int calleId)
        {
            return repositorio.Existe<CallePorRecorrido>(x => x.FechaEgreso == null && x.Calle.Id == calleId);
        }

        public AutomatismoGranoDto ObtenerAutomatismoGranos(int id)
        {
            var includes = new List<Expression<Func<AutomatismoGrano, object>>> { x => x.Material, x => x.CallePreBalanza, x => x.CallePreHidraulica, x => x.TipoVariedad, x => x.Almacen, x => x.Hidraulicas };
            var automatismo = repositorio.Obtener<AutomatismoGrano>(includes, a => a.Id == id);

            return conversor.Convertir<AutomatismoGrano, AutomatismoGranoDto>(automatismo);
        }

        public AutomatismoGranoDto ObtenerAutomatismoGranoPorRecorridoGuid(Guid workflowInstanceId)
        {
            return repositorio.ObtenerConsultaEscalar(new ConsultarAutomatismoGranoActivoDisponible(workflowInstanceId));
        }

        public List<AutomatismoNoGranoDto> ListarAutomatismoNoGrano()
        {
            var automatismos = Listar<AutomatismoNoGrano, AutomatismoNoGranoDto>().ToList();
            return automatismos;
        }

        public PuntoDeCargaDto ObtenerPuntoDeCarga(int id)
        {
            return Obtener<PuntoDeCarga, PuntoDeCargaDto>(id);
        }

        public IList<AlmacenDto> ListarAlmacenesActivosAutomatismoNoGrano()
        {
            return Listar<Almacen, AlmacenDto>(x => (bool)x.EstadoAutomatismo);
        }

        public IList<PuntoDeCargaDto> ListarPuntosDeCargaActivosAutomatismoNoGrano()
        {
            return Listar<PuntoDeCarga, PuntoDeCargaDto>(x => (bool)x.EstadoAutomatismo);
        }

        public IList<CalleDto> ListarCallesAutomatismoActivoPorTipoCalle(TipoCalle tipo)
        {
            return Listar<Calle, CalleDto>(x => x.TipoCalle == tipo && x.ActivoAutomatico);
        }

        public AutomatismoNoGranoDto ObtenerAutomatismoNoGrano(int id)
        {
            return Obtener<AutomatismoNoGrano, AutomatismoNoGranoDto>(a => a.Id == id);
        }

        public AsignacionAutomatismoGranoEnRecorridoDto ObtenerAsignacionAutomatismoGranoEnRecorrido(Guid workflowInstanceId)
        {
            return Obtener<AsignacionAutomatismoGranoEnRecorrido, AsignacionAutomatismoGranoEnRecorridoDto>(x => x.Recorrido.InstanciaWorkflow == workflowInstanceId);
        }

        public bool ValidarDisponibilidadAsignacionEnCallePreBalanza(int callePBId, int callePHId)
        {
            var callePB = repositorio.Obtener<Calle>(x => x.Id == callePBId);
            var callePH = repositorio.Obtener<Calle>(x => x.Id == callePHId);
            if (callePB == null || callePH == null)
                return false;

            if (callePB.Deshabilitada)
                return false;

            if (callePH.AutomatismoTipoLlamado.Codigo == Constantes.AutomatismoTipoLlamado.PorFila
                && callePB.Bloqueada
                && callePB.FechaLLamada != null)
                return false;

            return repositorio.Contar<CallePorRecorrido>(x => x.Calle.Id == callePBId && x.FechaEgreso == null) < callePB.CantidadDeCamiones;
        }

        public CargaDeCupoDto ObtenerCargaDeCupoPorCTG(string nroCTG)
        {
            return ObtenerUltimo<CargaDeCupo, CargaDeCupoDto>(x => x.CTG == nroCTG, y => y.Id);
        }

        public IList<CalleDto> ListarCallesPlayaInternaAutomatismoDisponibles()
        {
            var callesAutomatismoActivas = Listar<Calle, CalleDto>(x => x.TipoCalle == TipoCalle.PlayaInterna && x.ActivoAutomatico);
            var callesGranosUsadas = repositorio.Listar<AutomatismoGrano>().Select(q => q.CallePreHidraulicaId);
            var callesUtilizables = callesAutomatismoActivas.Where(q => !callesGranosUsadas.Contains(q.Id)).ToList();
            return callesUtilizables;
        }

        public IList<HistorialMensajeCartelLedDto> ListarCamionesLlamados()
        {
            return Listar<HistorialMensajeCartelLed, HistorialMensajeCartelLedDto>(x => x.MensajeCartelLed.Codigo == CodigoMensajeCartelLed.LlamadoCamionNoGrano && x.Recorrido != null);
        }

        public AutomatismoNoGranoDto ObtenerAutomatismoNoGranoActivoPorMaterialId(int materialId)
        {
            return Obtener<AutomatismoNoGrano, AutomatismoNoGranoDto>(a => a.CallePlanta.Material.Id == materialId && a.Activo == true);
        }

        public int? ObtenerCallePlantaIdPorMaterialId(int materialId)
        {
            var calle = repositorio.Obtener<Calle>(x => x.Material.Id == materialId && x.Deshabilitada == false && x.TipoCalle == TipoCalle.PlantaNoGranos);

            if (calle == null)
                return null;

            return calle.Id;
        }

        public int ObtenerMaterialIdPorCalleId(int calleId)

        {
            return repositorio.ObtenerProyeccion<Calle, int>(x => x.Id == calleId, x => x.Material != null ? x.Material.Id : 0);
        }

        public MensajeCartelLedDto ObtenerCartelDisponible(string codigo)
        {
            var result = repositorio.ObtenerPrimero<MensajeCartelLed>(x => x.Codigo == codigo && x.HistorialMensajeCartelLed.Recorrido == null);

            return conversor.Convertir<MensajeCartelLed, MensajeCartelLedDto>(result);
        }

        public RecorridoDto ObtenerPrimerRecorridoDisponibleParaLlamadoAutomaticoNoGranos()
        {
            var recorridosEnCartel = this.RecorridosEnCartel(CodigoMensajeCartelLed.LlamadoCamionNoGrano);

            var primerosCamionesPorCalle = Listar<CallePorRecorrido, CallePorRecorridoDto>(x => x.Calle.TipoCalle == TipoCalle.NoGranos
                                                && x.FechaEgreso == null && x.Recorrido.Calle != null && !recorridosEnCartel.Any(id => id == x.Recorrido.Id))
                 .GroupBy(c => c.CalleId)
                    .Select(g => new
                    {
                        Calle_Id = g.Key,
                        Recorrido_Id = g.OrderBy(c => c.FechaIngeso).FirstOrDefault().RecorridoId,
                        FechaIngresoMasAntigua = g.Min(c => c.FechaIngeso)
                    });

            var recorridoId = primerosCamionesPorCalle?.OrderBy(x => x?.FechaIngresoMasAntigua)?.FirstOrDefault()?.Recorrido_Id;

            return Obtener<Recorrido, RecorridoDto>(x => x.Id == recorridoId);
        }

        public IList<RecorridoDto> ObtenerPrimerosRecorridosDisponibleParaLlamadoAutomaticoNoGranos()
        {
            var recorridosEnCartel = this.RecorridosEnCartel(CodigoMensajeCartelLed.LlamadoCamionNoGrano);

            var primerosCamionesPorCalle = Listar<CallePorRecorrido, CallePorRecorridoDto>(x => x.Calle.TipoCalle == TipoCalle.NoGranos
                                                && x.FechaEgreso == null && x.Recorrido.Calle != null && !recorridosEnCartel.Any(id => id == x.Recorrido.Id))
                 .GroupBy(c => c.CalleId)
                    .Select(g => new
                    {
                        Calle_Id = g.Key,
                        Recorrido_Id = g.OrderBy(c => c.FechaIngeso).FirstOrDefault().RecorridoId,
                        FechaIngresoMasAntigua = g.Min(c => c.FechaIngeso)
                    });

            var idsRecorridos = primerosCamionesPorCalle?.OrderBy(x => x?.FechaIngresoMasAntigua)?.Select(x => x.Recorrido_Id);

            var recorridos =  Listar<Recorrido, RecorridoDto>(x => idsRecorridos.Contains(x.Id));

            return recorridos;
        }

        private IEnumerable<int> RecorridosEnCartel(string codigo)
        {
            var res = Listar<MensajeCartelLed, MensajeCartelLedDto>(x => x.Codigo == codigo && x.HistorialMensajeCartelLed.Recorrido != null);

            if (res == null)
                return Enumerable.Empty<int>();

            var r = res.Select(x => x.HistorialMensajeCartelLed.RecorridoId.Value);

            return r;
        }

        public int ObtenerDisponibilidadEnPlayaInternaNoGranos(int callePlayaInternaId)
        {
            var cantidadDeCamionesAsignados = repositorio.Contar<AsignacionNoGranoEnRecorrido>(x => x.Recorrido.Calle.Id == callePlayaInternaId);

            var cantidadDeCamionesLlamados = repositorio.Contar<HistorialMensajeCartelLed>(x => x.Recorrido.Calle.Id == callePlayaInternaId);

            var cantidadDeEspaciosPorCalle = repositorio.Obtener<Calle>(x => x.Id == callePlayaInternaId).CantidadDeCamiones;

            return (cantidadDeEspaciosPorCalle - cantidadDeCamionesAsignados - cantidadDeCamionesLlamados);
        }

        public CallePorRecorridoDto ObtenerCallePorRecorridoPlayaExternaNoGranosPorRecorridoId(int recorridoId)
        {
            return Obtener<CallePorRecorrido, CallePorRecorridoDto>(x => x.Recorrido.Id == recorridoId && x.FechaEgreso == null && x.Calle.TipoCalle == TipoCalle.NoGranos);
        }

        public bool ExisteAsigacionGranoEnRecorrido(int recorridoId)
        {
            return repositorio.Existe<AsignacionAutomatismoGranoEnRecorrido>(x => x.RecorridoId == recorridoId);
        }

        public PuestosDeCargaDescargaDto ObtenerPuestoDeCargaDescarga(int puestoId)
        {
            return Listar<PuestosDeCargaDescarga, PuestosDeCargaDescargaDto>(f => f.Id == puestoId).FirstOrDefault();
        }

        public bool EstaDisponibleParaLlamadoAutomaticoHidraulica(int hidraulicaId)
        {
            return repositorio.Existe<LlamadoAutomaticoHidraulica>(x => x.Hidraulica.Id == hidraulicaId && x.Estado != EstadoHidraulica.Inhabilitado);
        }

        public IList<AutomatismoTipoLlamadoDto> ListarAutomatismoTipoLlamado()
        {
            return Listar<AutomatismoTipoLlamado, AutomatismoTipoLlamadoDto>(f => f.Activo);
        }

        public IList<AutomatismoNoGranoDto> ObtenerAutomatismosNoGranoActivoPorPuntoDeCarga(int puntoDeCargaId)
        {
            return Listar<AutomatismoNoGrano, AutomatismoNoGranoDto>(a => a.PuntoDeCarga.Id == puntoDeCargaId && a.Activo == true);
        }

        public IList<AutomatismoNoGranoDto> ObtenerAutomatismosNoGranoActivos()
        {
            return Listar<AutomatismoNoGrano, AutomatismoNoGranoDto>(a => a.Activo == true);
        }

        public bool ValidarEspacioDisponibleEnCallePreHidraulica(int callePlayaInternaId)
        {
            var callePreHidraulica = repositorio.Obtener<Calle>(callePlayaInternaId);
            return callePreHidraulica.CantidadDeCamiones > ObtenerCantidadCamionesEnCallePreHidraulica(callePlayaInternaId);
        }

        public int ObtenerCantidadCamionesEnCallePreHidraulica(int callePlayaInternaId)
        {
            var cantidadCamionesLlamadosUnoAUno = repositorio.Contar<CallePreBalanzaPlayaInterna>(x => x.CallePlayaInternaId == callePlayaInternaId && x.RecorridoId.HasValue && !x.EsCamionEnEspera);
            var callesIds = repositorio.Listar<CallePreBalanzaPlayaInterna>(x => x.CallePlayaInterna.Id == callePlayaInternaId && !x.RecorridoId.HasValue)
                                                            .Select(q => q.CallePreBalanzaId)
                                                            .ToList();
            callesIds.Add(callePlayaInternaId);
            return cantidadCamionesLlamadosUnoAUno + repositorio.Contar<CallePorRecorrido>(x => callesIds.Contains(x.Calle.Id) && x.FechaEgreso == null);
        }

        public bool ExisteEspacioDisponibleParaLlamarEnCartel(string codigo)
        {
            return repositorio.Existe<MensajeCartelLed>(x => x.Codigo == codigo && (x.HistorialMensajeCartelLed == null || x.HistorialMensajeCartelLed.Calle == null));
        }

        public bool ExisteLlamadoCallePreBalanzaPorTipoDeLlamado(int callePHId, string codigoTipoLlamado)
        {
            return repositorio.Existe<CallePreBalanzaPlayaInterna>(x => x.CallePlayaInternaId == callePHId && x.CodigoAutomatismoTipoLlamado == codigoTipoLlamado && !x.EsCamionEnEspera);
        }

        public IList<CallePreBalanzaPlayaInternaDto> ListarCallePreBalanzaLlamadasPorAutomatismo()
        {
            return Listar<CallePreBalanzaPlayaInterna, CallePreBalanzaPlayaInternaDto>(x => !string.IsNullOrEmpty(x.CodigoAutomatismoTipoLlamado) && !x.EsCamionEnEspera);
        }

        public CallePorRecorridoDto ObtenerCallePorRecorrido(int calleId, int recorridoId)
        {
            return Obtener<CallePorRecorrido, CallePorRecorridoDto>(x => x.Recorrido.Id == recorridoId && x.Calle.Id == calleId);
        }

        public CallePreBalanzaPlayaInternaDto ObtenerCallePreBalanzaPlayaInternaDeCamionEnEspera(int callePHId)
        {
            return Obtener<CallePreBalanzaPlayaInterna, CallePreBalanzaPlayaInternaDto>(x => x.CallePlayaInternaId == callePHId && x.RecorridoId.HasValue && x.EsCamionEnEspera && x.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.UnoAUno);
        }

        public CallePreBalanzaPlayaInternaDto ObtenerCallePrebalanzaPlayaInterna(int callePBId)
        {
            return Obtener<CallePreBalanzaPlayaInterna, CallePreBalanzaPlayaInternaDto>(x => x.CallePreBalanzaId == callePBId && x.CodigoAutomatismoTipoLlamado != Constantes.AutomatismoTipoLlamado.UnoAUno);
        }
    }


}