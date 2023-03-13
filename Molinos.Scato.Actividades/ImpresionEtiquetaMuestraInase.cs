using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Globalization;

namespace Molinos.Scato.Actividades
{
    public class ImpresionEtiquetaMuestraInase : CodeActivity<Resultado>
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
            var cartaPorte = repositorio.ObtenerCartaPortePorInstanceId(workflowId);
            var proveedor = repositorio.ObtenerProveedorPorId(cartaPorte.TitularCartaPorteId);

            if(!proveedor.EnvioCamaraInase || cartaPorte.MaterialId != 4)
            {
                return resultado;
            }
            var logActividad = new LogActividadDto
            {
                Actividad = "Impresion Etiqueta Muestra INASE",
                ActividadXaml = "ImpresionEtiquetaMuestraInase",
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
                //LoggerHelper.WriteLine($"1. documento obtenido {documento}");
                if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }
                //LoggerHelper.WriteLine($"2. documento obtenido {documento.Id}");

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

                var dto = new ImpEtiquetaMuestraInaseDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Centro = documento.CentroDescripcion,
                    Codigo = codigo,
                    Patente = patente,
                    NumeroCartaPorte = cartaPorte.NroCartaPorte,
                    NombreUsuario = nombreUsuario,
                    WorkflowId = workflowId,
                    CuitProductor = proveedor.Cuil,
                    NroMuestra = numeroCartaPorte,
                    Material = material,
                    
                };

                resultado = servicio.Ejecutar(new ImprimirMuestraInase { Dto = dto, CantidadCopias = cantCopias });
                //LoggerHelper.WriteLine($"3. impresion correcta obtenido {dto.ToJson()}");
                //LoggerHelper.WriteLine($"4. impresion correcta carta porte {cartaPorte.ToJson()}");

            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
                //LoggerHelper.WriteLine($"4. error excepcion. {e.Message}");

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
