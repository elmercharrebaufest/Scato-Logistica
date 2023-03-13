using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public sealed class DarDeAltaCtgDGManual : CodeActivity
    {
        public OutArgument<Resultado> Resultado { get; set; }
        public InArgument<string> CodigoCTG { get; set; }
        public InArgument<string> Sucursal { get; set; }
        public InArgument<string> NroOrden { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var repositorio = context.GetExtension<IServicioRepositorio>();

                var codigoCTG = CodigoCTG.Get<string>(context);
                var sucursal = Sucursal.Get<string>(context);
                var nroOrden = NroOrden.Get<string>(context);

                var resultado = servicioComandos.Ejecutar(new CrearAltaCTGDG 
                { 
                    Dto = new AltaCTGDto 
                    {
                        CodigoCTG = codigoCTG.Trim(),
                        Fecha = DateTime.Now,
                        WorkflowId = context.WorkflowInstanceId,
                        Sucursal = sucursal.Trim(),
                        NroOrden = nroOrden.Trim()
                    }
                });
                Resultado.Set(context, resultado);
            }
            catch (Exception e)
            {
                var resultado = new Resultado();
                resultado.Errores.Add("", e.Message);
                Resultado.Set(context, resultado);
            }
        }
    }
}