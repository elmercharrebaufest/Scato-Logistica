using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
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
                              .Where(x => x.FechaEgreso.Equals(null))
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
                                  EsDemorado = x.Recorrido != null && x.Recorrido.VehiculoDemorado,
                                  EsSojaEUDR = x.Recorrido != null && x.Recorrido.TipoVariedad != null && x.Recorrido.TipoVariedad.Codigo == Constantes.TipoVariedadMaterial.EUDR,
                                  EsSojaIMPO = x.Recorrido != null && x.Recorrido.TipoVariedad != null && x.Recorrido.TipoVariedad.Codigo == Constantes.TipoVariedadMaterial.Importacion,
                                  EsSojaEPAyEUDR = x.Recorrido != null && x.Recorrido.TipoVariedad != null && x.Recorrido.TipoVariedad.Codigo == Constantes.TipoVariedadMaterial.EPAyEUDR
                              })
                              .OrderBy(q => q.FechaIngreso)
                              .ToList();

            return listadoCamiones;
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