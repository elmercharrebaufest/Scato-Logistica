using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ObtenerValoresSap : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> NumeroDeDocumento { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<int> WorkflowDefinicionId { get; set; }
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        [RequiredArgument]
        public InOutArgument<VehiculoDto> Vehiculo { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();
            try
            {
                var numeroDeDocumento = NumeroDeDocumento.Get<string>(context);
                var centroId = CentroId.Get<int>(context);
                var workflowDefinicionId = WorkflowDefinicionId.Get<int>(context);
                var instanceId = InstanceId.Get<Guid>(context);
                var vehiculo = Vehiculo.Get<VehiculoDto>(context);

                var resultado = servicioComandos.Ejecutar(new ActualizarValoresSap
                {
                    NumeroDeDocumento = numeroDeDocumento,
                    CentroId = centroId,
                    WorkflowDefinicionId = workflowDefinicionId,
                    InstanceId = instanceId
                }) as ResultadoActualizarValoresSap;
                if (!resultado.HayErrores)
                {
                    vehiculo.NumeroDeDocumentoSap = resultado.NumeroDeDocumentoSap;
                    vehiculo.DocumentoInternoSap = resultado.DocumentoInternoSap;
                    Vehiculo.Set(context, vehiculo);
                }
            }
            catch
            {
            }
        }
    }
}
