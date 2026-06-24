using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Molinos.Scato.Repositorio
{
    /// <summary>
    /// Representa el comportamiento de un repositorio
    /// </summary>
    public interface IRepositorio
    {
        /// <summary>
        /// Devuelve una entidad dada su clave primaria
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a devolver</typeparam>
        /// <param name="id">clave primaria de la entidad</param>
        /// <returns>La entidad que posee esa clave o null si no se encuentra ninguna</returns>
        TEntidad Obtener<TEntidad>(object id) where TEntidad : class;

        /// <summary>
        /// Devuelve una entidad dada su clave primaria con estado Unchanged
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a devolver</typeparam>
        /// <param name="id">clave primaria de la entidad</param>
        /// <returns>La entidad que posee esa clave con estado Unchanged o null si no se encuentra ninguna</returns>
        TEntidad ObtenerUnchanged<TEntidad>(object id) where TEntidad : class;

        /// <summary>
        /// Devuelve una unica entidad que cumple con la condición
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a devolver</typeparam>
        /// <param name="condicion">Expresion que dada una entidad devuelve si debe devolverse o no</param>
        /// <returns>La unica entidad que se corresponde con la condicion o null si no la encuentra</returns>
        /// <exception cref="InvalidOperationException">Se lanza cuando más de una entidad comple con la condición</exception>
        TEntidad Obtener<TEntidad>(Expression<Func<TEntidad, bool>> condicion) where TEntidad : class;

        TEntidad Obtener<TEntidad>(IEnumerable<Expression<Func<TEntidad, object>>> includes, Expression<Func<TEntidad, bool>> condicion) where TEntidad : class;

        TEntidad ObtenerPrimero<TEntidad>(Expression<Func<TEntidad, bool>> condicion) where TEntidad : class;

        /// <summary>
        /// Devuelve la entidad más reciente que cumple la condición
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a devolver</typeparam>
        /// <param name="filtro">Expresion que dada una entidad devuelve si debe devolverse o no</param>
        /// <param name="columnaFecha">Indica cual es la columna de tipo Datetime que se debe usar para el ordenamiento</param>
        /// <returns>La entidad más reciente que se corresponde con la condicion o null si no encuentra ninguna</returns>
        TEntidad ObtenerMasReciente<TEntidad>(Expression<Func<TEntidad, bool>> filtro,
                                              Expression<Func<TEntidad, DateTime>> columnaFecha) where TEntidad : class;

        TProyeccion ObtenerProyeccion<TEntidad, TProyeccion>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TProyeccion>> proyeccion)
            where TEntidad : class;

        TProyeccion ObtenerMayor<TEntidad, TOrden, TProyeccion>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TOrden>> columnaOrden, Expression<Func<TEntidad, TProyeccion>> proyeccion)
            where TEntidad : class where TOrden : IComparable;

        TEntidad ObtenerMenor<TEntidad, TOrden>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TOrden>> columnaOrden)
            where TEntidad : class
            where TOrden : IComparable;

        TEntidad ObtenerMayor<TEntidad, TOrden>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TOrden>> columnaOrden)
            where TEntidad : class
            where TOrden : IComparable;

        TProyeccion ObtenerMenor<TEntidad, TOrden, TProyeccion>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TOrden>> columnaOrden, Expression<Func<TEntidad, TProyeccion>> proyeccion)
            where TEntidad : class
            where TOrden : IComparable;

        /// <summary>
        /// Lista todas las entidades que complen con la condicion
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a devolver</typeparam>
        /// <param name="condicion">Expresion que dada una entidad devuelve si debe devolverse o no</param>
        /// <returns>Todas las entidades que cumplen con la condicion</returns>
        IList<TEntidad> Listar<TEntidad>(Expression<Func<TEntidad, Boolean>> condicion = null) where TEntidad : class;

        IList<TEntidad> Listar<TEntidad>(IEnumerable<Expression<Func<TEntidad, object>>> includes, Expression<Func<TEntidad, Boolean>> condicion = null) where TEntidad : class;

        IList<TProyeccion> Listar<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, Boolean>> condicion = null) where TEntidad : class;

        IList<TEntidad> ListarNoTracking<TEntidad>(Expression<Func<TEntidad, bool>> filtro = null) where TEntidad : class;

        IList<TProyeccion> ListarNoTracking<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, Boolean>> condicion = null) where TEntidad : class;

        List<TProyeccion> ListarDistintos<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, Boolean>> condicion, int maxResultados) where TEntidad : class;

        IList<TEntidad> Listar<TEntidad>(Expression<Func<TEntidad, Boolean>> condicion, int maxResultados) where TEntidad : class;

        ListaPaginada<TProyeccion> Listar<TEntidad, TKey, TProyeccion>(Expression<Func<TProyeccion, TKey>> funcProyeccion, Expression<Func<TEntidad, TKey>> funcEntidad, Paginacion paginacion, Expression<Func<TEntidad, bool>> filtroEntidad = null, Expression<Func<TProyeccion, bool>> filtroProyeccion = null)
            where TEntidad : class
            where TProyeccion : class;

        ListaPaginada<TEntidad> Listar<TEntidad>(Expression<Func<TEntidad, Boolean>> condicion, Paginacion paginacion) where TEntidad : class;

        ListaPaginada<TProyeccion> Listar<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, Boolean>> condicion, Paginacion paginacion) where TEntidad : class;

        List<TProyeccion> Listar<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, Boolean>> condicion, int maxResultados) where TEntidad : class;

        ListaPaginada<TEntidad> ListarConsultaPaginada<TEntidad>(IConsultaPaginada<TEntidad> consulta) where TEntidad : class;

        List<TEntidad> ListarConsulta<TEntidad>(IConsulta<TEntidad> consulta) where TEntidad : class;

        TEntidad ObtenerConsultaEscalar<TEntidad>(IConsultaEscalar<TEntidad> consulta);

        decimal Sumar<TEntidad>(Expression<Func<TEntidad, decimal>> proyeccion, Expression<Func<TEntidad, bool>> filtro = null) where TEntidad : class;

        /// <summary>
        /// Lista todas las entidades que cumplen con la condicion devolviendo solo una pagina de resultados
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a devolver</typeparam>
        /// <param name="consulta">Consulta paginada que contiene la condicion, el orden y la pagina a devolver</param>
        /// <returns>Todas las entidades que cumplen con la condicion paginando el resultado</returns>
        // ListaPaginada<TEntidad> Listar<TEntidad>(ConsultaPaginada<TEntidad> consulta) where TEntidad : class;

        /// <summary>
        /// Devuelve la cantidad de entidades en el repositorio
        /// </summary>
        /// <typeparam name="TEntidad">Tipo entidades a contar</typeparam>
        /// <returns>La cantidad de entidades de un tipo determinado en el repositorio</returns>
        int Contar<TEntidad>() where TEntidad : class;

        /// <summary>
        /// Devuelve la cantidad de entidades en el repositorio que cumplen con la condicion
        /// </summary>
        /// <typeparam name="TEntidad">Tipo entidades a contar</typeparam>
        /// <param name="condicion">Expresion que dada una entidad devuelve si debe contarse o no</param>
        /// <returns>La cantidad de entidades de un tipo determinado en el repositorio</returns>
        int Contar<TEntidad>(Expression<Func<TEntidad, Boolean>> condicion) where TEntidad : class;

        /// <summary>
        /// Devuelve si existe una entidad que cumpla con la condicion
        /// </summary>
        /// <typeparam name="TEntidad">Tipo entidades a comprobar</typeparam>
        /// <param name="condicion">Expresion que dada una entidad devuelve si debe contarse o no</param>
        /// <returns>true si existe alguna entidad que compla con la condición</returns>
        bool Existe<TEntidad>(Expression<Func<TEntidad, Boolean>> condicion) where TEntidad : class;

        /// <summary>
        /// Agrega una entidad al repositorio
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a agregar</typeparam>
        /// <param name="entidad">Entidad a agregar</param>
        /// <returns>Entidad agregada</returns>
        TEntidad Agregar<TEntidad>(TEntidad entidad) where TEntidad : class;

        void ActualizarTodos<TEntidad>(IEnumerable<TEntidad> items, List<KeyValuePair<string, string>> properties = null, string columnaJoin = "Id", string where = "") where TEntidad : class;

        /// <summary>
        /// Remueve una entidad del repositorio
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a remover</typeparam>
        /// <param name="entidad">Entidad a remover</param>
        /// <returns>Entidad removida</returns>
        TEntidad Remover<TEntidad>(TEntidad entidad) where TEntidad : class;

        /// <summary>
        /// Remueve una entidad del repositorio dada su clave primaria
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a remover</typeparam>
        /// <param name="id">clave de la entidad a remover</param>
        /// <returns>Entidad removida</returns>
        TEntidad Remover<TEntidad>(object id) where TEntidad : class;

        void RemoverTodos<TEntidad>(IEnumerable<TEntidad> entidades) where TEntidad : class;

        /// <summary>
        /// Ejecuta un comando insert/update en la base
        /// </summary>
        /// <param name="comando"></param>
        TResultado EjecutarComando<TResultado>(IComando<TResultado> comando);

        /// <summary>
        /// Persiste los cambios hechos al repositorio
        /// </summary>
        /// <returns>Cantidad de entidades agregadas/actualizadas</returns>
        int GuardarCambios();

        /// <summary>
        /// Retorna un numero de secuencia para crear un numero de documento
        /// </summary>
        /// <returns>Ultima semilla de la tabla GeneradorNumeroDocumento</returns>
        int ObtenerNumeroDocumentoGenerado();

        /// <summary>
        /// Retorna un numero de secuencia para crear un numero de muestra auditoria
        /// </summary>
        /// <returns>Ultima semilla de la tabla GeneradorNumeroMuestraAuditoria</returns>
        int ObtenerNumeroMuestraAuditoriaGenerado();

        /// <summary>
        /// Retorna un numero de secuencia para crear un numero de documento
        /// </summary>
        /// <returns>Ultima semilla de la tabla GeneradorNumeroDeTicket</returns>
        int ObtenerNumeroDeTicketGenerado(int puestoDeTrabajoId, bool pagoConMercadoPago);

        int ObtenerNumeroDeTicketImportacionGenerado();

        /// <summary>
        /// Retorna un numero de secuencia para crear un numero de documento fason
        /// </summary>
        /// <returns>Ultima semilla de la tabla GeneradorNumeroDocumentoFason</returns>
        int ObtenerNumeroDocumentoFasonGenerado();

        /// <summary>
        /// Retorna un numero de secuencia para crear un numero de OrdenDeDescarga
        /// </summary>
        /// <returns>Ultima semilla de la tabla GeneradorNumeroOrdenDeDescarga</returns>
        int ObtenerNumeroOrdenDeDescargaGenerado();

        /// <summary>
        /// Retorna un numero de secuencia para crear un numero de OrdenEntrePlantas
        /// </summary>
        /// <returns>Ultima semilla de la tabla GeneradorNumeroOrdenEntrePlantas</returns>
        int ObtenerNumeroOrdenEntrePlantasGenerado();

        /// <summary>
        /// Retorna un numero de secuencia para crear un numero de OrdenDeDescargaFason
        /// </summary>
        /// <returns>Ultima semilla de la tabla GeneradorNumeroOrdenDeDescargaFason</returns>
        int ObtenerNumeroOrdenDeDescargaFasonGenerado();

        /// <summary>
        /// Retorna un numero de secuencia para crear un numero de HojaDeRuta
        /// </summary>
        /// <returns>Ultima semilla de la tabla GeneradorNumeroHojaDeRuta</returns>
        int ObtenerNumeroHojaDeRutaGenerado();

        /// <summary>
        /// Retorna un numero de secuencia para crear un numero de Secuencia envio a cámara
        /// </summary>
        /// <returns>Ultima semilla de la tabla GeneradorSecuenciaEnvioACamara</returns>
        int ObtenerSecuenciaEnvioACamara();

        /// <summary>
        /// Retorna un numero de secuencia para crear un numero de Remito
        /// </summary>
        /// <returns>Ultima semilla de la tabla GeneradorNumeroRemito</returns>
        int ObtenerNumeroRemitoGenerado();

        /// <summary>
        /// Retorna un numero de secuencia para crear un numero de OrdenDeCargaContenedor
        /// </summary>
        /// <returns>Ultima semilla de la tabla GeneradorNumeroOrdenDeCargaContenedor</returns>
        int ObtenerNumeroOrdenDeCargaContenedorGenerado();

        /// <summary>
        /// Retorna un numero aleatorio
        /// </summary>
        int ObtenerNumeroAleatorio();

        int ObtenerNumeroDeControlDeCargaGenerado();

        /// <summary>
        /// Retorna Secuencia para numero de CTG por defecto.
        /// </summary>
        /// <returns></returns>
        int ObtenerSequenciaCPENroCTG();

        /// <summary>
        /// Retorna el pago disponible por patente o numero de documento.
        /// </summary>
        /// <returns></returns>
        List<TEntidad> ObtenerPagoTasaMunicipal<TEntidad>(TipoCategoriaVehiculo tipoCategoria, string patente, string numeroDocumento, int dias, int centroId, string codigoDiferencia);

        /// <summary>
        /// Elimina las cartas de porte electronica de documentos no ingresados
        /// </summary>
        /// <returns>Cantidad de cartas de porte eliminadas</returns>
        int EliminaCartaPorteElectronicaDocumentosNoIngresados(int diasLimiteDeBusqueda, int diasInicioDeBuqedaDeRecorrido);


        /// <summary>
        /// Elimina las cartas de porte electronica de documentos ingresados
        /// </summary>
        /// <returns>Cantidad de cartas de porte eliminadas</returns>
        int EliminaCartaPorteElectronicaDocumentoIngresados(int diasInicioDeBuqedaDeRecorrido);

        /// <summary>
        /// Obtiene los datos de los recorridos relacionados con cartas de porte electronica
        /// </summary>
        /// <returns>Lista de datos de recorridos relacionados con cartas de porte electronica</returns>
        List<RecorridoCpeDto> ObtenerDatosRecorridoRelacionadosConCartaPorteElectronica(DateTime fechaInicio, DateTime fechaFin); 
    }
}