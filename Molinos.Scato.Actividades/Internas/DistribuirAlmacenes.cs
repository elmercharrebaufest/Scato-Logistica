using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class DistribuirAlmacenes : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        [RequiredArgument]
        public InArgument<DistribucionDeAlmacenesDto> DistribucionDeAlmacenes { get; set; }


        protected override Resultado Execute(CodeActivityContext context)
        {
            var instanceId = InstanceId.Get<Guid>(context);
            var distribucionDeAlmacenes = DistribucionDeAlmacenes.Get<DistribucionDeAlmacenesDto>(context);

            var resultado = new Resultado();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();

                resultado =
                    servicioComandos.Ejecutar(new CrearDistribucionDeAlmacenes
                    {
                        InstanceId = instanceId,
                        Dto = distribucionDeAlmacenes
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
