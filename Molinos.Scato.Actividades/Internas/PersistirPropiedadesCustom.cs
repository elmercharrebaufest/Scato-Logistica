using System;
using System.Activities;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    /// <summary>
    ///     Persiste las propiedades custom necesarias para el tracking de los workflows utilizando el mecanismo de property promotion de WF
    /// </summary>
    public class PersistirPropiedadesCustom : CodeActivity
    {
        public InArgument<string> Patente { get; set; }

        public InArgument<int?> MaterialId { get; set; }

        public InArgument<int?> TransportistaId { get; set; }

        public InArgument<string> Cuit { get; set; }

        public InArgument<string> Calidad { get; set; }

        public InArgument<int?> TipoDocumentoDeIngreso { get; set; }

        public InArgument<string> NumeroDocumentoDeIngreso { get; set; }

        public InArgument<int?> CentroId { get; set; }

        public InArgument<string> Workflow { get; set; }

        public InArgument<int?> TipoVehiculo { get; set; }

        public InArgument<string> TipoComercial { get; set; }

        public InArgument<int?> TipoComercialId { get; set; }

        public InArgument<string> ChoferDNI { get; set; }

        public InArgument<string> ChoferNombre { get; set; }

        public InArgument<string> Procedencia { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioRepositorio>();

            var patente = Patente.Get<string>(context);
            var workflow = Workflow.Get<string>(context);
            var centroId = CentroId.Get<int?>(context);
            var materialId = MaterialId.Get<int?>(context);
            var transportistaId = TransportistaId.Get<int?>(context);
            var cuit = Cuit.Get<string>(context);
            var calidad = Calidad.Get<string>(context);
            var tipoDocumentoDeIngreso = TipoDocumentoDeIngreso.Get<int?>(context);
            var numeroDocumentoDeIngreso = NumeroDocumentoDeIngreso.Get<string>(context);
            var tipoVehiculo = TipoVehiculo.Get<int?>(context);
            var tipoComercialId = TipoComercialId.Get<int?>(context);
            var tipoComercial = TipoComercial.Get<string>(context);
            var choferDNI = ChoferDNI.Get<string>(context);
            var choferNombre = ChoferNombre.Get<string>(context);
            var procedencia = Procedencia.Get<string>(context);

            var recorrido = context.WorkflowInstanceId != null ? servicio.ObtenerRecorridoPorGuid(context.WorkflowInstanceId) : null;

            if (patente != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().Patente = patente;
            }
            if (workflow != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().Workflow = workflow;
            }
            if (centroId != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().CentroId = centroId;
            }
            if (centroId.HasValue)
            {
                var dto = servicio.ObtenerCentro(centroId.Value);
                if (dto != null)
                {
                    context.GetExtension<ScatoPersistenceParticipant>().Centro = dto.Descripcion;
                    context.GetExtension<ScatoPersistenceParticipant>().CentroCodigoSap = dto.CodigoSAP ?? "";
                }
            }
            if (materialId != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().MaterialId = materialId;
            }
            if (materialId.HasValue)
            {
                var materialDto = servicio.ObtenerMaterial(materialId.Value);
                if (materialDto != null)
                {
                    context.GetExtension<ScatoPersistenceParticipant>().Material = materialDto.Descripcion;
                    context.GetExtension<ScatoPersistenceParticipant>().MaterialCodigoSap = materialDto.CodigoSAP ?? "";
                }
            }
            if (transportistaId != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().TransportistaId = transportistaId;
            }
            if (transportistaId.HasValue)
            {
                var transportistaDto = servicio.ObtenerTransportista(transportistaId.Value);
                if (transportistaDto != null)
                {
                    context.GetExtension<ScatoPersistenceParticipant>().Transportista = transportistaDto.RazonSocial;
                }
            }
            if (cuit != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().Cuit = cuit;
            }
            if (calidad != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().Calidad = calidad;
            }
            if (tipoDocumentoDeIngreso != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().TipoDocumentoDeIngreso = tipoDocumentoDeIngreso;
            }
            if (numeroDocumentoDeIngreso != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().NumeroDocumentoDeIngreso = numeroDocumentoDeIngreso;
            }
            if (tipoVehiculo != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().TipoVehiculo = tipoVehiculo;
            }
            if (tipoComercial != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().TipoComercial = tipoComercial;
            }
            if (tipoComercial != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().TipoComercialId = tipoComercialId;
            }
            if (choferDNI != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().ChoferDNI = choferDNI;
            }
            if (choferNombre != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().ChoferNombre = choferNombre;
            }
            if (Procedencia != null)
            {
                context.GetExtension<ScatoPersistenceParticipant>().Procedencia = procedencia;
            }
            if(recorrido != null)
            {
                var cartaPorte = servicio.ObtenerCartaDePortePorrecorrido(recorrido.Id);
                if(cartaPorte != null)
                {
                    context.GetExtension<ScatoPersistenceParticipant>().Entregador = cartaPorte.Entregador;
                }
                context.GetExtension<ScatoPersistenceParticipant>().EsGrano = recorrido.Material.EsGrano.ToString();
                context.GetExtension<ScatoPersistenceParticipant>().EsRechazado = recorrido.Rechazado.ToString();
            }
        }
    }
}