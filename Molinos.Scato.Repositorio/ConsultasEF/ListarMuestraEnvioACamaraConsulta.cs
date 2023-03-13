using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarMuestraEnvioACamaraConsulta : IConsulta<MuestraEnvioACamaraDto>
    {
        private readonly int centroId;
        private readonly string numeroDeMuestra;
        private readonly int loteId;
        private readonly bool listarRechazadosYNoTerminados;
        private readonly bool soloPendientes;
        private readonly bool incluirPreLote;
        private readonly Paginacion paginacion;

        public ListarMuestraEnvioACamaraConsulta(int centroId, string numeroDeMuestra = "", int loteId = 0,bool incluirPreLote = false, bool listarRechazadosYNoTerminados = false, bool soloPendientes = false, Paginacion paginacion = null)
        {
            this.centroId = centroId;
            this.numeroDeMuestra = numeroDeMuestra;
            this.loteId = loteId;
            this.listarRechazadosYNoTerminados = listarRechazadosYNoTerminados;
            this.soloPendientes = soloPendientes;
            this.incluirPreLote = incluirPreLote;
            this.paginacion = paginacion;
        }

        public List<MuestraEnvioACamaraDto> Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var resultado = (from muestra in contexto.Set<MuestraEnvioACamara>()

                             join rec in contexto.Set<Recorrido>() on muestra.Calado.Id equals
                                 rec.Calado.Id into recJoined
                             from rec in recJoined.DefaultIfEmpty()

                             join remito in contexto.Set<Remito>() on rec.Id equals remito.Recorrido.Id into
                                 remitoJoined
                             from remito in remitoJoined.DefaultIfEmpty()

                             where
                                 (loteId > 0 && muestra.Lote.Id == loteId)
                                 || (loteId == 0 && rec.Centro.Id == centroId
                                 && (muestra.EsPreLote == incluirPreLote || muestra.EsPreLote == null)
                                 && (muestra.EstadoMuestra == EstadoMuestra.Pendiente || !soloPendientes)
                                 && (rec.Terminado || listarRechazadosYNoTerminados)
                                 && (!rec.Rechazado || listarRechazadosYNoTerminados))
                             orderby muestra.Id descending
                             select new MuestraEnvioACamaraDto
                                 {
                                     Id = muestra.Id,
                                     NombreUsuario = muestra.NombreUsuario,
                                     CentroId = muestra.Centro.Id,
                                     CaladoId = muestra.Calado.Id,
                                     CamaraId = muestra.Camara.Id,
                                     NroMuestra = muestra.NroMuestra,
                                     NroMuestraTerceros = muestra.NroMuestraTerceros,
                                     Material = rec.Material.Descripcion,
                                     FechaDescarga = muestra.FechaDescarga,
                                     PesoNeto = muestra.PesoNeto,
                                     NroCartaPorte = rec.Vehiculo != null ? rec.Vehiculo.CartaPorte.NroCartaPorte : (remito != null ? remito.OrdenRemito : ""),
                                     FechaCartaPorte = rec.Vehiculo != null ? rec.Vehiculo.CartaPorte.FechaCP : (remito != null ? remito.FechaOD : DateTime.Now),
                                     Proveedor = rec.Vehiculo != null ? rec.Vehiculo.CartaPorte.TitularCartaPorte.Descripcion : (remito != null ? remito.ProveedorOrigen.Descripcion : ""),
                                     ProveedorCodigoSap = rec.Vehiculo != null ? rec.Vehiculo.CartaPorte.TitularCartaPorte.CodigoSap : (remito != null ? remito.ProveedorOrigen.CodigoSap : ""),
                                     Vendedor = rec.Vehiculo != null ? rec.Vehiculo.CartaPorte.Destinatario.Descripcion : "",
                                     Corredor = rec.Vehiculo != null ? rec.Vehiculo.CartaPorte.Corredor.Descripcion : "",
                                     Localidad = rec.Vehiculo != null? rec.Vehiculo.CartaPorte.Procedencia.Descripcion : (remito != null ? remito.Procedencia.Descripcion : ""),
                                     Patente = rec.Patente,
                                     WorkflowInstanceId = rec.InstanciaWorkflow,
                                     EstadoMuestra = muestra.EstadoMuestra,
                                     TieneAnalisisInterno = muestra.TieneAnalisisInterno,
                                     CamaraDesc = muestra.Camara.Descripcion,
                                     EsPreLote = muestra.EsPreLote
                                 }).Distinct();

            if (!String.IsNullOrEmpty(numeroDeMuestra))
            {
                resultado = resultado.Where(x => x.NroMuestra == numeroDeMuestra);
            }

            if (paginacion != null)
            {
                var selectorOrden = Expresiones.Propiedad<MuestraEnvioACamaraDto>(paginacion.OrdenarPor);
                resultado = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultado.OrderBy(selectorOrden)
                                 : resultado.OrderByDescending(selectorOrden);
            }
            return resultado.ToList();
        }
    }
}
