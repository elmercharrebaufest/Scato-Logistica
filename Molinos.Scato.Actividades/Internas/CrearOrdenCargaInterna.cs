using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class CrearOrdenCargaInterna : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<OrdenCargaInternaDto> Orden { get; set; }

        [RequiredArgument]
        public InArgument<string> NombreWorkflow { get; set; }

        [RequiredArgument]
        public InArgument<string> NombreUsuario { get; set; }

        [RequiredArgument]
        public InArgument<int> WorkflowDefinicionId { get; set; }

        [RequiredArgument]
        public InArgument<Guid> InstanciaWorkflowId { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }

        public OutArgument<OrdenCargaInternaDto> OrdenCargaInterna { get; set; }

        public OutArgument<TipoDocumentoIngreso> TipoDocumentoIngreso { get; set; }

        public OutArgument<string> NumeroDocumentoIngreso { get; set; }

        public OutArgument<DateTime> FechaInicio { get; set; } 
       
        protected override Resultado Execute(CodeActivityContext context)
        {
            var orden = Orden.Get<OrdenCargaInternaDto>(context);
            var nombreWorkflow = NombreWorkflow.Get<string>(context);
            var instanciaWorkflow = InstanciaWorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var workflowDefinicionId = WorkflowDefinicionId.Get<int>(context);
            var nombreUsuario = NombreUsuario.Get<string>(context);
            var resultado = new ResultadoCrearWorkflow();
            resultado.InstanciaWorkflowId = InstanciaWorkflowId.Get(context);
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var resultadoCrear = servicioComandos.Ejecutar(new Dominio.Comandos.CrearOrdenCargaInterna
                {
                    Orden = orden,
                    NombreWorkflow = nombreWorkflow,
                    InstanciaWorkflowId = instanciaWorkflow,
                    CentroId = centroId,
                    WorkflowDefinicionId = workflowDefinicionId,
                    Usuario = nombreUsuario
                }) as ResultadoCrear;
                resultado.Id = resultadoCrear.Id;

                var ordenDto = srvRepositorio.ObtenerOrdenCargaInterna(resultado.Id);
                if (ordenDto != null)
                {
                    OrdenCargaInterna.Set(context,ordenDto);
                    TipoDocumentoIngreso.Set(context, Dominio.Enums.TipoDocumentoIngreso.OrdenCargaInterna);
                    NumeroDocumentoIngreso.Set(context, orden.NumeroOrden);   
                    FechaInicio.Set(context, DateTime.Now);
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
