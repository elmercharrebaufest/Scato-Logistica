using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoDatosProximaActividad : ProcesadorComando<ModificarRecorridoDatosProximaActividad>
    {
        public ProcesadorModificarRecorridoDatosProximaActividad(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarRecorridoDatosProximaActividad comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorModificarRecorridoDatosProximaActividad con RecorridoId = {0} y DatosProximaActividad = {1}", comando.InstanceId, comando.DatosProximaActividad);
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
                recorridoAEditar.DatosProximaActividad = comando.DatosProximaActividad;
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Ha ocurrido un error en ProcesadorModificarRecorridoDatosProximaActividad con RecorridoId = {0} y DatosProximaActividad = {1}", comando.InstanceId, comando.DatosProximaActividad);
                resultado.Error("", Textos.Error_Generico);
            }
            return resultado;
        }
    }
}
