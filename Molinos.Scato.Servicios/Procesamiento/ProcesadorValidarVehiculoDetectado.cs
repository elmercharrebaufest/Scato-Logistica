using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Comandos.Validaciones;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorValidarVehiculoDetectado : ProcesadorComando<ValidarVehiculoDetectado>
    {
        public ProcesadorValidarVehiculoDetectado(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ValidarVehiculoDetectado comando)
        {
            var resultado = new ResultadoValidarVehiculoDetectado();
            try
            {
                Validar(comando, resultado);
                if (!resultado.HayErrores)
                    CrearLecturaPuestoDeTrabajo(comando, resultado);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                resultado.Error(nameof(Exception), Textos.Error_Generico);
            }

            return resultado;
        }

        private void CrearLecturaPuestoDeTrabajo(ValidarVehiculoDetectado comando, ResultadoValidarVehiculoDetectado resultado)
        {
            var recorrido = Repositorio.Obtener<Recorrido>(r => r.Patente == comando.Patente && !r.Terminado);
            var puestosDeTrabajo = Repositorio.Listar<PuestoDeTrabajo>(x => x.SensorVehicular == comando.CodigoDispositivo);
            foreach (var puestoDeTrabajo in puestosDeTrabajo)
            {
                var lecturaPuestoDeTrabajo = new LecturaPuestoDeTrabajoDto
                {
                    PuestoDeTrabajoId = puestoDeTrabajo.Id,
                    CentroId = puestoDeTrabajo.Centro.Id,
                    NumeroDeTarjeta = recorrido.TarjetaDeAcceso ?? string.Empty,
                    PuestoDeTrabajoPidePantente = puestoDeTrabajo.PidePatente,
                    TarjetaValida = true,
                    PuestoDeTrabajoImprimeTarjetaDeAcceso = puestoDeTrabajo.ImprimeTarjetaDeAcceso,
                    Entrada = puestoDeTrabajo.Entradas(),
                    Salida = puestoDeTrabajo.CierresEntrada(),
                    Automatizado = puestoDeTrabajo.AutomatizadoFull && !puestoDeTrabajo.PausaAutoFull,
                    VideoCamaras = Conversor.ConvertirList<VideoCamara, VideoCamaraDto>(puestoDeTrabajo.VideoCamaras.ToList()),
                    Patente = comando.Patente,
                    PatenteLeida = comando.Patente,
                    CodigoDispositivo = comando.CodigoDispositivo,
                    Firmware = puestoDeTrabajo.Firmware,
                    ReconocimientoExitoso = true,
                    OcrActivo = true,
                    TipoIngresoPorPuesto = TipoIdentificacionPorPuesto.IngresoPorPatente,
                };

                resultado.LecturaPuestosDeTrabajo.Add(lecturaPuestoDeTrabajo);
            }
        }

        protected void Validar(ValidarVehiculoDetectado comando, ResultadoValidarVehiculoDetectado resultado)
        {
            if (string.IsNullOrWhiteSpace(comando.CodigoDispositivo))
                resultado.Error(nameof(ValidarVehiculoDetectado.CodigoDispositivo), string.Format(Textos.Error_Requerido, nameof(ValidarVehiculoDetectado.CodigoDispositivo)));

            if (!Repositorio.Existe<PuestoDeTrabajo>(x => x.SensorVehicular == comando.CodigoDispositivo))
                resultado.Error(nameof(ValidarVehiculoDetectado.CodigoDispositivo), $"No se encontró puesto de trabajo asociado al sensor {comando.CodigoDispositivo}");
            
            if (string.IsNullOrWhiteSpace(comando.Patente))
                resultado.Error(nameof(ValidarVehiculoDetectado.Patente), string.Format(Textos.Error_Requerido, nameof(ValidarVehiculoDetectado.Patente)));

            if (!Repositorio.Existe<Recorrido>(x => x.Patente == comando.Patente && !x.Terminado))
                resultado.Error(nameof(ValidarVehiculoDetectado.Patente), $"No se encontró recorrido activo asociado a la patente {comando.Patente}");
        }
    }
}
