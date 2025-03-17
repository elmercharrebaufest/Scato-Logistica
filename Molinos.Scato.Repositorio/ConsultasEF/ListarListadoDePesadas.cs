using Molinos.Scato.Dominio.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarListadoDePesadas : IConsulta<ListadoDePesadasDto>
    {
        private readonly List<int> centros;
        private readonly DateTime fechaInicio;
        private readonly DateTime fechaFin;
        private readonly List<int> tiposComerciales;
        private readonly List<int> materiales;
        private readonly bool incluirRechazados;

        public ListarListadoDePesadas(List<int> centros, DateTime fechaInicio, DateTime fechaFin, List<int> tiposComerciales, List<int> materiales, bool incluirRechazados)
        {
            this.centros = centros;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaFin;
            this.tiposComerciales = tiposComerciales;
            this.materiales = materiales;
            this.incluirRechazados = incluirRechazados;
        }

        private static List<ListadoDePesadasDto> ListadoDePesadas(DbContext contexto, List<int> centros, DateTime fechaInicio, DateTime fechaFin, List<int> tiposComerciales, List<int> materiales, bool incluirRechazados)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var query = @"select
	                    CentroOrigen = CASE when w.TipoDeWorkflow = 1 THEN wc.CodigoSAP ELSE isnull(remc.CodigoSAP,'') END,
                        TipoDocumentoDeIngreso = rti.Descripcion,
                        NumeroDocumentoDeIngreso = CASE when r.TipoDocumentoIngreso = 1 THEN SUBSTRING(r.NumeroDocumentoIngreso, 0,5)  + '-' + SUBSTRING(r.NumeroDocumentoIngreso,5, 9) ELSE r.NumeroDocumentoIngreso END,
	                    Patente = r.Patente,
                        PatenteAcoplado = isnull(v.PatenteAcoplado , isnull(ofas.PatenteAcoplado , isnull(oint.PatenteAcoplado , isnull(ofason.PatenteAcoplado , isnull(ocont.PatenteAcoplado , isnull(odesc.PatenteAcoplado , isnull(odescfason.PatenteAcoplado , isnull(oplantas.PatenteAcoplado , isnull(rem.PatenteAcoplado , isnull(h.PatenteAcoplado , '')))))))))),
	                    TipoComercial = isnull(tcom.Descripcion , ''),
                        Material = isnull(mat.Descripcion , ''),
	                    FechaIngreso = convert(varchar, r.FechaInicio, 103)+ ' ' + convert(varchar, r.FechaInicio, 8),
                        FechaEgreso = convert(varchar, r.FechaEgreso, 103)+ ' ' + convert(varchar, r.FechaEgreso, 8),
                        FechaNeto = convert(varchar, r.PesoTaraFecha, 103) + ' ' + convert(varchar, r.PesoTaraFecha, 8),
                        BrutoPlanta = isnull(ltrim(r.PesoBruto),''),
                        TaraPlanta = isnull(ltrim(r.PesoTara),''),
                        NetoPlanta = case when r.PesoBruto is not null and r.PesoTara is not null then ltrim(r.PesoBruto-r.PesoTara) else '' end,
	                    BrutoOrigen = isnull(ltrim(r.PesoBrutoOrigen),''),
                        TaraOrigen = isnull(ltrim(r.PesoTaraOrigen),''),
	                    NetoOrigen = case when r.PesoBrutoOrigen is not null and r.PesoTaraOrigen is not null then ltrim(r.PesoBrutoOrigen-r.PesoTaraOrigen) else '' end,
	                    Procedencia = CASE WHEN w.TipoDeWorkflow = 1 THEN rcl.Descripcion else isnull(cpl.Descripcion , isnull(ofasc.Localidad , isnull(odescpl.Descripcion ,isnull( odescfasonl.Descripcion , isnull(remcl.Descripcion ,isnull( hycl.Descripcion , '')))))) end,
                        Transportista = isnull(t.RazonSocial , ''),
                        CuitTransportista = replace( isnull(t.cuit , ''),'-',''),
                        Chofer =ltrim( isnull(c.nombre , '') + ' ' + isnull(c.apellido , '')),
                        CuilChofer = replace( isnull(c.cuil , ''),'-',''),
                        Humedad = isnull(ltrim(rca.Humedad),''),
                        AlmacenDestino =case when w.TipoDeWorkflow = 0 then isnull( ra.Descripcion , '') else '' end,
                        AlmacenOrigen = case when w.TipoDeWorkflow = 1 then isnull( ra.Descripcion , '') else '' end,
                        RemitoSAP =isnull( rem.OrdenRemito , isnull(ruva.NroRemito , (case when w.TipoDeWorkflow = 1 then isnull( r.NumeroDeDocumentoSap , '') else '' end ))),
                        TipoDeVehiculo = rtv.Descripcion,
                        DocumentoSap = r.DocumentoInternoSap,
                        BalanzaBruto = isnull( rbbruto.Nombre , ''),
                        BalanzaTara = isnull( rbtara.Nombre , ''),
                        Rechazado = case when r.Rechazado= 1 then 'Si' else 'No' end,
                        CentroDestino = isnull( CentroDestino.CodigoSAP , isnull(oplantasc.CodigoSAP , hyc.CodigoSAP)),
                        Corredor = isnull(cpprov.Descripcion,''),
                        CuitCorredor = replace( isnull(cpprov.Cuil, ''),'-',''),
                        TitularCP = isnull (cptit.Descripcion ,isnull( odescp.Descripcion,'')),
                        CuitTitularCP = replace( isnull(cptit.Cuil ,isnull( odescp.Cuil,'')),'-',''),
                        Intermediario =isnull(cpint.Descripcion,''),
                        CuitIntermediario = replace(cpint.Cuil,'-',''),
                        RemitenteComercial = CASE WHEN ofason.Id IS NOT NULL THEN ISNULL(ofasonCliR.Descripcion , ofasonCliC.Descripcion)
                                             WHEN cprtte.Id IS NOT NULL THEN cprtte.Descripcion
                                             ELSE ISNULL(ofasr.Descripcion, '') END,
                        CuitRemitenteComercial = CASE WHEN ofason.Id IS NOT NULL THEN replace(ISNULL(ofasonCliR.Cuit , ofasonCliC.Cuit), '-','')
                                                 WHEN cprtte.Id IS NOT NULL THEN replace( ISNULL(cprtte.Cuil, ''),'-','')
                                                 ELSE  replace( ISNULL(ofasr.Cuil,  ''),'-','') END,
                        Destinatario = CASE WHEN cpdest.Descripcion IS NOT NULL THEN cpdest.Descripcion
                                       WHEN hyp.Descripcion IS NOT NULL THEN hyp.Descripcion
                                       ELSE ofasd.Descripcion END,
                        CuitDestinatario = replace(CASE WHEN cpdest.Cuil IS NOT NULL THEN cpdest.Cuil
                                                   WHEN hyp.Cuil IS NOT NULL THEN hyp.Cuil
                                                   ELSE ofasd.Cuit END, '-', ''),
                        Consignatario = CASE WHEN ofason.Id IS NOT NULL AND ofasonCliC.Id IS NOT NULL THEN ofasonCliC.Descripcion 
                                        WHEN ofas.Id IS NOT NULL AND ofasCliC.Descripcion IS NOT NULL THEN ofasCliC.Descripcion 
                                        ELSE '' END,
                        CuitConsignatario = CASE WHEN ofason.Id IS NOT NULL AND ofasonCliC.Id IS NOT NULL THEN replace(ISNULL(ofasonCliC.Cuit, ''), '-', '') 
                                            WHEN ofas.Id IS NOT NULL AND ofasCliC.Cuit IS NOT NULL THEN replace(ISNULL(ofasCliC.Cuit, ''), '-', '') 
                                            ELSE '' END,
                        Cliente = isnull( cpcli.Descripcion , isnull(ofasc.Descripcion ,isnull( ointc.Descripcion , isnull(ofasonc.Descripcion , isnull(ocontc.Descripcion ,odescfasonc.Descripcion))))),
                        CuitCliente = replace( isnull(cpcli.Cuit , isnull(ofasc.Cuit ,isnull( ointc.Cuit ,isnull( ofasonc.Cuit ,isnull( ocontc.Cuit , odescfasonc.Cuit))))),'-',''),
                        IntermediarioFlete = isnull(cpintf.Descripcion,''),
                        CuitIntermediarioFlete = replace(cpintf.Cuil,'-',''),
                        Entregador = cpe.DescripcionCorta,
                        Variedad = cp.Variedad,
                        KmARecorrer = isnull(ltrim(cp.KmRecorrer),''),
                        TarifaPorTonelada = isnull(ltrim( cp.TarifaTonelada),''),
                        CTG = cp.CTG,
	                    Camara = isnull(cpcc.CodigoSAP,''),
	                    NumeroDeLote = isnull(cpcl.NumeroDeLote,''),
                        ValorDevueltoPorAfipArriboCTG = isnull(cpb.CodigoDeBaja, ''),
	                    ValorDevueltoPorAfipDefinitivoCTG = isnull(cpb.CodigoDeBajaDefinitivo,''),
                        NumeroOrdenDeCargaSAP = ofas.NumeroOrden,
                        RemitoDeProveedores = isnull( odescfason.NumeroRemito , rem.DocLegalRemito),
                        NombreEstablecimiento = e.NombreDeEstablecimiento,
                        CodigoEstablecimiento = e.CodigoDeEstablecimiento,
                        Cosecha = cp.Cosecha,
                        Usuario = r.PesoBrutoUsuario,
                        ModalidadPesadaBruto = case when r.PesoBrutoModalidad is null then '' when r.PesoBrutoModalidad = 0 then 'Manual' else 'Automatica' end,
                        ModalidadPesadaTara = case when r.PesoTaraModalidad is null then '' when r.PesoTaraModalidad = 0 then 'Manual' else 'Automatica' end,
                        CPEDG = case when cpdg.Sucursal is null or cpdg.NroOrden is null then '' else cpdg.Sucursal + cpdg.NroOrden end,
						CTGDG = case when cpdg.NroCTG is null then '' else cpdg.NroCTG end,
                        TipoVariedad = tv.Descripcion
                    from
	                    Recorrido r
	                    inner join TipoDocumentoIngreso rti on rti.Id = r.TipoDocumentoIngreso
	                    inner join Workflow w on r.Workflow_Id = w.Id
	                    inner join Centro rc on r.Centro_Id = rc.Id
	                    inner join TipoComercial tcom on r.TipoComercial_Id = tcom.Id
	                    inner join TipoVehiculo rtv on rtv.id = r.TipoVehiculo
	                    inner join Chofer c on r.Chofer_Id = c.Id
	                    left join Localidad rcl on rc.Localidad_Id = rcl.Id
	                    left join Establecimiento e on r.Establecimiento_Id = e.Id
	                    left join Centro wc on w.Centro_Id = wc.Id
	                    left join Material mat on r.Material_Id  = mat.Id
	                    left join OrdenCargaFas ofas on r.Id = ofas.Recorrido_Id
                        left join Cliente ofasd on ofasd.Id = ofas.Destinatario_Id
	                    left join Cliente ofasc on ofasc.Id = ofas.Cliente_Id
                        left join Proveedor ofasr on ofasr.id = ofas.Remitente_Id
	                    left join OrdenCargaInterna oint on r.Id = oint.Recorrido_Id
	                    left join Cliente ointc on ointc.Id = oint.Destino_Id
	                    left join OrdenCargaInternaFason ofason on r.Id = ofason.Recorrido_Id
	                    left join Cliente ofasonc on ofasonc.Id = ofason.Cliente_Id
	                    left join OrdenDeCargaContenedor ocont on r.Id = ocont.Recorrido_Id
	                    left join Cliente ocontc on ocontc.Id = ocont.Destino_Id
	                    left join OrdenDeDescarga odesc on r.Id = odesc.Recorrido_Id
	                    left join Proveedor odescp on odescp.Id = odesc.Proveedor_Id
	                    left join Localidad odescpl on odescpl.Id = odescp.Localidad_Id
	                    left join OrdenDeDescargaFason odescfason on r.Id = odescfason.Recorrido_Id
	                    left join Localidad odescfasonl on odescfasonl.Id = odescfason.Procedencia_Id
	                    left join Cliente odescfasonc on odescfasonc.Id = odescfason.Cliente_Id
	                    left join OrdenEntrePlantas oplantas on r.Id = oplantas.Recorrido_Id
	                    left join Centro oplantasc on oplantasc.Id = oplantas.CentroDestino_Id
	                    left join Remito rem on r.Id = rem.Recorrido_Id
	                    left join Centro remc on remc.Id = rem.CentroOrigen_Id
	                    left join Localidad remcl on remcl.Id = remc.Localidad_Id
	                    left join RemitoBodegaUva ruva on r.Id = ruva.Recorrido_Id
	                    left join HojaDeRuta h on r.Id = h.Recorrido_Id
	                    left join HojaDeRutaYerbatera hy on r.Id = hy.Recorrido_Id
	                    left join Proveedor hyp on hyp.Id = hy.Destinatario_Id
	                    left join Centro hyc on hyc.Id = hy.CentroDestino_Id
	                    left join Localidad hycl on hycl.Id = hyc.Localidad_Id
	                    left join Vehiculo v on r.vehiculo_id = v.id
	                    left join CartaPorte cp on v.CartaPorte_Id = cp.id
	                    left join Proveedor cpprov on cpprov.id = cp.Corredor_Id
	                    left join Proveedor cptit on cptit.id = cp.TitularCartaPorte_Id
	                    left join Proveedor cpint on cpint.id = cp.Intermediario_Id
	                    left join Proveedor cpintf on cpintf.id = cp.IntermediarioFlete_Id
	                    left join Proveedor cprtte on cprtte.id = cp.RtteComercial_Id
	                    left join Proveedor cpdest on cpdest.id = cp.Destinatario_Id
	                    left join Cliente cpcli on cpcli.id = cp.ClienteDestino_Id
	                    left join Entregador cpe on cpe.id = cp.Entregador_Id
	                    left join Centro CentroDestino on cp.CentroDestino_Id = CentroDestino.id
	                    left join Localidad cpl on cp.Procedencia_Id = cpl.id
	                    left join BajaCTG cpb on cpb.Id = (select top 1 Id from BajaCTG where CartaPorte_Id = cp.Id order by Id desc)
	                    left join MuestraEnvioACamara cpc on  cpc.Id = (select top 1 Id from MuestraEnvioACamara where CartaPorte_Id = cp.Id order by Id desc)
	                    left join Camara cpcc on cpcc.Id = cpc.Camara_Id
	                    left join Lote cpcl on cpcl.Id = cpc.Lote_Id
	                    left join Transportista t on r.Transportista_Id = t.Id
	                    left join CaracteristicasAnalizadas rca on rca.recorrido_id = r.id
	                    left join Almacen ra on ra.Id = r.almacen_id
	                    left join Balanza rbbruto on rbbruto.id = r.BalanzaBruto_Id
	                    left join Balanza rbtara on rbtara.id = r.BalanzaTara_Id
                        left join Cliente ofasonCliR on ofasonCliR.Id = ofason.Remitente_Id
                        left join Cliente ofasonCliC on ofasonCliC.Id = ofason.Comisionista_Id
                        left join Cliente ofasCliC on ofasCliC.Id = ofas.Comisionista_Id
                        left join CartaPorteDerivadoGranario cpdg on r.Id = cpdg.Recorrido_Id
                        left join TipoVariedad tv on tv.Id = r.TipoVariedad_Id
                    where
	                    r.Terminado = 1
	                    and r.Centro_Id IN (" + String.Join(",", centros) + ")"

                        + (materiales.Count > 0 ? ("and r.Material_Id not IN (" + String.Join(",", materiales) + ")") : "")

                        + "and r.TipoComercial_Id IN (" + String.Join(",", tiposComerciales) + @")
	                    and r.fechaEgreso >= '" + fechaInicio.ToString("yyyyMMdd") + @"' AND r.FechaEgreso <= '" + fechaFin.AddDays(1).ToString("yyyyMMdd") + @"'
	                    and (" + (incluirRechazados ? "1" : "0") + @"=1 or (" + (incluirRechazados ? "1" : "0") + @"=0 and r.Rechazado =0))

	                    order by r.id ";

            var resultado = contexto.Database.SqlQuery<ListadoDePesadasDto>(
                query).ToList();

            return resultado;
        }

        public virtual List<ListadoDePesadasDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return ListadoDePesadas(contexto, centros, fechaInicio, fechaFin, tiposComerciales, materiales,
                                        incluirRechazados);
            }
        }
    }
}