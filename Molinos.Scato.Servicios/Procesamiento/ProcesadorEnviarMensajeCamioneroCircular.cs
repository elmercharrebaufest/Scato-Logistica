using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEnviarMensajeCamioneroCircular : ProcesadorComando<EnviarMensajeCamioneroCircular>
    {
        private readonly IServicioCircular servicioCircular;

        public ProcesadorEnviarMensajeCamioneroCircular(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioCircular servicioCircular)
            : base(repositorio, conversor, log)
        {
            this.servicioCircular = servicioCircular;
        }

        public override Resultado Ejecutar(EnviarMensajeCamioneroCircular comando)
        {
            var resultado = new ResultadoCircular();
            try
            {
                Log.Debug($"ProcesadorEnviarMensajeCamioneroCircular: CartaPorte:{comando.CartaPorte},  Mensaje: {comando.Mensaje}");
                var cargaDeCupo = Repositorio.Obtener<CargaDeCupo>(x => x.NumeroCartaPorte == comando.CartaPorte || x.CTG == comando.CartaPorte);
                Log.Debug($"ProcesadorEnviarMensajeCamioneroCircular:  InformaCircular: {cargaDeCupo.SacoTurnoConCircular}, CartaPorte:{comando.CartaPorte}, Mensaje: {comando.Mensaje}");

                if (cargaDeCupo.Centro.InformaCircular && (!comando.SePuedeDesactivar || cargaDeCupo.Centro.NotificarCamioneroCircular))
                {
                    servicioCircular.EnviarNotificacionCamionero(comando.CartaPorte, comando.Mensaje);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error al procesar el informe de arribo del camión a Circular App: {e.Message}");
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
