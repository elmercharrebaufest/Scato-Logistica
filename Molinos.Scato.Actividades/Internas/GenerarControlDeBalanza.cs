using System;
using System.Activities;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class GenerarControlDeBalanza : CodeActivity
    {
        [RequiredArgument]
        public InArgument<TipoPesada> TipoPesada { get; set; }
        [RequiredArgument]
        public InArgument<int> BalanzaId { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> Fecha { get; set; }
        [RequiredArgument]
        public InArgument<int> Peso { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var tipoPesada = TipoPesada.Get<TipoPesada>(context);
            var balanzaId = BalanzaId.Get<int>(context);
            var fecha = Fecha.Get<DateTime>(context);
            var peso = Peso.Get<int>(context);
            var servicioComandos = context.GetExtension<IServicioComandos>();
            servicioComandos.Ejecutar(new ControlDeBalanzaComando
                {
                    Dto = new ControlDeBalanzaDto
                        {
                            InstanciaWorkflow = context.WorkflowInstanceId,
                            TipoPesada = tipoPesada,
                            ControlesDeBalanzasPesadas = new List<ControlDeBalanzaPesadaDto>()
                                {
                                    new ControlDeBalanzaPesadaDto
                                        {
                                            BalanzaId = balanzaId,
                                            Fecha = fecha,
                                            Peso = peso
                                        }
                                }
                        }
                });
        }
    }
}
