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

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarListadoDeCalidades : IConsulta<ListadoDeCalidadesDto>
    {
        private readonly List<int> centros;
        private readonly DateTime fechaInicio;
        private readonly DateTime fechaFin;
        private readonly List<int> tiposComerciales;
        private readonly int material;

        public ListarListadoDeCalidades(List<int> centros, DateTime fechaInicio, DateTime fechaFin, List<int> tiposComerciales, int material)
        {
            this.centros = centros;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaFin;
            this.tiposComerciales = tiposComerciales;
            this.material = material;
        }

        private static List<ListadoDeCalidadesDto> ListadoDeCalidades(DbContext contexto, List<int> centros, DateTime fechaInicio, DateTime fechaFin, List<int> tiposComerciales, int material)
        {
            ((IObjectContextAdapter) contexto).ObjectContext.CommandTimeout = 180;
            var resultado = from recorrido in contexto.Set<Recorrido>()
                            join ordenCargaFas in contexto.Set<OrdenCargaFas>().DefaultIfEmpty() on recorrido.Id equals ordenCargaFas.Recorrido.Id into ordenCargaFasJoined
                            from ordenCargaFas in ordenCargaFasJoined.DefaultIfEmpty()
                            join ordenCargaInterna in contexto.Set<OrdenCargaInterna>().DefaultIfEmpty() on recorrido.Id equals ordenCargaInterna.Recorrido.Id into ordenCargaInternaJoined
                            from ordenCargaInterna in ordenCargaInternaJoined.DefaultIfEmpty()
                            join ordenCargaInternaFason in contexto.Set<OrdenCargaInternaFason>().DefaultIfEmpty() on recorrido.Id equals ordenCargaInternaFason.Recorrido.Id into ordenCargaInternaFasonJoined
                            from ordenCargaInternaFason in ordenCargaInternaFasonJoined.DefaultIfEmpty()
                            join ordenDeCargaContenedor in contexto.Set<OrdenDeCargaContenedor>().DefaultIfEmpty() on recorrido.Id equals ordenDeCargaContenedor.Recorrido.Id into ordenDeCargaContenedorJoined
                            from ordenDeCargaContenedor in ordenDeCargaContenedorJoined.DefaultIfEmpty()
                            join ordenDeDescarga in contexto.Set<OrdenDeDescarga>().DefaultIfEmpty() on recorrido.Id equals ordenDeDescarga.Recorrido.Id into ordenDeDescargaJoined
                            from ordenDeDescarga in ordenDeDescargaJoined.DefaultIfEmpty()
                            join ordenDeDescargaFason in contexto.Set<OrdenDeDescargaFason>().DefaultIfEmpty() on recorrido.Id equals ordenDeDescargaFason.Recorrido.Id into ordenDeDescargaFasonJoined
                            from ordenDeDescargaFason in ordenDeDescargaFasonJoined.DefaultIfEmpty()
                            join ordenEntrePlantas in contexto.Set<OrdenEntrePlantas>().DefaultIfEmpty() on recorrido.Id equals ordenEntrePlantas.Recorrido.Id into ordenEntrePlantasJoined
                            from ordenEntrePlantas in ordenEntrePlantasJoined.DefaultIfEmpty()
                            join remito in contexto.Set<Remito>().DefaultIfEmpty() on recorrido.Id equals remito.Recorrido.Id into remitoJoined
                            from remito in remitoJoined.DefaultIfEmpty()
                            join remitoBodegaUva in contexto.Set<RemitoBodegaUva>().DefaultIfEmpty() on recorrido.Id equals remitoBodegaUva.Recorrido.Id into remitoBodegaUvaJoined
                            from remitoBodegaUva in remitoBodegaUvaJoined.DefaultIfEmpty()
                            join hojaDeRuta in contexto.Set<HojaDeRuta>().DefaultIfEmpty() on recorrido.Id equals hojaDeRuta.Recorrido.Id into hojaDeRutaJoined
                            from hojaDeRuta in hojaDeRutaJoined.DefaultIfEmpty()
                            join hojaDeRutaYerbatera in contexto.Set<HojaDeRutaYerbatera>().DefaultIfEmpty() on recorrido.Id equals hojaDeRutaYerbatera.Recorrido.Id into hojaDeRutaYerbateraJoined
                            from hojaDeRutaYerbatera in hojaDeRutaYerbateraJoined.DefaultIfEmpty()
                            where centros.Contains(recorrido.Centro.Id) && recorrido.Terminado && !recorrido.Rechazado &&
                                                           recorrido.FechaInicio >= fechaInicio &&
                                                           recorrido.FechaInicio <= fechaFin &&
                                                           tiposComerciales.Contains(recorrido.TipoComercial.Id) &&
                                                           material == recorrido.Material.Id
                            orderby recorrido.Id
                            select new
                                {
                                    CentroSap = recorrido.Centro.CodigoSAP,
                                    Centro = recorrido.Centro.Descripcion,
                                    TipoDocumentoDeIngreso = recorrido.TipoDocumentoIngreso,
                                    NumeroDocumentoDeIngreso = recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte ? recorrido.NumeroDocumentoIngreso.Substring(0, 4) + "-" + recorrido.NumeroDocumentoIngreso.Substring(4, 8) : recorrido.NumeroDocumentoIngreso,
                                    recorrido.Patente,
                                    PatenteAcoplado = (recorrido.Vehiculo.PatenteAcoplado ?? ordenCargaFas.PatenteAcoplado ?? ordenCargaInterna.PatenteAcoplado ?? ordenCargaInternaFason.PatenteAcoplado) ?? ordenDeCargaContenedor.PatenteAcoplado ?? ordenDeDescarga.PatenteAcoplado ?? ordenDeDescargaFason.PatenteAcoplado ?? ordenEntrePlantas.PatenteAcoplado ?? remito.PatenteAcoplado ?? hojaDeRuta.PatenteAcoplado ?? "",
                                    TipoComercial = recorrido.TipoComercial != null ? recorrido.TipoComercial.Descripcion : "",
                                    MaterialSap = recorrido.Material != null ? recorrido.Material.CodigoSAP : "",
                                    Material = recorrido.Material != null ? recorrido.Material.Descripcion : "",
                                    FechaNeto = recorrido.PesoTaraFecha,
                                    
                                    BrutoPlanta = recorrido.PesoBruto,
                                    TaraPlanta = recorrido.PesoTara,

                                    BrutoOrigen = recorrido.PesoBrutoOrigen,
                                    TaraOrigen = recorrido.PesoTaraOrigen,

                                    TitularCP = recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Descripcion ?? ordenCargaFas.Cliente.Descripcion ?? ordenCargaInterna.Destino.Descripcion ?? ordenCargaInternaFason.Cliente.Descripcion ?? ordenDeCargaContenedor.Destino.Descripcion ?? ordenDeDescarga.Proveedor.Descripcion ?? ordenDeDescargaFason.Cliente.Descripcion ?? hojaDeRutaYerbatera.Destinatario.Descripcion,

                                    Procedencia = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? recorrido.Centro.Localidad.Descripcion : (recorrido.Vehiculo.CartaPorte.Procedencia.Descripcion ?? ordenCargaFas.Cliente.Localidad ?? ordenDeDescarga.Proveedor.Localidad.Descripcion ?? ordenDeDescargaFason.Procedencia.Descripcion ?? remito.CentroOrigen.Localidad.Descripcion ?? hojaDeRutaYerbatera.CentroDestino.Localidad.Descripcion ?? ""),

                                    Transportista = recorrido.Transportista.RazonSocial ?? "",

                                    AlmacenOrigenSap = recorrido.Almacen.CodigoSAP ?? "",
                                    AlmacenOrigen = recorrido.Almacen.Descripcion ?? "",

                                    Caracteristicas = (from car in contexto.Set<CaracteristicaDeCalidad>()
                                                       join cal in recorrido.Calado.CaladosPorCaracteristica on car.CaracteristicaDeCalidadMaestro.Id equals cal.CaracteristicaDeCalidad.CaracteristicaDeCalidadMaestro.Id into calJoined
                                                       from cal in calJoined.DefaultIfEmpty()
                                                       join ana in recorrido.AnalisisDeCalidad.CaracteristicasAnalizadas on car.CaracteristicaDeCalidadMaestro.Id equals ana.CaracteristicaDeCalidad.CaracteristicaDeCalidadMaestro.Id into anaJoined
                                                       from ana in anaJoined.DefaultIfEmpty()
                                                       where car.MaterialPorCentro.Material.Id == recorrido.Material.Id && car.MaterialPorCentro.Centro.Id == recorrido.Centro.Id
                                                       
                                                       select new {
                                                           car.CaracteristicaDeCalidadMaestro.Descripcion,
                                                           Valor = ana.ValorAnalisis ?? cal.ValorCalado ?? 0,
                                                           PorcentajeDeDescuento = ana.DescuentoEnPorcentaje ?? cal.DescuentoEnKg ?? 0
                                                       }
                                     ),

                                    Camara = contexto.Set<MuestraEnvioACamara>().Where(x => x.CartaPorte.Id == recorrido.Vehiculo.CartaPorte.Id).OrderByDescending(x => x.Id).FirstOrDefault().Camara.CodigoSAP,

                                    Intermediario = recorrido.Vehiculo.CartaPorte.Intermediario.Descripcion,
                                    RemitenteComercial = recorrido.Vehiculo.CartaPorte.RtteComercial.Descripcion,
                                    Destinatario = recorrido.Vehiculo.CartaPorte.Destinatario.Descripcion ?? hojaDeRutaYerbatera.Destinatario.Descripcion,
                                    recorrido.Vehiculo.CartaPorte.Variedad,
                                    Contrato = recorrido.Vehiculo.CartaPorte.AcuerdoMarco ?? ""
                                };

            return resultado.ToList().Select(recorrido => new ListadoDeCalidadesDto
            {
                NetoDescontado =
                    Math.Round((recorrido.BrutoPlanta.HasValue && recorrido.TaraPlanta.HasValue
                        ? recorrido.BrutoPlanta.Value - recorrido.TaraPlanta.Value
                        : 0) -
                               recorrido.Caracteristicas.ToList().Sum(keyValue => keyValue.PorcentajeDeDescuento)*
                               (recorrido.BrutoPlanta.HasValue && recorrido.TaraPlanta.HasValue
                                   ? recorrido.BrutoPlanta.Value - recorrido.TaraPlanta.Value
                                   : 0)/100).ToString(CultureInfo.InvariantCulture),
                CentroSap = recorrido.CentroSap,
                Centro = recorrido.Centro,
                TipoDocumentoDeIngreso = recorrido.TipoDocumentoDeIngreso.ToString(),
                NumeroDocumentoDeIngreso = recorrido.NumeroDocumentoDeIngreso,
                Patente = recorrido.Patente,
                PatenteAcoplado = recorrido.PatenteAcoplado,
                TipoComercial = recorrido.TipoComercial,
                MaterialSap = recorrido.MaterialSap,
                Material = recorrido.Material,
                FechaNeto = string.Format("{0:dd/MM/yyyy}", recorrido.FechaNeto),
                NetoPlanta =
                    recorrido.BrutoPlanta.HasValue && recorrido.TaraPlanta.HasValue
                        ? (recorrido.BrutoPlanta.Value - recorrido.TaraPlanta.Value).ToString(
                            CultureInfo.InvariantCulture)
                        : "",
                NetoOrigen =
                    recorrido.BrutoOrigen.HasValue && recorrido.TaraOrigen.HasValue
                        ? (recorrido.BrutoOrigen.Value - recorrido.TaraOrigen.Value).ToString(
                            CultureInfo.InvariantCulture)
                        : "",
                Procedencia = recorrido.Procedencia,
                Transportista = recorrido.Transportista,
                AlmacenOrigenSap = recorrido.AlmacenOrigenSap,
                AlmacenOrigen = recorrido.AlmacenOrigen,
                Calidades = recorrido.Caracteristicas.ToDictionary(kv => kv.Descripcion, kv => (decimal?) kv.Valor),
                TitularCP = recorrido.TitularCP,
                Camara = recorrido.Camara,
                Contrato = recorrido.Contrato,
                Destinatario = recorrido.Destinatario,
                Intermediario = recorrido.Intermediario,
                RemitenteComercial = recorrido.RemitenteComercial,
                Variedad = recorrido.Variedad
            }).ToList();
        }

        public virtual List<ListadoDeCalidadesDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required,new TransactionOptions {IsolationLevel = IsolationLevel.ReadUncommitted}))
            {
                return ListadoDeCalidades(contexto, centros, fechaInicio, fechaFin, tiposComerciales, material);
            }
        }

    }
}
