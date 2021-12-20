using System;
using System.Activities;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarCorrespondeLlamadaSap : CodeActivity
    {
        [RequiredArgument]
        public InArgument<FuncionSAP> FuncionSap { get; set; }
        public OutArgument<bool> CorrespondeLlamadaSap { get; set; }
        public InArgument<int> MaterialId { get; set; }
        public InArgument<int> CentroId { get; set; }
        public InArgument<int> TipoComercialId { get; set; }
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        public OutArgument<bool> EsIngreso { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var materialId = MaterialId.Get<int>(context);
            var centroId = CentroId.Get<int>(context);
            var funcion = FuncionSap.Get<FuncionSAP>(context);
            var guid = InstanceId.Get<Guid>(context);
            try
            {
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var existe = srvRepositorio.ExisteTransaccionSAP(materialId, centroId, funcion,guid);
                if (existe)
                {
                    if (funcion == FuncionSAP.InformarCupo)
                    {
                        var centro = srvRepositorio.ObtenerCentro(centroId);
                        existe = centro.RequiereCupo;
                    }                
                }
                CorrespondeLlamadaSap.Set(context, existe);
                var tipo = srvRepositorio.ObtenerTipoDeWorkflowPorGuid(guid);
                EsIngreso.Set(context, tipo == TipoDeWorkflow.Ingreso);
            }
            catch
            {
                
            }
            
        }
    }
}
