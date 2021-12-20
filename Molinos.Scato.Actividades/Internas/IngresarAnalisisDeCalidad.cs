using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresarAnalisisDeCalidad : CodeActivity<Resultado>
    {
        public InArgument<string> NumeroDeOrden { get; set; }
        public InOutArgument<AnalisisPorCaracteristicaDto[]> CaracteristicasAnalizadas { get; set; }
        public InArgument<int> PesoNetoOrigen { get; set; }
        public InArgument<int> CaladoId { get; set; }
        public InArgument<string> Usuario { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var caracteristicasAnalizadas = CaracteristicasAnalizadas.Get<AnalisisPorCaracteristicaDto[]>(context);
            var numeroDeOrden = NumeroDeOrden.Get<string>(context);
            var pesoNetoOrigen = PesoNetoOrigen.Get<int>(context);
            var caladoId = CaladoId.Get<int>(context);
            var usuario = Usuario.Get<string>(context);
            var resultado = new Resultado();
            try
            {

                var servicioComandos = context.GetExtension<IServicioComandos>();
                resultado = servicioComandos.Ejecutar(new CrearAnalisisDeCalidad { WorkflowInstanceId = context.WorkflowInstanceId, Caracteristicas = caracteristicasAnalizadas, NumeroDeOrden = numeroDeOrden, PesoNetoOrigen = pesoNetoOrigen, CaladoId = caladoId, Usuario = usuario});
                if (!resultado.HayErrores)
                {
                    var servicio = context.GetExtension<IServicioRepositorio>();
                    var analisis = servicio.ObtenerAnalisisDeCalidadPorInstanceId(context.WorkflowInstanceId);
                    CaracteristicasAnalizadas.Set(context, analisis.CaracteristicasAnalizadas);
                }
            }
            catch
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}
