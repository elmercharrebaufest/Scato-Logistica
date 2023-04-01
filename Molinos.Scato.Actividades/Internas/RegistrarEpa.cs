using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class RegistrarEpa : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<string> Cosecha { get; set; }
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        
        protected override Resultado Execute(CodeActivityContext context)
        {
            var cosecha = Cosecha.Get<string>(context);
            var instanceId = InstanceId.Get<Guid>(context);
            Resultado resultado = new Resultado();
            var servicioComandos = context.GetExtension<IServicioComandos>();
            try
            {
                var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
                if (servicioRepositorio.ValidaStockEPA(instanceId))
                {
                    if (servicioRepositorio.EsRecorridoConEstablecimiento(instanceId))
                    {
                        var descuentaPesoDescontado = servicioRepositorio.DescuentaPesoDescontado(cosecha);
                        var pesoNeto = descuentaPesoDescontado
                            ? servicioRepositorio.ObtenerPesoNetoConDescuento(instanceId)
                            : servicioRepositorio.ObtenerPesoNetoSinDescuento(instanceId);

                        var registro = new RegistroStockEPADto()
                        {
                            InstanceId = instanceId,
                            EPApesoDescontadoTildado = descuentaPesoDescontado,
                            PesoNeto = pesoNeto.Value
                        };
                        resultado = servicioComandos.Ejecutar(new CrearRegistroStockEpa() {Dto = registro});
                    }
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
