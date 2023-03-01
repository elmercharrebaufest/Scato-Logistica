using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class CrearOrdenCargaFas : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<OrdenCargaFasDto> Orden { get; set; }

        [RequiredArgument]
        public InArgument<string> NombreWorkflow { get; set; }

        [RequiredArgument]
        public InArgument<int> WorkflowDefinicionId { get; set; }

        [RequiredArgument]
        public InArgument<Guid> InstanciaWorkflowId { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }

        public OutArgument<OrdenCargaFasDto> OrdenCargaFas { get; set; }

        public OutArgument<TipoDocumentoIngreso> TipoDocumentoIngreso { get; set; }

        public OutArgument<string> NumeroDocumentoIngreso { get; set; }

        public OutArgument<bool> ValidaCompliance { get; set; }
       
        public OutArgument<bool> VehiculoDemorado { get; set; }

        [RequiredArgument]
        public InArgument<string> NombreUsuario { get; set; }
        public OutArgument<bool> Rechazado { get; set; }


        protected override Resultado Execute(CodeActivityContext context)
        {
            var orden = Orden.Get<OrdenCargaFasDto>(context);
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

                var resultadoCrear = servicioComandos.Ejecutar(new Dominio.Comandos.CrearOrdenCargaFas
                {
                    Orden = orden,
                    NombreWorkflow = nombreWorkflow,
                    InstanciaWorkflowId = instanciaWorkflow,
                    CentroId = centroId,
                    WorkflowDefinicionId = workflowDefinicionId,
                    Usuario = nombreUsuario
                }) as ResultadoCrear;
                resultado.Id = resultadoCrear.Id;

                var ordenDto = srvRepositorio.ObtenerOrdenCargaFas(resultado.Id);
                if (ordenDto != null)
                {
                    OrdenCargaFas.Set(context,ordenDto);
                    VehiculoDemorado.Set(context, orden.VehiculoDemorado);
                    TipoDocumentoIngreso.Set(context, Dominio.Enums.TipoDocumentoIngreso.OrdenCargaFas);
                    NumeroDocumentoIngreso.Set(context, orden.Id.ToString());   
                    ValidaCompliance.Set(context, orden.ValidaCompliance);
                    Rechazado.Set(context, orden.Rechazado);
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
