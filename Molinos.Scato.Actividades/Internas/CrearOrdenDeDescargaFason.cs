using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class CrearOrdenDeDescargaFason : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<OrdenDeDescargaFasonDto> Orden { get; set; }

        [RequiredArgument]
        public InArgument<string> NombreWorkflow { get; set; }

        [RequiredArgument]
        public InArgument<int> WorkflowDefinicionId { get; set; }

        [RequiredArgument]
        public InArgument<Guid> InstanciaWorkflowId { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }

        public OutArgument<OrdenDeDescargaFasonDto> OrdenDeDescargaFason { get; set; }

        public OutArgument<string> NumeroOrdenDeDescarga { get; set; }

        public OutArgument<TipoDocumentoIngreso> TipoDocumentoIngreso { get; set; }

        public OutArgument<DateTime> FechaInicio { get; set; }

        public InArgument<string> Usuario { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var orden = Orden.Get<OrdenDeDescargaFasonDto>(context);
            var nombreWorkflow = NombreWorkflow.Get<string>(context);
            var instanciaWorkflow = InstanciaWorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var usuario = Usuario.Get<string>(context);
            var workflowDefinicionId = WorkflowDefinicionId.Get<int>(context);
            var resultado = new ResultadoCrearWorkflow();
            resultado.InstanciaWorkflowId = InstanciaWorkflowId.Get(context);
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();

                var resultadoCrear = servicioComandos.Ejecutar(new Dominio.Comandos.CrearOrdenDeDescargaFason
                {
                    Orden = orden,
                    NombreWorkflow = nombreWorkflow,
                    InstanciaWorkflowId = instanciaWorkflow,
                    CentroId = centroId,
                    Usuario = usuario,
                    WorkflowDefinicionId = workflowDefinicionId
                }) as ResultadoCrear;
                resultado.Id = resultadoCrear.Id;

                var ordenDto = srvRepositorio.ObtenerOrdenDeDescargaFason(resultado.Id);
                if (ordenDto != null)
                {
                    OrdenDeDescargaFason.Set(context, ordenDto);
                    FechaInicio.Set(context, DateTime.Now);
                    NumeroOrdenDeDescarga.Set(context, ordenDto.Numero);
                    TipoDocumentoIngreso.Set(context, Dominio.Enums.TipoDocumentoIngreso.OrdenDeDescargaFason);
                }
                if (resultadoCrear.HayErrores)
                {
                    resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
                }
            }
            catch (Exception)
            {
                resultado.Errores.Add("1", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}
