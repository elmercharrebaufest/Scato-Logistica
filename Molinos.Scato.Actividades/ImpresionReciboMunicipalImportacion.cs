using System;
using System.Activities;
using System.Globalization;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionReciboMunicipalImportacion : CodeActivity<Resultado>
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
                    Actividad = "Impresion Recibo Municipal Importacion",
                    ActividadXaml = "ImpresionReciboMunicipalImportacion",
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
                if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }

                var recorrido = repositorio.ObtenerRecorridoImpresionReciboMunicipal(workflowId);
                var cartaPorte = repositorio.ObtenerCartaDePortePorrecorrido(recorrido.RecorridoId);
                if (cartaPorte.TitularCartaPorteCodigoSap == Constantes.ValoresPorDefecto.CodigoSapACA 
                    && cartaPorte.CodEstab == Constantes.ValoresPorDefecto.EstablecimientoACA)
                {
                    throw new Exception("No aplica impresión");
                }

                var dto = new ImpReciboMunicipalImportacionDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Codigo = codigo,
                    TicketNro = repositorio.ObtenerNumeroDeTicketImportacionGenerado().ToString(CultureInfo.InvariantCulture).PadLeft(7, '0'),
                    Ordenanza = recorrido.Ordenanza,
                    Valor = recorrido.Monto,
                    FechaImpresion = DateTime.Now,
                    WorkflowId = workflowId,
                    Patente = recorrido.Patente,
                    NroDocumentoLegal = recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.Remito ? recorrido.DocumentoInternoSap ?? "" : recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte ? recorrido.NumeroDocumentoIngreso : ""
                };
                resultado = servicio.Ejecutar(new ImprimirReciboMunicipalImportacion { Dto = dto, CantidadCopias = cantCopias });
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionReciboMunicipal", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
