using System;
using System.Activities;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionCertificadoDeCartaPorte : CodeActivity<Resultado>
    {

        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroIngreso { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroDocumento { get; set; }
        [RequiredArgument]
        public InArgument<string> TipoDocumento { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoBruto { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoTara { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoNeto { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaEntrada { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        [RequiredArgument]
        public InArgument<string> Material { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        [RequiredArgument]
        public InArgument<int?> PesoNetoOrigen { get; set; }
        public InArgument<int?> CantCopias { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();
            var workflowId = WorkflowId.Get<Guid>(context);
            var material = Material.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var pesoNetoOrigen = PesoNetoOrigen.Get<int?>(context);
            var nroDocumento = NumeroDocumento.Get<string>(context);
            var centroId = CentroId.Get<int>(context);
            var numeroIngreso = NumeroIngreso.Get<string>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var tipoDocumento = TipoDocumento.Get<string>(context);
            var pesoBruto = PesoBruto.Get<int>(context);
            var pesoTara = PesoTara.Get<int>(context);
            var pesoNeto = PesoNeto.Get<int>(context);
            var fechaEntrada = FechaEntrada.Get<DateTime>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            var centro = repositorio.ObtenerCentro(centroId);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;

            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresion Certificacion De Carta Porte",
                    ActividadXaml = "ImpresionCertificadoDeCartaPorte",
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
                var recorridoId = repositorio.ObtenerRecorridoIdPorGuid(workflowId);
                var asignacion = repositorio.ObtenerAsignacionDePuestoComando(workflowId.ToString());
                var balanza = repositorio.ObtenerBalanza(asignacion.BalanzaBrutoId ?? 0);

                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }


                var memberInfo = typeof(TipoDocumentoIngreso).GetMember(tipoDocumento);
                if (memberInfo.Length > 0)
                {
                    var attrs = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                    if (attrs.Length > 0)
                    {
                        tipoDocumento = ((DisplayAttribute)attrs[0]).GetName();
                    }
                }

                var dto = new ImpCertificadoDeCartaPorteDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Codigo = codigo,
                    NumeroIngreso = numeroIngreso,
                    NumeroDocumento = nroDocumento,
                    Material = material,
                    PesoBruto = pesoBruto.ToString(CultureInfo.InvariantCulture),
                    PesoTara = pesoTara.ToString(CultureInfo.InvariantCulture),
                    PesoNeto = pesoNeto.ToString(CultureInfo.InvariantCulture),
                    TipoDocumento = tipoDocumento,
                    Centro = centro.Descripcion,
                    Patente = patente,
                    PesoNetoOrigen = pesoNetoOrigen.HasValue ? pesoNetoOrigen.Value.ToString(CultureInfo.InvariantCulture) : "",
                    NumeroCertificacion = centroId.ToString("D4") + "-" + recorridoId.ToString("D8"),
                    Diferencia = (pesoNeto - pesoNetoOrigen ?? 0).ToString(),
                    FechaEntrada = fechaEntrada,
                    FechaSalida = DateTime.Now,
                    Balanza = balanza != null ? balanza.Nombre : string.Empty,
                    WorkflowId = workflowId,
                };

                resultado = servicio.Ejecutar(new ImprimirCertificadoDeCartaPorte { Dto = dto, CantidadCopias = cantCopias});
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionCertificadoDeCartaPorte", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }
            return resultado;
        }
    }
}