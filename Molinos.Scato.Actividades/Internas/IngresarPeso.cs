using System;
using System.Activities;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresarPeso : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        public InArgument<int> Peso { get; set; }

        public InArgument<int> AlamacenId { get; set; }

        public InArgument<int?> HidraulicaId { get; set; }

        public InArgument<int?> CalleId { get; set; }

        public InArgument<int> BalanzaId { get; set; }

        public InArgument<int?> ProximaBalanzaId { get; set; }

        public InArgument<TipoPesada> TipoPesada { get; set; }

        public InArgument<string> NombreUsuario { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var instanceId = InstanceId.Get<Guid>(context);
            var tipoPesada = TipoPesada.Get<TipoPesada>(context);
            var peso = Peso.Get<int>(context);
            var almacenId = AlamacenId.Get<int>(context);
            var hidraulicaId = HidraulicaId.Get<int?>(context);
            var calleId = CalleId.Get<int?>(context);
            var balanzaId = BalanzaId.Get<int>(context);
            var proximaBalanzaId = ProximaBalanzaId.Get<int?>(context);
            var nombreUsuario = NombreUsuario.Get<string>(context);
            var resultadoDestino = new Resultado();

            var servicioComandos = context.GetExtension<IServicioComandos>();

            try
            {


                resultadoDestino =
                    servicioComandos.Ejecutar(new ModificarRecorridoDestino
                    {
                        InstanceId = instanceId,
                        AlmacenId = almacenId,
                        HidraulicaId = hidraulicaId,
                        CalleId = calleId,
                        TipoPesada = tipoPesada,
                        ProximaBalanzaId = proximaBalanzaId
                    });


                if (resultadoDestino.HayErrores)
                {
                    return resultadoDestino;
                }

                resultadoDestino =
                    servicioComandos.Ejecutar(new ModificarRecorridoPeso
                    {
                        InstanceId = instanceId,
                        TipoPesada = tipoPesada,
                        Peso = peso,
                        BalanzaId = balanzaId,
                        Usuario = nombreUsuario
                    });
                context.GetExtension<ScatoPersistenceParticipant>().DatosProximaActividad = null;
            }
            catch (Exception)
            {
                resultadoDestino.Errores.Add("", Textos.Error_ActualizarGenerico);
            }

            try
            {
                servicioComandos.Ejecutar(new InformarPesadaCircular { WorkflowInstanceId = context.WorkflowInstanceId, TipoPesada = tipoPesada, Peso = peso });
            }
            catch (Exception e)
            {

            }

            return resultadoDestino;
        }
    }
}
