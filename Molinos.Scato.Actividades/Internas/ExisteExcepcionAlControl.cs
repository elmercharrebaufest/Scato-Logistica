using System;
using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    /// <summary>
    ///     Retorna True solo si el chofer existe y tanto el como el camion estan habilitados
    /// </summary>
    public class ExisteExcepcionAlControl : CodeActivity<bool>
    {
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }

        [RequiredArgument]
        public InArgument<int> TransportistaId { get; set; }

        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }

        public InArgument<int?> CentroDestinoId { get; set; }
        public InArgument<int?> ClienteDestinoId { get; set; }

        protected override bool Execute(CodeActivityContext context)
        {
            var repositorio = context.GetExtension<IServicioRepositorio>();

            var centroId = CentroId.Get<int>(context);
            var transportistaId = TransportistaId.Get<int>(context);
            var materialId = MaterialId.Get<int>(context);
            var centroDestinoId = CentroDestinoId.Get<int?>(context);
            var clienteDestinoId = ClienteDestinoId.Get<int?>(context);

            var cartaPorte = repositorio.ObtenerCartaPortePorInstanceId(context.WorkflowInstanceId);

            return cartaPorte.IntermediarioFleteId > 0 
                ? repositorio.BuscarExcepcionAlControlProveedor(materialId, cartaPorte.IntermediarioFleteId, centroId, DateTime.Today, centroDestinoId, clienteDestinoId)
                : repositorio.BuscarExcepcionAlControl(materialId, transportistaId, centroId, DateTime.Today, centroDestinoId, clienteDestinoId);
        }
    }
}