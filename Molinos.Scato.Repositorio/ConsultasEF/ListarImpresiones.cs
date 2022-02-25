using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarImpresiones : IConsultaPaginada<ImpresionDto>
    {
        private readonly Paginacion paginacion;
        private readonly TipoDocumentoIngreso? tipodoc;
        private readonly string numerodoc;
        private readonly string patente;
        private readonly TipoImpresion? tipoImpresion;

        public ListarImpresiones(TipoDocumentoIngreso? tipodoc, string numerodoc, string patente, TipoImpresion? tipoImpresion, Paginacion paginacion)
        {
            this.paginacion = paginacion;
            this.numerodoc = numerodoc;
            this.tipodoc = tipodoc;
            this.patente = patente;
            this.tipoImpresion = tipoImpresion;
        }

        public ListaPaginada<ImpresionDto> Ejecutar(DbContext contexto)
        {
            try
            {
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

                var queryCount = GenerarQueryCount(tipodoc, numerodoc, patente, tipoImpresion, paginacion.ItemsPorPagina, paginacion.Pagina);
                var query = GenerarQuery(tipodoc, numerodoc, patente, tipoImpresion, paginacion.ItemsPorPagina, paginacion.Pagina);

                if(!string.IsNullOrEmpty(queryCount) && !string.IsNullOrEmpty(query))
                {
                    var itemsTotales = ObtenerCountItems(contexto, queryCount);
                    var resultados = contexto.Database.SqlQuery<ImpresionDto>(query);
                    var resultado = resultados.OrderByDescending(x => x.FechaImpresion).Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina).ToList();

                    return new ListaPaginada<ImpresionDto>(resultado, paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
                }
                else
                {
                    return consultarImpresion(contexto);
                }
            }
            catch (Exception)
            {
                return consultarImpresion(contexto);
            }
        }

        private ListaPaginada<ImpresionDto> consultarImpresion(DbContext contexto)
        {
            try
            {
                var resultadosCount = contexto.Database.SqlQuery<ImpresionDto>(
                "Select 1 from Impresion where Impresion.Eliminada = 0 and (((@tipoImp is null or @tipoImp = TipoImpresion) and @numeroDocumentoIngreso = '') or EXISTS(select Top 1 1 from Recorrido as R where R.InstanciaWorkflow = Impresion.WorkflowId and (@tipoDocumentoIngreso is null or R.TipoDocumentoIngreso = @tipoDocumentoIngreso) and (@numeroDocumentoIngreso = '' or R.NumeroDocumentoIngreso = @numeroDocumentoIngreso) and (@patente = '' or R.Patente = @patente) and (@tipoImp is null or @tipoImp = TipoImpresion)))"
                , new SqlParameter("@tipoDocumentoIngreso", (object)tipodoc ?? DBNull.Value),
                new SqlParameter("@numeroDocumentoIngreso", numerodoc ?? ""),
                new SqlParameter("@patente", patente ?? ""),
                new SqlParameter("@tipoImp", (object)tipoImpresion ?? DBNull.Value));
                var resultados = contexto.Database.SqlQuery<ImpresionDto>(
                "Select * from Impresion where Impresion.Eliminada = 0 and (((@tipoImp is null or @tipoImp = TipoImpresion) and @numeroDocumentoIngreso = '') or EXISTS(select Top 1 1 from Recorrido as R where R.InstanciaWorkflow = Impresion.WorkflowId and (@tipoDocumentoIngreso is null or R.TipoDocumentoIngreso = @tipoDocumentoIngreso) and (@numeroDocumentoIngreso = '' or R.NumeroDocumentoIngreso = @numeroDocumentoIngreso) and (@patente = '' or R.Patente = @patente) and (@tipoImp is null or @tipoImp = TipoImpresion)))"
                , new SqlParameter("@tipoDocumentoIngreso", (object)tipodoc ?? DBNull.Value),
                new SqlParameter("@numeroDocumentoIngreso", numerodoc ?? ""),
                new SqlParameter("@patente", patente ?? ""),
                new SqlParameter("@tipoImp", (object)tipoImpresion ?? DBNull.Value));
                var itemsTotales = resultadosCount.Count();

                var resultado = resultados.OrderByDescending(x => x.FechaImpresion).Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina).ToList();

                return new ListaPaginada<ImpresionDto>(resultado, paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private int ObtenerCountItems(DbContext contexto, string query)
        {
            return contexto.Database.SqlQuery<Int32>(query).FirstOrDefault();
        }

        private string GenerarQueryCount(TipoDocumentoIngreso? tipodoc, string numerodoc, string patente, TipoImpresion? tipoImpresion, int itemPorPagina, int pagina)
        {
            string query = string.Empty;

            try
            {
                StringBuilder sbfilter = new StringBuilder();
                sbfilter.AppendFormat("Select count(*) as cnt from Impresion where Impresion.Eliminada = 0 and ");
                sbfilter.Append("(");
                sbfilter.Append("EXISTS");
                sbfilter.Append("(");
                sbfilter.Append("select Top 1 1 from Recorrido as R where R.InstanciaWorkflow = Impresion.WorkflowId ");

                if (!(tipodoc is null))
                {
                    sbfilter.AppendFormat("and (R.TipoDocumentoIngreso = {0}) ", (int)tipodoc);
                }

                if (!string.IsNullOrEmpty(numerodoc))
                {
                    sbfilter.AppendFormat("and (R.NumeroDocumentoIngreso = '{0}') ", numerodoc);
                }

                if (!string.IsNullOrEmpty(patente))
                {
                    sbfilter.AppendFormat("and (R.Patente = '{0}') ", patente);
                }

                if (!(tipoImpresion is null))
                {
                    sbfilter.AppendFormat("and ({0} = TipoImpresion)", (int)tipoImpresion);
                }

                sbfilter.Append(")");
                sbfilter.Append(")");

                query = sbfilter.ToString();

                if (!string.IsNullOrEmpty(numerodoc) && (tipodoc is null || TipoDocumentoIngreso.CartaPorte == tipodoc) && (tipoImpresion is null || TipoImpresion.CartaDePorteElectronica == tipoImpresion))
                {
                    query = $"select sum(cnt) as total from ({sbfilter.ToString()} UNION SELECT COUNT(*) from CartaPorteElectronica where Pdf is not null and NroCTG = '{numerodoc}' ) tmp";
                }

            }
            catch (Exception e)
            {
            }

            return query;
        }

        private string GenerarQuery(TipoDocumentoIngreso? tipodoc, string numerodoc, string patente, TipoImpresion? tipoImpresion, int itemPorPagina, int pagina)
        {
            string query = string.Empty;
            string top = $"top {itemPorPagina * pagina}";

            try
            {
                StringBuilder sbfilter = new StringBuilder();
                sbfilter.AppendFormat("Select {0} *, null as Ctg from Impresion where Impresion.Eliminada = 0 and ", !string.IsNullOrEmpty(numerodoc) || !string.IsNullOrEmpty(patente) ? string.Empty : top);
                sbfilter.Append("(");
                sbfilter.Append("EXISTS");
                sbfilter.Append("(");
                sbfilter.Append("select Top 1 1 from Recorrido as R where R.InstanciaWorkflow = Impresion.WorkflowId ");

                if (!(tipodoc is null))
                {
                    sbfilter.AppendFormat("and (R.TipoDocumentoIngreso = {0}) ", (int)tipodoc);
                }

                if (!string.IsNullOrEmpty(numerodoc))
                {
                    sbfilter.AppendFormat("and (R.NumeroDocumentoIngreso = '{0}') ", numerodoc);
                }

                if (!string.IsNullOrEmpty(patente))
                {
                    sbfilter.AppendFormat("and (R.Patente = '{0}') ", patente);
                }

                if (!(tipoImpresion is null))
                {
                    sbfilter.AppendFormat("and ({0} = TipoImpresion)", (int)tipoImpresion);
                }

                sbfilter.Append(")");
                sbfilter.Append(")");

                if (string.IsNullOrEmpty(numerodoc))
                {
                    sbfilter.Append(" order by Impresion.FechaImpresion DESC");
                }

                if (!string.IsNullOrEmpty(numerodoc) && (tipodoc is null || TipoDocumentoIngreso.CartaPorte == tipodoc) && (tipoImpresion is null || TipoImpresion.CartaDePorteElectronica == tipoImpresion))
                {
                    sbfilter.AppendFormat(" union (select 0, '', 29, FechaEmision, SUBSTRING(Dominio,0,CHARINDEX(',',Dominio,0)), 'CartaPorteElectronica', NEWID(), '0', NroCTG from CartaPorteElectronica where Pdf is not null and NroCTG = '{0}')", numerodoc);
                }

                query = sbfilter.ToString();

            }
            catch (Exception e)
            {
            }

            return query;
        }
    }
}
