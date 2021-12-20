using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresarCIU : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        [RequiredArgument]
        public InArgument<string> Numero { get; set; }


        protected override Resultado Execute(CodeActivityContext context)
        {
            var instanceId = InstanceId.Get<Guid>(context);
            var numero = Numero.Get<string>(context);
            var servicioComandos = context.GetExtension<IServicioComandos>();

            var resultado = new Resultado();
            try
            {
                resultado =
                    servicioComandos.Ejecutar(new ModificarRecorridoNumeroCIU
                    {
                        InstanceId = instanceId,
                        Numero = numero
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
