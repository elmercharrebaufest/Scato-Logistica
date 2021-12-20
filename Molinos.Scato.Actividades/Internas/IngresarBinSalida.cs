using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresarBinSalida : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<CargaDeBinesDto[]> CargaDeBines { get; set; }
        [RequiredArgument]
        public InArgument<Guid> InstanciaWorkflowId { get; set; }
        [RequiredArgument]
        public InArgument<string> Observacion { get; set; }
        [RequiredArgument]
        public OutArgument<int> PesoTaraBodega { get; set; }


        protected override Resultado Execute(CodeActivityContext context)
        {
            var instanciaWorkflow = InstanciaWorkflowId.Get<Guid>(context);
            var cargaDeBines = CargaDeBines.Get<CargaDeBinesDto[]>(context);
            var observacion = Observacion.Get<string>(context);
            var servicioComandos = context.GetExtension<IServicioComandos>();
            var resultado = new ResultadoCargarBinesSalida();
            try
            {
                resultado = (ResultadoCargarBinesSalida) servicioComandos.Ejecutar(new CargarBinesSalida
                    {
                        InstanciaWorkflow = instanciaWorkflow,
                        Bines = cargaDeBines,
                        Observacion = observacion,
                    });
                PesoTaraBodega.Set(context, resultado.PesoTaraBodega);
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }

            return resultado;
        }
    }
}
