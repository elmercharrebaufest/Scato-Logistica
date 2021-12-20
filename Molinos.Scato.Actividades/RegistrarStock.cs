using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class RegistrarStock : CodeActivity<Resultado>
    {
        protected override Resultado Execute(CodeActivityContext context)
        {
            Resultado resultado = new Resultado();
            var servicioComandos = context.GetExtension<IServicioComandos>();
            try
            {
                resultado = servicioComandos.Ejecutar(new ValidarStock { InstanceId = context.WorkflowInstanceId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}
