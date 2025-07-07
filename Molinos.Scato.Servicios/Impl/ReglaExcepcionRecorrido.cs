using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Servicios.Impl
{
    public class ReglaExcepcionRecorrido : IReglaExcepcionTasaMunicipal
    {
        private readonly IRepositorio _repositorio;
        private readonly IServicioRepositorio _srvRepositorio;
        private readonly ILogger _log;
        public ReglaExcepcionRecorrido(IRepositorio repositorio, IServicioRepositorio servicioRepositorio, ILogger logger)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio), "El servicio repositorio no puede ser nulo.");
            _srvRepositorio = servicioRepositorio ?? throw new ArgumentNullException(nameof(servicioRepositorio), "El servicio repositorio no puede ser nulo.");
            _log = logger ?? throw new ArgumentNullException(nameof(logger), "El logger no puede ser nulo.");
        }
        public bool Aplica(DatosExcepcionTasaMunicipal datos)
        {
            bool tieneExcepcion = false;
            if (datos.EsValidacionAlIngreso)
            {
                tieneExcepcion = _repositorio.Existe<MaterialPorCentro>(m => m.Material.Id == datos.MaterialId && m.Centro.Id == datos.CentroId && m.ImprimeReciboMunicipal == false);
                if (tieneExcepcion)
                    return tieneExcepcion;
                else
                    tieneExcepcion = _repositorio.Existe<LogExceptuadosTicketMunicipal>(r => r.Patente == datos.Patente && r.Material.Id == datos.MaterialId && r.PagaTicketMunicipal == false && r.NumeroDocumentoIngreso == datos.Ctg);
                
                return tieneExcepcion;
            }
            else
            {
                _log.Info($"Validando excepcion de recorrido para InstanceId: {datos.InstanceId}");
                var tieneExcepcionDeMaterial = !_srvRepositorio.MaterialImprimeReciboMunicipal(datos.InstanceId ?? Guid.Empty);
                _log.Info($"Tiene excepcion de material: {tieneExcepcionDeMaterial} para InstanceId: {datos.InstanceId}");
                if (tieneExcepcionDeMaterial)
                {
                    tieneExcepcion = true;
                }
                else
                {
                    var tieneExcepcionDeReccorrido = !_srvRepositorio.LogPagaTicketMunicipal(datos.InstanceId ?? Guid.Empty) ?? false;
                    _log.Info($"Tiene excepcion de recorrido: {tieneExcepcionDeReccorrido} para InstanceId: {datos.InstanceId}");
                    if (tieneExcepcionDeReccorrido)
                    {
                        tieneExcepcion = true;
                    }
                }

                if (tieneExcepcion)
                {
                    var pago = _repositorio.Obtener<PagosTasaMunicipal>(p => p.IdInstance == datos.InstanceId);
                    if (pago != null)
                    {
                        pago.Disponible = true;
                        pago.IdInstance = null;
                        _repositorio.GuardarCambios();
                    }
                }

                return tieneExcepcion;
            }
               
        } 

        public Dictionary<TipoValidacionPagoTasaMunicipal, bool> ValidarExcepcion(DatosExcepcionTasaMunicipal datos)
        {
            return new Dictionary<TipoValidacionPagoTasaMunicipal, bool>
            {
                { TipoValidacionPagoTasaMunicipal.Abonado, true }
            };
        }
    }
}
