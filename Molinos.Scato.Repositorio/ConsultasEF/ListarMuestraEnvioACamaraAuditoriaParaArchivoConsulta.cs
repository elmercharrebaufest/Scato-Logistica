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
    public class ListarMuestraEnvioACamaraAuditoriaParaArchivoConsulta : IConsulta<MuestraEnvioACamaraAuditoriaDto>
    {
        private readonly int loteId;
        private readonly string codigoSapFirma;

        public ListarMuestraEnvioACamaraAuditoriaParaArchivoConsulta(int loteId, string codigoSapFirma)
        {
            this.loteId = loteId;
            this.codigoSapFirma = codigoSapFirma;
        }

        public List<MuestraEnvioACamaraAuditoriaDto> Ejecutar(DbContext contexto)
        {

            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var empresa = contexto.Set<Proveedor>().SingleOrDefault(x => x.CodigoSap == codigoSapFirma);
            return (from muestra in contexto.Set<MuestraEnvioACamaraAuditoria>()
                    join conversionMaterial in contexto.Set<ConversionMaterial>().DefaultIfEmpty() on new { Key1 = muestra.Recorrido.Material.Id, Key2 = muestra.LoteAuditoria.Camara.Id } equals new { Key1 = conversionMaterial.Material.Id, Key2 = conversionMaterial.Camara.Id } into conversionMaterialJoined
                    from conversionMaterial in conversionMaterialJoined.DefaultIfEmpty()
                    join conversionCentro in contexto.Set<ConversionCentro>().DefaultIfEmpty() on new { Key1 = muestra.Recorrido.Centro.Id, Key2 = muestra.LoteAuditoria.Camara.Id } equals new { Key1 = conversionCentro.Centro.Id, Key2 = conversionCentro.Camara.Id } into conversionCentroJoined
                    from conversionCentro in conversionCentroJoined.DefaultIfEmpty()
                    join conversionGrupo in contexto.Set<ConversionGrupo>().DefaultIfEmpty() on new { Key1 = muestra.Recorrido.Material.Id, Key2 = muestra.LoteAuditoria.Camara.Id } equals new { Key1 = conversionGrupo.Material.Id, Key2 = conversionGrupo.Camara.Id } into conversionGrupoJoined
                    from conversionGrupo in conversionGrupoJoined.DefaultIfEmpty()

                    join remito in contexto.Set<Remito>() on muestra.Recorrido.Id equals remito.Recorrido.Id into
                                 remitoJoined
                    from remito in remitoJoined.DefaultIfEmpty()

                    where muestra.LoteAuditoria.Id == loteId
                    orderby muestra.Recorrido.Id
                    select new MuestraEnvioACamaraAuditoriaDto
                    {
                        CamaraId = muestra.LoteAuditoria.Camara.Id,
                        MaterialId = muestra.Recorrido.Material.Id,
                        WorkflowInstanceId = muestra.Recorrido.InstanciaWorkflow,
                        Material = muestra.Recorrido.Material.Descripcion,
                        CodigoDeCamara = conversionCentro.CodigoCamara,
                        Patente = muestra.Recorrido.Patente,
                        TipoDocumento = muestra.Recorrido.TipoDocumentoIngreso,
                        FechaDescarga = muestra.Recorrido.PesoTaraFecha.HasValue ? muestra.Recorrido.PesoTaraFecha.Value : muestra.Recorrido.FechaInicio,
                        NumeroVehiculo = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.NumeroVehiculo : 1,
                        PesoNeto = (muestra.Recorrido.PesoBruto ?? 0) - (muestra.Recorrido.PesoTara ?? 0),

                        CorredorCuil = muestra.Recorrido.Vehiculo.CartaPorte.Corredor.Cuil,
                        Corredor = muestra.Recorrido.Vehiculo.CartaPorte.Corredor.RazonSocial,

                        Vendedor = muestra.Recorrido.Vehiculo.CartaPorte.Destinatario.Descripcion,
                        VendedorId = muestra.Recorrido.Vehiculo.CartaPorte.Destinatario != null ? muestra.Recorrido.Vehiculo.CartaPorte.Destinatario.Id : 0,

                        RtteComercial = muestra.Recorrido.Vehiculo.CartaPorte.RtteComercial.Descripcion,
                        RtteComercialMail = muestra.Recorrido.Vehiculo.CartaPorte.RtteComercial.Mail,
                        RtteComercialId = muestra.Recorrido.Vehiculo.CartaPorte.RtteComercial != null ? muestra.Recorrido.Vehiculo.CartaPorte.RtteComercial.Id : 0,
                        RtteComercialCuit = muestra.Recorrido.Vehiculo.CartaPorte.RtteComercial.Cuil,

                        TitularCartaPorte = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Descripcion : (remito.ProveedorOrigen != null ? remito.ProveedorOrigen.Descripcion : empresa.Descripcion),
                        TitularCartaPorteMail = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Mail : (remito.ProveedorOrigen != null ? remito.ProveedorOrigen.Mail : empresa.Mail),
                        TitularCartaPorteCuil = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Cuil : (remito.ProveedorOrigen != null ? remito.ProveedorOrigen.Cuil : empresa.Cuil),

                        CodigoCamaraMaterial = conversionMaterial.CodigoCamara,
                        CodigoCamaraGrupo = conversionGrupo.CodigoSegunCamara,
                        Sucursal = muestra.Recorrido.Vehiculo.CartaPorte.Sucursal,
                        NroCartaPorte = muestra.Recorrido.Vehiculo.CartaPorte != null ? muestra.Recorrido.Vehiculo.CartaPorte.NroCartaPorte : (remito != null ? remito.OrdenRemito : muestra.Recorrido.NumeroDocumentoIngreso),
                        CPE = muestra.Recorrido.Vehiculo.CartaPorte != null ? muestra.Recorrido.Vehiculo.CartaPorte.Cpe == (bool?)true : false,
                        CTG = muestra.Recorrido.Vehiculo.CartaPorte != null ? muestra.Recorrido.Vehiculo.CartaPorte.CTG : "0",
                        CodEstab = muestra.Recorrido.Vehiculo != null ? (muestra.Recorrido.Vehiculo.CartaPorte.CodEstab.StartsWith("999") ? "" : muestra.Recorrido.Vehiculo.CartaPorte.CodEstab) : (remito.CodEstab.StartsWith("999") ? "" : remito.CodEstab),
                        Direccion = muestra.Recorrido.Centro.Direccion,
                        ProcedenciaCodigoSap = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.Procedencia.CodigoAfip : remito.Procedencia.CodigoAfip,
                        LocalidadCodigoSap = muestra.Recorrido.Centro.Localidad.CodigoAfip,

                        DestinatarioCodigoSap = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.Destinatario.CodigoSap : empresa.CodigoSap,
                        Destinatario = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.Destinatario.Descripcion : empresa.Descripcion,
                        DestinatarioCuil = muestra.Recorrido.Vehiculo != null ? (muestra.Recorrido.Vehiculo.CartaPorte.Destinatario != null ? muestra.Recorrido.Vehiculo.CartaPorte.Destinatario.Cuil : muestra.Recorrido.Vehiculo.CartaPorte.DestinatarioCliente.Cuit) : empresa.Cuil,
                        DestinatarioId = muestra.Recorrido.Vehiculo.CartaPorte.Destinatario != null ? muestra.Recorrido.Vehiculo.CartaPorte.Destinatario.Id : (remito != null ? empresa.Id : 0),
                        DestinatarioMail = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.Destinatario.Mail : empresa.Mail,

                        CantidadDeVagones = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.Vehiculos.Count : 0,
                        CodigoEstablecimiento = muestra.Recorrido.Centro.CodigoEstablecimiento,
                        TipoVehiculo = muestra.Recorrido.TipoVehiculo,
                        Caratula = muestra.Recorrido.Vehiculo.CartaPorte.Caratula,
                        CentroCodigoPostal = muestra.Recorrido.Centro.CodigoPostal,
                        FechaEmision = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.FechaEmision : (remito != null ? remito.FechaOD : muestra.Recorrido.FechaEgreso.Value),

                        Intermediario = null,
                        IntermediarioCuit = null,

                        Cosecha = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.Cosecha : remito.Cosecha,
                        Entregador = muestra.Recorrido.Vehiculo.CartaPorte.Entregador.RazonSocial,
                        Procedencia = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.Procedencia.Descripcion : remito.Procedencia.Descripcion,
                        CodigoTecnologia = muestra.Recorrido.Vehiculo.CartaPorte.Tecnologia != null ? muestra.Recorrido.Vehiculo.CartaPorte.Tecnologia.Codigo : "00",
                        CamaraFormatoDeArchivo = muestra.LoteAuditoria.Camara.FormatoDeArchivo != null ? muestra.LoteAuditoria.Camara.FormatoDeArchivo.Value : CamaraFormatoDeArchivo.NoEspecificado
                    }).ToList();

        }
    }
}
