using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.Consultas;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarPanelPagoMunicipal : ProcesadorComando<ConsultarPanelPagoMunicipal>
    {
        private readonly IServicioComandos servicioComandos;

        public ProcesadorConsultarPanelPagoMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioRepositorio servicioRepositorio) : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
        }        

        public override Resultado Ejecutar(ConsultarPanelPagoMunicipal comando)
        {
            var resultado = new ResultadoConsultarPanelPagoMunicipal();
            try
            {
                var consulta = new PanelPagoMunicipalConsulta(comando.Filtro, comando.Paginacion);
                var lista = Repositorio.ListarConsultaPaginada(consulta);
                resultado.ListaResultados = lista;
                return resultado;
            }

            catch (Exception ex)
            {
                Log.Error($"Error Obtener ConsultarPanelPagoMunicipal: {ex.Message}");
                resultado.Error(string.Empty, "Ocurrió un error al obtener LogExceptuadosTicketMunicipal.");
            }

            return resultado;
        } 
    }
}
