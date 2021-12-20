using System;
using System.Activities;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionTicketPesada : CodeActivity<Resultado>
    {
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<int> CantCopias { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroDocumento { get; set; }
        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }
        [RequiredArgument]
        public InArgument<int> TransportistaId { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }

        public InArgument<string> PatenteAcoplado { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroIngreso { get; set; }
        public InArgument<string> Observaciones { get; set; }
        [RequiredArgument]
        public InArgument<string> TipoDocumento { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoBruto { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoTara { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoNeto { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var firmaProvider = context.GetExtension<IFirmaProvider>();

            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);
            var numeroDocumento = NumeroDocumento.Get<string>(context);
            var materialId = MaterialId.Get<int>(context);
            var transportistaId = TransportistaId.Get<int>(context);
            var patente = Patente.Get<string>(context);
            var patenteAcoplado = PatenteAcoplado.Get<string>(context);
            var centroId = CentroId.Get<int>(context);
            var numeroIngreso = NumeroIngreso.Get<string>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var cantCopias = CantCopias.Get<int>(context);
            var observaciones = Observaciones.Get<string>(context);
            var tipoDocumento = TipoDocumento.Get<string>(context);
            var pesoBruto = PesoBruto.Get<int>(context);
            var pesoTara = PesoTara.Get<int>(context);
            var pesoNeto = PesoNeto.Get<int>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            
            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresion Ticket Pesada",
                    ActividadXaml = "ImpresionTicketPesada",
                    WorkflowInstanceId = workflowId,
                    Fecha = DateTime.Now
                };
            try
            {
                resultado = servicio.Ejecutar(new CrearLogActividad { Dto = logActividad });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.LogActividad_ErrorEnLaCarga);
            }
            
            try
            {
                var material = repositorio.ObtenerMaterial(materialId);
                var transportista = repositorio.ObtenerTransportista(transportistaId);
                var firma = firmaProvider.ObtenerFirmaSinLogo();
                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }
                
                var dto = new ImpTicketPesadaDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Codigo = codigo,
                    Observaciones = observaciones,
                    PatenteAcoplado = patenteAcoplado ?? "",
                    NumeroIngreso = numeroIngreso,
                    NumeroDocumento = numeroDocumento,
                    Patente = patente,
                    Material = material != null ? material.Descripcion : "",
                    PesoBruto = pesoBruto.ToString(CultureInfo.InvariantCulture),
                    PesoTara = pesoTara.ToString(CultureInfo.InvariantCulture),
                    PesoNeto = pesoNeto.ToString(CultureInfo.InvariantCulture),
                    TipoDocumento = tipoDocumento,
                    CuitTransportista = transportista != null ? transportista.Cuit : "",
                    DoimicilioCentro = documento.CentroDescripcion,
                    Transportista = transportista != null ? transportista.RazonSocial : "",
                    Remitente = firma.Descripcion,
                    Emisor = firma.Descripcion,
                    WorkflowId = workflowId,
                };

                resultado = servicio.Ejecutar(new ImprimirTicketPesada { Dto = dto, CantidadCopias = cantCopias });
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionTicketPesada", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
