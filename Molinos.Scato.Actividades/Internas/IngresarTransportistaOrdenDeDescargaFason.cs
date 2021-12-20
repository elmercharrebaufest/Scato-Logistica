using System;
using System.Activities;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresarTransportistaOrdenDeDescargaFason : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<TransportistaDto> Transportista { get; set; }
        public InArgument<int> OrdenDeDescargaFasonId { get; set; }
        public InOutArgument<ControlRecorridoDto> ControlRecorrido { get; set; }
        public OutArgument<OrdenDeDescargaFasonDto> OrdenDeDescargaFason { get; set; }
        public InArgument<Guid> WorkflowId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var transportista = Transportista.Get<TransportistaDto>(context);
            var odenDeDescargaFasonId = OrdenDeDescargaFasonId.Get<int>(context);
            var workflowId = WorkflowId.Get<Guid>(context);
            var controlRecorrido = ControlRecorrido.Get<ControlRecorridoDto>(context);

            var resultado = new Resultado();
            try
            {
                resultado = servicioComandos.Ejecutar(new CrearTransportistaOrdenDeDescargaFason { Dto = transportista, OrdenDeDescargaFasonId = odenDeDescargaFasonId, WorkflowId = workflowId });
                var orden = repositorio.ObtenerOrdenDeDescargaFason(odenDeDescargaFasonId);
                OrdenDeDescargaFason.Set(context, orden);
                context.GetExtension<ScatoPersistenceParticipant>().TransportistaId = orden.TransportistaId;
                context.GetExtension<ScatoPersistenceParticipant>().Transportista = orden.Transportista;
            }
            catch (Exception e)
            {
                resultado.Errores.Add("", Textos.Transportista_ErrorEnLaCarga + ": " + e.Message);
            }

            controlRecorrido.Actividad = Textos.ActIngresoDeTransportistaOrdenDeDescargaFason;
            ControlRecorrido.Set(context, controlRecorrido);

            return resultado;
        }
    }
}
