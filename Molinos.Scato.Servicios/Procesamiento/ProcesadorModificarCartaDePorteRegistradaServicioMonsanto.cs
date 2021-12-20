using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCartaDePorteRegistradaServicioMonsanto : ProcesadorComando<ModificarCartaDePorteRegistradaServicioMonsanto>
    {
        public ProcesadorModificarCartaDePorteRegistradaServicioMonsanto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarCartaDePorteRegistradaServicioMonsanto comando)
        {
            var resultado = new ResultadoCrear();

            Log.Info("Iniciando ModificarCartaDePorteRegistradaServicioMonsanto");

            var transmision = Repositorio.Obtener<CartaDePorteRegistradaServicioMonsanto>(x => x.Recorrido.InstanciaWorkflow == comando.InstanceId);
            var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);

            var registro = new CartaDePorteRegistradaServicioMonsanto
            {
                LaboratorioCuit = comando.LaboratorioCuit,
                LaboratorioRazonSocial = comando.LaboratorioRazonSocial,
                Recorrido = recorrido,
                TipoAnalisis = comando.TipoAnalisis
            };

            if (transmision == null)
            {
                Log.Info("Se va a crear CartaDePorteRegistradaServicioMonsanto");
                Repositorio.Agregar(registro);
                transmision = registro;
            }
            else
            {
                Log.Info("Se va a modificar CartaDePorteRegistradaServicioMonsanto");
                transmision = registro;
            }
            Log.Info("Se van a guardar los cambios en ModificarCartaDePorteRegistradaServicioMonsanto");
            Repositorio.GuardarCambios();
            resultado.Id = transmision.Id;
            Log.Info("Cambios guardados");
            return resultado;
        }
    }
}