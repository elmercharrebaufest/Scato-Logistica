using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
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

        private List<CallePorRecorridoListadoCamionesDto> ListadoCamiones(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var listadoCamiones = new List<CallePorRecorridoListadoCamionesDto>();
            listadoCamiones = contexto.Set<CallePorRecorrido>()
                              .Where(x => x.FechaEgreso == null)
                              .Select(x => new CallePorRecorridoListadoCamionesDto
                              {
                                  Id = x.Id,
                                  Calidad = x.Recorrido.CaracteristicasAnalizadasList.FirstOrDefault().Calidad,
                                  RecorridoMaterialId = x.Recorrido.Material.Id,
                                  CargaCupoMaterialId = x.CargaDeCupo.Material.Id,
                                  RecorridoMaterialDescripcion = x.Recorrido.Material.Descripcion,
                                  CargaCupoMaterialDescripcion = x.CargaDeCupo.Material.Descripcion,
                                  RecorridoPatente = x.Recorrido.Patente,
                                  CargaDeCupoPatente = x.CargaDeCupo.Patente,
                                  CargaDeCupoRecorridoPatente = x.CargaDeCupo.Recorrido.Patente,
                                  CalleId = x.Calle.Id,
                                  FechaIngreso = x.FechaIngeso,
                                  UltimoDeLaFila = x.UltimoDeLaFila,
                                  Rechazado = x.Recorrido.Rechazado,
                                  AsignadoEnPuestoComando = x.Recorrido != null && x.Recorrido.Calle != null,
                                  TipoCalle = x.Calle.TipoCalle,
                                  TipoVehiculo = x.Recorrido.TipoVehiculo,
                                  EsSojaEPA = x.Recorrido != null && x.Recorrido.TipoVariedad != null && x.Recorrido.TipoVariedad.Codigo == Constantes.TipoVariedadMaterial.EPA,
                                  MaterialColorFondo = x.Recorrido.Material.ColorFondo,
                                  MaterialColorTexto = x.Recorrido.Material.ColorTexto,
                                  CargaCupoColorFondo = x.CargaDeCupo.Material.ColorFondo,
                                  CargaCupoColorTexto = x.CargaDeCupo.Material.ColorTexto,
                                  RecorridoCodigoSAP =  x.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.CodigoSap,
                                  CargaDeCupoCodigoSAP = x.CargaDeCupo.TitularCartaPorteCodigoSap,
                                  EsDemorado = x.Recorrido != null ?  x.Recorrido.VehiculoDemorado : (x.CargaDeCupo.Recorrido != null && x.CargaDeCupo.Recorrido.VehiculoDemorado),
                                  EsSojaEUDR = x.Recorrido != null && x.Recorrido.TipoVariedad != null && x.Recorrido.TipoVariedad.Codigo == Constantes.TipoVariedadMaterial.EUDR,
                                  EsSojaIMPO = x.Recorrido != null && x.Recorrido.TipoVariedad != null && x.Recorrido.TipoVariedad.Codigo == Constantes.TipoVariedadMaterial.Importacion,
                                  EsSojaEPAyEUDR = x.Recorrido != null && x.Recorrido.TipoVariedad != null && x.Recorrido.TipoVariedad.Codigo == Constantes.TipoVariedadMaterial.EPAyEUDR,
                                  IdRecorrido = x.Recorrido != null ? x.Recorrido.Id : (int?)null,
                                  PagoTasaMunicipalAdeudado = x.Recorrido != null
                                      ? !x.Recorrido.PagoTasaMunicipalInformado && !x.Recorrido.IngresoContingenciaPagoMunicipal
                                      : (x.CargaDeCupo.Recorrido != null 
                                          && !x.CargaDeCupo.Recorrido.PagoTasaMunicipalInformado
                                          && !x.CargaDeCupo.Recorrido.IngresoContingenciaPagoMunicipal),
                                  InstanceId = x.Recorrido != null ? x.Recorrido.InstanciaWorkflow : x.CargaDeCupo.Recorrido != null  ?  x.CargaDeCupo.Recorrido.InstanciaWorkflow : Guid.Empty

                              })
                              .OrderBy(q => q.FechaIngreso)
                              .ToList();

            var listadoPagos = contexto.Set<PagosTasaMunicipal>().ToList();

            var resultado = from cpr in listadoCamiones
                            join pago in listadoPagos
                                on cpr.InstanceId equals pago.IdInstance
                                into pagosGroup
                            from pago in pagosGroup.DefaultIfEmpty()
                            select new CallePorRecorridoListadoCamionesDto
                            {
                                Id = cpr.Id,
                                Calidad = cpr.Calidad,
                                RecorridoMaterialId = cpr.RecorridoMaterialId,
                                CargaCupoMaterialId = cpr.CargaCupoMaterialId,
                                RecorridoMaterialDescripcion = cpr.RecorridoMaterialDescripcion,
                                CargaCupoMaterialDescripcion = cpr.CargaCupoMaterialDescripcion,
                                RecorridoPatente = cpr.RecorridoPatente,
                                CargaDeCupoPatente = cpr.CargaDeCupoPatente,
                                CargaDeCupoRecorridoPatente = cpr.CargaDeCupoRecorridoPatente,
                                CalleId = cpr.CalleId,
                                FechaIngreso = cpr.FechaIngreso,
                                UltimoDeLaFila = cpr.UltimoDeLaFila,
                                Rechazado = cpr.Rechazado,
                                AsignadoEnPuestoComando =cpr.AsignadoEnPuestoComando,
                                TipoCalle = cpr.TipoCalle,
                                TipoVehiculo = cpr.TipoVehiculo,
                                EsSojaEPA = cpr.EsSojaEPA,
                                MaterialColorFondo = cpr.MaterialColorFondo,
                                MaterialColorTexto = cpr.MaterialColorTexto,
                                CargaCupoColorFondo = cpr.CargaCupoColorFondo,
                                CargaCupoColorTexto = cpr.CargaCupoColorTexto,
                                RecorridoCodigoSAP = cpr.RecorridoCodigoSAP,
                                CargaDeCupoCodigoSAP = cpr.CargaDeCupoCodigoSAP,
                                EsDemorado = cpr.EsDemorado,
                                EsSojaEUDR = cpr.EsSojaEUDR,
                                EsSojaIMPO = cpr.EsSojaIMPO,
                                EsSojaEPAyEUDR = cpr.EsSojaEPAyEUDR,
                                IdRecorrido = cpr.IdRecorrido,
                                PagoTasaMunicipalAdeudado = pago == null ? true : false,
                                InstanceId = cpr.InstanceId

                            };
            return resultado.OrderBy(q => q.FechaIngreso).ToList();
        }

        public virtual List<CallePorRecorridoListadoCamionesDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return ListadoCamiones(contexto);
            }
        }
    }
}