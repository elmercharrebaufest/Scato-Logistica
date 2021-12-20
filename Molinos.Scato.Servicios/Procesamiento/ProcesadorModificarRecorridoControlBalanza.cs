using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoControlBalanza : ProcesadorComando<ModificarRecorridoControlBalanza>
    {
        public ProcesadorModificarRecorridoControlBalanza(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarRecorridoControlBalanza comando)
        {
            var resultado = new Resultado();
            try
            {
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
                recorridoAEditar.ControlBalanza = comando.ControlBalanza;
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo guardar el flag de control de balanza para " + comando.InstanceId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
