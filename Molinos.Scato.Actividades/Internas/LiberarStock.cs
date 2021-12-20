using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{

    public class LiberarStock : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid?> InstanceId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            
            var instanceId = InstanceId.Get<Guid>(context);
            var resultado = new Resultado();
            try
            {
                resultado = context.GetExtension<IServicioComandos>().Ejecutar(new LiberarStockSojaEpa { InstanceId = instanceId });

            }
            catch (Exception e)
            {

                resultado.Errores.Add("", "Error al liberar el stock");
            }

            return resultado;
        }
    }
}
