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
    public class ListarEstadoDeCalle : IConsulta<CallePorRecorridoDto>
    {
        public ListarEstadoDeCalle()
        {
        }

        private List<CallePorRecorridoDto> ListadoCamiones(DbContext contexto)
        {
          

            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var listadoCamiones = contexto.Set<CallePorRecorrido>()
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
                                  AsignadoEnPuestoComando = x.Recorrido.Calle != null,
                                  TipoCalle = x.Calle.TipoCalle,
                                  TipoVehiculo = x.Recorrido.TipoVehiculo,
                                  ColorFondo = x.Recorrido.Establecimiento != null && x.Recorrido.Establecimiento.EPA ? Constantes.ValoresPorDefecto.ColorFondoSojaEPA :  (x.Recorrido.Material.ColorFondo ?? x.CargaDeCupo.Material.ColorFondo),
                                  ColorTexto = x.Recorrido.Establecimiento != null && x.Recorrido.Establecimiento.EPA ? Constantes.ValoresPorDefecto.ColorTextoSojaEPA :  (x.Recorrido.Material.ColorTexto ?? x.CargaDeCupo.Material.ColorTexto),
                                  EsSojaEPA = x.Recorrido.Establecimiento != null && x.Recorrido.Establecimiento.EPA,
                                  EsSojaIMPO = x.Recorrido != null && x.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.CodigoSap != null ? x.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.CodigoSap == Constantes.ValoresPorDefecto.CodigoSapTPR  : 
                                               x.CargaDeCupo != null && x.CargaDeCupo.TitularCartaPorteCodigoSap != null ? x.CargaDeCupo.TitularCartaPorteCodigoSap == Constantes.ValoresPorDefecto.CodigoSapTPR : false
                              })
                              .OrderBy(q => q.FechaIngreso)
                              .ToList();

            var resultado = new List<CallePorRecorridoDto>();

            foreach (var item in listadoCamiones)
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
                    Escalable = item.TipoVehiculo == Dominio.Enums.TipoVehiculo.CamiónC
                    || item.TipoVehiculo == Dominio.Enums.TipoVehiculo.CamiónD
                    || item.TipoVehiculo == Dominio.Enums.TipoVehiculo.CamiónE,
                    ColorFondo = item.ColorFondo,
                    ColorTexto = item.ColorTexto,
                    EsSojaEPA = item.EsSojaEPA,
                    EsSojaIMPO = item.EsSojaIMPO
                };

                resultado.Add(callePorRecorrido);
            }

            return resultado;
        }

        public virtual List<CallePorRecorridoDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return ListadoCamiones(contexto);
            }
        }
    }
}