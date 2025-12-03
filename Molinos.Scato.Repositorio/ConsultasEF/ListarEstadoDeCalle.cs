using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarEstadoDeCalle : IConsulta<CallePorRecorridoListadoCamionesDto>
    {
        public ListarEstadoDeCalle()
        {
        }

        public virtual List<CallePorRecorridoListadoCamionesDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return ListadoCamiones(contexto);
            }
        }

        private List<CallePorRecorridoListadoCamionesDto> ListadoCamiones(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var query = from cpr in contexto.Set<CallePorRecorrido>()
                        where cpr.FechaEgreso == null
                        let instanceId = cpr.Recorrido != null 
                            ? cpr.Recorrido.InstanciaWorkflow 
                            : (cpr.CargaDeCupo.Recorrido != null 
                                ? cpr.CargaDeCupo.Recorrido.InstanciaWorkflow 
                                : (Guid?)null)
                        let tienePago = contexto.Set<PagosTasaMunicipal>().Any(p => p.IdInstance.HasValue && p.IdInstance == instanceId)
                        let pagoTasaMunicipalAdeudado = cpr.Recorrido != null
                            ? !cpr.Recorrido.IngresoContingenciaPagoMunicipal
                            : (cpr.CargaDeCupo.Recorrido != null && !cpr.CargaDeCupo.Recorrido.IngresoContingenciaPagoMunicipal)
                        let fueExceptuado = cpr.Recorrido != null
                            ? (cpr.Recorrido.RecorridoTasaMunicipal != null && cpr.Recorrido.RecorridoTasaMunicipal.Exceptuado)
                            : (cpr.CargaDeCupo.Recorrido != null 
                                ? (cpr.CargaDeCupo.Recorrido.RecorridoTasaMunicipal != null 
                                    && cpr.CargaDeCupo.Recorrido.RecorridoTasaMunicipal.Exceptuado) 
                                : false)
                        select new CallePorRecorridoListadoCamionesDto
                        {
                            Id = cpr.Id,
                            Calidad = cpr.Recorrido.CaracteristicasAnalizadasList.FirstOrDefault().Calidad,
                            RecorridoMaterialId = cpr.Recorrido.Material.Id,
                            CargaCupoMaterialId = cpr.CargaDeCupo.Material.Id,
                            RecorridoMaterialDescripcion = cpr.Recorrido.Material.Descripcion,
                            CargaCupoMaterialDescripcion = cpr.CargaDeCupo.Material.Descripcion,
                            RecorridoPatente = cpr.Recorrido.Patente,
                            CargaDeCupoPatente = cpr.CargaDeCupo.Patente,
                            CargaDeCupoRecorridoPatente = cpr.CargaDeCupo.Recorrido.Patente,
                            CalleId = cpr.Calle.Id,
                            FechaIngreso = cpr.FechaIngeso,
                            UltimoDeLaFila = cpr.UltimoDeLaFila,
                            Rechazado = cpr.Recorrido.Rechazado,
                            AsignadoEnPuestoComando = cpr.Recorrido != null && cpr.Recorrido.Calle != null,
                            TipoCalle = cpr.Calle.TipoCalle,
                            TipoVehiculo = cpr.Recorrido.TipoVehiculo,
                            EsSojaEPA = cpr.Recorrido != null 
                                && cpr.Recorrido.TipoVariedad != null 
                                && cpr.Recorrido.TipoVariedad.Codigo == Constantes.TipoVariedadMaterial.EPA,
                            MaterialColorFondo = cpr.Recorrido.Material.ColorFondo,
                            MaterialColorTexto = cpr.Recorrido.Material.ColorTexto,
                            CargaCupoColorFondo = cpr.CargaDeCupo.Material.ColorFondo,
                            CargaCupoColorTexto = cpr.CargaDeCupo.Material.ColorTexto,
                            RecorridoCodigoSAP = cpr.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.CodigoSap,
                            CargaDeCupoCodigoSAP = cpr.CargaDeCupo.TitularCartaPorteCodigoSap,
                            EsDemorado = cpr.Recorrido != null 
                                ? cpr.Recorrido.VehiculoDemorado 
                                : (cpr.CargaDeCupo.Recorrido != null && cpr.CargaDeCupo.Recorrido.VehiculoDemorado),
                            EsSojaEUDR = cpr.Recorrido != null 
                                && cpr.Recorrido.TipoVariedad != null 
                                && cpr.Recorrido.TipoVariedad.Codigo == Constantes.TipoVariedadMaterial.EUDR,
                            EsSojaIMPO = cpr.Recorrido != null 
                                && cpr.Recorrido.TipoVariedad != null 
                                && cpr.Recorrido.TipoVariedad.Codigo == Constantes.TipoVariedadMaterial.Importacion,
                            EsSojaEPAyEUDR = cpr.Recorrido != null 
                                && cpr.Recorrido.TipoVariedad != null 
                                && cpr.Recorrido.TipoVariedad.Codigo == Constantes.TipoVariedadMaterial.EPAyEUDR,
                            IdRecorrido = cpr.Recorrido != null ? cpr.Recorrido.Id : (int?)null,
                            PagoTasaMunicipalAdeudado = pagoTasaMunicipalAdeudado && !tienePago && !fueExceptuado,
                            InstanceId = instanceId ?? Guid.Empty
                        };

            return query.OrderBy(x => x.FechaIngreso).ToList();
        }
    }
}