using System;
using System.Activities;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class CrearIngresoDeDatosDeExportacion : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        [RequiredArgument]
        public InArgument<IngresoDeDatosDeExportacionDto> Dto { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var instanceId = InstanceId.Get<Guid>(context);
            var dto = Dto.Get<IngresoDeDatosDeExportacionDto>(context);
            var resultado = new Resultado();
            
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                resultado =
                    servicioComandos.Ejecutar(new Dominio.Comandos.CrearIngresoDeDatosDeExportacion
                    {
                        Dto = dto
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