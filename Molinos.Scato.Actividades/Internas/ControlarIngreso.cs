using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class ControlarIngreso : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<OrdenDeDescargaFasonDto> Orden { get; set; }

        public OutArgument<OrdenDeDescargaFasonDto> OrdenDeDescargaFason { get; set; }


        protected override Resultado Execute(CodeActivityContext context)
        {
            var orden = Orden.Get<OrdenDeDescargaFasonDto>(context);
            var resultado = new Resultado();

            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();

                resultado = servicioComandos.Ejecutar(new ModificarOrdenDeDescargaFason{Orden = orden});

                var ordenDto = srvRepositorio.ObtenerOrdenDeDescargaFason(orden.Id);
                if (ordenDto != null)
                {
                    OrdenDeDescargaFason.Set(context, ordenDto);
                }
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}
