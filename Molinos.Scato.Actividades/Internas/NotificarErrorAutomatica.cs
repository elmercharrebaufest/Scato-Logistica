using System;
using System.Activities;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class NotificarErrorAutomatica : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Etapa { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoId { get; set; }

        [RequiredArgument]
        public InArgument<Guid> Workflow { get; set; }
        [RequiredArgument]
        public InArgument<string> Error { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var servicio = context.GetExtension<IServicioNotificarUsuario>();
                var repositorio = context.GetExtension<IServicioRepositorio>();
                var puestoId = PuestoId.Get<int>(context);
                var workflowId = Workflow.Get<Guid>(context);
                var etapa = Etapa.Get<string>(context);
                var error = Error.Get<string>(context);

                var recorrido = repositorio.ObtenerDatosRecorridoActivoPorWorkflow(workflowId);
                var tipoPesada = recorrido.TipoDeWorkflow == TipoDeWorkflow.Ingreso && !recorrido.PesoTara.HasValue ? TipoPesada.Bruto :
                                 recorrido.TipoDeWorkflow == TipoDeWorkflow.Ingreso ? TipoPesada.Tara :
                                 recorrido.TipoDeWorkflow == TipoDeWorkflow.Egreso && !recorrido.PesoBruto.HasValue ? TipoPesada.Tara : TipoPesada.Bruto;


                var peso = tipoPesada == TipoPesada.Bruto ?  recorrido.PesoBruto ?? 0 : recorrido.PesoTara ?? 0;
                var pesoNeto = (recorrido.PesoBruto ?? 0) - (recorrido.PesoTara ?? 0);
                pesoNeto = pesoNeto > 0 ? pesoNeto : 0;
                var notificacion = new NotificacionPesadaAutomaticaDto
                {
                    Id = puestoId,
                    Error = error,
                    CartaPorte = recorrido.CartaDePorte,
                    Diferencia = pesoNeto.ToString(),
                    DifNeto = (pesoNeto - (recorrido.PesoNetoOrigen ?? 30000)).ToString(),
                    Peso = peso.ToString(),
                    DifPeso = (peso - (tipoPesada == TipoPesada.Bruto ? recorrido.PesoBrutoOrigen ?? 0 : recorrido.PesoTaraOrigen ?? 0)).ToString(),
                    Entregador = recorrido.Entregador?"Si":"No",
                    Material = recorrido.Material,
                    WorkflowInstanceId = workflowId,
                    TipoPeso = tipoPesada == TipoPesada.Bruto ? "Bruto:" : "Tara:",
                    Patente = recorrido.Patente,
                    Actividad = etapa,
                    TipoVehiculo = recorrido.TipoVehiculo.ToString(),
                    Tarjeta = recorrido.TarjetaDeAcceso,
                    Calle = recorrido.Calle,
                    TipoPesoOrigen = tipoPesada == TipoPesada.Bruto ? "Bruto Org:" : "Tara Org:",
                    PesoBrutoOrigen = recorrido.PesoBrutoOrigen.ToString(),
                    PesoNetoOrigen = recorrido.PesoNetoOrigen.ToString()
                    
                };
                servicio.Notificar(new NotificacionDto
                {
                    Grupo = "Automaticas",
                    Mensaje = notificacion.ToJson(),
                    TipoAlerta = TipoAlerta.Automatica,
                    Leido = false,
                    PuestoId = puestoId
                });
            }
            catch
            {

            }
        }
    }
}
