using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;


namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarConfiguracionCalleHidraulica: ProcesadorComando<ActualizarConfiguracionCalleHidraulica>
    {
        private readonly IConfiguracionProvider configuracion;

        public ProcesadorActualizarConfiguracionCalleHidraulica(IRepositorio repositorio, IConversor conversor, ILogger log, IConfiguracionProvider configuracion)
            : base(repositorio, conversor, log)
        {
            this.configuracion = configuracion;
        }

        public override Resultado Ejecutar(ActualizarConfiguracionCalleHidraulica comando)
        {
            var resultado = new Resultado();
            var hidraulicaCalle = Repositorio.Obtener<ConfiguracionCalleHidraulica>(comando.Id);
            //hidraulicaCalle.CalleId = comando.CalleId;
            //hidraulicaCalle.CalleNombre = comando.CalleNombre;
            hidraulicaCalle.CodigoCamaraALPR = comando.CodigoCamaraALPR;
            hidraulicaCalle.CodigoCartel = comando.CodigoCartel;
            hidraulicaCalle.CodigoSensorCirculacion = comando.CodigoSensorCirculacion;
            Repositorio.GuardarCambios();
            return resultado;
        }
    }
}
