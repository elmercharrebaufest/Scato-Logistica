using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarLlamadoAutomaticoHidraulica : ProcesadorComando<ActualizarLlamadoAutomaticoHidraulica>
    {
        private readonly IConfiguracionProvider configuracion;

        public ProcesadorActualizarLlamadoAutomaticoHidraulica(IRepositorio repositorio, IConversor conversor, ILogger log, IConfiguracionProvider configuracion)
            : base(repositorio, conversor, log)
        {
            this.configuracion = configuracion;
        }

        public override Resultado Ejecutar(ActualizarLlamadoAutomaticoHidraulica comando)
        {
            var resultado = new Resultado();
            var hidraulica = Repositorio.Obtener<LlamadoAutomaticoHidraulica>(q => q.Hidraulica.Id == comando.Id);
            hidraulica.Estado = comando.Estado;
            hidraulica.UltimaPatenteLlamada = comando.Patente;
            hidraulica.FechaUltimaModificacionEstado = DateTime.Now;
            Repositorio.GuardarCambios();
            return resultado;
        }
    }
}