using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerDatosEmbarcandoPuerto : IConsultaEscalar<ReportePesadaDto>
    {
        private readonly string balanza;

        public ObtenerDatosEmbarcandoPuerto(string balanza)
        {
            this.balanza = balanza;
        }

        public ReportePesadaDto Ejecutar(DbContext contexto)
        {

            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var reporteCargas = (from balanzadas in contexto.Set<Balanzada>()
                                 join carga in contexto.Set<Carga>() on
                                            new { key1 = balanzadas.CargaInicial_Id, key2 = balanzadas.CargaInicial_NumeroBalanza } equals new { key1 = carga.Id, key2 = carga.NumeroBalanza }
                                                                            
                                 where balanzadas.CargaInicial_NumeroBalanza == balanza && carga.CargaOpuesta_Id == null
                                 group new { carga, balanzadas } by carga into g
                                 orderby g.Key.Id descending
                                 select new ReportePesadaDto()
                                 {
                                     Fecha = g.Key.Fecha,
                                     Bodega = g.Key.Bodega.Nombre,
                                     Commodity = g.Key.Material.Descripcion,
                                     Destino = g.Key.Destino.Nombre,
                                     Exportador = g.Key.Exportador.Nombre,
                                     IdCarga = g.Key.Id,
                                     NumeroBalanza = g.Key.NumeroBalanza,
                                     PesoProgramado = g.Key.PesoProgramado,
                                     TotalEmbarcado = g.Sum(x => x.balanzadas.PesoNeto),
                                     Vapor = g.Key.Vapor.Nombre,
                                 }
                                
                                 );
 
            return reporteCargas.FirstOrDefault();
        }
    }
}
