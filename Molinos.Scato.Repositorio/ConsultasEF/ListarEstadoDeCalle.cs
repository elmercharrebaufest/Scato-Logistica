using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Transactions;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarEstadoDeCalle : IConsulta<CallePorRecorridoDto>
    {
        public ListarEstadoDeCalle()
        {
        }

        private static List<CallePorRecorridoDto> ListadoCamiones(DbContext contexto)
        {
            ((IObjectContextAdapter) contexto).ObjectContext.CommandTimeout = 180;
            var resultado = from x in contexto.Set<CallePorRecorrido>()
                            where x.FechaEgreso == null
                            select new CallePorRecorridoDto
                            {
                                Id = x.Id,
                                Calidad = (int?)x.Recorrido.CaracteristicasAnalizadasList.FirstOrDefault().Calidad ?? 0,
                                MaterialId = x.Recorrido != null ? x.Recorrido.Material.Id : x.CargaDeCupo != null && x.CargaDeCupo.Material != null ? x.CargaDeCupo.Material.Id : 0,
                                MaterialDesc = x.Recorrido != null ? x.Recorrido.Material.Descripcion : x.CargaDeCupo != null && x.CargaDeCupo.Material != null ? x.CargaDeCupo.Material.Descripcion : "Desconocido",
                                Patente = x.Recorrido != null ? x.Recorrido.Patente : 
                                (x.CargaDeCupo != null && x.CargaDeCupo.Patente  != null ? 
                                    x.CargaDeCupo.Patente : 
                                        (x.CargaDeCupo != null && x.CargaDeCupo.Recorrido != null ? 
                                            x.CargaDeCupo.Recorrido.Patente : 
                                                (x.CargaDeCupo != null ? 
                                                    x.CargaDeCupo.Numero : SqlFunctions.StringConvert((double)x.Id)))),
                                CalleId = x.Calle.Id,
                                FechaIngeso = x.FechaIngeso,
                                UltimoDeLaFila = x.UltimoDeLaFila,
                                Rechazado = x.Recorrido != null & x.Recorrido.Rechazado,
                                AsignadoEnPuestoComando = x.Recorrido != null && x.Recorrido.Calle != null,
                                TipoCalle = x.Calle.TipoCalle,
                                Escalable = x.Recorrido != null && (x.Recorrido.TipoVehiculo == Dominio.Enums.TipoVehiculo.CamiónC ||
                                            x.Recorrido.TipoVehiculo == Dominio.Enums.TipoVehiculo.CamiónD ||
                                            x.Recorrido.TipoVehiculo == Dominio.Enums.TipoVehiculo.CamiónE)
                            };

            return resultado.ToList();
        }

        public virtual List<CallePorRecorridoDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required,new TransactionOptions {IsolationLevel = IsolationLevel.ReadUncommitted}))
            {
                return ListadoCamiones(contexto);
            }
        }
    }
}
