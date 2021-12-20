using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class AutorizarTiempoEnTransito : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<AutorizacionTiempoEnTransitoDto> AutorizacionTiempoDto { get; set; }

        public OutArgument<string> NombreUsuario { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var autorizacionTiempo = AutorizacionTiempoDto.Get<AutorizacionTiempoEnTransitoDto>(context);
                autorizacionTiempo.Fecha = DateTime.Now;
                resultado = servicioComandos.Ejecutar(new CrearAutorizarTiempoEnTransito() { Dto = autorizacionTiempo });
                NombreUsuario.Set(context, autorizacionTiempo.NombreUsuario);
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Recrorrido_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
