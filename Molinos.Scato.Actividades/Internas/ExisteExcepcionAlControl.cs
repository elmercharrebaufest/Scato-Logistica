using Molinos.Scato.Servicios;
using System;
using System.Activities;

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
        public InArgument<int?> IntermediarioId { get; set; }
        public InArgument<bool?> EsCartaPorte { get; set; }

        protected override bool Execute(CodeActivityContext context)
        {
            var repositorio = context.GetExtension<IServicioRepositorio>();

            var centroId = CentroId.Get<int>(context);
            var transportistaId = TransportistaId.Get<int>(context);
            var materialId = MaterialId.Get<int>(context);
            var centroDestinoId = CentroDestinoId.Get<int?>(context);
            var clienteDestinoId = ClienteDestinoId.Get<int?>(context);
            var intermediarioId = IntermediarioId.Get<int>(context);
            var esCartaPorte = EsCartaPorte.Get<bool?>(context);

            return esCartaPorte.HasValue && esCartaPorte.Value
                ? ExisteExcepcionAlControlParaCartaPorte(repositorio, materialId, transportistaId, intermediarioId, centroId, DateTime.Today, centroDestinoId, clienteDestinoId)
                : ExisteExcepcionAlControlParaOrdenes(repositorio, materialId, transportistaId, intermediarioId, centroId, DateTime.Today, centroDestinoId, clienteDestinoId);
        }

        private bool ExisteExcepcionAlControlParaOrdenes(IServicioRepositorio repositorio, int materialId, int transportistaId, int intermediarioId, int centroId, DateTime date, int? centroDestinoId, int? clienteDestinoId)
        {
            return intermediarioId > 0
                ? repositorio.BuscarExcepcionAlControlProveedor(materialId, intermediarioId, centroId, date, centroDestinoId, clienteDestinoId)
                : repositorio.BuscarExcepcionAlControl(materialId, transportistaId, centroId, date, centroDestinoId, clienteDestinoId);
        }

        private bool ExisteExcepcionAlControlParaCartaPorte(IServicioRepositorio repositorio, int materialId, int transportistaId, int intermediarioId, int centroId, DateTime date, int? centroDestinoId, int? clienteDestinoId)
        {
            return intermediarioId > 0
                ? repositorio.ExisteExcepcionAlControlProveedorParaCartaPorte(materialId, intermediarioId, centroId, date, centroDestinoId, clienteDestinoId)
                : repositorio.ExisteExcepcionAlControlParaCartaPorte(materialId, transportistaId, centroId, date, centroDestinoId, clienteDestinoId);
        }
    }
}