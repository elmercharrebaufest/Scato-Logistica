using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class CrearRemitoBodegaVino : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<RemitoBodegaVinoDto> Orden { get; set; }

        [RequiredArgument]
        public InArgument<string> NombreWorkflow { get; set; }
        
        [RequiredArgument]
        public InArgument<int> WorkflowDefinicionId { get; set; }

        [RequiredArgument]
        public InArgument<Guid> InstanciaWorkflowId { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        
        public OutArgument<RemitoBodegaVinoDto> RemitoBodegaVino { get; set; }

        public OutArgument<TipoDocumentoIngreso> TipoDeDocumentoIngreso { get; set; }

        public OutArgument<string> NumeroDocumentoIngreso { get; set; }

        public OutArgument<DateTime> FechaInicio { get; set; } 
        
    
        protected override Resultado Execute(CodeActivityContext context)
        {

            var orden = Orden.Get<RemitoBodegaVinoDto>(context);
            var nombreWorkflow = NombreWorkflow.Get<string>(context);
            var instanciaWorkflow = InstanciaWorkflowId.Get<Guid>(context);
            var workflowDefinicionId = WorkflowDefinicionId.Get<int>(context);
            var centro = CentroId.Get<int>(context);
            var resultado = new ResultadoCrearWorkflow();
            resultado.InstanciaWorkflowId = InstanciaWorkflowId.Get<Guid>(context);

            try
            {

                var srvComando = context.GetExtension<IServicioComandos>();
                var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
                
                var resultadoCrear = srvComando.Ejecutar(new Dominio.Comandos.CrearRemitoBodegaVino
                    {
                        Orden = orden,
                        NombreWorkflow = nombreWorkflow,
                        InstanciaWorkflowId = instanciaWorkflow,
                        CentroId = centro,
                        WorkflowDefinicionId = workflowDefinicionId
                    }) as ResultadoCrear;
                if (resultadoCrear != null)
                {
                    resultado.Id = resultadoCrear.Id;
                }
                
                var ordenDto = servicioRepositorio.ObtenerRemitoBodegaVino(resultado.Id);
               
                if (ordenDto != null)
                {
                    
                    RemitoBodegaVino.Set(context,ordenDto);
                    FechaInicio.Set(context,DateTime.Now);
                    TipoDeDocumentoIngreso.Set(context,TipoDocumentoIngreso.RemitoBodegaVino);
                    NumeroDocumentoIngreso.Set(context,orden.NroRemito);
                }
                if (resultadoCrear != null && resultadoCrear.HayErrores)
                {
                    resultado.Errores.Add("",Textos.Error_ActualizarGenerico);
                }
                
            }
            catch (Exception)
            {
                resultado.Errores.Add("",Textos.Error_ActualizarGenerico);
            }

            return resultado;
        }
    }
}
