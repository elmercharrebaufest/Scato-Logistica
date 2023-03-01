using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public sealed class DarDeBajaCtgDGDefinitivoAutomatica : CodeActivity
    {
        public InOutArgument<int> Intentos { get; set; }
        public InArgument<OrdenDeDescargaFasonDto> Orden { get; set; }
        public OutArgument<Resultado> Resultado { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            var orden = Orden.Get<OrdenDeDescargaFasonDto>(context);
            
            try{

                var servicioComandos = context.GetExtension<IServicioComandos>();
                var resultado = servicioComandos.Ejecutar(new ConfirmarArriboDGDefinitivo { Dto = orden });
                   
                Resultado.Set(context, resultado);

            }
            catch (Exception e)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("", e.Message);
                Resultado.Set(context, resultado);
            }
            var intentos = Intentos.Get<int>(context);
            Intentos.Set(context, ++intentos);
        }
    }
}