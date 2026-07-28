using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Globalization;
using System.ServiceModel;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarDesvioTara : CodeActivity
    {
        public InArgument<Guid> InstanceId { get; set; }

        public OutArgument<bool> Corresponde { get; set; }
        public OutArgument<string> Asunto { get; set; }
        public OutArgument<string> Body { get; set; }
        public OutArgument<string> Destino { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            var Log = context.GetExtension<ILogger>();
            var instanceId = InstanceId.Get(context);
            try
            {                
                var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
                var recorrido = servicioRepositorio.ObtenerRecorridoPorGuid(instanceId);

            if (recorrido == null)
            {
                throw new InvalidOperationException("No se encontro el recorrido para verificar el desvio de tara.");
            }

                var destino = ObtenerConfiguracionRequerida(
                servicioRepositorio,
                Constantes.ConfiguracionGeneral.Pantalla.DiferenciaPesoTaraWFE,
                Constantes.ConfiguracionGeneral.DiferenciaPesoTaraWFE.ListaDistribucion);

                var umbralKg = int.Parse(ObtenerConfiguracionRequerida(
                servicioRepositorio,
                Constantes.ConfiguracionGeneral.Pantalla.DiferenciaPesoTaraWFE,
                        Constantes.ConfiguracionGeneral.DiferenciaPesoTaraWFE.DiferenciaTolerancia));

            if (!recorrido.PesoTara.HasValue)
            {
                Corresponde.Set(context, false);
                Asunto.Set(context, string.Format(CultureInfo.CurrentCulture, "Desvio tara - {0}", recorrido.Patente));
                Destino.Set(context, destino);
                Body.Set(context, "No se pudo calcular el desvio de tara: el recorrido no tiene PesoTara.");
                return;
            }
                
                Log.Debug("ID de Recorrido: "+recorrido.Id);                

                var promedioTaraPorVehiculo = ObtenerPromedioTaraPorVehiculoSeguro(servicioRepositorio, recorrido.Id, Log);
                var patenteCamion = string.IsNullOrWhiteSpace(promedioTaraPorVehiculo.PatenteCamion)
                    ? recorrido.Patente
                    : promedioTaraPorVehiculo.PatenteCamion;
                var patenteAcoplado = string.IsNullOrWhiteSpace(promedioTaraPorVehiculo.PatenteAcoplado)
                    ? (recorrido.Vehiculo != null ? recorrido.Vehiculo.PatenteAcoplado : string.Empty)
                    : promedioTaraPorVehiculo.PatenteAcoplado;
                var chofer = ObtenerChofer(recorrido, promedioTaraPorVehiculo);
                var choferDocumento = ObtenerChoferDocumento(recorrido, promedioTaraPorVehiculo);
                int promedioTara = promedioTaraPorVehiculo.PromedioTara;
                int taraActual = (int)recorrido.PesoTara.Value;
                int desvioKg = Math.Abs(taraActual - promedioTara);

                var corresponde = promedioTara > 0 && desvioKg > umbralKg;

                Log.Debug("Promedio PesoTara: " + promedioTara);
                Log.Debug("Peso Tara Actual: " + taraActual);

                Corresponde.Set(context, corresponde);
            Asunto.Set(context, string.Format(CultureInfo.CurrentCulture, "Desvio tara - {0}", patenteCamion));
            Destino.Set(context, destino);
            Body.Set(context, GenerarBody(recorrido, patenteCamion, patenteAcoplado, chofer, choferDocumento, taraActual, promedioTara, desvioKg, umbralKg, corresponde));
        }
            catch (Exception ex)
            {
                Log.Error("Error al verificar desvio de tara para el recorrido: " + ex.ToString());
                Log.Error("Error al verificar desvio de tara para el recorrido: " + instanceId);
            }
         }

        private static string GenerarBody(RecorridoDto recorrido, string patenteCamion, string patenteAcoplado, string chofer, string choferDocumento, int taraActual, int promedioTara, int desvioKg, int umbralKg, bool corresponde)
        {
            var resultado = corresponde ? "SUPERA UMBRAL" : "DENTRO DE UMBRAL";

            return string.Format(CultureInfo.CurrentCulture,
                "<b>Verificacion de Desvio de Tara</b><br/>" +
                "Patente: <b>{0}</b><br/>" +
                "Acoplado: <b>{1}</b><br/>" +
                "Nro de Documento: <b>{2}</b><br/>" +
                "Chofer: <b>{3}</b><br/>" +
                "Nro Documento Chofer: <b>{4}</b><br/>" +
                "Tara actual: <b>{5:N0}</b> kg<br/>" +
                "Promedio historico: <b>{6:N0}</b> kg<br/>" +
                "Desvio absoluto: <b>{7:N0}</b> kg<br/>" +
                "Umbral configurado: <b>{8:N0}</b> kg<br/>" +
                "Resultado: <b>{9}</b>",
                patenteCamion,
                patenteAcoplado,
                recorrido.NumeroDocumentoIngreso,
                chofer,
                choferDocumento,
                taraActual,
                promedioTara,
                desvioKg,
                umbralKg,
                resultado);
        }

        private static string ObtenerChofer(RecorridoDto recorrido, PromedioTaraVehiculoDto promedioTaraPorVehiculo)
        {
            var apellido = !string.IsNullOrWhiteSpace(promedioTaraPorVehiculo.ChoferApellido)
                ? promedioTaraPorVehiculo.ChoferApellido
                : (recorrido.Chofer != null ? recorrido.Chofer.Apellido : string.Empty);
            var nombre = !string.IsNullOrWhiteSpace(promedioTaraPorVehiculo.ChoferNombre)
                ? promedioTaraPorVehiculo.ChoferNombre
                : (recorrido.Chofer != null ? recorrido.Chofer.Nombre : string.Empty);

            apellido = string.IsNullOrWhiteSpace(apellido) ? string.Empty : apellido.ToUpperInvariant();
            nombre = string.IsNullOrWhiteSpace(nombre) ? string.Empty : nombre.ToUpperInvariant();

            if (string.IsNullOrEmpty(apellido))
            {
                return nombre;
            }

            if (string.IsNullOrEmpty(nombre))
            {
                return apellido;
            }

            return string.Format(CultureInfo.CurrentCulture, "{0}, {1}", apellido, nombre);
        }

        private static string ObtenerChoferDocumento(RecorridoDto recorrido, PromedioTaraVehiculoDto promedioTaraPorVehiculo)
        {
            if (!string.IsNullOrWhiteSpace(promedioTaraPorVehiculo.ChoferNumeroDocumento))
            {
                return promedioTaraPorVehiculo.ChoferNumeroDocumento;
            }

            return recorrido.Chofer != null ? recorrido.Chofer.NumeroDeDocumento : string.Empty;
        }

        private static PromedioTaraVehiculoDto ObtenerPromedioTaraPorVehiculoSeguro(IServicioRepositorio servicioRepositorio, int recorridoId, ILogger log)
        {
            try
            {
                return servicioRepositorio.ObtenerPromedioTaraPorVehiculo(recorridoId) ?? new PromedioTaraVehiculoDto();
            }
            catch (FaultException<ExceptionDetail> ex)
            {
                log.Error(string.Format(CultureInfo.CurrentCulture,
                    "No se pudo obtener promedio de tara para recorrido {0}. Se continuara con valores por defecto. Detalle: {1}",
                    recorridoId,
                    ex.Detail != null ? ex.Detail.Message : ex.Message));
                return new PromedioTaraVehiculoDto();
            }
            catch (FaultException ex)
            {
                log.Error(string.Format(CultureInfo.CurrentCulture,
                    "No se pudo obtener promedio de tara para recorrido {0}. Se continuara con valores por defecto. Detalle: {1}",
                    recorridoId,
                    ex.Message));
                return new PromedioTaraVehiculoDto();
            }
        }

        private static string ObtenerConfiguracionRequerida(IServicioRepositorio servicioRepositorio, string pantalla, string nombre)
        {
            var configuracion = servicioRepositorio.ObtenerConfiguracionGeneral(pantalla, nombre);
            if (configuracion == null || string.IsNullOrWhiteSpace(configuracion.Valor))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "No se encontro configuracion obligatoria para pantalla '{0}' y nombre '{1}'.", pantalla, nombre));
            }

            return configuracion.Valor;
        }

        private static decimal ObtenerDecimalConfiguracionRequerida(IServicioRepositorio servicioRepositorio, string pantalla, string nombre)
        {
            var valor = ObtenerConfiguracionRequerida(servicioRepositorio, pantalla, nombre);
            decimal resultado;
            if (decimal.TryParse(valor, NumberStyles.Number, CultureInfo.CurrentCulture, out resultado))
            {
                return resultado;
            }

            if (decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out resultado))
            {
                return resultado;
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                "La configuracion '{0}/{1}' no es un decimal valido. Valor: {2}.", pantalla, nombre, valor));
        }
    }
}
