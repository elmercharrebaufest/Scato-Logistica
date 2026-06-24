using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class CrearCartaPorteByPass : CrearCartaPorte
    {     
        protected override Resultado Execute(CodeActivityContext context)
        {
            var orden = Orden.Get<CartaPorteDto>(context);
            var vehiculo = Vehiculo.Get<VehiculoDto>(context);
            var nombreWorkflow = NombreWorkflow.Get<string>(context);
            var instanciaWorkflow = InstanciaWorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var usuario = Usuario.Get<string>(context);
            var workflowDefinicionId = WorkflowDefinicionId.Get<int>(context);
            var resultado = new ResultadoCrearWorkflow();
            resultado.InstanciaWorkflowId = instanciaWorkflow;

            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
                var log = context.GetExtension<ILogger>();
                var titularCartaPorteCodigoSap = servicioRepositorio.ObtenerProveedor(orden.TitularCartaPorteId)?.CodigoSap;
                var esEpa = orden.TipoVariedadCodigo == Constantes.TipoVariedadMaterial.EPA;
                var esEudr = orden.TipoVariedadCodigo == Constantes.TipoVariedadMaterial.EUDR;
                if (orden.TipoVariedadCodigo == Constantes.TipoVariedadMaterial.EPAyEUDR)
                {
                    esEpa = true;
                    esEudr = true;
                }

                var tipoMaterialPorVariedad = servicioRepositorio.ObtenerVariedadIdPorMaterial(orden.MaterialId, titularCartaPorteCodigoSap, orden.CodEstab, esEpa, esEudr, remitenteComercialCuit: orden.RtteComercialCuit, remitenteComercialVentaSecundariaCuit: orden.RtteComercialVentaSecundarioCuil);
                var resultadoCartaPorte = servicioComandos.Ejecutar(new Dominio.Comandos.CrearCartaPorte
                {
                    Orden = orden,
                    NombreWorkflow = nombreWorkflow,
                    InstanciaWorkflowId = instanciaWorkflow,
                    CentroId = centroId,
                    Usuario = usuario,
                    Vehiculo = vehiculo,
                    WorkflowDefinicionId = workflowDefinicionId,
                    TipoVariedadId = tipoMaterialPorVariedad,
                }) as ResultadoCrear;
                resultado.Id = resultadoCartaPorte.Id;

                var ordenDto = servicioRepositorio.ObtenerCartaPorte(resultado.Id);
                if (ordenDto != null)
                {
                    ordenDto.VehiculoDemorado = orden.VehiculoDemorado;
                    ordenDto.TipoVariedadCodigo = orden.TipoVariedadCodigo;
                    if(orden.CupoSalida != null)
                    {
                        log.Debug($"Tiene cupo de salida : {orden.CupoSalida}");
                        ordenDto.CupoSalida = orden.CupoSalida;
                    }
                    if(orden.KmRecorrerSalida != null)
                    {
                        log.Debug($"Tiene km de salida : {orden.KmRecorrerSalida}");
                        ordenDto.KmRecorrerSalida = orden.KmRecorrerSalida;
                    }
                       
                    if(orden.TarifaToneladaSalida != null)
                    {
                        log.Debug($"Tiene tarifa de salida {orden.TarifaToneladaSalida}");
                        ordenDto.TarifaToneladaSalida = orden.TarifaToneladaSalida;
                    }
                    
                    CartaPorte.Set(context, ordenDto);
                    FechaInicio.Set(context, DateTime.Now);
                    NumeroCartaPorte.Set(context, ordenDto.NroCartaPorte);
                    TipoDocumentoIngreso.Set(context, Dominio.Enums.TipoDocumentoIngreso.CartaPorte);
                    VehiculoDemorado.Set(context, ordenDto.VehiculoDemorado);
                }

                if (resultadoCartaPorte.HayErrores)
                {
                    resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
                }
            }
            catch (Exception)
            {
                resultado.Errores.Add("1", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}