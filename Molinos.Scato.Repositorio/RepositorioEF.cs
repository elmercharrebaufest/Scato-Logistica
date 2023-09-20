using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Repositorio.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace Molinos.Scato.Repositorio
{
    public class RepositorioEF : IRepositorio
    {
        private const int SqlFkError = 547;

        private readonly DbContext context;

        public RepositorioEF(DbContext context)
        {
            this.context = context;
        }

        private IDbSet<TEntidad> Set<TEntidad>() where TEntidad : class
        {
            return context.Set<TEntidad>();
        }

        public TEntidad Obtener<TEntidad>(object id) where TEntidad : class
        {
            return Set<TEntidad>().Find(id);
        }

        public TEntidad ObtenerUnchanged<TEntidad>(object id) where TEntidad : class
        {
            var entity = Set<TEntidad>().Find(id);
            context.Entry(entity).State = EntityState.Unchanged;
            return entity;
        }

        public TEntidad Obtener<TEntidad>(Expression<Func<TEntidad, bool>> filtro) where TEntidad : class
        {
            return Set<TEntidad>().SingleOrDefault(filtro);
        }

        public TEntidad Obtener<TEntidad>(IEnumerable<Expression<Func<TEntidad, object>>> includes, Expression<Func<TEntidad, bool>> filtro) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            foreach (var i in includes)
            {
                resultado = resultado.Include(i);
            }
            return resultado.SingleOrDefault(filtro);
        }

        public TEntidad ObtenerPrimero<TEntidad>(Expression<Func<TEntidad, bool>> condicion) where TEntidad : class
        {
            return Set<TEntidad>().FirstOrDefault(condicion);
        }

        public TEntidad ObtenerMasReciente<TEntidad>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, DateTime>> columnaFecha) where TEntidad : class
        {
            return Set<TEntidad>().Where(filtro).OrderByDescending(columnaFecha).FirstOrDefault();
        }

        public TProyeccion ObtenerProyeccion<TEntidad, TProyeccion>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TProyeccion>> proyeccion)
            where TEntidad : class
        {
            return Set<TEntidad>().Where(filtro).Select(proyeccion).FirstOrDefault();
        }

        public TProyeccion ObtenerMayor<TEntidad, TOrden, TProyeccion>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TOrden>> columnaOrden, Expression<Func<TEntidad, TProyeccion>> proyeccion)
            where TEntidad : class
            where TOrden : IComparable
        {
            return Set<TEntidad>().Where(filtro).OrderByDescending(columnaOrden).Select(proyeccion).FirstOrDefault();
        }

        public TEntidad ObtenerMenor<TEntidad, TOrden>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TOrden>> columnaOrden)
            where TEntidad : class
            where TOrden : IComparable
        {
            return Set<TEntidad>().Where(filtro).OrderBy(columnaOrden).FirstOrDefault();
        }

        public TEntidad ObtenerMayor<TEntidad, TOrden>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TOrden>> columnaOrden)
            where TEntidad : class
            where TOrden : IComparable
        {
            return Set<TEntidad>().Where(filtro).OrderByDescending(columnaOrden).FirstOrDefault();
        }

        public TProyeccion ObtenerMenor<TEntidad, TOrden, TProyeccion>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TOrden>> columnaOrden, Expression<Func<TEntidad, TProyeccion>> proyeccion)
            where TEntidad : class
            where TOrden : IComparable
        {
            return Set<TEntidad>().Where(filtro).OrderBy(columnaOrden).Select(proyeccion).FirstOrDefault();
        }

        public IList<TEntidad> Listar<TEntidad>(Expression<Func<TEntidad, bool>> filtro = null) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            if (filtro != null)
            {
                resultado = resultado.Where(filtro);
            }
            return resultado.ToList();
        }

        public IList<TEntidad> Listar<TEntidad>(IEnumerable<Expression<Func<TEntidad, object>>> includes, Expression<Func<TEntidad, bool>> filtro) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            foreach (var i in includes)
            {
                resultado = resultado.Include(i);
            }

            if (filtro != null)
            {
                resultado = resultado.Where(filtro);
            }
            return resultado.ToList();
        }

        public IList<TProyeccion> Listar<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, bool>> filtro = null) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            if (filtro != null)
            {
                resultado = resultado.Where(filtro);
            }
            return resultado.Select(proyeccion).ToList();
        }

        public IList<TEntidad> ListarNoTracking<TEntidad>(Expression<Func<TEntidad, bool>> filtro = null) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            if (filtro != null)
            {
                resultado = resultado.Where(filtro).AsNoTracking();
            }
            return resultado.ToList();
        }

        public IList<TProyeccion> ListarNoTracking<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, bool>> filtro = null) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            if (filtro != null)
            {
                resultado = resultado.Where(filtro).AsNoTracking();
            }
            return resultado.Select(proyeccion).ToList();
        }

        public decimal Sumar<TEntidad>(Expression<Func<TEntidad, decimal>> proyeccion, Expression<Func<TEntidad, bool>> filtro = null) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            if (filtro != null)
            {
                resultado = resultado.Where(filtro);
            }
            return resultado.Select(proyeccion).DefaultIfEmpty(0).Sum();
        }

        public IList<TEntidad> Listar<TEntidad>(Expression<Func<TEntidad, bool>> condicion, int maxResultados) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            if (condicion != null)
            {
                resultado = resultado.Where(condicion);
            }
            return resultado.Take(maxResultados).ToList();
        }

        public ListaPaginada<TProyeccion> Listar<TEntidad, TKey, TProyeccion>(Expression<Func<TProyeccion, TKey>> funcProyeccion, Expression<Func<TEntidad, TKey>> funcEntidad, Paginacion paginacion, Expression<Func<TEntidad, bool>> filtroEntidad = null, Expression<Func<TProyeccion, bool>> filtroProyeccion = null)
            where TEntidad : class
            where TProyeccion : class
        {
            IQueryable<TEntidad> entidades = Set<TEntidad>();
            IQueryable<TProyeccion> proyecciones = Set<TProyeccion>();
            if (filtroEntidad != null)
            {
                entidades = entidades.Where(filtroEntidad);
            }
            proyecciones = entidades.Join(proyecciones, funcEntidad, funcProyeccion, (x, y) => y);

            if (filtroProyeccion != null)
            {
                proyecciones = proyecciones.Where(filtroProyeccion);
            }

            int itemsTotales = proyecciones.Count();

            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<TProyeccion>(paginacion.OrdenarPor);
                proyecciones = paginacion.DireccionOrden == DirOrden.Asc
                                 ? proyecciones.OrderBy(selectorOrden)
                                 : proyecciones.OrderByDescending(selectorOrden);
            }

            proyecciones = proyecciones.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<TProyeccion>(proyecciones.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }

        public ListaPaginada<TEntidad> Listar<TEntidad>(Expression<Func<TEntidad, bool>> condicion, Paginacion paginacion) where TEntidad : class
        {
            IQueryable<TEntidad> resultados = Set<TEntidad>();
            if (condicion != null)
            {
                resultados = resultados.Where(condicion);
            }

            int itemsTotales = resultados.Count();

            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<TEntidad>(paginacion.OrdenarPor);
                resultados = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultados.OrderBy(selectorOrden)
                                 : resultados.OrderByDescending(selectorOrden);
            }

            if (paginacion.ItemsPorPagina > 0) 
                resultados = resultados.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<TEntidad>(resultados.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }

        public ListaPaginada<TProyeccion> Listar<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, bool>> filtro, Paginacion paginacion) where TEntidad : class
        {
            IQueryable<TEntidad> entidades = Set<TEntidad>();
            if (filtro != null)
            {
                entidades = entidades.Where(filtro);
            }
            var resultados = entidades.Select(proyeccion);
            int itemsTotales = resultados.Count();

            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<TProyeccion>(paginacion.OrdenarPor);
                resultados = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultados.OrderBy(selectorOrden)
                                 : resultados.OrderByDescending(selectorOrden);
            }

            resultados = resultados.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<TProyeccion>(resultados.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }

        public List<TProyeccion> Listar<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, Boolean>> condicion, int maxResultados) where TEntidad : class
        {
            IQueryable<TEntidad> entidades = Set<TEntidad>();
            if (condicion != null)
            {
                entidades = entidades.Where(condicion);
            }
            entidades = entidades.Take(maxResultados);
            return entidades.Select(proyeccion).ToList();
        }

        public List<TProyeccion> ListarDistintos<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, Boolean>> condicion, int maxResultados) where TEntidad : class
        {
            IQueryable<TEntidad> entidades = Set<TEntidad>();
            if (condicion != null)
            {
                entidades = entidades.Where(condicion).GroupBy(proyeccion).Select(g => g.FirstOrDefault());
            }
            entidades = entidades.Take(maxResultados);
            return entidades.Select(proyeccion).ToList();
        }

        public ListaPaginada<TEntidad> ListarConsultaPaginada<TEntidad>(IConsultaPaginada<TEntidad> consulta) where TEntidad : class
        {
            return consulta.Ejecutar(context);
        }

        public List<TEntidad> ListarConsulta<TEntidad>(IConsulta<TEntidad> consulta) where TEntidad : class
        {
            return consulta.Ejecutar(context);
        }

        public TEntidad ObtenerConsultaEscalar<TEntidad>(IConsultaEscalar<TEntidad> consulta)
        {
            return consulta.Ejecutar(context);
        }

        public int Contar<TEntidad>() where TEntidad : class
        {
            return Set<TEntidad>().Count();
        }

        public int Contar<TEntidad>(Expression<Func<TEntidad, bool>> filtro) where TEntidad : class
        {
            return Set<TEntidad>().Count(filtro);
        }

        public bool Existe<TEntidad>(Expression<Func<TEntidad, bool>> filtro) where TEntidad : class
        {
            // Esto genera una query más optima que usar un Any()
            return Set<TEntidad>().Where(filtro).Select(x => 1).FirstOrDefault() != 0;
        }

        public TEntidad Agregar<TEntidad>(TEntidad entidad) where TEntidad : class
        {
            return Set<TEntidad>().Add(entidad);
        }

        public virtual void ActualizarTodos<TEntidad>(IEnumerable<TEntidad> items, List<KeyValuePair<string, string>> properties = null, string columnaJoin = "Id", string where = "") where TEntidad : class
        {
            var enumerable = items as IList<TEntidad> ?? items.ToList();
            if (enumerable.Any())
            {
                var dataTable = enumerable.ToDataTable(properties != null, properties);
                context.SqlBulkUpdate(dataTable, dataTable.TableName, columnaJoin, where);
            }
        }

        public TEntidad Remover<TEntidad>(object id) where TEntidad : class
        {
            return Remover(Obtener<TEntidad>(id));
        }

        public TEntidad Remover<TEntidad>(TEntidad entidad) where TEntidad : class
        {
            return entidad != null ? Set<TEntidad>().Remove(entidad) : null;
        }

        public void RemoverTodos<TEntidad>(IEnumerable<TEntidad> entidades) where TEntidad : class
        {
            foreach (var entidad in entidades)
            {
                Set<TEntidad>().Remove(entidad);
            }
        }

        public TResultado EjecutarComando<TResultado>(IComando<TResultado> comando)
        {
            return comando.Ejecutar(context);
        }

        public int GuardarCambios()
        {
            try
            {
                return context.SaveChanges();
            }
            catch (DataException e)
            {
                if (ObtenerCodigoError(e) == SqlFkError)
                {
                    throw new EntidadReferenciadaException(string.Empty, e);
                }
                throw;
            }
        }

        private int ObtenerCodigoError(DataException e)
        {
            var code = 0;
            if (e.InnerException != null)
            {
                var sqlEx = e.InnerException.InnerException as SqlException;
                if (sqlEx != null)
                {
                    code = sqlEx.Number;
                }
            }
            return code;
        }

        public int ObtenerNumeroDocumentoGenerado()
        {
            return context.Database.SqlQuery<int>(
                @"
		            begin
		                  declare @NewSeqValue int
		                  set NOCOUNT ON
                          SET TRANSACTION ISOLATION LEVEL READ COMMITTED
		                  insert into GeneradorNumeroDocumento (SeqVal) values ('a')
		                  set @NewSeqValue = scope_identity()
		                  delete from GeneradorNumeroDocumento WITH (READPAST)
		            select @NewSeqValue
		            end "
                ).First();
        }

        public int ObtenerNumeroMuestraAuditoriaGenerado()
        {
            return context.Database.SqlQuery<int>(
                @"
		            begin
		                  declare @NewSeqValue int
		                  set NOCOUNT ON
		                  insert into GeneradorNumeroMuestraAuditoria (SeqVal) values ('a')
		                  set @NewSeqValue = scope_identity()
		                  delete from GeneradorNumeroMuestraAuditoria WITH (READPAST)
		            select @NewSeqValue
		            end "
                ).First();
        }

        public int ObtenerNumeroDeTicketGenerado(int puestoDeTrabajoId, bool pagoConMercadoPago)
        {
            if (puestoDeTrabajoId == 0)
            {
                puestoDeTrabajoId = context.Database.SqlQuery<int>(@"select top 1 PuestoDeTrabajo_Id from GeneradorNumeroDeTicket2").First();
            }
            return context.Database.SqlQuery<int>(
                @"
		            begin
		                  declare @NewSeqValue int
		                  set NOCOUNT ON
		                  insert into GeneradorNumeroDeTicket2 (PuestoDeTrabajo_Id,pagoConMercadoPago) values (" + puestoDeTrabajoId + "," + (pagoConMercadoPago ? 1 : 0) + @")

                            SELECT @NewSeqValue = ISNULL(MAX([Id]),0)
		                    FROM GeneradorNumeroDeTicket2
		                    WHERE [PuestoDeTrabajo_Id] = " + puestoDeTrabajoId + @" AND [PagoConMercadoPago] = " + (pagoConMercadoPago ? 1 : 0) + @"

                          delete from GeneradorNumeroDeTicket2 WITH (READPAST) WHERE [PuestoDeTrabajo_Id] = " + puestoDeTrabajoId + "AND [PagoConMercadoPago] = " + (pagoConMercadoPago ? 1 : 0) + @" and id <> @NewSeqValue
		            select @NewSeqValue
		            end"
                ).First();
        }

        public int ObtenerNumeroDeTicketImportacionGenerado()
        {
            return context.Database.SqlQuery<int>(
                @"
		            begin
		                  declare @NewSeqValue int
		                  set NOCOUNT ON
		                  insert into GeneradorNumeroDeTicketImportacion (SeqVal) values ('a')
		                  set @NewSeqValue = scope_identity()
		                  delete from GeneradorNumeroDeTicketImportacion WITH (READPAST)
		            select @NewSeqValue
		            end "
                ).First();
        }

        public int ObtenerSecuenciaEnvioACamara()
        {
            return context.Database.SqlQuery<int>(
                @"
		            begin
		                  declare @NewSeqValue int
		                  set NOCOUNT ON
		                  insert into GeneradorSecuenciaEnvioACamara (SeqVal) values ('a')
		                  set @NewSeqValue = scope_identity()
		                  delete from GeneradorSecuenciaEnvioACamara WITH (READPAST)
		            select @NewSeqValue
		            end "
                ).First();
        }

        public int ObtenerNumeroDocumentoFasonGenerado()
        {
            return context.Database.SqlQuery<int>(
                @"
		            begin
		                  declare @NewSeqValue int
		                  set NOCOUNT ON
		                  insert into GeneradorNumeroDocumentoFason (SeqVal) values ('a')
		                  set @NewSeqValue = scope_identity()
		                  delete from GeneradorNumeroDocumentoFason WITH (READPAST)
		            select @NewSeqValue
		            end "
                ).First();
        }

        public int ObtenerNumeroOrdenDeDescargaGenerado()
        {
            return context.Database.SqlQuery<int>(
                @"
		            begin
		                  declare @NewSeqValue int
		                  set NOCOUNT ON
		                  insert into GeneradorNumeroOrdenDeDescarga (SeqVal) values ('a')
		                  set @NewSeqValue = scope_identity()
		                  delete from GeneradorNumeroOrdenDeDescarga WITH (READPAST)
		            select @NewSeqValue
		            end "
                ).First();
        }

        public int ObtenerNumeroOrdenEntrePlantasGenerado()
        {
            return context.Database.SqlQuery<int>(
                @"
		            begin
		                  declare @NewSeqValue int
		                  set NOCOUNT ON
		                  insert into GeneradorNumeroOrdenEntrePlantas (SeqVal) values ('a')
		                  set @NewSeqValue = scope_identity()
		                  delete from GeneradorNumeroOrdenEntrePlantas WITH (READPAST)
		            select @NewSeqValue
		            end "
                ).First();
        }

        public int ObtenerNumeroOrdenDeDescargaFasonGenerado()
        {
            return context.Database.SqlQuery<int>(
                @"
		            begin
		                  declare @NewSeqValue int
		                  set NOCOUNT ON
		                  insert into GeneradorNumeroOrdenDeDescargaFason (SeqVal) values ('a')
		                  set @NewSeqValue = scope_identity()
		                  delete from GeneradorNumeroOrdenDeDescargaFason WITH (READPAST)
		            select @NewSeqValue
		            end "
                ).First();
        }

        public int ObtenerNumeroHojaDeRutaGenerado()
        {
            return context.Database.SqlQuery<int>(
                @"
		            begin
		                  declare @NewSeqValue int
		                  set NOCOUNT ON
		                  insert into GeneradorNumeroHojaDeRuta (SeqVal) values ('a')
		                  set @NewSeqValue = scope_identity()
		                  delete from GeneradorNumeroHojaDeRuta WITH (READPAST)
		            select @NewSeqValue
		            end "
                ).First();
        }

        public int ObtenerNumeroRemitoGenerado()
        {
            return context.Database.SqlQuery<int>(
                @"
		            begin
		                  declare @NewSeqValue int
		                  set NOCOUNT ON
		                  insert into GeneradorNumeroRemito (SeqVal) values ('a')
		                  set @NewSeqValue = scope_identity()
		                  delete from GeneradorNumeroRemito WITH (READPAST)
		            select @NewSeqValue
		            end "
                ).First();
        }

        public int ObtenerNumeroOrdenDeCargaContenedorGenerado()
        {
            return context.Database.SqlQuery<int>(
                @"
		            begin
		                  declare @NewSeqValue int
		                  set NOCOUNT ON
		                  insert into GeneradorNumeroOrdenDeCargaContenedor (SeqVal) values ('a')
		                  set @NewSeqValue = scope_identity()
		                  delete from GeneradorNumeroOrdenDeCargaContenedor WITH (READPAST)
		            select @NewSeqValue
		            end "
                ).First();
        }

        public int ObtenerNumeroDeControlDeCargaGenerado()
        {
            return context.Database.SqlQuery<int>(
                @"
		            begin
		                  declare @NewSeqValue int
		                  set NOCOUNT ON
		                  insert into GeneradorControlDeCarga (SeqVal) values ('a')
		                  set @NewSeqValue = scope_identity()
		                  delete from GeneradorNumeroControlDeCarga WITH (READPAST)
		            select @NewSeqValue
		            end "
                ).First();
        }

        public int ObtenerNumeroAleatorio()
        {
            return context.Database.SqlQuery<int>(
                @"
		            begin
		                  declare @Value int;
                          set @Value = CAST(RAND(CAST(NEWID() AS varbinary)) * 100 AS INT)
		            select @Value
		            end "
                ).First();
        }

        public int ObtenerSequenciaCPENroCTG()
        {
            Int64 nextId = context.Database.SqlQuery<Int64>("SELECT NEXT VALUE FOR NumeroCPESeq").FirstOrDefault();
            return (int)nextId;
        }
    }
}