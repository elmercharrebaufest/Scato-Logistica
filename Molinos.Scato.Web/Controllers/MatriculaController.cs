using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmMatricula)]
    public class MatriculaController : BaseController
    {

        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public MatriculaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }


        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarPaginadoUsuarios(filtro, paginacion);
        }


        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerUsuario(id);
            return View(new UsuarioMatriculaDto
            {
                Matricula = aModificar.Matricula,
                Apellido = aModificar.Apellido,
                Nombre = aModificar.Nombre,
                Id = aModificar.Id,
                NombreUsuario = aModificar.NombreUsuario
            });
        }

        [HttpPost]
        public ActionResult Modificar(UsuarioMatriculaDto model)
        {
            if (ModelState.IsValid)
            {
                if (model.FirmaFile != null)
                {
                    var image = Image.FromStream(model.FirmaFile.InputStream);
                    if (image.Height > 300 || image.Width > 300)
                    {
                        ModelState.AddModelError("", Textos.Error_ExcedeTamaño);
                        return View(model);
                    }
                    ZPLConveter zp = new ZPLConveter();
                    zp.setCompressHex(true);
                    zp.setBlacknessLimitPercentage(50);
                    model.Firma = zp.convertfromImg(image);

                    MemoryStream ms = new MemoryStream();
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                    model.FirmaImagen = ms.ToArray();
                    model.FirmaFile = null;
                }
                var resultado = servicioComandos.Ejecutar(new ModificarUsuarioMatricula { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

    }
}