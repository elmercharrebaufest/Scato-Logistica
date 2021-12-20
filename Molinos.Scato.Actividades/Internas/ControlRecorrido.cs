using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ControlRecorrido : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<ControlRecorridoDto> ControlRecorridoDto { get; set; }

        public OutArgument<string> NombreUsuario { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var controlRecorrido = ControlRecorridoDto.Get<ControlRecorridoDto>(context);
                controlRecorrido.Fecha = DateTime.Now;
                resultado = servicioComandos.Ejecutar(new CrearControlRecorrido { Dto = controlRecorrido });
                if (!String.IsNullOrEmpty(controlRecorrido.NombreUsuario))
                {
                    NombreUsuario.Set(context, controlRecorrido.NombreUsuario);
                }
                
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Recrorrido_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
