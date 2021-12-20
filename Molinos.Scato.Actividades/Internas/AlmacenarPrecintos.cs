using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class AlmacenarPrecintos : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<PrecintoDto[]> Precintos { get; set; }
        
        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();

            var precintos = Precintos.Get<PrecintoDto[]>(context);

            var resultado = new Resultado();
            try
            {
                resultado = servicioComandos.Ejecutar(new Dominio.Comandos.AlmacenarPrecintos { Precintos = precintos });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Precinto_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
