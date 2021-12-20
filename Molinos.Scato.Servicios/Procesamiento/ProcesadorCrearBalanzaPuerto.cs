using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearBalanzaPuerto : ProcesadorCrear<CrearBalanzaPuerto, BalanzaPuerto>
    {
        private readonly IServicioOrquestador orquestador;
        private readonly IConfiguracionProvider config;

        public ProcesadorCrearBalanzaPuerto(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOrquestador orquestador, IConfiguracionProvider config)
            : base(repositorio, conversor, log)
        {
            this.orquestador = orquestador;
            this.config = config;
        }


        protected override BalanzaPuerto CrearEntidad(CrearBalanzaPuerto comando)
        {
            var entidad = Conversor.Convertir<BalanzaPuertoDto, BalanzaPuerto>(comando.Dto);
            entidad.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            Suscribir(comando.Dto.CodigoBalanza);

            return entidad;
        }

        protected override void Validar(CrearBalanzaPuerto comando, Resultado resultado)
        {
            if (Repositorio.Existe<BalanzaPuerto>(e => e.CodigoDispositivo == comando.Dto.CodigoDispositivo && (comando.Dto.Id == 0 || e.Id != comando.Dto.Id)))
            {
                resultado.Error("CrearBalanzaPuerto", string.Format(Textos.Error_ActualizarGenerico));
            }
        }


        private void Suscribir(string codigoDispositivo)
        {
            try
            {
                orquestador.Suscribir(new ComandoSuscribir
                {
                    CodigoDispositivo = codigoDispositivo,
                    CodigoEvento = "BalanzadaRecibida",
                    RutaAccesoSuscriptor = config.AppSettings["UrlNotificaciones"],
                    Persistente = true
                });
            }
            catch (Exception e)
            {
                Log.Error("Error al suscribir balanza de puerto: ",e);
            }
        }
    }
}
