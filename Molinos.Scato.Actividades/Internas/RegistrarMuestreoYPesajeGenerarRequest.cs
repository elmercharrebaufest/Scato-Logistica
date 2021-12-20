using System;
using System.Activities;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;

namespace Molinos.Scato.Actividades.Internas
{
    public class RegistrarMuestreoYPesajeGenerarRequest : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        [RequiredArgument]
        public InArgument<TipoVehiculo> TipoVehiculo { get; set; }
        [RequiredArgument]
        public InArgument<string> NroCartaPorte { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaDeDescarga { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoNeto { get; set; }
        [RequiredArgument]
        public InArgument<CaladoDto> Calado { get; set; }

        public OutArgument<registerSampleAndWeightRequest> Request { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            registerSampleAndWeightRequest request = null;
            try
            {
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var tipoVehiculo = TipoVehiculo.Get<TipoVehiculo>(context);
                var instanceId = InstanceId.Get<Guid>(context);
                
                var fechaDeDescarga = FechaDeDescarga.Get<DateTime>(context);
                var nroCartaPorte = NroCartaPorte.Get<string>(context);
                var registro = srvRepositorio.ObtenerCartaDePorteRegistradaServicioMonsanto(instanceId, tipoVehiculo);
                if (tipoVehiculo != Dominio.Enums.TipoVehiculo.Tren)
                {
                    var pesoNeto = PesoNeto.Get<int>(context);
                    var calado = Calado.Get<CaladoDto>(context);
                    var carAnalizadas = srvRepositorio.ObtenerAnalisisDeCalidadPorCaladoId(calado.Id);
                    var kilosNetosDescontados = pesoNeto - TotalKilosDescuentos(calado, carAnalizadas, pesoNeto);
                    
                    request = new registerSampleAndWeightRequest
                    {
                        Item = new MuestreoPesajeTransporteAutomotor
                            {
                               cuitLaboratorio = registro != null ? registro.LaboratorioCuit : null,
                               fechaHoraConfirmacionDefinitiva = fechaDeDescarga,
                               fechaHoraConfirmacionDefinitivaSpecified = true,
                               idMuestra = nroCartaPorte,
                               kilosNetosSecos = Convert.ToInt32(kilosNetosDescontados),
                               kilosNetosSecosSpecified = true,
                               nroCartaPorte = !String.IsNullOrEmpty(nroCartaPorte) ? Convert.ToInt64(nroCartaPorte) : 0,
                            }

                    };
                }
                else
                {
                    var analisisvagones = srvRepositorio.ObtenerAnalisisPorVagones(instanceId);

                    request = new registerSampleAndWeightRequest
                    {
                        Item = new MuestreoPesajeVagonFerroviario
                            {
                                cuitLaboratorio = registro != null ? registro.LaboratorioCuit : null,
                                datosPorVagon = analisisvagones.Select(vagon => new vagon
                                    {
                                        idMuestra = nroCartaPorte, 
                                        kilosNetosSecos = Convert.ToInt32(vagon.PesoNeto - TotalKilosDescuentos(vagon.Calado, vagon.AnalisisDeCalidad, vagon.PesoNeto)), 
                                        kilosNetosSecosSpecified = true, 
                                        numero = vagon.NumeroVagon.ToString(CultureInfo.InvariantCulture)
                                    }).ToArray(),
                                fechaHoraConfirmacionDefinitiva = fechaDeDescarga,
                                fechaHoraConfirmacionDefinitivaSpecified = true,
                                nroCartaPorte = !String.IsNullOrEmpty(nroCartaPorte) ? Convert.ToInt64(nroCartaPorte) : 0,
                            }
                    };
                }
                
                try
                {
                    if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                    {
                        var srv = context.GetExtension<IServicioComandos>();
                        srv.Ejecutar(new CrearControlRecorrido
                            {
                                Dto = new ControlRecorridoDto
                                    {
                                        Actividad = "RegistrarMuestreoYPesajeGenerarRequest",
                                        Fecha = DateTime.Now,
                                        Comentario = request.ToXml(),
                                        NombreUsuario = "",
                                        WorkflowInstanceId = context.WorkflowInstanceId,
                                    }
                            });
                    }
                }
                catch
                {
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("", e.Message);
                
            }
            Request.Set(context,request);
            Resultado.Set(context, resultado);
        }

        private decimal TotalKilosDescuentos(CaladoDto calado, AnalisisDeCalidadDto analisis, int pesoNeto)
        {
            var descuentos = calado.CaladosPorCaracteristica.ToDictionary(x => x.CaracteristicaCodigoSap, x => x.DescuentoEnPorcentaje);
            if (analisis != null)
            {
                foreach (var valorAnalisis in analisis.CaracteristicasAnalizadas)
                {
                    descuentos[valorAnalisis.CaracteristicaCodigoSap] = valorAnalisis.DescuentoEnPorcentaje;
                }
            }
            return (descuentos.Sum(keyValue => keyValue.Value) * pesoNeto) / 100;
        }
    }
}
