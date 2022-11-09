using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarMuestraEnvioACamaraParaArchivoConsulta : IConsulta<MuestraEnvioACamaraDto>
    {
        private readonly int loteId;
        private readonly string codigoSapFirma;

        public ListarMuestraEnvioACamaraParaArchivoConsulta(int loteId, string codigoSapFirma)
        {
            this.loteId = loteId;
            this.codigoSapFirma = codigoSapFirma;
        }

        public List<MuestraEnvioACamaraDto> Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var empresa = contexto.Set<Proveedor>().SingleOrDefault(x => x.CodigoSap == codigoSapFirma);
            var resultado = (from muestra in contexto.Set<MuestraEnvioACamara>()

                             join rec in contexto.Set<Recorrido>() on muestra.Calado.WorkflowInstanceId equals
                                 rec.InstanciaWorkflow into recJoined
                             from rec in recJoined.DefaultIfEmpty()

                             join remito in contexto.Set<Remito>() on rec.Id equals remito.Recorrido.Id into
                                 remitoJoined
                             from remito in remitoJoined.DefaultIfEmpty()

                             join conversionMaterial in contexto.Set<ConversionMaterial>().DefaultIfEmpty() on new { Key1 = rec.Material.Id, Key2 = muestra.Camara.Id } equals new { Key1 = conversionMaterial.Material.Id, Key2 = conversionMaterial.Camara.Id } into conversionMaterialJoined
                             from conversionMaterial in conversionMaterialJoined.DefaultIfEmpty()
                             join conversionCentro in contexto.Set<ConversionCentro>().DefaultIfEmpty() on new { Key1 = rec.Centro.Id, Key2 = muestra.Camara.Id } equals new { Key1 = conversionCentro.Centro.Id, Key2 = conversionCentro.Camara.Id } into conversionCentroJoined
                             from conversionCentro in conversionCentroJoined.DefaultIfEmpty()
                             join conversionGrupo in contexto.Set<ConversionGrupo>().DefaultIfEmpty() on new { Key1 = rec.Material.Id, Key2 = muestra.Camara.Id } equals new { Key1 = conversionGrupo.Material.Id, Key2 = conversionGrupo.Camara.Id } into conversionGrupoJoined
                             from conversionGrupo in conversionGrupoJoined.DefaultIfEmpty()

                             where muestra.Lote.Id == loteId
                             select new MuestraEnvioACamaraDto
                                 {
                                     Id = muestra.Id,
                                     NombreUsuario = muestra.NombreUsuario,
                                     CaladoId = muestra.Calado.Id,
                                     CamaraId = muestra.Camara.Id,
                                     NroMuestra = muestra.NroMuestra,
                                     NroMuestraTerceros = muestra.NroMuestraTerceros,
                                     NroCartaPorte = muestra.CartaPorte != null ? muestra.CartaPorte.NroCartaPorte : remito.OrdenRemito,
                                     FechaDescarga = muestra.FechaDescarga,
                                     PesoNeto  = (rec.PesoBruto ?? 0) - (rec.PesoTara ?? 0),
                                     CentroId = muestra.Centro.Id,
                                     Localidad = muestra.CartaPorte != null ? muestra.CartaPorte.Procedencia.Descripcion : remito.Procedencia.Descripcion,
                                     Patente = rec.Patente,
                                     WorkflowInstanceId = rec.InstanciaWorkflow,
                                     EstadoMuestra = muestra.EstadoMuestra,
                                     TieneAnalisisInterno = muestra.TieneAnalisisInterno,
                                     TipoDocumento = rec.TipoDocumentoIngreso,
                                     NroDocumento = muestra.NroMuestra,

                                     Material = rec.Material.Descripcion,
                                     Vendedor = muestra.CartaPorte.Destinatario.Descripcion,
                                     Corredor = muestra.CartaPorte.Corredor.Descripcion,
                                     Proveedor = muestra.CartaPorte != null ? muestra.CartaPorte.TitularCartaPorte.Descripcion : remito.ProveedorOrigen.Descripcion,
                                     ProveedorCodigoSap = muestra.CartaPorte != null ? muestra.CartaPorte.TitularCartaPorte.CodigoSap : remito.ProveedorOrigen.CodigoSap,
                                     FechaCartaPorte = muestra.CartaPorte != null ? muestra.CartaPorte.FechaCP : (remito != null ? remito.FechaOD : rec.FechaEgreso.Value),
                                     
                                     MaterialCodigoCamara = conversionMaterial.CodigoCamara,
                                     TitularCartaPorteCuil = muestra.CartaPorte != null ? muestra.CartaPorte.TitularCartaPorte.Cuil : (remito.ProveedorOrigen != null ? remito.ProveedorOrigen.Cuil :empresa.Cuil),
                                     TitularCartaPorteMail = muestra.CartaPorte != null ? muestra.CartaPorte.TitularCartaPorte.Mail : (remito.ProveedorOrigen != null ? remito.ProveedorOrigen.Mail : empresa.Mail),
                                     TitularCartaPorte = muestra.CartaPorte != null ? muestra.CartaPorte.TitularCartaPorte.Descripcion : (remito.ProveedorOrigen != null ? remito.ProveedorOrigen.Descripcion : empresa.Descripcion),

                                     DestinatarioId = muestra.CartaPorte != null ? muestra.CartaPorte.Destinatario.Id : (remito != null ? empresa.Id : 0),
                                     Destinatario = muestra.CartaPorte != null ? muestra.CartaPorte.Destinatario.Descripcion : empresa.Descripcion,
                                     DestinatarioCodigoSap = muestra.CartaPorte != null ? muestra.CartaPorte.Destinatario.CodigoSap : empresa.CodigoSap,
                                     DestinatarioCuil = muestra.CartaPorte != null ? muestra.CartaPorte.Destinatario.Cuil : empresa.Cuil,
                                     DestinatarioMail = muestra.CartaPorte != null ? muestra.CartaPorte.Destinatario.Mail : empresa.Mail,
                                     
                                     CorredorCuil = muestra.CartaPorte.Corredor.Cuil,
                                     CorredorId = muestra.CartaPorte.Corredor != null ? muestra.CartaPorte.Corredor.Id : 0,

                                     RtteComercial = muestra.CartaPorte.RtteComercial.Descripcion,
                                     RtteComercialCuit = muestra.CartaPorte.RtteComercial.Cuil,
                                     RtteComercialId = muestra.CartaPorte != null ? muestra.CartaPorte.RtteComercial.Id : 0,
                                     RtteComercialMail = muestra.CartaPorte.RtteComercial.Mail,
                                     
                                     Caratula = muestra.CartaPorte != null && muestra.CartaPorte.Caratula.HasValue ? muestra.CartaPorte.Caratula.Value : 0,

                                     CentroCodigoPostal = rec.Centro.CodigoPostal,
                                     CentroCodigoCamara = conversionCentro.CodigoCamara,
                                     CodigoTecnologia = muestra.CartaPorte != null && muestra.CartaPorte.Tecnologia != null ? muestra.CartaPorte.Tecnologia.Codigo : "00",
                                     GrupoCodigoCamara = conversionGrupo.CodigoSegunCamara,
                                     Sucursal = muestra.CartaPorte.Sucursal,
                                     CPE = muestra.CartaPorte != null ? muestra.CartaPorte.Cpe == (bool?)true : false,
                                     CTG = muestra.CartaPorte != null ? muestra.CartaPorte.CTG : "0",
                                     CodEstab = muestra.CartaPorte != null ? muestra.CartaPorte.CodEstab : remito.CodEstab,
                                     Direccion = rec.Centro.Direccion,
                                     Procedencia = muestra.CartaPorte != null ? muestra.CartaPorte.Procedencia.Descripcion : remito.Procedencia.Descripcion,
                                     ProcedenciaCodigoSap = muestra.CartaPorte != null ? muestra.CartaPorte.Procedencia.CodigoAfip : remito.Procedencia.CodigoAfip,
                                     LocalidadCodigoSap = rec.Centro.Localidad.CodigoAfip,
                                     PesoNetoFecha = muestra.FechaDescarga ?? muestra.Calado.FechaCreacion ?? rec.FechaInicio,
                                     TipoVehiculo = rec.TipoVehiculo,
                                     CantidadVehiculos = muestra.CartaPorte != null ?  muestra.CartaPorte.Vehiculos.Count : 0,
                                     NumeroVehiculo = rec.Vehiculo != null ? rec.Vehiculo.NumeroVehiculo : 0,
                                     CentroDestinoCodigoEstablecimiento = rec.Centro.CodigoEstablecimiento,
                                     Entregador = muestra.CartaPorte.Entregador.RazonSocial,
                                     Intermediario = muestra.CartaPorte != null ? muestra.CartaPorte.Intermediario != null ? muestra.CartaPorte.Intermediario.Descripcion : null : null,
                                     IntermediarioCuit = muestra.CartaPorte != null ? muestra.CartaPorte.Intermediario != null ? muestra.CartaPorte.Intermediario.Cuil : null : null,
                                     Cosecha = muestra.CartaPorte != null ? muestra.CartaPorte.Cosecha : remito.Cosecha,
                                     ProcedenciaCodigoPostal = muestra.CartaPorte != null ? muestra.CartaPorte.Procedencia.CodigoPostal : remito.Procedencia.CodigoPostal != null ? remito.Procedencia.CodigoPostal : 0 ,
                                     ProcedenciaSubcodigoPostal = muestra.CartaPorte != null ? muestra.CartaPorte.Procedencia.SubcodigoPostal : remito.Procedencia.SubcodigoPostal != null ? remito.Procedencia.SubcodigoPostal : 0,
                                     EsPreLote = muestra.EsPreLote,
                                 Caracteristicas =
                                         muestra.CaracteristicasDeCalidad.Select(
                                             x =>
                                             new CaracteristicaDeCalidadDto
                                                 {
                                                     Descripcion = x.CaracteristicaDeCalidadMaestro.Descripcion,
                                                     Ensayo = x.Ensayo,
                                                     Id = x.Id,
                                                     CodigoCamara = contexto.Set<ConversionCaracteristica>().Where(y => y.Camara.Id == muestra.Camara.Id && y.Caracteristica.Id == x.Id).Select(y => y.CodigoCamara).FirstOrDefault()
                                                 })
                                 });
    
            return resultado.ToList();
        }
    }
}
