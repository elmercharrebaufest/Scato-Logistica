using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarPuestoDeTrabajo : ProcesadorPuestoDeTrabajo<ModificarPuestoDeTrabajo>
    {
        public ProcesadorModificarPuestoDeTrabajo(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOrquestador servicioOrquestador, IConfiguracionProvider config)
            : base(repositorio, conversor, log, servicioOrquestador, config)
        {
        }

        public override Resultado Ejecutar(ModificarPuestoDeTrabajo comando)
        {
            var resultado = new Resultado();
            Validar(comando, resultado);
            if (!resultado.HayErrores)
            {
                var puestoActual = Repositorio.Obtener<PuestoDeTrabajo>(comando.Dto.Id);
                var codigoCIVAnterior = puestoActual?.CodigoConfigIdentificacionVehicular;

                CancelarDispositivos(comando.Dto, resultado, codigoCIVAnterior);
                ModificarEntidad(comando);
                Repositorio.GuardarCambios();
                SuscribirDispositivos(comando.Dto, resultado);
            }

            return resultado;
        }

        private void ModificarEntidad(ModificarPuestoDeTrabajo comando)
        {
            var camara = Repositorio.Obtener<PuestoDeTrabajo>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, camara);
            camara.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            camara.Balanza = Repositorio.Obtener<Balanza>(comando.Dto.BalanzaId);
            camara.ConfigSensores = Repositorio.Obtener<ConfigSensores>(comando.Dto.ConfigSensor_Id);
            camara.VisualizacionBarrera = Repositorio.Obtener<VisualizacionBarrera>(comando.Dto.VisualizacionBarrera_Id);
            var videocamarasAEliminar = Repositorio.Listar<VideoCamara>(x => x.PuestoDeTrabajo == null && x.ActividadPorDispositivo == null);
            Repositorio.RemoverTodos(videocamarasAEliminar);

        }

        private void Validar(ModificarPuestoDeTrabajo comando, Resultado resultado)
        {
            if (Repositorio.Existe<PuestoDeTrabajo>(e => e.NombrePuesto == comando.Dto.NombrePuesto && (comando.Dto.Id == 0 || e.Id != comando.Dto.Id)))
            {
                resultado.Error("NombrePuesto", string.Format(Textos.Error_Existente, Textos.PuestoDeTrabajo_NombrePuesto));
            }
            if (Repositorio.Existe<PuestoDeTrabajo>(e => e.Balanza.Id == comando.Dto.BalanzaId && (comando.Dto.Id == 0 || e.Id != comando.Dto.Id)))
            {
                resultado.Error("BalanzaId", string.Format(Textos.Error_Existente, Textos.PuestoDeTrabajo_Balanza));
            }
        }
    }
}
