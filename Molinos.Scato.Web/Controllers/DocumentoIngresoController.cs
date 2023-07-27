using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Helpers;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    public abstract class DocumentoIngresoController : BaseController
    {
        protected readonly IServicioComandos servicioComandos;
        protected readonly ILogger log;

        protected DocumentoIngresoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos) : base(servicio)
        {
            this.servicioComandos = servicioComandos;
            this.log = log;
        }

        protected bool SetearChofer(ChoferDto choferDto)
        {
            if (!ModelState.IsValid)
            {
                var Error = ModelState.Values.Where(c => c.Errors.Count > 0).ToList();
                return false;
            }
            log.Info("SetearChofer para el chofer con el CUIL: " + choferDto.Cuil);
            var chofer = servicio.BuscarChoferes(new ChoferFiltro { Cuil = choferDto.Cuil }).FirstOrDefault();
            if (chofer != null) //Chofer existente
            {
                log.Info("SetearChofer se actualizará el chofer con CUIL: " + choferDto.Cuil);
                var resultadoChofer = servicioComandos.Ejecutar(new ModificarChofer() { Dto = choferDto });
                //Verifico si hay errores
                if (resultadoChofer.HayErrores)
                {
                    log.Debug("SetearChofer - Errores: ");
                    resultadoChofer.Errores.ToList().ForEach(f =>
                    {
                        ModelState.AddModelError("Chofer." + f.Key, f.Value);
                        log.Debug(f.Value);
                    });
                    ModelState.AgregarErrores(resultadoChofer);
                    return false;
                }
            }
            else //ChoferNuevo
            {
                log.Info("SetearChofer se dará de alta el chofer con el CUIL: " + choferDto.Cuil);
                var resultadoChofer = servicioComandos.Ejecutar(new CrearChofer { Dto = choferDto });
                //Verifico si hay errores
                if (resultadoChofer.HayErrores)
                {
                    log.Debug("SetearChofer - Errores: ");
                    resultadoChofer.Errores.ToList().ForEach(f =>
                    {
                        ModelState.AddModelError("Chofer." + f.Key, f.Value);
                        log.Debug(f.Value);
                    });
                    ModelState.AgregarErrores(resultadoChofer);
                    return false;
                }
                choferDto.Id = (resultadoChofer as ResultadoCrear).Id;
            }
            return true;
        }

        protected bool SetearTransportista(ref int transportistaId, int tipoComercialId, bool esTransportista, bool esTransportistaTramo2 = false)
        {
            var tipoComercial = servicio.ObtenerTipoComercial(tipoComercialId);
            if (tipoComercial.TransportistaEsProveedor && (transportistaId == 0))
            {
                log.Debug("El transportista es obligatorio para el tipo comercial");
                if (esTransportistaTramo2)
                    ModelState.AddModelError("TransportistaTramo2", string.Format(Textos.Error_Requerido, Textos.Transportista_Segundo_Tramo));
                else
                    ModelState.AddModelError("Transportista", string.Format(Textos.Error_Requerido, Textos.Transportista));
                return false;
            }

            if (!esTransportista)
            {
                var proveedor = servicio.ObtenerProveedor(transportistaId);
                if (proveedor == null)
                {
                    if (!esTransportistaTramo2)
                        ModelState.AddModelError("Transportista", string.Format(Textos.Error_ProveedorInvalido));
                    else
                        ModelState.AddModelError("TransportistaTramo2", string.Format(Textos.Error_ProveedorInvalido));
                    transportistaId = 0;
                    return false;
                }

                var transportista = servicio.ObtenerTransportistaPorCuit(proveedor.Cuil);
                if (transportista != null) //Transportista Existente
                {
                    transportistaId = transportista.Id;
                }
                else //Creo el nuevo transportista
                {
                    try
                    {
                        var resultadoTransportista = servicioComandos.Ejecutar(new CrearTransportista
                        {
                            Dto = new TransportistaDto
                            {
                                Cuit = proveedor.Cuil,
                                Domicilio = proveedor.Domicilio,
                                LocalidadId = proveedor.LocalidadId,
                                ProvinciaId = proveedor.ProvinciaId,
                                RazonSocial = proveedor.RazonSocial
                            }
                        });
                        if (resultadoTransportista.HayErrores)
                        {
                            resultadoTransportista.Errores.ToList().ForEach(f => ModelState.AddModelError("Transportista", f.Value));
                            ModelState.AgregarErrores(resultadoTransportista);
                            return false;
                        }
                        transportistaId = (resultadoTransportista as ResultadoCrear).Id;
                    }
                    catch
                    {
                        ModelState.AddModelError("Transportista", string.Format(Textos.Error_ProveedorInvalido));
                    }
                }
            }
            return true;
        }
    }
}