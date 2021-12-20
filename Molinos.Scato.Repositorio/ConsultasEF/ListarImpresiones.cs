using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
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
            catch(Exception e)
            {
                return null;
            }
           
        }
    }
}
