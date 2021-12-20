using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarListadoCamiones : IConsulta<ListadoCamionesDto>
    {
        private readonly List<int> centros;
        private readonly DateTime fechaInicio;
        private readonly DateTime fechaFin;
        private readonly List<int> tiposComerciales;
        private readonly List<int> materiales;
        private readonly bool incluirRechazados;

        public ListarListadoCamiones(List<int> centros, DateTime fechaInicio, DateTime fechaFin, List<int> tiposComerciales, List<int> materiales, bool incluirRechazados)
        {
            this.centros = centros;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaFin;
            this.tiposComerciales = tiposComerciales;
            this.materiales = materiales;
            this.incluirRechazados = incluirRechazados;
        }

        private static List<ListadoCamionesDto> ListadoCamiones(DbContext contexto, List<int> centros, DateTime fechaInicio, DateTime fechaFin, List<int> tiposComerciales, List<int> materiales, bool incluirRechazados)
        {


            ((IObjectContextAdapter) contexto).ObjectContext.CommandTimeout = 180;
            var resultado = from recorrido in contexto.Set<Recorrido>()
                            where centros.Contains(recorrido.Centro.Id) && recorrido.Terminado && recorrido.Vehiculo != null &&
                                                          recorrido.FechaEgreso >= fechaInicio &&
                                                          recorrido.FechaEgreso <= fechaFin &&
                                                          tiposComerciales.Contains(recorrido.TipoComercial.Id) &&
                                                          !materiales.Contains(recorrido.Material.Id) &&
                                                          (incluirRechazados || (!incluirRechazados && !recorrido.Rechazado))

                            orderby recorrido.Id
                            select new ListadoCamionesDto
                                {
                                    NroCartaDePorte = recorrido.Vehiculo.CartaPorte.NroCartaPorte,
                                    FechaDeCarga = recorrido.Vehiculo.CartaPorte.FechaEmision,
                                    LocalidadDeOrigen = recorrido.Vehiculo.CartaPorte.Procedencia.Descripcion,
                                    ProvinciaDeOrigen = recorrido.Vehiculo.CartaPorte.Procedencia.Provincia.Descripcion,
                                    LocalidadDeDestino = recorrido.Vehiculo.CartaPorte.CentroDestino != null ? recorrido.Vehiculo.CartaPorte.CentroDestino.Localidad.Descripcion : recorrido.Vehiculo.CartaPorte.ClienteDestino.Localidad,
                                    ProvinciaDeDestino = recorrido.Vehiculo.CartaPorte.CentroDestino != null ? recorrido.Vehiculo.CartaPorte.CentroDestino.Provincia.DescripcionCollate_CI_AS : recorrido.Vehiculo.CartaPorte.ClienteDestino.Provincia,
                                    CuitTransportista = recorrido.Vehiculo.CartaPorte.Transportista.Cuit,
                                    NombreTransportista = recorrido.Vehiculo.CartaPorte.Transportista.RazonSocial,
                                    CuilDelChofer = recorrido.Vehiculo.CartaPorte.Chofer.Cuil,
                                    NombreChofer = recorrido.Vehiculo.CartaPorte.Chofer.Nombre
                                };

            return resultado.ToList();
        }

        public virtual List<ListadoCamionesDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required,new TransactionOptions {IsolationLevel = IsolationLevel.ReadUncommitted}))
            {
                return ListadoCamiones(contexto, centros, fechaInicio, fechaFin, tiposComerciales, materiales,
                                       incluirRechazados);
            }
        }
    }
}
