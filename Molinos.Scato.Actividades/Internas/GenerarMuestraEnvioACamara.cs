using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class GenerarMuestraEnvioACamara : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        [RequiredArgument]
        public InOutArgument<CaladoPorCaracteristicaDto[]> CaladosPorCaracteristica { get; set; }

        [RequiredArgument]
        public InArgument<Int32> MaterialId { get; set; }

        public InArgument<string> NombreUsuario { get; set; }

        public InArgument<Int32> CaladoId { get; set; }

        public OutArgument<MuestraEnvioACamaraDto> MuestraEnvioACamara { get; set; }

        public OutArgument<ControlRecorridoDto> ControlRecorrido { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();

            var instanceId = InstanceId.Get<Guid>(context);

            var materialId = MaterialId.Get<Int32>(context);
            var nombreUsuario = NombreUsuario.Get<string>(context);
            var caladoId = CaladoId.Get<Int32>(context);

            var control = new ControlRecorridoDto
            {
                WorkflowInstanceId = instanceId,
                NombreUsuario = nombreUsuario,
                Actividad = Textos.Actividad_GenerarMuestraEnvioACamara,
                Decision = true
            };
            var caracteristicas = srvRepositorio.ListarAnalisisYCaladoPorCaracteristica(instanceId).Where(x => x.EnviaACamara && !x.HuboExcepcion)
                    .Select(y => y.CaracteristicaId).ToList();

            if (caracteristicas.Any())
            {
                var matPorCentro = srvRepositorio.ObtenerMaterialPorCentroPorInstanceId(instanceId);
                var camaraExcepcion = srvRepositorio.ObtenerCamaraDeExcepcionDescuento(instanceId, matPorCentro.MaterialId, matPorCentro.CentroId);
                var camara = camaraExcepcion != null ? camaraExcepcion.Id : matPorCentro.CamaraId;

                if (matPorCentro != null && camara.HasValue)
                {
                    var muestraEnvioACamaraDto = new MuestraEnvioACamaraDto
                    {
                        NombreUsuario = nombreUsuario,
                        CaladoId = caladoId,
                        CamaraId = camara.Value,
                        CaracteristicasDeCalidad = srvRepositorio.ListarCaracteristicasDeCalidadPorMaterial(materialId, matPorCentro.CentroId).Select(x =>
                            new CaracteristicaDeCalidadDto { Descripcion = x.Descripcion, Id = x.Id, SeEnviaACamara = caracteristicas.Any(b => b == x.Id), SituacionEnvioACamara = x.SituacionEnvioACamara }).ToList(),
                        Actividad = Textos.Actividad_GenerarMuestraEnvioACamara,
                        WorkflowInstanceId = instanceId,
                        CentroId = matPorCentro.CentroId,
                        HuboExcepcion = camaraExcepcion != null
                    };
                    MuestraEnvioACamara.Set(context, muestraEnvioACamaraDto);
                }
                else
                {
                    control.Mensaje = Textos.MaterialSinCamaraAsignada;
                    MuestraEnvioACamara.Set(context, null);
                }
            }
            else
            {
                MuestraEnvioACamara.Set(context, null);
            }

            ControlRecorrido.Set(context, control);
            return new Resultado();
        }
    }
}
