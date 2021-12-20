using System;
using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{

    public class VerificarCorrespondeImprimirReciboMunicipal : CodeActivity
    {
        public OutArgument<bool> CorrespondeImprimirReciboMunicipal { get; set; }
        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var materialId = MaterialId.Get<int>(context);
                //var centroId = CentroId.Get<int>(context);
                var workflowId = WorkflowId.Get<Guid>(context);
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var imprime = srvRepositorio.RecorridoPagaTicketMunicipal(workflowId) ?? srvRepositorio.MaterialImprimeReciboMunicipal(workflowId);
                // TODO: falta definir lógica por origen-destino del camion
                //if (imprime)
                //{
                //    var cartaPorte = srvRepositorio.ObtenerCartaPortePorInstanceId(workflowId);
                //}
                CorrespondeImprimirReciboMunicipal.Set(context, imprime);
            }
            catch (Exception)
            {
                CorrespondeImprimirReciboMunicipal.Set(context, true);
            }
        }
    }
}
