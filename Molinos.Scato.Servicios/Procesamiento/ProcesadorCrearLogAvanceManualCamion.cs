using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLogAvanceManualCamion : ProcesadorComando<CrearLogAvanceManualCamion>
    {
        public ProcesadorCrearLogAvanceManualCamion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearLogAvanceManualCamion comando)
        {
            var resultado = new Resultado();

            try
            {
                var logEntry = new LogAvanceManualCamion
                {
                    NombreUsuario     = comando.NombreUsuario,
                    FechaEvento       = DateTime.Now,
                    MotivoFallo       = comando.MotivoFallo,
                    PuestoDeTrabajo   = Repositorio.Obtener<PuestoDeTrabajo>(comando.PuestoDeTrabajoId),
                    PatenteLeida     = comando.PatenteLeida,
                    PatenteIngresada = comando.PatenteIngresada,
                    LogIdentificacionVehicularOrigen  = Repositorio.Obtener<LogIdentificacionVehicular>(comando.LogIdentificacionVehicularOrigenId),
                    LogIdentificacionVehicularDestino = Repositorio.Obtener<LogIdentificacionVehicular>(comando.LogIdentificacionVehicularDestinoId),
                };

                Repositorio.Agregar(logEntry);
                Repositorio.GuardarCambios();

                Log.Info("AvanceManual — log registrado. Usuario: {0}, MotivoFallo: {1}",
                    comando.NombreUsuario, comando.MotivoFallo);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al registrar log de avance manual. MotivoFallo: {0}", comando.MotivoFallo);
                resultado.Error(nameof(Exception), ex.Message);
            }

            return resultado;
        }
    }
}
