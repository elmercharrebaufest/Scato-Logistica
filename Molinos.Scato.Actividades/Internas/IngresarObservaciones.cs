using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresarObservaciones : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<ObservacionDto> Observacion { get; set; }
        
        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();

            var observacion = Observacion.Get<ObservacionDto>(context);

            var resultado = new Resultado();
            try
            {
                resultado = servicioComandos.Ejecutar(new AlmacenarObservacion { Dto = observacion });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Observacion_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
