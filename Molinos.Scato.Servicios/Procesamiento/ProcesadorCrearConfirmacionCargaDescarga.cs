using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearConfirmacionCargaDescarga : ProcesadorComando<CrearConfirmacionCargaDescarga>
    {
        private readonly IServicioComandos servicioComandos;
        public ProcesadorCrearConfirmacionCargaDescarga(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(CrearConfirmacionCargaDescarga comando)
        {
            var resultado = new Resultado();
            Validar(resultado, comando.Dto.RecorridoId);
            if(resultado.HayErrores)
            {
                return resultado;
            }

            try
            {
                var confirmacion = new ConfirmacionCargaDescarga()
                {
                    NombreUsuario = comando.Dto.NombreUsuario,
                    FechaConfirmacion = DateTime.Now,
                    Recorrido = Repositorio.Obtener<Recorrido>(x => x.Id == comando.Dto.RecorridoId)
                };
                Repositorio.Agregar(confirmacion);
                Repositorio.GuardarCambios();

                var controlRecorrido = new ControlRecorridoDto()
                {
                    WorkflowInstanceId = comando.Dto.WorkflowInstanceId,
                    Actividad = EtapaWorkflow.ConfirmacionCargaDescarga,
                    ActividadXaml = EtapaWorkflow.ConfirmacionCargaDescarga,
                    NombreUsuario = comando.Dto.NombreUsuario
                };
                resultado = servicioComandos.Ejecutar(new CrearControlRecorrido
                {
                    Dto = controlRecorrido
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Ocurrió un error al registrar la confirmación carga/descarga al workflow {comando.Dto.WorkflowInstanceId}");
                resultado.Error("confirmacion", "Ocurrió un error al registrar la confirmación carga/descarga al recorrido");
            }
            return resultado;
        }

        public void Validar(Resultado resultado, int recorridoId)
        {
            if(Repositorio.Existe<ConfirmacionCargaDescarga>(x => x.Recorrido.Id == recorridoId))
            {
                resultado.Error("confirmacion", "La carga/descarga del recorrido ya fue confirmado");
            }
        }
    }
}
