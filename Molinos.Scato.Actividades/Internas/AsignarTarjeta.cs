using System;
using System.Activities;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class AsignarTarjeta : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }
        [RequiredArgument]
        public InArgument<string> Numero { get; set; }


        protected override Resultado Execute(CodeActivityContext context)
        {
            var instanceId = InstanceId.Get<Guid>(context);
            var numero = Numero.Get<string>(context);
            var resultado = new Resultado();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                resultado =
                    servicioComandos.Ejecutar(new ModificarRecorridoTarjetaDeAcceso
                    {
                        InstanceId = instanceId,
                        Numero = numero
                    });

                resultado = servicioComandos.Ejecutar(new ActualizarCargaDeCupo
                    {
                        Numero = numero,
                        InstanceId = instanceId
                    });
                if (!resultado.HayErrores)
                {
                    context.GetExtension<ScatoPersistenceParticipant>().NumeroDeTarjeta = numero;
                }
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }

            try
            {
                var servicio = context.GetExtension<IServicioRepositorio>();
                servicio.RecuperarFotosTemporalesPorTarjeta(instanceId);
            }
            catch (Exception)
            {

            }
            return resultado;          
        }
    }
}
