using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearMotivoQuiebreBarrera : ProcesadorComando<CrearMotivoQuiebreBarrera>
    {
        private readonly IServicioOrquestador servicioOrquestador;

        public ProcesadorCrearMotivoQuiebreBarrera(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOrquestador servicioOrquestador) : base(repositorio, conversor, log)
        {
            this.servicioOrquestador = servicioOrquestador;
        }

        public override Resultado Ejecutar(CrearMotivoQuiebreBarrera comando)
        {
            var resultado = new ResultadoMotivoQuiebre();
            
            // No sabemos si el sensor no puede estar configurado en más de un puesto, asi que lo informamos para todos los puestos
            var puestos = Repositorio.Listar<PuestoDeTrabajo>(x => x.SensorQuiebre == comando.CodigoDispositivo);
            var date = DateTime.Now;
            foreach (var puesto in puestos)
            {
                var filename = FotoCamionHelper.GenerarNombreTemporal(puesto.Centro.CodigoSAP + "-" + puesto.SensorQuiebre, date);
                try
                {
                    servicioOrquestador.Ejecutar(
                        new EjecutarTomarFoto
                        {
                            CodigoDispositivo = puesto.VideoCamara,
                            FilePath = puesto.VideoCamaraDirectorio,
                            SubPath = date.ToString("yyyyMMdd"),
                            FileName = filename
                        });
                    resultado.Fotos.Add(filename);
                }
                catch (Exception e)
                {
                    Log.Error($"Error al tomar foto en sensor {comando.CodigoDispositivo}", e);
                }

                var motivo = new MotivoQuiebreBarrera
                    {
                        Fecha = DateTime.Now,
                        PuestoTrabajo = puesto,
                        Apertura = comando.Apertura,
                        FileName = filename
                };
                Repositorio.Agregar(motivo);
                resultado.PuestosDeTrabajo.Add(puesto.NombrePuesto);
            }
            Repositorio.GuardarCambios();
            return resultado;
        }
    }
}
