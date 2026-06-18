using System;
using System.Activities;
using System.Threading;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class LogActividad : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<LogActividadDto> LogActividadDto { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();

            var logActividad = LogActividadDto.Get<LogActividadDto>(context);

            context.GetExtension<ScatoPersistenceParticipant>().Actividad = logActividad.ActividadXaml;
            
            logActividad.Fecha = DateTime.Now;
            var resultado = new Resultado();

            if (logActividad.ActividadXaml == "EnTransito" || logActividad.ActividadXaml == "PasoPorBalanza")
            {
                servicioComandos.Ejecutar(new RegistrarMarcaDeTiempo
                {
                    Tipo = TipoRegistroMarcaDeTiempo.Fin,
                    InstanceId = logActividad.WorkflowInstanceId
                });
            }

            if (logActividad.ActividadXaml == "SalidaDeCentro")
            {
                servicioComandos.Ejecutar(new RegistrarMarcaDeTiempo
                {
                    Tipo = TipoRegistroMarcaDeTiempo.Inicio,
                    InstanceId = logActividad.WorkflowInstanceId
                });
            }

            try
            {
                resultado = servicioComandos.Ejecutar(new CrearLogActividad { Dto = logActividad });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.LogActividad_ErrorEnLaCarga);
            }

            var intentos = 1;
            while (resultado.HayErrores && intentos < 10)
            {
                try
                {
                    Thread.Sleep(500*intentos);
                    resultado = servicioComandos.Ejecutar(new CrearLogActividad {Dto = logActividad});
                }
                catch
                {
                     // agregamos solo una vez los errores
                }
                intentos++;
            }

            return resultado;
        }
    }
}
