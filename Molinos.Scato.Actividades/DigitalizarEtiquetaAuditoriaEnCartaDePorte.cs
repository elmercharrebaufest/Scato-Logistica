using System;
using System.Activities;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class DigitalizarEtiquetaAuditoriaEnCartaDePorte : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();
            var workflowId = WorkflowId.Get<Guid>(context);


            var logActividad = new LogActividadDto
            {
                Actividad = "Digitalizar Etiqueta Auditoria En Carta De Porte",
                ActividadXaml = "DigitalizarEtiquetaAuditoriaEnCartaDePorte",
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
                var recorrido = repositorio.ObtenerRecorridoPorGuid(workflowId);
                var cp = repositorio.ObtenerCartaPortePorCentroYNumero(recorrido.NumeroDocumentoIngreso, recorrido.Centro.Id);

                if (!string.IsNullOrEmpty(cp.FotoRutaDestino))
                {
                    var balanzaBruto = recorrido.BalanzaBrutoId.HasValue ? repositorio.ObtenerBalanza(recorrido.BalanzaBrutoId.Value).Nombre : string.Empty;
                    var balanzaTara = recorrido.BalanzaTaraId.HasValue ? repositorio.ObtenerBalanza(recorrido.BalanzaTaraId.Value).Nombre : string.Empty;
                    var pesoNetoConDescuento = (decimal?)null;
                    var usuario = repositorio.ObtenerUsuarioMatricula(recorrido.AnalisisDeCalidad != null ? recorrido.AnalisisDeCalidad.Usuario : recorrido.Calado != null ? recorrido.Calado.Usuario : string.Empty);
                    var pesoBruto = recorrido.PesoBruto.HasValue ? recorrido.PesoBruto.Value.ToString(CultureInfo.CurrentCulture) : "";
                    var pesoTara = recorrido.PesoTara.HasValue ? recorrido.PesoTara.Value.ToString(CultureInfo.CurrentCulture) : "";
                    var pesoNeto = recorrido.PesoNeto.HasValue ? recorrido.PesoNeto.Value.ToString(CultureInfo.CurrentCulture) : "";
                    var dto = new ImpEtiquetaAuditoriaDto();
                    var esTren = false;


                    if (recorrido.TipoVehiculo == TipoVehiculo.Tren)
                    {
                        var vagones = repositorio.ObtenerPesoNetoTren(cp.Id);
                        if(vagones != null)
                        {                        
                            pesoBruto = vagones.PesoBruto.ToString(CultureInfo.CurrentCulture);
                            pesoTara = vagones.PesoTara.ToString(CultureInfo.CurrentCulture);
                            pesoNeto = vagones.PesoNeto.ToString(CultureInfo.CurrentCulture);
                            pesoNetoConDescuento = vagones.PesoDescuento;
                            esTren = true;
                        }
                    }
                    else
                    {
                        pesoNetoConDescuento = repositorio.ObtenerPesoNetoConDescuento(recorrido.InstanciaWorkflow);
                    }
                    if (recorrido.TipoVehiculo != TipoVehiculo.Tren || (recorrido.TipoVehiculo == TipoVehiculo.Tren && esTren))
                    {

                    
                    dto = new ImpEtiquetaAuditoriaDto
                    {
                        Centro = recorrido.Centro.Descripcion,
                        CentroDomicilio = recorrido.Centro.Direccion + " - " + recorrido.Centro.LocalidadDesc + " - " + recorrido.Centro.ProvinciaDesc,
                        PesoBruto = pesoBruto,
                        BalanzaBruto = balanzaBruto,
                        UsuarioBruto = recorrido.PesoBrutoUsuario,
                        PesoTara = pesoTara,
                        BalanzaTara = balanzaTara,
                        UsuarioTara = recorrido.PesoTaraUsuario,
                        EntregadorCuit = cp.EntregadorCuit,
                        EntregadorRazonSocial = cp.Entregador,
                        Codigo = "DigitalizarEtiquetaAuditoriaEnCartaDePorte",
                        FechaImpresion = DateTime.Today,
                        PesoNeto = pesoNeto,
                        PesoNetoConDescuento = pesoNetoConDescuento.HasValue ? Math.Round(pesoNetoConDescuento.Value).ToString(CultureInfo.CurrentCulture) : "",
                        NumeroCartaPorte = cp.NroCartaPorte.Substring(0, 4) + "-" + cp.NroCartaPorte.Substring(4, 8),
                        Patente = recorrido.Patente,
                        PatenteAcoplado = cp.Vehiculos.First().PatenteAcoplado,
                        FechaDescarga = recorrido.PesoTaraFecha,
                        WorkflowId = recorrido.InstanciaWorkflow,
                        RecibidorNombre = usuario != null ? usuario.Nombre : string.Empty,
                        RecibidorApellido = usuario != null ? usuario.Apellido : string.Empty,
                        RecibidorMatricula = usuario != null ? usuario.Matricula : string.Empty,
                        RecibidorFirma = usuario != null ? usuario.Firma : string.Empty,
                        FirmaImagen = usuario.FirmaImagen,
                        TipoVehiculo = recorrido.TipoVehiculo
                    };
                    }
                    servicio.Ejecutar(new EtiquetaAuditoriaEnCartaDePorte { Dto = dto, RutaFotoCartaDePorte = cp.FotoRutaDestino });
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "DigitalizarEtiquetaAuditoriaEnCartaDePorte", PuestoDeTrabajoId = 0 });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }



            return resultado;
        }


    }
}
