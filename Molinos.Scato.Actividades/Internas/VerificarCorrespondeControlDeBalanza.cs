using System;
using System.Activities;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarCorrespondeControlDeBalanza : CodeActivity<Resultado>
    {
        public OutArgument<bool> CorrespondeControlDeBalanza { get; set; }
        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            try
            {
                var instanceId = context.WorkflowInstanceId;
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var controlBalanza = srvRepositorio.VerificarCorrespondeControlDeBalanza(instanceId);
                CorrespondeControlDeBalanza.Set(context, controlBalanza);
                if (controlBalanza)
                {
                    //resultado.Errores.Add(new KeyValuePair<string, string>("InstanciaWorkflow", Textos.ControlDeBalanza_Esperando)); 
                }
                return resultado;
            }
            catch (Exception)
            {
                resultado.Errores.Add(new KeyValuePair<string, string>("InstanciaWorkflow",Textos.Error_Generico));
                return resultado;
            }
        }
    }
}
