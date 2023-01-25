using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmMaterial)]
    public class MaterialController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private ILogger log;

        public MaterialController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, string ordenarPor = "Material.Id", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View();
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, string ordenarPor = "Material.Id", DirOrden dirOrden = DirOrden.Asc, int pagina = 1)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar");
        }

        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, itemsPorPagina: 10);

            ViewBag.Items = servicio.ListarPaginadoMateriales(filtro, centroId, paginacion);
            ViewBag.Almacenes = servicio.ObtenerAlmacenesPorCentro(centroId);
            ViewBag.Embalajes = servicio.ListarEmbalaje().Where(c=> c.Activo == true).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture) , x => x.Descripcion);
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            var material = new MaterialDto();

            CargarAlmacenes(datosUsuario.CentroId, 0);
            CargarCamaras(null);
            CargarEmbalajes();

            ViewBag.AnalisisInterno = 0;
            ViewBag.MuestraAuditoria = null;
            ViewBag.AlmacenPredId = null;
            ViewBag.CamaraId = null;
            ViewBag.CorrespondeDescarga = false;
            ViewBag.RequiereTecnologia = false;
            ViewBag.MaterialDeTerceros = false;
            ViewBag.Variedades = servicio.ListarVariedades().ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.ImprimeReciboMunicipal = false;
            ViewBag.MostrarEnWebMobile = false;
            ViewBag.DescripcionWebMobile = "";
            ViewBag.Orden = 0;
            ViewBag.NoValidaCG = false;
            ViewBag.IgnoraContingencia = false;
            
            return View(material);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(DatosUsuario datosUsuario, MaterialDto model, int? almacenPredId, int? analisisInterno, int? camaraId, bool correspondeDescarga, bool requiereTecnologia, bool materialDeTerceros, string almacenes, string almacenesEliminados, decimal? muestraAuditoria, bool mostrarEnWebMobile, string descripcionWebMobile, int? orden, bool imprimeReciboMunicipal, bool noValidaCg, bool ignoraContingencia)
        {
            if (model.EsDerivadoGranario)
            {
                ValidarDerivadoGranario(model);
            }

            if (ModelState.IsValid)
            {
                var matPorCentro = new MaterialPorCentroDto
                {
                    CentroId = datosUsuario.CentroId,
                    AlmacenPredId = almacenPredId,
                    AnalisisInterno = analisisInterno,
                    CorrespondeDescarga = correspondeDescarga,
                    CamaraId = camaraId,
                    PorcentajeMuestraAuditoria = muestraAuditoria,
                    RequiereTecnologia = requiereTecnologia,
                    MaterialDeTerceros = materialDeTerceros,
                    Almacenes = almacenes,
                    AlmacenesEliminados = almacenesEliminados,
                    ImprimeReciboMunicipal = imprimeReciboMunicipal,
                    NoValidaCG = noValidaCg,
                    MostrarEnWebMobile = mostrarEnWebMobile,
                    DescripcionWebMobile = descripcionWebMobile,
                    Orden = orden,
                    IgnoraContingencia = ignoraContingencia
                };

                var resultado = (ResultadoCrear)servicioComandos.Ejecutar(new CrearMaterial { Dto = model, MaterialPorCentroDto = matPorCentro, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    CargarAlmacenesPorMaterial(almacenes, almacenesEliminados, resultado.Id);
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            CargarAlmacenes(datosUsuario.CentroId, 0);
            CargarCamaras(camaraId);
            CargarEmbalajes();
            ViewBag.AlmacenPredId = almacenPredId;
            ViewBag.AnalisisInterno = analisisInterno;
            ViewBag.CamaraId = camaraId;
            ViewBag.CorrespondeDescarga = correspondeDescarga;
            ViewBag.MuestraAuditoria = muestraAuditoria;
            ViewBag.RequiereTecnologia = requiereTecnologia;
            ViewBag.MaterialDeTerceros = materialDeTerceros;
            ViewBag.Almacenes = almacenes;
            ViewBag.AlmacenesEliminados = almacenesEliminados;
            ViewBag.Variedades = servicio.ListarVariedades().ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.ImprimeReciboMunicipal = imprimeReciboMunicipal;
            ViewBag.MostrarEnWebMobile = mostrarEnWebMobile;
            ViewBag.DescripcionWebMobile = descripcionWebMobile;
            ViewBag.Orden = orden;
            ViewBag.NoValidaCG = noValidaCg;
            ViewBag.IgnoraContingencia = ignoraContingencia;
            
            return View(model);
        }

        [DatosUsuario]
        public ActionResult Modificar(DatosUsuario datosUsuario, int id)
        {
            var material = servicio.ObtenerMaterial(id);
            var materialPorCentro = servicio.ObtenerMaterialPorCentro(datosUsuario.CentroId, id);

            CargarAlmacenes(datosUsuario.CentroId, id);
            CargarCamaras(materialPorCentro != null ? materialPorCentro.CamaraId : null);
            CargarEmbalajes();

            ViewBag.AnalisisInterno = materialPorCentro != null ? materialPorCentro.AnalisisInterno : 0;
            ViewBag.MuestraAuditoria = materialPorCentro != null ? materialPorCentro.PorcentajeMuestraAuditoria : null;
            ViewBag.AlmacenPredId = materialPorCentro != null ? materialPorCentro.AlmacenPredId : 0;
            ViewBag.CamaraId = materialPorCentro != null ? materialPorCentro.CamaraId : 0;
            ViewBag.CorrespondeDescarga = materialPorCentro != null && materialPorCentro.CorrespondeDescarga;
            ViewBag.RequiereTecnologia = materialPorCentro != null && materialPorCentro.RequiereTecnologia;
            ViewBag.MaterialDeTerceros = materialPorCentro != null && materialPorCentro.MaterialDeTerceros;
            ViewBag.Variedades = servicio.ListarVariedades().ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.ImprimeReciboMunicipal = materialPorCentro != null && materialPorCentro.ImprimeReciboMunicipal;
            ViewBag.NoValidaCG = materialPorCentro != null && materialPorCentro.NoValidaCG;
            ViewBag.MostrarEnWebMobile = materialPorCentro != null && materialPorCentro.MostrarEnWebMobile;
            ViewBag.DescripcionWebMobile = materialPorCentro != null ? materialPorCentro.DescripcionWebMobile : "";
            ViewBag.EpaStockPorCorte = materialPorCentro != null ? materialPorCentro.EpaStockPorCorte : 0;
            ViewBag.Orden = materialPorCentro != null ? materialPorCentro.Orden : 0;
            ViewBag.IgnoraContingencia = materialPorCentro != null && materialPorCentro.IgnoraContingencia;
            
            return View(material);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Modificar(DatosUsuario datosUsuario, MaterialDto material, int? almacenPredId, int? analisisInterno, int? camaraId, bool correspondeDescarga, bool requiereTecnologia, bool materialDeTerceros, string almacenes, string almacenesEliminados, decimal? muestraAuditoria, bool imprimeReciboMunicipal, bool mostrarEnWebMobile, string descripcionWebMobile, int? orden, bool noValidaCg, int? epaStockPorCorte, bool ignoraContingencia)
        {
            if (material.EsDerivadoGranario)
            {
                ValidarDerivadoGranario(material);
            }

            if (ModelState.IsValid)
            {
                var matPorCentro = servicio.ObtenerMaterialPorCentro(datosUsuario.CentroId, material.Id);
                if (matPorCentro != null)
                {
                    matPorCentro.AnalisisInterno = analisisInterno;
                    matPorCentro.PorcentajeMuestraAuditoria = muestraAuditoria;
                    matPorCentro.AlmacenPredId = almacenPredId;
                    matPorCentro.CamaraId = camaraId;
                    matPorCentro.CorrespondeDescarga = correspondeDescarga;
                    matPorCentro.RequiereTecnologia = requiereTecnologia;
                    matPorCentro.MaterialDeTerceros = materialDeTerceros;
                    matPorCentro.Almacenes = almacenes;
                    matPorCentro.AlmacenesEliminados = almacenesEliminados;
                    matPorCentro.ImprimeReciboMunicipal = imprimeReciboMunicipal;
                    matPorCentro.NoValidaCG = noValidaCg;
                    matPorCentro.EpaStockPorCorte = epaStockPorCorte;
                    matPorCentro.MostrarEnWebMobile = mostrarEnWebMobile;
                    matPorCentro.DescripcionWebMobile = descripcionWebMobile;
                    matPorCentro.Orden = orden;
                    matPorCentro.IgnoraContingencia = ignoraContingencia;
                }
                else
                {
                    matPorCentro = new MaterialPorCentroDto
                    {
                        MaterialId = material.Id,
                        CentroId = datosUsuario.CentroId,
                        AlmacenPredId = almacenPredId,
                        AnalisisInterno = analisisInterno,
                        PorcentajeMuestraAuditoria = muestraAuditoria,
                        CorrespondeDescarga = correspondeDescarga,
                        RequiereTecnologia = requiereTecnologia,
                        MaterialDeTerceros = materialDeTerceros,
                        Almacenes = almacenes,
                        AlmacenesEliminados = almacenesEliminados,
                        CamaraId = camaraId,
                        ImprimeReciboMunicipal = imprimeReciboMunicipal,
                        NoValidaCG = noValidaCg,
                        EpaStockPorCorte = epaStockPorCorte,
                        MostrarEnWebMobile = mostrarEnWebMobile,
                        DescripcionWebMobile = descripcionWebMobile,
                        Orden = orden,
                        IgnoraContingencia = ignoraContingencia
                    };
                }
                var resultado = servicioComandos.Ejecutar(new ModificarMaterial { Dto = material, MaterialPorCentroDto = matPorCentro, Usuario = datosUsuario.NombreUsuario});
                if (!resultado.HayErrores)
                {
                    CargarAlmacenesPorMaterial(almacenes, almacenesEliminados, material.Id);
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }

            CargarAlmacenes(datosUsuario.CentroId, material.Id);
            CargarCamaras(camaraId);;
            CargarEmbalajes();
            ViewBag.AnalisisInterno = analisisInterno;
            ViewBag.AlmacenPredId = almacenPredId;
            ViewBag.CamaraId = camaraId;
            ViewBag.CorrespondeDescarga = correspondeDescarga;
            ViewBag.MuestraAuditoria = muestraAuditoria;
            ViewBag.RequiereTecnologia = requiereTecnologia;
            ViewBag.MaterialDeTerceros = materialDeTerceros;
            ViewBag.Variedades = servicio.ListarVariedades().ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion);
            ViewBag.ImprimeReciboMunicipal = imprimeReciboMunicipal;
            ViewBag.MostrarEnWebMobile = mostrarEnWebMobile;
            ViewBag.DescripcionWebMobile = descripcionWebMobile;
            ViewBag.Orden = orden;
            ViewBag.NoValidaCG = noValidaCg;
            ViewBag.IgnoraContingencia = ignoraContingencia;
            return View(material);
        }

        public JsonResult CargarAlmacenesPorMaterial(string almacenes, string almacenesEliminar, int materialId)
        {
            var listaAlmacenes = almacenes.FromJson<AlmacenDto[]>();
            var listaAlmaceneseliminar = almacenesEliminar.FromJson<AlmacenDto[]>();

            if (listaAlmacenes != null)
            {
                foreach (var dto in listaAlmacenes.Where(x => x.EsNuevo == true).Select(l => new AlmacenPorMaterial {MaterialId = materialId, AlmacenId = l.Id}))
                {
                    servicioComandos.Ejecutar(new CrearAlmacenPorMaterial {Dto = dto});
                }
            }
            if (listaAlmaceneseliminar != null)
            {
                foreach (var dto in listaAlmaceneseliminar.Where(x => x.FueEliminado).Select(l => new AlmacenPorMaterial { MaterialId = materialId, AlmacenId = l.Id }))
                {
                    servicioComandos.Ejecutar(new EliminarAlmacenPorMaterial { Dto = dto });
                }
            }
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        private void CargarAlmacenes(int centroId, int materialId)
        {
            var almacenes = servicio.ListarAlmacenesPorMaterial(centroId, materialId).OrderBy(c => c.Descripcion).Select(x => new AlmacenDto { Id = x.Id, EsNuevo = false, FueEliminado = false, CodigoSAP = x.CodigoSAP, Descripcion = x.Descripcion }).ToArray();
            ViewBag.AlmacenesList = almacenes;
            ViewBag.AlmacenesTodos = servicio.ListarAlmacenesPorCentro(centroId).OrderBy(c => c.Descripcion).Where(c => almacenes.All(x => x.Id != c.Id)).Select(x => new AlmacenDto { Id = x.Id, EsNuevo = true, FueEliminado = false, CodigoSAP = x.CodigoSAP, Descripcion = x.Descripcion }).ToArray();

            var almacenesSinCentro = new List<SelectListItem>
                {
                    new SelectListItem {Selected = true, Value = "", Text = Textos.Default_Almacen}
                };

            almacenesSinCentro.AddRange(servicio.ListarAlmacenes().ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion + "(" + x.CodigoSAP + ")"));
            ViewBag.AlmacenesSinCentro = almacenesSinCentro;
        }

        private void CargarCamaras(int? camaraId)
        {
            ViewBag.Camaras = servicio.ListarCamaras().OrderBy(c => c.Descripcion).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Descripcion, camaraId.HasValue ? camaraId.Value.ToString(CultureInfo.InvariantCulture) : "");
        }

        private void CargarEmbalajes()
        {
            
            ViewBag.Embalajes = servicio.ListarEmbalaje().Where(c => c.Activo == true).ToSelectList(x => x.Codigo.Trim(), x => x.Descripcion);
        }

        private void ValidarDerivadoGranario(MaterialDto material)
        {
            if (!material.CodigoEspecie.HasValue)
            {
                ModelState.AddModelError("CodigoEspecie", string.Format(Textos.Error_Requerido, Textos.Material_CodigoEspecie));
            }

            if (!material.CodigoGranoPadre.HasValue)
            {
                ModelState.AddModelError("CodigoGranoPadre", string.Format(Textos.Error_Requerido, Textos.Material_CodigoGranoPadre));
            }

            if (!material.TipoEmbalajeId.HasValue)
            {
                ModelState.AddModelError("TipoEmbalajeId", string.Format(Textos.Error_Requerido, Textos.Material_TipoEmbalaje));
            }
        }
    }
}
