using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    public class FotoController : BaseController
    {
        private ILogger log;
        private readonly IServicioComandos servicioComandos;

        public FotoController(ILogger log, IServicioRepositorio servicio, IConfiguracionProvider config, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index(Guid id, string actividad = "")
        {
            var fotos = servicio.ListarFotosCamion(id, actividad);
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            var idsInt = new List<int>();
            if (recorrido != null)
            {
                idsInt.Add(recorrido.Id);
            }
            ObtenerFotoSustentable(fotos, idsInt);
            return View(fotos);
        }

        public ActionResult IndexPorCargaDeCupo(int id)
        {
            var fotos = servicio.ListarFotosCamionPorCargaDeCupo(id);

            return View("Index", fotos);
        }

        public ActionResult IndexPorTarjeta(string id, string actividad = "")
        {
            var fotos = servicio.ListarFotosCamionPorTarjeta(id, actividad);

            return View("Index", fotos);
        }

        public ActionResult IndexPorRecorrido(int id, string actividad = "", bool conJs = true)
        {
            var fotos = servicio.ListarFotosCamionPorRecorrido(id, actividad);
            ViewBag.ConJs = conJs;
            return View("Index", fotos);
        }

        public JsonResult ObtenerUltimaFoto(string id, int puestoDeTrabajoId = 0)
        {
            var fotos = servicio.ObtenerUltimaFotoPorTarjeta(id, puestoDeTrabajoId);

            return Json(Convert.ToBase64String(fotos), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ObtenerPorQuiebre(int id)
        {
            var fotos = servicio.ListarFotosQuiebre(id);

            return View("Index", fotos);
        }

        public ActionResult ObtenerFotoDeCartaPorte(string id)
        {
            var idsInt = id.Split('-').Select(x => int.Parse(x)).ToList();
            var fotos = servicio.ObtenerFotoCPDeCartasDePortePorrecorrido(idsInt);
            ObtenerFotoSustentable(fotos, idsInt);
            ObtenerFotoDescargada(fotos, idsInt);
            ViewBag.NoReemplazarCp = idsInt.Count != 1;
            return View("Index",fotos);
        }

        public ActionResult DescargarFotoDeCartaPorte(string id)
        {
            var idsInt = id.Split('-').Select(x => int.Parse(x)).ToList();
            var fotos = servicio.ObtenerFotoCPDeCartasDePortePorrecorrido(idsInt);
            ObtenerFotoSustentable(fotos, idsInt);
            ObtenerFotoDescargada(fotos, idsInt);
            using (var ms = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach(var foto in fotos.Fotos)
                    {
                        var entry = zipArchive.CreateEntry(foto.Actividad + ".jpeg");
                        using (var entryStream = entry.Open())
                        {
                            using (var f = new MemoryStream(foto.Foto))
                            {
                                f.CopyTo(entryStream);
                            }
                        }
                    }
                }
                var fileStreamret = new MemoryStream(ms.ToArray());
                fileStreamret.Seek(0, SeekOrigin.Begin);
                return File(fileStreamret, "application/octet-stream", "CartasPorteDigitalizadas.zip");
            }
        }

        [HttpPost]
        [DatosUsuario]
        public JsonResult AgregarFotoARecorrido(string foto, int RecorridoId, string puestoDeTrabajo, string numeroDocumentoIngreso, string Patente, DatosUsuario datosUsuario)
        {
            var cartaPorte = servicio.ObtenerCartaDePortePorrecorrido(RecorridoId);
            if (cartaPorte == null)
            {
                return Json(new { CodigoDeError = 1, Error = Textos.Foto_ErrorRecorridoSinCP }, JsonRequestBehavior.AllowGet);
            }
            DateTime date = cartaPorte.FechaEmision;
            if (!String.IsNullOrEmpty(cartaPorte.FotoRutaDestino))
            {
                int cant = cartaPorte.FotoRutaDestino.Split('\\').Length;
                string fecha = cartaPorte.FotoRutaDestino.Split('\\')[cant - 2];
                string hora = cartaPorte.FotoRutaDestino.Split('\\').Last().Split('-').Last().Split('.').First();
                puestoDeTrabajo = cartaPorte.FotoRutaDestino.Split('\\')[cant - 3];
                date = DateTime.ParseExact(fecha + hora, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }

            var path = servicioComandos.Ejecutar(
                    new GuardarfotoMesaDigitalizacion
                    {
                        Fecha = date,
                        FotoMesaDigitalizacion = foto,
                        Directorio = puestoDeTrabajo,
                        CentroId = datosUsuario.CentroId,
                        NumeroDocumentoIngreso = numeroDocumentoIngreso,
                        Patente = Patente,
                        Usuario = datosUsuario.NombreUsuario,
                        TipoVehiculo = cartaPorte.Vehiculos.First().TipoVehiculo
                    }) as ResultadoGuardarFoto;

            if (String.IsNullOrEmpty(cartaPorte.FotoRutaDestino) && path != null)
            {
                cartaPorte.FotoRutaDestino = path.Path;
                servicioComandos.Ejecutar(new ModificarCartaPorteFoto {Orden= cartaPorte , Usuario= datosUsuario.NombreUsuario});
            }

            if (path != null)
            {
                return Json(new { CodigoDeError = 0, data = path.Path }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { CodigoDeError = 1, Error = Textos.Foto_ErrorActualizarFoto }, JsonRequestBehavior.AllowGet);
            }

        }

        private void ObtenerFotoSustentable(FotosDto fotos, List<int> ids)
        {
            foreach (var recorridoId in ids)
            {
                var recorrido = servicio.ObtenerRecorrido(recorridoId);
                if (recorrido != null && recorrido.Centro != null)
                {
                    var fotoSustentable = servicio.ObtenerFotoSustentable(recorrido.Centro.Id, recorrido.NumeroDocumentoIngreso, recorrido.NumeroDocumentoIngreso + "-sustentable");
                    if (fotoSustentable != null)
                    {
                        fotos.Fotos.Add(fotoSustentable);
                    }
                }
            }
        }

        private void ObtenerFotoDescargada(FotosDto fotos, List<int> ids)
        {
            foreach (var recorridoId in ids)
            {
                var recorrido = servicio.ObtenerRecorrido(recorridoId);
                if (recorrido != null && recorrido.Centro != null)
                {
                    var fotoDescargada = servicio.ObtenerFotoDescargada(recorrido.Centro.Id, recorrido.NumeroDocumentoIngreso, recorrido.NumeroDocumentoIngreso + "-descargado", recorrido.EsSustentable);
                    if (fotoDescargada != null)
                    {
                        fotos.Fotos.Add(fotoDescargada);
                    }
                }
            }
        }

    }
}
