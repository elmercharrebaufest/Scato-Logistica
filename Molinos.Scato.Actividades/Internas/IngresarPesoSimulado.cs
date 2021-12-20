using System;
using System.Activities;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class IngresarPesoSimulado : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<int> PesoBruto { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoTara { get; set; }
        [RequiredArgument]
        public InArgument<string> NombreUsuario { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var pesoBruto = PesoBruto.Get<int>(context);
            var pesoTara = PesoTara.Get<int>(context);
            var nombreUsuario = NombreUsuario.Get<string>(context);
            var resultadoPeso = new Resultado();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();

                resultadoPeso = 
                    servicioComandos.Ejecutar(new ModificarRecorridoPeso
                        {
                            InstanceId = context.WorkflowInstanceId,
                            TipoPesada = TipoPesada.Bruto,
                            Peso = pesoBruto,
                            BalanzaId = 0
                        });
                context.GetExtension<ScatoPersistenceParticipant>().DatosProximaActividad = null;

                resultadoPeso =
                    servicioComandos.Ejecutar(new ModificarRecorridoPeso
                    {
                        InstanceId = context.WorkflowInstanceId,
                        TipoPesada = TipoPesada.Tara,
                        Peso = pesoTara,
                        BalanzaId = 0,
                        Usuario = nombreUsuario
                    });
                context.GetExtension<ScatoPersistenceParticipant>().DatosProximaActividad = null;
            }
            catch (Exception)
            {
                resultadoPeso.Errores.Add("", Textos.Error_ActualizarGenerico);
            }
            return resultadoPeso;
        }
    }
}
