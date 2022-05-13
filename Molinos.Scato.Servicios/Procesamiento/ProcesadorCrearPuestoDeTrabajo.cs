using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearPuestoDeTrabajo : ProcesadorPuestoDeTrabajo<CrearPuestoDeTrabajo>
    {

        public ProcesadorCrearPuestoDeTrabajo(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOrquestador servicioOrquestador, IConfiguracionProvider config)
            : base(repositorio, conversor, log,servicioOrquestador,config)
        {
        }

        public override Resultado Ejecutar(CrearPuestoDeTrabajo comando)
        {
            var resultado = new ResultadoCrear();
            Validar(comando, resultado);
            if (!resultado.HayErrores)
            {
                var entidad = CrearEntidad(comando);
                Repositorio.Agregar(entidad);
                Repositorio.GuardarCambios();
                resultado.Id = entidad.Id;
                SuscribirDispositivos(comando.Dto, resultado);
            }

            return resultado;
        }

        private PuestoDeTrabajo CrearEntidad(CrearPuestoDeTrabajo comando)
        {
            var entidad = Conversor.Convertir<PuestoDeTrabajoDto, PuestoDeTrabajo>(comando.Dto);
            entidad.Centro = Repositorio.Obtener<Centro>(x => x.Id == comando.Dto.CentroId);
            entidad.Balanza = Repositorio.Obtener<Balanza>(x => x.Id == comando.Dto.BalanzaId);
            entidad.VisualizacionBarrera = Repositorio.Obtener<VisualizacionBarrera>(comando.Dto.VisualizacionBarrera_Id);
            entidad.ConfigSensores = Repositorio.Obtener<ConfigSensores>(comando.Dto.ConfigSensor_Id);
            var videocamarasAEliminar = Repositorio.Listar<VideoCamara>(x => x.PuestoDeTrabajo == null && x.ActividadPorDispositivo == null);
            Repositorio.RemoverTodos(videocamarasAEliminar);

            return entidad;
        }

        private void Validar(CrearPuestoDeTrabajo comando, Resultado resultado)
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
