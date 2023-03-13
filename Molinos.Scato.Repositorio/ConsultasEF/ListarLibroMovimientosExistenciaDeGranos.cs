using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Transactions;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarLibroMovimientosExistenciaDeGranos : IConsulta<ImpImpresionGenericaDto>
    {
        private readonly int centro;
        private readonly DateTime fechaInicio;
        private readonly DateTime fechaFin;
        private readonly string codigoSapFirma;
        private readonly IList<int> materiales;
        private readonly decimal stockInicial;

        public ListarLibroMovimientosExistenciaDeGranos(int centro, IList<int> materiales, decimal stockInicial, DateTime fechaInicio, DateTime fechaFin, string codigoSapFirma)
        {
            this.centro = centro;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaFin;
            this.codigoSapFirma = codigoSapFirma;
            this.materiales = materiales;
            this.stockInicial = stockInicial;
        }

        private static List<ImpImpresionGenericaDto> LibroMovimientosExistenciaDeGranos(DbContext contexto, int centro, IList<int> material, decimal stockInicial, DateTime fechaInicio, DateTime fechaFin, string codigoSapFirma)
        {


            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var molinos = contexto.Set<Proveedor>().SingleOrDefault(x => x.CodigoSap == codigoSapFirma);
            var resultado = from recorrido in contexto.Set<Recorrido>()
                            join remito in contexto.Set<Remito>().DefaultIfEmpty() on recorrido.Id equals remito.Recorrido.Id into remitoJoined
                            from remito in remitoJoined.DefaultIfEmpty()
                            where centro == recorrido.Centro.Id && recorrido.Terminado && !recorrido.Rechazado &&
                                                           (recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso ? recorrido.PesoTaraFecha <= fechaFin : recorrido.PesoBrutoFecha <= fechaFin) && (recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso ? recorrido.PesoTaraFecha >= fechaInicio : recorrido.PesoBrutoFecha >= fechaInicio)
                                                           && material.Any(x => x == recorrido.Material.Id)
                            orderby recorrido.Id
                            select new
                            {
                                Material = recorrido.Material != null ? recorrido.Material.Descripcion : "",
                                MaterialDescCorta = recorrido.Material != null ? recorrido.Material.DescripcionCorta : "",
                                MaterialCodigoONCCA = recorrido.Material != null ? recorrido.Material.CodigoONCCA : "",
                                FechaEmision = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso ? recorrido.PesoTaraFecha.Value : recorrido.PesoBrutoFecha.Value,
                                TipoDeComprobanteONCCA = recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte ? "01" : (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.Remito ? "02" : "04"),
                                TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte ? Textos.CartaPorte : (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.Remito ? Textos.Remito : Textos.Otros),
                                NumeroDeDocumentoDeIngreso = recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.Remito && remito != null ? remito.OrdenRemito : recorrido.NumeroDocumentoIngreso,
                                CuitDestinatario = recorrido.Vehiculo.CartaPorte.Destinatario != null ? recorrido.Vehiculo.CartaPorte.Destinatario.Cuil : (recorrido.Vehiculo.CartaPorte.DestinatarioCliente != null ? recorrido.Vehiculo.CartaPorte.DestinatarioCliente.Cuit : (remito != null ? remito.ProveedorOrigen.Cuil : "")),
                                CuitTitularCP = recorrido.Vehiculo.CartaPorte.TitularCartaPorte != null ? recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Cuil : "",
                                CuitRtteComercial = recorrido.Vehiculo.CartaPorte.RtteComercial != null ? recorrido.Vehiculo.CartaPorte.RtteComercial.Cuil : (remito != null ? remito.ProveedorOrigen.Cuil : ""),
                                Destinatario = recorrido.Vehiculo.CartaPorte.Destinatario != null ? recorrido.Vehiculo.CartaPorte.Destinatario.Descripcion : (recorrido.Vehiculo.CartaPorte.DestinatarioCliente != null ? recorrido.Vehiculo.CartaPorte.DestinatarioCliente.Descripcion : (remito != null ? remito.ProveedorOrigen.Descripcion : "")),
                                TitularCP = recorrido.Vehiculo.CartaPorte.TitularCartaPorte != null ? recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Descripcion : "",
                                RtteComercial = recorrido.Vehiculo.CartaPorte.RtteComercial != null ? recorrido.Vehiculo.CartaPorte.RtteComercial.Descripcion : (remito != null ? remito.ProveedorOrigen.Descripcion : ""),
                                recorrido.Workflow.TipoDeWorkflow,
                                PesoNeto =  (decimal)(recorrido.PesoBruto ?? 0) - (recorrido.PesoTara ?? 0),
                                ObservacionesONCCA = "",
                                PesoNetoCalculado = (recorrido.PesoBruto ?? 0) - (recorrido.PesoTara ?? 0) - recorrido.DescuentoEnKgOncca,
                                CPE = recorrido.Vehiculo.CartaPorte.Cpe?? false,
                                CTG= recorrido.Vehiculo.CartaPorte.CTG,
                                CartaDePorte = recorrido.Vehiculo.CartaPorte.NroCartaPorte,
                                Sucursal = recorrido.Vehiculo.CartaPorte.Sucursal
                            };

            var ajustes = from ajusteStock in contexto.Set<AjusteDeStock>()
                          where (material.Any(x => x == ajusteStock.Material.Id) && ajusteStock.Fecha <= fechaFin && ajusteStock.Fecha >= fechaInicio && ajusteStock.Centro.Id == centro)
                          orderby ajusteStock.Id
                          select new
                          {
                              Material = ajusteStock.Material != null ? ajusteStock.Material.Descripcion : "",
                              MaterialDescCorta = ajusteStock.Material != null ? ajusteStock.Material.DescripcionCorta : "",
                              MaterialCodigoONCCA = ajusteStock.Material != null ? ajusteStock.Material.CodigoONCCA : "",
                              FechaEmision = ajusteStock.Fecha,
                              TipoDeComprobanteONCCA = ajusteStock.TipoComprobanteOncca.CodigoOncca,
                              TipoDocumentoIngreso = ajusteStock.TipoComprobanteOncca.Descripcion,
                              NumeroDeDocumentoDeIngreso = ajusteStock.NumeroDocumentoIngreso,
                              CuitDestinatario = molinos.Cuil,
                              CuitTitularCP = molinos.Cuil,
                              CuitRtteComercial = molinos.Cuil,
                              Destinatario = molinos.Descripcion,
                              TitularCP = molinos.Descripcion,
                              RtteComercial = molinos.Descripcion,
                              TipoDeWorkflow = (ajusteStock.PesoBrutoIngreso != 0 || ajusteStock.PesoNetoIngreso != 0) ? TipoDeWorkflow.Ingreso : TipoDeWorkflow.Egreso,
                              PesoNeto = ((ajusteStock.PesoNetoEgreso != 0 || ajusteStock.PesoNetoIngreso != 0) ? (ajusteStock.PesoNetoEgreso != 0 ? ajusteStock.PesoNetoEgreso.Value : ajusteStock.PesoNetoIngreso.Value) : 0),
                              ObservacionesONCCA = ajusteStock.Observaciones,
                              PesoNetoCalculado = ((ajusteStock.PesoNetoEgreso != 0 || ajusteStock.PesoNetoIngreso != 0) ? (ajusteStock.PesoNetoEgreso != 0 ? ajusteStock.PesoNetoEgreso.Value : ajusteStock.PesoNetoIngreso.Value) : 0),
                              CPE = false,
                              CTG = ajusteStock.NumeroCTG,
                              CartaDePorte = "",
                              Sucursal = (int?)null

                          };


            var resultados = resultado.Union(ajustes).OrderBy(x => x.FechaEmision);

            return resultados.ToList().Select(recorrido => new ImpImpresionGenericaDto
            {
                Material = recorrido.Material,
                MaterialDescCorta = recorrido.MaterialDescCorta,
                MaterialCodigoONCCA = recorrido.MaterialCodigoONCCA,
                FechaEmision = recorrido.FechaEmision,
                TipoDeComprobanteONCCA = recorrido.TipoDeComprobanteONCCA,
                NumeroDeDocumentoDeIngreso =recorrido.CPE ? string.Format("{0} {1}",recorrido.Sucursal.Value.ToString().PadLeft(5,'0'), recorrido.CTG): recorrido.NumeroDeDocumentoDeIngreso.Replace('R', '-'),
                CuitDestinatario = recorrido.CuitDestinatario,
                CuitTitularCP = recorrido.CuitTitularCP,
                RtteComercial = recorrido.RtteComercial,
                CuitRtteComercial = recorrido.CuitRtteComercial,
                Destinatario = recorrido.Destinatario,
                TitularCP = recorrido.TitularCP,
                PesoNetoNumerico = recorrido.PesoNeto,
                PesoNetoSinhumedadNumerico = recorrido.PesoNetoCalculado,
                PesoNeto = Formatted(recorrido.PesoNeto),
                PesoNetoSinHumedad = Formatted(recorrido.PesoNetoCalculado),
                ObservacionesONCCA = recorrido.ObservacionesONCCA,
                TipoDeWorkflow = recorrido.TipoDeWorkflow,
                SaldosSTOCK = CalcularStock(out stockInicial, stockInicial, recorrido.TipoDeWorkflow, recorrido.PesoNetoCalculado),
                CTG = recorrido.CPE ? recorrido.CartaDePorte : recorrido.CTG,
                CTGSap = recorrido.CPE ? ConvertirACTGSap(recorrido.CartaDePorte) : recorrido.CTG

            }).ToList();
        }

        public virtual List<ImpImpresionGenericaDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return LibroMovimientosExistenciaDeGranos(contexto, centro, materiales, stockInicial, fechaInicio, fechaFin, codigoSapFirma);
            }
        }

        private static string CalcularStock(out decimal stockInicialOut, decimal stockInicial, TipoDeWorkflow tipo, decimal pesoNeto)
        {
            stockInicialOut = tipo == TipoDeWorkflow.Ingreso ? stockInicial + Decimal.ToInt32(pesoNeto) : stockInicial - Decimal.ToInt32(pesoNeto);
            return Formatted(stockInicialOut);
        }

        private static string Formatted(decimal? number)
        {
            return number != null ? String.Format(CultureInfo.InvariantCulture, "{0:0,0}", Decimal.ToInt64(number.Value)).Replace(',', '.') : "";
        }

        private static string ConvertirACTGSap(string cp)
        {
             return "000" + cp.Substring(Math.Max(0, cp.Length - 9));
        }
    }
}
