using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades
{
    public class RecuperarFotosPorTarjeta : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var instanceId = InstanceId.Get<Guid>(context);
            var resultado = new Resultado();
            try
            {
                var servicio = context.GetExtension<IServicioRepositorio>();
                servicio.RecuperarFotosTemporalesPorTarjeta(instanceId);
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}
