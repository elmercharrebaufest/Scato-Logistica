using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarExcepcionPagoTicketMunicipal : ProcesadorComando<ConsultarExcepcionPagoTicketMunicipal>
    {
        private readonly IServicioComandos servicioComandos;

        public ProcesadorConsultarExcepcionPagoTicketMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioRepositorio servicioRepositorio) : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
        }        

        public override Resultado Ejecutar(ConsultarExcepcionPagoTicketMunicipal comando)
        {
            var resultado = new ResultadoConsultarExcepcionPagoTicketMunicipal();
            try
            {
                resultado.ListaResultados = comando.Filtro == default ?

                     Repositorio.Listar<ExceptuadosTicketMunicipal, ExceptuadosTicketMunicipalDto>(x =>
                                         new ExceptuadosTicketMunicipalDto
                                         {
                                             Id = x.Id,
                                             Patente = x.Patente,
                                             NombreUsuario = x.NombreUsuario,
                                             NumeroDocumentoIngreso = x.NumeroDocumentoIngreso,
                                             FechaCreacionExcepcion = x.FechaCreacionExcepcion,
                                             WorkflowCodigo = x.WorkflowCodigo,
                                             WorkflowDescripcion = x.WorkflowDescripcion,
                                             Activo = x.Activo
                                         },
                                         x => x.Activo == true, comando.paginacion)

                     :
                     Repositorio.Listar<ExceptuadosTicketMunicipal, ExceptuadosTicketMunicipalDto>(x =>
                                             new ExceptuadosTicketMunicipalDto
                                             {
                                                 Id = x.Id,
                                                 Patente = x.Patente,
                                                 NombreUsuario = x.NombreUsuario,
                                                 NumeroDocumentoIngreso = x.NumeroDocumentoIngreso,
                                                 FechaCreacionExcepcion = x.FechaCreacionExcepcion,
                                                 WorkflowCodigo = x.WorkflowCodigo,
                                                 WorkflowDescripcion = x.WorkflowDescripcion,
                                                 Activo = x.Activo
                                             },
                                             x =>
                                              (comando.Filtro.Patente == x.Patente || comando.Filtro.Patente == null)
                                             && (comando.Filtro.NumeroDocumentoIngreso == x.NumeroDocumentoIngreso || comando.Filtro.NumeroDocumentoIngreso == null)
                                             && (comando.Filtro.WorkflowCodigo == x.WorkflowCodigo || comando.Filtro.WorkflowCodigo == null)
                                             && x.Activo == true, comando.paginacion);
            }

            catch (Exception ex)
            {
                Log.Error($"Error Obtener LogExceptuadosTicketMunicipal: {ex.Message}");
                resultado.Error(string.Empty, "Ocurrió un error al obtener LogExceptuadosTicketMunicipal.");
            }

            return resultado;
        } 
    }
}
