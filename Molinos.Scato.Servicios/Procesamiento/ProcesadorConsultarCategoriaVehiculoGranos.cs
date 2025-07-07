using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarCategoriaVehiculoGranos : ProcesadorCategoriaVehiculo<ConsultarCategoriaVehiculoGranos>
    {
        private readonly IServicioComandos _servicioComandos;
        public ProcesadorConsultarCategoriaVehiculoGranos(IRepositorio repositorio, IConversor conversor, ILogger log, ICategorizadorVehiculo categorizador, IServicioComandos servicioComandos)
           : base(repositorio, conversor, log, categorizador)
        {
            _servicioComandos = servicioComandos;
        }
        protected override void Validar(ConsultarCategoriaVehiculoGranos comando, Resultado resultado)
        {
            Log.Info($"Validando comando: {comando.ToJson()}");
            if (string.IsNullOrEmpty(comando.Ctg))
                resultado.Error($"{nameof(comando.Ctg)}", $"La ctg no puede ser menor o igual a cero.");
            if (comando.CentroId <= 0)
                resultado.Error($"{nameof(comando.CentroId)}", $"El centro: {comando.CentroId} no puede ser menor o igual a cero.");
            Log.Info($"Comando validado correctamente");
        }
        protected override TipoVehiculo ConsultarTipoVehiculo(ConsultarCategoriaVehiculoGranos comando)
        {
            Log.Info($"Consultando tipo de vehículo para CTG: {comando.Ctg}, CentroId: {comando.CentroId}");
            int pesoBruto = ObtenerPesoBrutoPorCTG(comando.Ctg, comando.CentroId);

            var vehiculo = Repositorio.ObtenerMenor<PesoMaximoPorTipoVehiculo, int>(
                c => c.PesoMaxIngreso >= pesoBruto && c.Activo && c.Centro.Id == comando.CentroId,
                x => x.PesoMaxIngreso)
                ?? throw new InvalidOperationException($"Error en obtener: {nameof(PesoMaximoPorTipoVehiculo)}");

            Log.Info($"Tipo de vehículo encontrado: {vehiculo.TipoVehiculo} para peso bruto: {pesoBruto}");
            return vehiculo.TipoVehiculo;
        }
        private int ObtenerPesoBrutoPorCTG(string ctg, int CentroId)
        {
            Log.Info($"Obteniendo peso bruto por CTG: {ctg} y CentroId: {CentroId}");
            var cartaPorte = _servicioComandos.Ejecutar(new ConsultarCPDigital { CentroId = CentroId, NroCtg = long.Parse(ctg) }) as ResultadoCartaPorteElectronica
                                ?? throw new InvalidOperationException($"Error en obtener: {nameof(ResultadoCartaPorteElectronica)}");

            var vehiculo = cartaPorte.Cpe.Vehiculos?.FirstOrDefault()
                              ?? throw new InvalidOperationException($"Error en obtener: Vehiculos en {nameof(ResultadoCartaPorteElectronica)}");

            Log.Info($"Peso bruto origen obtenido: {vehiculo.PesoBrutoOrigen}");
            return vehiculo.PesoBrutoOrigen.Value;

        }

        protected override int IngresarCentroId(ConsultarCategoriaVehiculoGranos comando)
        {
            return comando.CentroId;
        }
    }
}