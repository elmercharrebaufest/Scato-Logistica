using System;
using System.Activities;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionReciboMunicipal : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        public InArgument<int?> CantCopias { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();
            var centroId = CentroId.Get<int>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var workflowId = WorkflowId.Get<Guid>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;

            var logActividad = new LogActividadDto
            {
                Actividad = "Impresion Recibo Municipal",
                ActividadXaml = "ImpresionReciboMunicipal",
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
                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null){throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo));}
                var recorrido = repositorio.ObtenerRecorridoImpresionReciboMunicipal(workflowId);
                
                bool pagoRealizado = false;
                //string numPuestoDeTrabajo = string.Empty;
                //string numDeTicket = string.Empty; 
             
                var numPuestoDeTrabajo = repositorio.ObtenerNumGaritaEntrada(puestoDeTrabajoId).PadLeft(4, '0');
                var numDeTicket = (pagoRealizado == false) ? repositorio.ObtenerNumeroDeTicketGenerado(puestoDeTrabajoId, recorrido.PagoConMercadoPago).ToString(CultureInfo.InvariantCulture).PadLeft(7, '0') : "0";
                //var numDeTicket = repositorio.ObtenerNumeroDeTicketGenerado(puestoDeTrabajoId, recorrido.PagoConMercadoPago).ToString(CultureInfo.InvariantCulture).PadLeft(7, '0');
         
                numPuestoDeTrabajo = (recorrido.PagoConMercadoPago) ? numPuestoDeTrabajo : string.Concat("1", numPuestoDeTrabajo.Substring(1));
                numDeTicket = (recorrido.PagoConMercadoPago && pagoRealizado == false) ? string.Concat(numDeTicket, " MP") : numDeTicket;
                //numDeTicket = (recorrido.PagoConMercadoPago) ? string.Concat(numDeTicket, " MP") : numDeTicket
                    
                var dto = new ImpReciboMunicipalDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Codigo = codigo,
                    TicketNro = (pagoRealizado == true && recorrido.Material.CodigoSAP == "99319") ? "Tasa abonada dentro del día" : $"{numPuestoDeTrabajo}-{numDeTicket}",
                    Ordenanza = recorrido.Ordenanza,
                    Valor = (pagoRealizado == true && recorrido.Material.CodigoSAP == "99319") ? "0" : recorrido.Monto,
                    FechaImpresion = DateTime.Now,
                    WorkflowId = workflowId,
                    Patente = recorrido.Patente,
                    TipoVehiculo = recorrido.TipoVehiculo,
                    NroDocumentoLegal = recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.Remito ? recorrido.DocumentoInternoSap ?? "" : recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte ? recorrido.NumeroDocumentoIngreso ?? "" :recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenDeDescargaFason ? recorrido.NumeroDocumentoIngreso : ""
                };

                resultado = servicio.Ejecutar(new ImprimirReciboMunicipal { Dto = dto, CantidadCopias = cantCopias });
                
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad
                {
                    InstanceId = workflowId,
                    Actividad = "ImpresionReciboMunicipal",
                    PuestoDeTrabajoId = puestoDeTrabajoId
                });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
