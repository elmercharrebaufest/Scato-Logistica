using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ActualizarAlmacen : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        [RequiredArgument]
        public InArgument<int> AlmacenId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var instanceId = InstanceId.Get<Guid>(context);
            var almacenId = AlmacenId.Get<int>(context);
            var servicioComandos = context.GetExtension<IServicioComandos>();
            var resultado = new Resultado();
            try
            {
                resultado = servicioComandos.Ejecutar(new ModificarRecorridoAlmacen
                    {
                        InstanceId = instanceId,
                        AlmacenId = almacenId
                    });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}
