using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarExcepcionPagoTicketMunicipal : ProcesadorComando<ConsultarExcepcionPagoTicketMunicipal>
    {
        public ProcesadorConsultarExcepcionPagoTicketMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }        

        public override Resultado Ejecutar(ConsultarExcepcionPagoTicketMunicipal comando)
        {
            var resultado = new ResultadoConsultarExcepcionPagoTicketMunicipal();
            try
            {
                var patenteFiltro = string.IsNullOrWhiteSpace(comando.Patente) ? null : comando.Patente.Trim();

                resultado.ListaResultados = 
                    Repositorio.Listar<ExceptuadosTicketMunicipal, ExceptuadosTicketMunicipalDto>(
                        x => 
                            new ExceptuadosTicketMunicipalDto
                            {
                                Id = x.Id,
                                Patente = x.Patente,
                                NombreUsuario = x.NombreUsuario,
                                FechaCreacionExcepcion = x.FechaCreacionExcepcion,
                                WorkflowInstanceId = x.WorkflowInstanceId
                            },
                        x =>
                            (patenteFiltro == null || x.Patente.Contains(patenteFiltro)) // LIKE '%patenteFiltro%'
                            && !x.WorkflowInstanceId.HasValue,
                        comando.Paginacion);
            }
            catch (Exception ex)
            {
                Log.Error($"Error al obtener ExceptuadosTicketMunicipal: {ex.Message}");
                resultado.Error(string.Empty, "Ocurrió un error al obtener ExceptuadosTicketMunicipal.");
            }

            return resultado;
        } 
    }
}