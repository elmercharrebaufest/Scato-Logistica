using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class BalanzaACeroSiEsTren : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<int> BalanzaId { get; set; }
        public InArgument<int> PuestoId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var balanzaId = BalanzaId.Get<int>(context);
            var puestoId = PuestoId.Get<int>(context);
            var resultado = new Resultado();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                resultado = servicioComandos.Ejecutar(new ModificarBalanzaEstaEnCero { BalanzaId = balanzaId, EstaEnCero = true });
                EnviarNotificacion(context, puestoId);
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
        private void EnviarNotificacion(CodeActivityContext context, int puestoId, TipoAlerta tipo = TipoAlerta.Automatica)
        {
            var servicio = context.GetExtension<IServicioNotificarUsuario>();
            try
            {
                var notificacion = new NotificacionPesadaAutomaticaDto
                {
                    Id = puestoId,
                    Error = null,
                    CartaPorte =  "",
                    Diferencia = "0",
                    DifPeso = "0",
                    DifNeto = "0",
                    Peso = "0",
                    Entregador =  "",
                    Material = "Material",
                    WorkflowInstanceId =new Guid(),
                    TipoPeso = "Peso:",
                    Patente = "" ,
                    Actividad ="En Espera",
                    TipoVehiculo = "Vehiculo",
                    Tarjeta = "",
                    TipoPesoOrigen = "Peso Org:",
                    PesoBrutoOrigen = "0",
                    PesoNetoOrigen = "0"

                };
                servicio.Notificar(new NotificacionDto
                {
                    Grupo = "Automaticas",
                    Mensaje = notificacion.ToJson(),
                    TipoAlerta = tipo,
                    Leido = true,
                    PuestoId = puestoId
                });
            }
            catch (Exception e)
            {
            }
        }

    }
}
