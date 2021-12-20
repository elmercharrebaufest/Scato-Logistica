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
    public class ListarMuestraEnvioACamaraAuditoriaParaImpresionConsulta : IConsultaPaginada<MuestraEnvioACamaraAuditoriaDto>
    {
        private readonly int loteId;
        private Paginacion paginacion;

        public ListarMuestraEnvioACamaraAuditoriaParaImpresionConsulta(int loteId, Paginacion paginacion = null)
        {
            this.loteId = loteId;
            this.paginacion = paginacion;
            
        }

        public ListaPaginada<MuestraEnvioACamaraAuditoriaDto> Ejecutar(DbContext contexto)
        {

            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var muestras = (from muestra in contexto.Set<MuestraEnvioACamaraAuditoria>()
                    join conversionMaterial in contexto.Set<ConversionMaterial>().DefaultIfEmpty() on new { Key1 = muestra.Recorrido.Material.Id, Key2 = muestra.LoteAuditoria.Camara.Id } equals new { Key1 = conversionMaterial.Material.Id, Key2 = conversionMaterial.Camara.Id } into conversionMaterialJoined
                    from conversionMaterial in conversionMaterialJoined.DefaultIfEmpty()
                    join conversionCentro in contexto.Set<ConversionCentro>().DefaultIfEmpty() on new { Key1 = muestra.Recorrido.Centro.Id, Key2 = muestra.LoteAuditoria.Camara.Id } equals new { Key1 = conversionCentro.Centro.Id, Key2 = conversionCentro.Camara.Id } into conversionCentroJoined
                    from conversionCentro in conversionCentroJoined.DefaultIfEmpty()
                    join conversionGrupo in contexto.Set<ConversionGrupo>().DefaultIfEmpty() on new { Key1 = muestra.Recorrido.Material.Id, Key2 = muestra.LoteAuditoria.Camara.Id } equals new { Key1 = conversionGrupo.Material.Id, Key2 = conversionGrupo.Camara.Id } into conversionGrupoJoined
                    from conversionGrupo in conversionGrupoJoined.DefaultIfEmpty()

                    join remito in contexto.Set<Remito>() on muestra.Recorrido.Id equals remito.Recorrido.Id into
                                 remitoJoined
                    from remito in remitoJoined.DefaultIfEmpty()
                    join caracteristicasAnalizadas in contexto.Set<CaracteristicasAnalizadas>().DefaultIfEmpty() on muestra.Recorrido.Id equals caracteristicasAnalizadas.Recorrido.Id into caracteristicasAnalizadasJoined
                            from caracteristicasAnalizadas in caracteristicasAnalizadasJoined.DefaultIfEmpty()
                    where muestra.LoteAuditoria.Id == loteId
                    orderby muestra.Recorrido.Id
                    select new MuestraEnvioACamaraAuditoriaDto
                    {
                        Id = muestra.Id,
                        CamaraId = muestra.LoteAuditoria.Camara.Id,
                        CentroId = muestra.Recorrido.Centro.Id,
                        Material = muestra.Recorrido.Material.Descripcion,
                        FechaDescarga = muestra.Recorrido.PesoTaraFecha.HasValue ? muestra.Recorrido.PesoTaraFecha.Value : muestra.Recorrido.FechaInicio,
                        PesoNeto = (muestra.Recorrido.PesoBruto ?? 0) - (muestra.Recorrido.PesoTara ?? 0),
                        NroCartaPorte = remito != null ? remito.OrdenRemito : muestra.Recorrido.NumeroDocumentoIngreso,
                        FechaCartaPorte = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.FechaCP : (remito != null ? remito.FechaOD : DateTime.Now),

                        Proveedor = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Descripcion : (remito != null ? remito.ProveedorOrigen.Descripcion : ""),
                        ProveedorCodigoSap = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.CodigoSap : (remito != null ? remito.ProveedorOrigen.CodigoSap : ""),
                        Vendedor = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.Destinatario.Descripcion : "",
                        Corredor = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.Corredor.Descripcion : "",
                        Localidad = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.Procedencia.Descripcion : (remito != null ? remito.Procedencia.Descripcion : ""),

                        Patente = muestra.Recorrido.Patente,
                        WorkflowInstanceId = muestra.Recorrido.InstanciaWorkflow,
                        EstadoMuestra = EstadoMuestra.Pendiente,
                        TieneAnalisisInterno = muestra.Recorrido.AnalisisDeCalidad != null,
                        CamaraDesc = muestra.LoteAuditoria.Camara.Descripcion,
                        CodigoDeCamara = conversionCentro.CodigoCamara,
                        NumeroVehiculo = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.NumeroVehiculo : 1,
                        CamaraFormatoDeArchivo = muestra.LoteAuditoria.Camara.FormatoDeArchivo != null ? muestra.LoteAuditoria.Camara.FormatoDeArchivo.Value : CamaraFormatoDeArchivo.NoEspecificado,
                        ValorCamara = muestra.ValorCamara,
                        ValorCalado = caracteristicasAnalizadas != null && caracteristicasAnalizadas.Grado.HasValue ? caracteristicasAnalizadas.Grado : 0,
                        Perito = muestra.Recorrido.Calado.Usuario
                    });
            
            var itemsTotales = muestras.Count();
            if (paginacion == null)
            {
                paginacion = new Paginacion(itemsPorPagina: itemsTotales);
            }
            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<MuestraEnvioACamaraAuditoriaDto>(paginacion.OrdenarPor);
                muestras = paginacion.DireccionOrden == DirOrden.Asc
                                 ? muestras.OrderBy(selectorOrden)
                                 : muestras.OrderByDescending(selectorOrden);
            }
            
            muestras = muestras.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<MuestraEnvioACamaraAuditoriaDto>(muestras.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
    }
}
