using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarMuestraInaseParaArchivoConsulta : IConsulta<MuestraDeInaseDto>
    {
        private readonly string codigoSapFirma;
        public ListarMuestraInaseParaArchivoConsulta(string codigoSapFirma)
        {
            this.codigoSapFirma = codigoSapFirma;
        }

        public List<MuestraDeInaseDto> Ejecutar(DbContext contexto)
        {

            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var empresa = contexto.Set<Proveedor>().SingleOrDefault(x => x.CodigoSap == codigoSapFirma);
            return (from muestra in contexto.Set<MuestraDeInase>()
                    join remito in contexto.Set<Remito>() on muestra.Recorrido.Id equals remito.Recorrido.Id into remitoJoined
                    from remito in remitoJoined.DefaultIfEmpty()
                    where !muestra.MuestraEnviada && !muestra.Recorrido.Rechazado
                    select new MuestraDeInaseDto
                    {
                        CentroId = muestra.Recorrido.Centro.Id,
                        RecorridoId = muestra.Recorrido.Id,
                        WorkflowInstanceId = muestra.Recorrido.InstanciaWorkflow,
                        CartaPorte = muestra.Recorrido.Vehiculo.CartaPorte != null ? muestra.Recorrido.Vehiculo.CartaPorte.NroCartaPorte : (remito != null ? remito.OrdenRemito : muestra.Recorrido.NumeroDocumentoIngreso),
                        Patente = muestra.Recorrido.Patente,
                        MaterialDescripcion = muestra.Recorrido.Material.Descripcion,
                        DestinatarioCuil = muestra.Recorrido.Vehiculo != null ? (muestra.Recorrido.Vehiculo.CartaPorte.Destinatario != null ? muestra.Recorrido.Vehiculo.CartaPorte.Destinatario.Cuil : muestra.Recorrido.Vehiculo.CartaPorte.DestinatarioCliente.Cuit) : empresa.Cuil,
                        RtteComercialCuit = muestra.Recorrido.Vehiculo.CartaPorte.RtteComercial.Cuil,
                        CorredorCuil = muestra.Recorrido.Vehiculo.CartaPorte.Corredor.Cuil,
                        TitularCartaPorte = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Descripcion : (remito.ProveedorOrigen != null ? remito.ProveedorOrigen.Descripcion : empresa.Descripcion),
                        CPE = muestra.Recorrido.Vehiculo.CartaPorte != null ? muestra.Recorrido.Vehiculo.CartaPorte.Cpe == (bool?)true : false,
                        Sucursal = muestra.Recorrido.Vehiculo.CartaPorte.Sucursal,
                        CTG = muestra.Recorrido.Vehiculo.CartaPorte != null ? muestra.Recorrido.Vehiculo.CartaPorte.CTG : "0",
                        TitularCartaPorteCuil = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Cuil : (remito.ProveedorOrigen != null ? remito.ProveedorOrigen.Cuil : empresa.Cuil),
                        CodEstab = muestra.Recorrido.Vehiculo != null ? (muestra.Recorrido.Vehiculo.CartaPorte.CodEstab.StartsWith("999") ? "" : muestra.Recorrido.Vehiculo.CartaPorte.CodEstab) : (remito.CodEstab.StartsWith("999") ? "" : remito.CodEstab),
                        Direccion = muestra.Recorrido.Centro.Direccion,
                        ProcedenciaCodigoSap = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.Procedencia.CodigoAfip : remito.Procedencia.CodigoAfip,
                        LocalidadCodigoSap = muestra.Recorrido.Centro.Localidad.CodigoAfip,
                        TipoVehiculo = muestra.Recorrido.TipoVehiculo,
                        CantidadDeVagones = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.Vehiculos.Count : 0,
                        CodigoEstablecimiento = muestra.Recorrido.Centro.CodigoEstablecimiento,
                        Corredor = muestra.Recorrido.Vehiculo.CartaPorte.Corredor.RazonSocial,
                        RtteComercial = muestra.Recorrido.Vehiculo.CartaPorte.RtteComercial.Descripcion,
                        Cosecha = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.Cosecha : remito.Cosecha,
                        ProcedenciaCodigoPostal = muestra.Recorrido.Vehiculo.CartaPorte != null ? muestra.Recorrido.Vehiculo.CartaPorte.Procedencia.CodigoPostal : remito.Procedencia.CodigoPostal != null ? remito.Procedencia.CodigoPostal : 0,
                        ProcedenciaSubcodigoPostal = muestra.Recorrido.Vehiculo.CartaPorte != null ? muestra.Recorrido.Vehiculo.CartaPorte.Procedencia.SubcodigoPostal : remito.Procedencia.SubcodigoPostal != null ? remito.Procedencia.SubcodigoPostal : 0,
                        PesoNeto = (muestra.Recorrido.PesoBruto ?? 0) - (muestra.Recorrido.PesoTara ?? 0),
                        FechaDescarga = muestra.Recorrido.PesoTaraFecha.HasValue ? muestra.Recorrido.PesoTaraFecha.Value : muestra.Recorrido.FechaInicio,
                        Intermediario = null,
                        IntermediarioCuit = null
                    }).ToList();
        }
    }
}
