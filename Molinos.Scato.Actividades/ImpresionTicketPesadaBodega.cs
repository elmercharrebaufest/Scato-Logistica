using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionTicketPesadaBodega : CodeActivity<Resultado>
    {
        public InArgument<string> CodigoDeImpresion { get; set; }
        public InArgument<int> CantCopias { get; set; }

        public InArgument<int> RecorridoId { get; set; }
        public InArgument<string> Comprobante { get; set; }
        public InArgument<int> MaterialId { get; set; }
        public InArgument<int?> ProveedorId { get; set; }
        public InArgument<string> NroDocumento { get; set; }
        public InArgument<int> TransportistaId { get; set; }
        public InArgument<int> ChoferId { get; set; }
        public InArgument<string> CIU { get; set; }
        public InArgument<string> Pedido { get; set; }
        public InArgument<string> PatenteCamion { get; set; }
        public InArgument<string> PatenteAcoplado { get; set; }
        public InArgument<Guid> WorkflowId { get; set; }
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();

            var resultado = new Resultado();

            var recorrido = repositorio.ObtenerRecorrido(RecorridoId.Get<int>(context));
            var workflowId = WorkflowId.Get<Guid>(context);
            var numeroDocumento = NroDocumento.Get<string>(context);
            var materialId = MaterialId.Get<int>(context);
            var transportistaId = TransportistaId.Get<int>(context);
            var patente = PatenteCamion.Get<string>(context);
            var patenteAcoplado = PatenteAcoplado.Get<string>(context);
            var centroId = recorrido.Centro.Id;
            var balanzaBruto = recorrido.BalanzaBrutoId.HasValue? repositorio.ObtenerBalanza(recorrido.BalanzaBrutoId.Value).Nombre : "";
            var balanzaTara = recorrido.BalanzaTaraId.HasValue? repositorio.ObtenerBalanza(recorrido.BalanzaTaraId.Value).Nombre : "";
            var codigo = CodigoDeImpresion.Get<string>(context);
            var cantCopias = CantCopias.Get<int>(context);
            var observacion = repositorio.ObtenerObservacion(workflowId);
            var pesoBrutoBodega = recorrido.PesoBruto.HasValue ? recorrido.PesoBruto.Value.ToString(CultureInfo.InvariantCulture) : "";
            var pesoTaraBodega = recorrido.PesoTaraBodega.HasValue ? recorrido.PesoTaraBodega.Value.ToString(CultureInfo.InvariantCulture) : recorrido.PesoTara.HasValue ? recorrido.PesoTara.Value.ToString(CultureInfo.InvariantCulture) : "" ;
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            var comprobante = Comprobante.Get<string>(context);
            var ciu = CIU.Get<string>(context);
            var pedido = Pedido.Get<string>(context);
            var chofer = repositorio.ObtenerChofer(ChoferId.Get<int>(context)).NombreCompleto;
            var fechaIngreso = recorrido.FechaInicio;
            var fechaEgreso = recorrido.FechaEgreso;
            var fechaUltimaPesada = repositorio.ObtenerUltimaPesadaFechaPorGuid(workflowId);
            var proveedorId = ProveedorId.Get<int?>(context);
            var proveedor = proveedorId.HasValue ? repositorio.ObtenerProveedor(proveedorId.Value).RazonSocial : "";
            var calado = repositorio.ObtenerCaladoPorGuid(workflowId);
            var calados = calado != null ? calado.CaladosPorCaracteristica : new List<CaladoPorCaracteristicaDto>();
            
            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresion Ticket Pesada Bodega",
                    ActividadXaml = "ImpresionTicketPesadaBodega",
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
                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }

                var dto = new ImpTicketPesadaBodegaDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Codigo = codigo,
                    Observaciones = observacion.Observaciones,
                    PatenteAcoplado = patenteAcoplado ?? "",
                    Patente = patente,
                    Material = material != null ? material.Descripcion : "",
                    PesoBrutoBodega = pesoBrutoBodega,
                    PesoTaraBodega = pesoTaraBodega,
                    Transportista = transportista != null ? transportista.RazonSocial : "",
                    BalanzaBruto = balanzaBruto ,
                    BalanzaTara = balanzaTara ,
                    ComprobanteInterno = comprobante,
                    CIU = ciu ?? "" ,
                    Chofer = chofer ,
                    FechaEgreso = fechaEgreso ,
                    FechaIngreso = fechaIngreso ,
                    FechaUltimaPesada = fechaUltimaPesada,
                    Pedido = pedido ?? "",
                    Proveedor = proveedor,
                    Remito = numeroDocumento,
                    RubrosCalados = calados,
                    WorkflowId = workflowId,
                    CentroDescripcion = recorrido.Centro.Descripcion,
                    CentroLocalidad = recorrido.Centro.LocalidadDesc,
                    CentroDireccion = recorrido.Centro.Direccion
                };

                resultado = servicio.Ejecutar(new ImprimirTicketPesadaBodega { Dto = dto, CantidadCopias = cantCopias });
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionTicketPesadaBodega", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
