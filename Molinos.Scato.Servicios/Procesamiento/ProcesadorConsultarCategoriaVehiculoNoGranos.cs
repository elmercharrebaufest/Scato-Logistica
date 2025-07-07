using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarCategoriaVehiculoNoGranos : ProcesadorCategoriaVehiculo<ConsultarCategoriaVehiculoNoGranos>
    {
        private readonly IServicioComandos _servicioComandos;
        public ProcesadorConsultarCategoriaVehiculoNoGranos(IRepositorio repositorio, IConversor conversor, ILogger log, ICategorizadorVehiculo categorizador, IServicioComandos servicioComandos)
           : base(repositorio, conversor, log, categorizador)
        {
            _servicioComandos = servicioComandos ?? throw new ArgumentNullException(nameof(servicioComandos));
        }

        protected override void Validar(ConsultarCategoriaVehiculoNoGranos comando, Resultado resultado)
        {
            Log.Info($"Validando comando: {comando.ToJson()}");
            if (string.IsNullOrEmpty(comando.Patente))
                resultado.Error($"{nameof(comando.Patente)}", $"La patente: {comando.Patente} no puede ser nula o vacia");

            if (string.IsNullOrEmpty(comando.PatenteAcoplado))
                resultado.Error($"{nameof(comando.PatenteAcoplado)}", $"La patente acoplado: {comando.PatenteAcoplado} no puede ser nula o vacia");
            Log.Info($"Comando validado correctamente");
        }

        protected override TipoVehiculo ConsultarTipoVehiculo(ConsultarCategoriaVehiculoNoGranos comando)
        {
            Log.Info($"Consultando tipo de vehículo para Patente: {comando.Patente}, Patente Acoplado: {comando.PatenteAcoplado}, CentroId: {comando.CentroId}");
            var comandoCNRT = GenerarComandoConsultarCNRT(comando);

            Log.Info($"Comando generado: {comandoCNRT.ToJson()}");
            var resultado = _servicioComandos.Ejecutar(comandoCNRT) as ResultadoEscalables;

            if (resultado == null)
                throw new InvalidOperationException("Error en CNRT: Resultado nulo");

            if (resultado.HayErrores)
            {
                var errores = string.Join(", ", resultado.Errores.Select(e => e.Value));
                throw new InvalidOperationException($"Error en CNRT: {errores}");
            }

            if (!resultado.Categoria.HasValue)
                throw new InvalidOperationException($"Error en CNRT: {Textos.CategoriaEscalable_Nula}");

            Log.Info($"Tipo de vehículo encontrado: {resultado.Categoria.Value} para Patente: {comando.Patente}, Patente Acoplado: {comando.PatenteAcoplado}");
            return resultado.Categoria.Value;
        }

        private Comando GenerarComandoConsultarCNRT(ConsultarCategoriaVehiculoNoGranos comando)
        {
            Log.Info($"Generando comando para consultar CNRT con Patente: {comando.Patente}, Patente Acoplado: {comando.PatenteAcoplado}");
            var config = Repositorio.Obtener<ConfiguracionGeneral>(x =>
                x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.IngresarOrdenCargaInternaFason &&
                x.Nombre == Constantes.ConfiguracionGeneral.CNRT.CNRTDummy &&
                x.CentroId == null);

            var esDummy = false;

            if (!string.IsNullOrEmpty(config?.Valor))
                esDummy = bool.TryParse(config.Valor, out var dummyActivo) && dummyActivo;

            return esDummy ?
             new ConsultarEscalablesDummy { Patente = comando.Patente, Acoplado = comando.PatenteAcoplado } as Comando :
             new ConsultarEscalables { Patente = comando.Patente, Acoplado = comando.PatenteAcoplado } as Comando;
        }

        protected override int IngresarCentroId(ConsultarCategoriaVehiculoNoGranos comando)
        {
            return comando.CentroId;
        }
    }
}