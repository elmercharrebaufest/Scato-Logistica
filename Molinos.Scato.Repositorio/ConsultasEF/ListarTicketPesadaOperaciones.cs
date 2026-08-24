using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarTicketPesadaOperaciones : IConsulta<TicketPesadaDto>
    {
        private readonly List<int> tiposComerciales;
        private readonly DateTime fechaInicio;
        private readonly DateTime fechaFin;
        private readonly int idCentro;
        private readonly bool esAdmin;
        private readonly string cuitTransportista;
        private readonly string cuitProveedor;
        private readonly string cuitIntermediarioFlete;
        private readonly string numeroCTG;
        private readonly string patente;


        public ListarTicketPesadaOperaciones(List<int> tiposComerciales, DateTime fechaInicio, DateTime fechaEgreso, bool esAdmin, string cuitTransportista, string cuitProveedor, 
            string cuitIntermediarioFlete, string numeroCTG, string patente)
        {
            this.tiposComerciales = tiposComerciales;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaEgreso;
            this.idCentro = Constantes.Centro.IdSanLorenzo;
            this.esAdmin = esAdmin;
            this.cuitTransportista = cuitTransportista;
            this.cuitProveedor = cuitProveedor;
            this.cuitIntermediarioFlete = cuitIntermediarioFlete;
            this.numeroCTG = numeroCTG;
            this.patente = patente;
        }

        public List<TicketPesadaDto> ListadoTicketPesadaOperaciones(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var query = new StringBuilder(@"
                    SELECT
                    CTG = CASE when r.TipoDocumentoIngreso = 1 THEN SUBSTRING(r.NumeroDocumentoIngreso, 0,5)  + '-' + SUBSTRING(r.NumeroDocumentoIngreso,5, 9) ELSE r.NumeroDocumentoIngreso END,
                    BrutoOrigen = r.PesoBrutoOrigen,
                    TaraOrigen = r.PesoTaraOrigen,
                    NetoOrigen = CASE
                                WHEN r.PesoBrutoOrigen IS NOT NULL AND r.PesoTaraOrigen IS NOT NULL
                                THEN r.PesoBrutoOrigen - r.PesoTaraOrigen
                                ELSE NULL
                                END,
                    BrutoPlanta = r.PesoBruto,
                    TaraPlanta = r.PesoTara,
                    NetoPlanta = CASE
                                WHEN r.PesoBruto IS NOT NULL AND r.PesoTara IS NOT NULL
                                THEN r.PesoBruto - r.PesoTara
                                ELSE NULL
                                END,
                    FechaHoraIngreso = r.FechaInicio,
                    FechaHoraEgreso = r.FechaEgreso,
                    Procedencia = CASE
                          WHEN w.TipoDeWorkflow = 1
                          THEN rcl.Descripcion
                          ELSE ISNULL(cpl.Descripcion,
                          ISNULL(ofasc.Localidad,
                          ISNULL(odescpl.Descripcion,
                          ISNULL(odescfasonl.Descripcion,
                          ISNULL(remcl.Descripcion,
                          ISNULL(hycl.Descripcion, ''))))))
                          END,
                    IntermediarioFlete = ISNULL(cpintf.Descripcion, ''),
                    IntermediarioFleteCUIT = REPLACE(cpintf.Cuil, '-', ''),
                    Transportista = ISNULL(t.RazonSocial, ''),
                    TransportistaCUIT = REPLACE(ISNULL(t.Cuit, ''), '-', ''),
                    Patente = r.Patente,
                    ChoferNombreApellido = LTRIM(ISNULL(c.Nombre, '') + ' ' + ISNULL(c.Apellido, '')),
                    Material = ISNULL(mat.Descripcion, ''),
                    TitularCPCUIT = REPLACE(ISNULL(cptit.Cuil, ISNULL(odescp.Cuil, '')), '-', '')

                FROM Recorrido r
                INNER JOIN Workflow w ON r.Workflow_Id = w.Id
                INNER JOIN Centro rc ON r.Centro_Id = rc.Id
                INNER JOIN Chofer c ON r.Chofer_Id = c.Id
                LEFT JOIN Vehiculo v ON r.Vehiculo_Id = v.Id
                LEFT JOIN CartaPorte cp ON v.CartaPorte_Id = cp.Id
                LEFT JOIN Proveedor cptit ON cptit.Id = cp.TitularCartaPorte_Id
                LEFT JOIN OrdenDeDescarga odesc ON r.Id = odesc.Recorrido_Id
                LEFT JOIN Proveedor odescp ON odescp.Id = odesc.Proveedor_Id
                LEFT JOIN Centro wc ON w.Centro_Id = wc.Id
                LEFT JOIN Localidad rcl ON rc.Localidad_Id = rcl.Id
                LEFT JOIN OrdenDeDescargaFason odescfason ON r.Id = odescfason.Recorrido_Id
                LEFT JOIN Localidad odescfasonl ON odescfasonl.Id = odescfason.Procedencia_Id
                LEFT JOIN Material mat ON r.Material_Id = mat.Id
                LEFT JOIN OrdenCargaFas ofas ON r.Id = ofas.Recorrido_Id
                LEFT JOIN Cliente ofasd ON ofasd.Id = ofas.Destinatario_Id
                LEFT JOIN Cliente ofasc ON ofasc.Id = ofas.Cliente_Id
                LEFT JOIN Cliente odescfasonc ON odescfasonc.Id = odescfason.Cliente_Id
                LEFT JOIN Localidad cpl ON cp.Procedencia_Id = cpl.Id
                LEFT JOIN Localidad odescpl ON odescpl.Id = odescp.Localidad_Id
                LEFT JOIN Remito rem ON r.Id = rem.Recorrido_Id
                LEFT JOIN Centro remc ON remc.Id = rem.CentroOrigen_Id
                LEFT JOIN Localidad remcl ON remcl.Id = remc.Localidad_Id
                LEFT JOIN HojaDeRutaYerbatera hy ON r.Id = hy.Recorrido_Id
                LEFT JOIN Centro hyc ON hyc.Id = hy.CentroDestino_Id                    
                LEFT JOIN Localidad hycl ON hycl.Id = hyc.Localidad_Id
                LEFT JOIN Proveedor cpintf ON cpintf.Id = cp.IntermediarioFlete_Id
                LEFT JOIN Transportista t ON r.Transportista_Id = t.Id

        WHERE r.Terminado = 1
        AND rc.Id = " + this.idCentro + @"
        AND r.TipoComercial_Id IN (" + string.Join(",", this.tiposComerciales) + @")
        AND r.FechaEgreso >= '" + this.fechaInicio.ToString("yyyy-MM-dd HH:mm:ss") + @"'
        AND r.FechaEgreso <= '" + this.fechaFin.ToString("yyyy-MM-dd HH:mm:ss") + @"'
        AND r.Rechazado = 0");

            // Condiciones adicionales según esAdmin
            if (this.esAdmin)
            {
                if (!string.IsNullOrEmpty(this.cuitTransportista))
                {
                    query.Append(" AND t.Cuit = '" + this.cuitTransportista + "'");
                }
                if (!string.IsNullOrEmpty(this.cuitProveedor))
                {
                    query.Append(" AND cptit.Cuil = '" + this.cuitProveedor + "'");
                }
                if (!string.IsNullOrEmpty(this.cuitIntermediarioFlete))
                {
                    query.Append(" AND cpintf.Cuil = '" + this.cuitIntermediarioFlete + "'");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(this.cuitProveedor))
                {
                    query.Append(" AND (t.Cuit = '" + this.cuitProveedor + "' OR cpintf.Cuil = '" + this.cuitProveedor + "' OR cptit.Cuil = '" + this.cuitProveedor + "')");
                }
            }

            // Condiciones comunes
            if (!string.IsNullOrEmpty(this.numeroCTG))
            {
                query.Append(" AND cp.CTG = '" + this.numeroCTG + "'");
            }
            if (!string.IsNullOrEmpty(this.patente))
            {
                query.Append(" AND r.Patente = '" + this.patente + "'");
            }

            query.Append(" ORDER BY r.Id");

            var resultado = contexto.Database.SqlQuery<TicketPesadaDto>(query.ToString()).ToList();

            return resultado;
        }

        public virtual List<TicketPesadaDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return ListadoTicketPesadaOperaciones(contexto);
            }
        }
    }
}