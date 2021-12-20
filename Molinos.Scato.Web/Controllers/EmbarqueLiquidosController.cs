using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.EmbarqueLiquidoPuerto)]
    public class EmbarqueLiquidosController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioNotificarUsuario notificador;
        public EmbarqueLiquidosController(ILogger log, IServicioNotificarUsuario notificador, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioOrquestador orquestador)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.orquestador = orquestador;
            this.notificador = notificador;
        }

        public ActionResult Modificar(int id, string numeroBalanza)
        {
            var aModificar = servicio.ObtenerBalanzada(id, numeroBalanza);
            return View(aModificar);
        }

        [HttpPost]
        public ActionResult Modificar(BalanzadaDto model)
        {
            if (ModelState.IsValid)
            {

                var resultado = servicioComandos.Ejecutar(new ModificarBalanzada { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarBalanzada { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public ActionResult Crear()
        {
            ViewBag.BalanzasAdministrativas = servicio.ObtenerTodasBalanzaPuertoAdministrativa().ToSelectList();
            return View(new EmbarqueLiquidosDto() {Fecha = DateTime.Now });
        }

        [HttpPost]
        public ActionResult Crear(EmbarqueLiquidosDto model)
        {
            if (ModelState.IsValid)
            {
                log.Debug($"EmbarqueLiquidos crear carga " + model.ToXml());
                var resultado = (ResultadoCrear)servicioComandos.Ejecutar(new CrearCarga { Dto = TransformarEmbarqueDtoEnCargaInicioDto(model) });
                if (!resultado.HayErrores)
                {
                    int CargaInicial_Id = resultado.Id;
                    log.Debug($"EmbarqueLiquidos crear balanzada");
                    resultado = (ResultadoCrear)servicioComandos.Ejecutar(new CrearBalanzada { Dto = TransformarEmbarqueDtoEnBalanzada(model, CargaInicial_Id) });
                    if (!resultado.HayErrores)
                    {
                        int balanzadaId = resultado.Id;
                        log.Debug($"EmbarqueLiquidos crear carga fin");
                        resultado = (ResultadoCrear)servicioComandos.Ejecutar(new CrearCarga { Dto = TransformarEmbarqueDtoEnCargaFinDto(model, CargaInicial_Id, balanzadaId) });
                        if (!resultado.HayErrores)
                        {
                            int CargaFin_Id = resultado.Id;
                            log.Debug($"EmbarqueLiquidos actualizar carga opuesta");
                            var resultadoActualizar = servicioComandos.Ejecutar(new ActualizarCargaOpuesta { Carga_Id = CargaInicial_Id, CargaOpuesta_Id = CargaFin_Id, NumeroBalanza = model.NumeroBalanza });
                            if (!resultadoActualizar.HayErrores)
                            {
                                try
                                {
                                    log.Debug($"EmbarqueLiquidos enviar a sap");
                                    servicioComandos.Ejecutar(new EnviarLecturaBalanzadaTransmisionASap { Id = balanzadaId, NumeroBalanza = model.NumeroBalanza });
                                    return new AjaxEditSuccessResult();

                                }
                                catch (Exception e)
                                {
                                    resultado.Errores.Add("EnvioASAPFallido", "Se creó el embarque. Falló el envío a SAP de la balanzada");
                                    return new AjaxEditSuccessResult();
                                }
                            }
                            else
                            {
                                resultado.Errores.Add(resultadoActualizar.Errores.First());
                            }
                        }
                    }
                }
                ViewBag.BalanzasAdministrativas = servicio.ObtenerTodasBalanzaPuertoAdministrativa().ToSelectList();
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        private CargaDto TransformarEmbarqueDtoEnCargaInicioDto(EmbarqueLiquidosDto dto)
        {
            return new CargaDto
            {
                Id = servicio.ObtenerProximoIdEmbarqueLiquido(dto.NumeroBalanza),
                Bodega = dto.Bodega,
                Destino = dto.Destino,
                EnviadoASap = false,
                Exportador = dto.Exportador,
                Fecha = dto.Fecha,
                Material = dto.Material,
                NumeroBalanza = dto.NumeroBalanza,
                PesoProgramado = dto.Peso,
                Tipo = "inicio",
                ToneladasAW = 0,
                Vapor = dto.Vapor
            };
        }

        private CargaDto TransformarEmbarqueDtoEnCargaFinDto(EmbarqueLiquidosDto dto, int CargaOpuesta_Id, int balanzadaId)
        {
            var inicio = servicio.ObtenerCarga(CargaOpuesta_Id, dto.NumeroBalanza);
            return new CargaDto
            {
                Id = balanzadaId + 1,
                Bodega = dto.Bodega,
                BodegaId = inicio != null? inicio.BodegaId : dto.BodegaId,
                Destino = dto.Destino,
                DestinoId = inicio != null ? inicio.DestinoId : dto.DestinoId,
                EnviadoASap = false,
                Exportador = dto.Exportador,
                ExportadorId = inicio != null ? inicio.ExportadorId : dto.ExportadorId,
                Fecha = dto.Fecha.AddMinutes(5),
                Material = dto.Material,
                MaterialId = inicio != null ? inicio.MaterialId : dto.MaterialId,
                NumeroBalanza = dto.NumeroBalanza,
                PesoProgramado = dto.Peso,
                Tipo = "fin",
                ToneladasAW = dto.Peso,
                Vapor = dto.Vapor,
                VaporId = inicio != null ? inicio.VaporId : dto.VaporId,
                FechaInicio = dto.Fecha,
                CargaOpuesta_Id = CargaOpuesta_Id,
                CargaOpuesta_NumeroBalanza = dto.NumeroBalanza
            };
        }

        private BalanzadaDto TransformarEmbarqueDtoEnBalanzada(EmbarqueLiquidosDto dto, int cargaId)
        {
            return new BalanzadaDto
            {
                Id = cargaId + 1,
                Capacidad = "0",
                CargaInicial_Id = cargaId,
                CargaInicial_NumeroBalanza = dto.NumeroBalanza,
                EnviadoASap = false,
                Fecha = dto.Fecha.AddMinutes(2),
                NumeroBalanza = dto.NumeroBalanza,
                PesoBruto = dto.Peso,
                PesoNeto = dto.Peso,
                PesoTara = 0
            };
        }


    }
}
