using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLogIdentificacionVehicular : ProcesadorComando<CrearLogIdentificacionVehicular>
    {
        public ProcesadorCrearLogIdentificacionVehicular(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearLogIdentificacionVehicular comando)
        {
            var resultado = new ResultadoCrear();

            var log = new LogIdentificacionVehicular
            {
                CodigoDispositivo = comando.CodigoDispositivo,
                Tarjeta = comando.Tarjeta,
                Error = comando.Error,
                Patente = comando.Patente,
                VehiculoPresente = comando.VehiculoPresente,
                FechaEvento = comando.FechaEvento,
                ResultadoWorkflow = null
            };

            Repositorio.Agregar(log);

            if (comando.Detalles != null)
            {
                foreach (var detalle in comando.Detalles)
                {
                    Repositorio.Agregar(new LogIdentificacionVehicularDetalle
                    {
                        LogIdentificacionVehicular = log,
                        ProveedorALPR = detalle.ProveedorALPR,
                        CodigoCamara = detalle.CodigoCamara,
                        RutaImagen = detalle.RutaImagen,
                        Intentos = detalle.Intentos,
                        Patente = detalle.Patente,
                        Certeza = detalle.Certeza,
                        Exitoso = (detalle.Patente != null && detalle.Patente.Equals(comando.Patente, StringComparison.InvariantCultureIgnoreCase)) || detalle.Exitoso
                    });
                }
            }

            Repositorio.GuardarCambios();
            resultado.Id = log.Id;

            return resultado;
        }
    }
}
