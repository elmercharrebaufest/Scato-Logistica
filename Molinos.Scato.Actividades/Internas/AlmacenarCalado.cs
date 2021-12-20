using System;
using System.Activities;
using System.Linq;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class AlmacenarCalado : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        [RequiredArgument]
        public InOutArgument<CaladoPorCaracteristicaDto[]> CaladosPorCaracteristica { get; set; }

        [RequiredArgument]
        public InArgument<Int32> CicloDeCalado { get; set; }

        public InArgument<string> MuestraConjunto { get; set; }

        public InArgument<string> Orden { get; set; }

        [RequiredArgument]
        public InArgument<bool> Rechazar { get; set; }

        public OutArgument<CaladoDto> Calado { get; set; }

        public InArgument<ControlRecorridoDto> ControlRecorrido { get; set; }

        public OutArgument<string> Observacion { get; set; }

        public OutArgument<bool> EnviaACamara { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();
            var servicioComandos = context.GetExtension<IServicioComandos>();

            var instanceId = InstanceId.Get<Guid>(context);
            var caladosPorCaracteristica = CaladosPorCaracteristica.Get<CaladoPorCaracteristicaDto[]>(context);
            var cicloDeCalado = CicloDeCalado.Get<Int32>(context);
            var muestraConjunto = MuestraConjunto.Get<string>(context);
            var orden = Orden.Get<string>(context);
            var rechazar = Rechazar.Get<bool>(context);
            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);

            var resultado = new Resultado();
            if (!rechazar)
            {
                try
                {
                    var recorridoDto = srvRepositorio.ObtenerRecorridoPorGuid(instanceId);
                    var calado = recorridoDto.Calado;
                    int? muestraConj;
                    try
                    {
                        muestraConj = muestraConjunto != null ? Convert.ToInt32(muestraConjunto) : (int?)null;
                    }
                    catch (Exception)
                    {
                        muestraConj = null;
                    }
                    calado.WorkflowInstanceId = instanceId;
                    calado.MuestraConjunto = muestraConj;
                    calado.NumeroOrden = orden;
                    calado.CicloDeCalado = cicloDeCalado;
                    calado.FechaCreacion = DateTime.Now;
                    calado.Usuario = controlRecorrido.NombreUsuario;
                    calado.Comentario = controlRecorrido.Comentario;
                    var humedad = caladosPorCaracteristica.FirstOrDefault(x => x.EsHumedad);
                    var pideAnalisis = caladosPorCaracteristica.Any(caladoPorCaracteristicaDto => caladoPorCaracteristicaDto.AnalisisPreliminar);
                    if (humedad != null && humedad.ValorCalado.HasValue)
                    {
                        context.GetExtension<ScatoPersistenceParticipant>().Calidad = humedad.ValorCalado.Value.ToString();
                        var calidad = srvRepositorio.ObtenerCalidadMaterialPorHumedadEInstanceId(humedad.ValorCalado.Value, pideAnalisis, instanceId);
                        if (calidad != null)
                        {
                            calado.CalidadMaterialId = calidad.Id;
                        }
                    }
                    resultado = servicioComandos.Ejecutar(new ModificarCalado { Dto = calado, CaladosPorCaracteristica = caladosPorCaracteristica, PesoNetoOrigen = recorridoDto.PesoNetoOrigen.HasValue ? recorridoDto.PesoNetoOrigen.Value : 0 });
                    calado = srvRepositorio.ObtenerCaladoPorGuid(instanceId);

                    CaladosPorCaracteristica.Set(context, calado.CaladosPorCaracteristica);
                    Calado.Set(context, calado);

                    EnviaACamara.Set(context, calado.CaladosPorCaracteristica.Any(x => x.EnviaACamara && !x.HuboExcepcion));
                }
                catch (Exception)
                {
                    resultado.Errores.Add("", Textos.Calado_ErrorEnLaCarga);
                }
            }
            else
            {
                Observacion.Set(context, controlRecorrido.Mensaje + "\n" + controlRecorrido.Comentario);
            }

            try
            {
                if (controlRecorrido.Actividad != "Calado/Rechazar" && caladosPorCaracteristica.FirstOrDefault() != null)
                {
                    servicioComandos.Ejecutar(new InformarCaladoCircular
                    {
                        WorkflowInstanceId = instanceId
                    });
                }
            }
            catch (Exception e)
            {

            }

            return resultado;
        }
    }
}
