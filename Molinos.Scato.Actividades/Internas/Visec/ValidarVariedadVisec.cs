using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class ValidarVariedadVisec : CodeActivity<Resultado>
    {
        public InArgument<Guid> InstanceId { get; set; }
        public OutArgument<bool> EsVisec { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var resulado = new Resultado();
            var servicioRepositorio = context.GetExtension<IServicioRepositorio>();

            var instanceId = InstanceId.Get<Guid>(context);
            
            var recorrido = servicioRepositorio.ObtenerRecorridoPorGuid(instanceId);
            var esVisec = recorrido.TipoVariedadCodigo == Constantes.TipoVariedadMaterial.EUDR
                || recorrido.TipoVariedadCodigo == Constantes.TipoVariedadMaterial.EPAyEUDR;

            EsVisec.Set(context, esVisec);
            return resulado;
        }
    }
}