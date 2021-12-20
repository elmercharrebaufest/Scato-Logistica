using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresarCaladoEnPlanta : CodeActivity<Resultado>
    {
        public InArgument<CaladoEnPlantaDto> CaladoEnPlanta { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var calado = CaladoEnPlanta.Get<CaladoEnPlantaDto>(context);
            var resultado = new Resultado();
            try
            {

                var servicioComandos = context.GetExtension<IServicioComandos>();
                resultado = servicioComandos.Ejecutar(new CrearCaladoEnPlanta { CaladoEnPlanta = calado});
            }
            catch
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}
