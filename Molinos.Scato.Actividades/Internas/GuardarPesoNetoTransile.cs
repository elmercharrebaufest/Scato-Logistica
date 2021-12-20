using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class GuardarPesoNetoTransile : CodeActivity
    {
        public InArgument<Guid> InstanceId { get; set; }
        public InArgument<int> Peso { get; set; }
        public InArgument<int> Tolerancia { get; set; }

        public OutArgument<bool> FinTransile { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var instanceId = InstanceId.Get<Guid>(context);
            var peso = Peso.Get<int>(context);
            var tolerancia = Tolerancia.Get<int>(context);

            var resultado = new Resultado();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();

                resultado = servicioComandos.Ejecutar(new ModificarRecorridoPesoNetoTransile
                {
                    InstanceId = instanceId,
                    PesoAgregado = peso
                });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }

            if ((peso > 0 && peso > tolerancia) || (peso < 0 && peso < tolerancia))
            {
                FinTransile.Set(context, false);
            }
            else
            {
                FinTransile.Set(context, true);
            }
        }
    }
}
