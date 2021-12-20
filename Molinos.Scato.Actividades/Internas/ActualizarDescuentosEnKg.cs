using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ActualizarDescuentosEnKg : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoNeto { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var instanceId = InstanceId.Get<Guid>(context);
            var pesoNeto = PesoNeto.Get<int>(context);
            var servicioComandos = context.GetExtension<IServicioComandos>();
            var resultado = new Resultado();
            try
            {
                resultado = servicioComandos.Ejecutar(new Dominio.Comandos.ActualizarDescuentosEnKg
                    {
                        InstanceId = instanceId,
                        PesoNeto = pesoNeto
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
