using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresarEnvioACamara : CodeActivity<Resultado>
    {
        public InArgument<MuestraEnvioACamaraDto> EnvioACamara { get; set; }
        public OutArgument<int> CamaraId { get; set; }
        public OutArgument<bool> EnviaMuestraACamara { get; set; }
        public OutArgument<int> MuestraId { get; set; }
        public OutArgument<CaladoDto> Calado { get; set; }
        protected override Resultado Execute(CodeActivityContext context)
        {
            var envioACamara = EnvioACamara.Get(context);
            CamaraId.Set(context, envioACamara.CamaraId);
            EnviaMuestraACamara.Set(context, true);

            var resultado = new ResultadoCrear();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var servicioRepositorio = context.GetExtension<IServicioRepositorio>();

                resultado = servicioComandos.Ejecutar(new CrearEnvioACamara { Dto = envioACamara }) as ResultadoCrear;
                MuestraId.Set(context, resultado.Id);
                Calado.Set(context, servicioRepositorio.ObtenerCaladoPorGuid(context.WorkflowInstanceId));
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.EnvioACamara_ErrorEnLaCarga);
            }
            return resultado;
        }
    }
}