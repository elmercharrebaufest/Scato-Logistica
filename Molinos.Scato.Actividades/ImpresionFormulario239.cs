using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionFormulario239 : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }
        [RequiredArgument]
        public InArgument<string> EmpresaDescripcion { get; set; }
        [RequiredArgument]
        public InArgument<string> EmpresaCuit { get; set; }
        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroCartaPorte { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroDeOrden { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroDeFormulario { get; set; }

        [RequiredArgument]
        public InArgument<string> Observaciones { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        public InArgument<string> PatenteAcoplado { get; set; }
        [RequiredArgument]
        public InArgument<string> Representante { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        public InArgument<int?> CantCopias { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var materialId = MaterialId.Get<int>(context);
            var empresaDescripcion = EmpresaDescripcion.Get<string>(context);
            var empresaCuit = EmpresaCuit.Get<string>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var numeroCartaPorte = NumeroCartaPorte.Get<string>(context);
            var numeroDeOrden = NumeroDeOrden.Get<string>(context);
            var observaciones = Observaciones.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var patenteAcoplado = PatenteAcoplado.Get<string>(context);
            var representante = Representante.Get<string>(context);
            var numeroDeFormulario = NumeroDeFormulario.Get<string>(context);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresion Formulario 239",
                    ActividadXaml = "ImpresionFormulario239",
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
                var datos = repositorio.ObtenerRecorridoMaterialCentroImpresionFormulario239(workflowId);

                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }

                var dto = new ImpFormulario239Dto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Centro = datos.CentroDescripcion,
                    CentroDireccion = datos.CentroDireccion,
                    CentroLocalidad = datos.CentroLocalidadDesc,
                    CentroProvincia = datos.CentroProvinciaDesc,
                    Empresa = empresaDescripcion ?? "",
                    EmpresaCuit = empresaCuit ?? "",
                    Fecha = DateTime.Now,
                    Codigo = codigo,
                    Material = datos.MaterialDescripcion,
                    MaterialCodigoSap = datos.MaterialCodigoSAP,
                    NumeroDocumentoEntrada = numeroCartaPorte,
                    NumeroDeOrden = numeroDeOrden,
                    NumeroDeFormulario = numeroDeFormulario,
                    Observaciones = observaciones,
                    Patente = patente,
                    PatenteAcoplado = patenteAcoplado ?? "",
                    Representante = representante,
                    TipoDocumentoEntrada = datos.TipoDocumentoIngreso,
                    WorkflowId = workflowId,
                    CTG = datos.CTG
                };

                resultado = servicio.Ejecutar(new ImprimirFormulario239 { Dto = dto, CantidadCopias = cantCopias });
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionFormulario239", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
