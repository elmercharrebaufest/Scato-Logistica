using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoPagaTicketMunicipal: ProcesadorComando<ModificarRecorridoPagaTicketMunicipal>
    {
        public ProcesadorModificarRecorridoPagaTicketMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarRecorridoPagaTicketMunicipal comando)
        {
            var resultado = new ResultadoCrear();

            try
            {
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(s => s.InstanciaWorkflow == comando.LogExceptuadosTicketMunicipalDto.InstanceId);

                recorridoAEditar.PagaTicketMunicipal = comando.LogExceptuadosTicketMunicipalDto.PagaTicketMunicipal;
                Repositorio.Agregar(new LogExceptuadosTicketMunicipal
                    {
                        Chofer = recorridoAEditar.Chofer,
                        FechaExcepcion = DateTime.Now,
                        FechaIngreso = recorridoAEditar.FechaInicio,
                        Material = recorridoAEditar.Material,
                        Motivo = comando.LogExceptuadosTicketMunicipalDto.Motivo ?? "",
                        PagaTicketMunicipal = comando.LogExceptuadosTicketMunicipalDto.PagaTicketMunicipal,
                        NombreUsuario = comando.LogExceptuadosTicketMunicipalDto.NombreUsuario,
                        NumeroDocumentoIngreso = recorridoAEditar.NumeroDocumentoIngreso,
                        Patente = recorridoAEditar.Patente,
                        WorkflowInstanceId = recorridoAEditar.InstanciaWorkflow
                    });
                Repositorio.GuardarCambios();
                Log.Debug("El valor de PagaTicketMunicipal del recorrido {0} paso de {1} a {2}, por el usuario {3}", recorridoAEditar.InstanciaWorkflow, !recorridoAEditar.PagaTicketMunicipal, recorridoAEditar.PagaTicketMunicipal, comando.LogExceptuadosTicketMunicipalDto.NombreUsuario);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al modificar el campo PagaTicketMunicipal del recorrido {0}", comando.LogExceptuadosTicketMunicipalDto.InstanceId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
