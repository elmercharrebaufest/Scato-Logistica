using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public sealed class DarDeAltaCtgDGAutomatica : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<Resultado> Resultado { get; set; }
        public InOutArgument<int> Intentos { get; set; }
        [RequiredArgument]
        public InArgument<TipoDocumentoIngreso> TipoDocumento { get; set; }
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var intentos = Intentos.Get<int>(context);
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var repositorio = context.GetExtension<IServicioRepositorio>();
                var tipoDocumento = TipoDocumento.Get<TipoDocumentoIngreso>(context);
                var puestoDeTrabajo = PuestoDeTrabajoId.Get<int>(context);

                var resultado = servicioComandos.Ejecutar(
                    new AutorizarCpeDG 
                    { 
                        WorkflowId = context.WorkflowInstanceId,
                        TipoDocumento = tipoDocumento,
                        PuestoDeTrabajoId = puestoDeTrabajo
                    });
                Resultado.Set(context, resultado);
            }
            catch (Exception e)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("", e.Message);
                Resultado.Set(context, resultado);
            }
            Intentos.Set(context, ++intentos);
        }
    }
}