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
    public class ImpresionEtiquetaAuditoriaCamara : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroCartaPorte { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        [RequiredArgument]
        public InArgument<string> NombreUsuario { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        public InArgument<int?> CantCopias { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }
        [RequiredArgument]
        public InArgument<string> Material { get; set; }
        [RequiredArgument]
        public InArgument<TipoVehiculo> TipoVehiculo { get; set; }
        

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);

            var codigo = CodigoDeImpresion.Get<string>(context);
            var numeroCartaPorte = NumeroCartaPorte.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var nombreUsuario = NombreUsuario.Get<string>(context);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            
            
            var material = Material.Get<string>(context);

            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresion Etiqueta Auditoria Camara",
                    ActividadXaml = "ImpresionEtiquetaAuditoria",
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
            var porcentaje = repositorio.PorcentajeMuestraAuditoria(workflowId);
            var random = repositorio.ObtenerNumeroAleatorio();
            if (random < porcentaje)
            {
                try
                {
                    var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                    if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }

                        var camara = repositorio.ObtenerCamaraPorMaterialPorCentro(workflowId);
                        var vehiculo = repositorio.ObtenerVehiculoPorGuid(workflowId);

                        if (camara != null && vehiculo != null)
                        {
                            var convCentro = repositorio.ObtenerConversionCentro(camara.Id, centroId);
                            var codigoDeCamara = convCentro != null ? convCentro.CodigoCamara : "";
                            numeroCartaPorte = camara.FormatoDeArchivo == CamaraFormatoDeArchivo.BahiaBlanca
                           ? numeroCartaPorte.Substring(numeroCartaPorte.Length - 10)
                           : (camara.FormatoDeArchivo == CamaraFormatoDeArchivo.Rosario ?
                            codigoDeCamara.Substring(0, codigoDeCamara.Length > 3 ? 3 : codigoDeCamara.Length) :
                            codigoDeCamara.Substring(0, codigoDeCamara.Length > 2 ? 2 : codigoDeCamara.Length)) +
                             vehiculo.NumeroVehiculo.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') +
                             numeroCartaPorte.Substring(numeroCartaPorte.Length - 10);
                        }
                        var dto = new ImpEtiquetaAuditoriaCamaraDto
                        {
                            Impresora = documento.ImpresoraDireccion ?? "",
                            Centro = documento.CentroDescripcion,
                            Codigo = codigo,
                            Patente = patente,
                            NumeroCartaPorte = numeroCartaPorte,
                            NombreUsuario = nombreUsuario,
                            WorkflowId = workflowId,

                            //LaboratiorioCuit = registro == null ? "" : registro.LaboratorioCuit,
                            //LaboratorioNombre = registro == null ? "" : registro.LaboratorioRazonSocial,
                            Material = material,
                            TipoDeAnalisis = vehiculo != null ? vehiculo.Patente : string.Empty
                        };

                        resultado = servicio.Ejecutar(new ImprimirEtiquetaAuditoriaCamara { Dto = dto, CantidadCopias = cantCopias });
                    
                }
                catch (Exception e)
                {
                    resultado.Errores.Add("1", e.Message);
                }
                try
                {
                    resultado = servicio.Ejecutar(new ModificarRecorridoMuestraAuditoriaCamara { InstanceId = workflowId });
                }
                catch (Exception)
                {
                    resultado.Errores.Add("2", Textos.Recorrido_ErrorAlTerminar);
                }
            }
            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionEtiquetaAuditoria", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }



            return resultado;
        }

       
    }
}
