using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;
using System;
using System.Activities;
using System.Linq;

namespace Molinos.Scato.Actividades
{
    public class CrearCartaPorteByPass : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<CartaPorteDto> Orden { get; set; }

        [RequiredArgument]
        public InArgument<Guid> InstanciaWorkflowId { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }

        [RequiredArgument]
        public InArgument<string> NombreWorkflow { get; set; }

        [RequiredArgument]
        public InArgument<int> WorkflowDefinicionId { get; set; }

        public OutArgument<CartaPorteDto> CartaPorte { get; set; }

        public OutArgument<string> NumeroCartaPorte { get; set; }

        public OutArgument<TipoDocumentoIngreso> TipoDocumentoIngreso { get; set; }

        public OutArgument<DateTime> FechaInicio { get; set; }

        public InArgument<string> Usuario { get; set; }

        public InArgument<VehiculoDto> Vehiculo { get; set; }
        public OutArgument<bool> VehiculoDemorado { get; set; }

        public OutArgument<int> PesoTara { get; set; }
        public OutArgument<int> PesoBruto { get; set; }


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
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var logger = context.GetExtension<ILogger>();

                var tipoMaterialPorVariedad = srvRepositorio.ObtenerTipoVariedadRecorridoAnterior(orden.NroCartaPorte, centroId);
                var tipoDeWorkFlow = srvRepositorio.ObtenerTipoDeWorkflowPorGuid(instanciaWorkflow);
                var centroDto = srvRepositorio.ObtenerCentro(orden.DestinoId);

                var planta = srvRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass, Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Planta);
                var localidad = srvRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass, Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Localidad);
                var provincia = srvRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass, Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Provincia);
                var centro = srvRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass, Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Centro);
                var tipocomercialSap = srvRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass, Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.TipoComercialEgreso);
                var proveedorMOA = srvRepositorio.ObtenerProveedorPorCuit(Constantes.Proveedores.CuitMolinos, new TiposProveedor { PR = true });
                var categoria = srvRepositorio.ObtenerCategoriaPorClasificacion(Constantes.ClasificacionCategorias.OPERADOR);

                orden.ProvinciaCodigoSap = orden.DestinoProvincia;
                orden.ProcedenciaCodigoSap = orden.DestinoLocalidadCodigoSap;
                orden.DestinoLocalidadCodigoAfip = Convert.ToInt32(localidad.Valor); 
                orden.DestinoProvinciaCodigoAfip = Convert.ToInt32(provincia.Valor); 
                orden.DestinoPlantaAfip = Convert.ToInt32(planta.Valor);
                orden.TipoComercialId = srvRepositorio.ObtenerTipoComercialPorCodigoSap(tipocomercialSap.Valor).Id ?? 0;
                orden.ProcedenciaId = centroDto.LocalidadId ?? 0;
                orden.DestinoId = Convert.ToInt32(centro.Valor); 
                logger.Info($"Creando carta de porte by pass para el centro {centro.Valor} con localidad {localidad.Valor} y provincia {provincia.Valor}");

                orden.TarifaReferencia = null;
                orden.FotoRutaDestino = null;
                orden.TitularCartaPorteId = proveedorMOA != null ? proveedorMOA.Id : 0;
                orden.RtteComercial = string.Empty;
                orden.RtteComercialCuit = string.Empty;
                orden.RtteComercialCodigoSap = string.Empty;
                orden.RtteComercialId = 0;
                orden.CorredorVendedor = string.Empty;
                orden.CorredorVendedorCuil = string.Empty;
                orden.CorredorVendedorCodigoSap = string.Empty;
                orden.CorredorVendedorId = 0;
                orden.CorredorVendedorSecundario = string.Empty;
                orden.CorredorVendedorSecundarioCuil = string.Empty;
                orden.CorredorVendedorSecundarioCodigoSap = string.Empty;
                orden.CorredorVendedorSecundarioId = 0;
                orden.CorredorId = 0;

                logger.Debug($"Datos para la carta de porte: TarifaReferencia: {orden.TarifaReferencia}, TitularCartaPorteId: {orden.TitularCartaPorteId}, RtteComercial: {orden.RtteComercial}, RtteComercialCuit: {orden.RtteComercialCuit}, RtteComercialCodigoSap: {orden.RtteComercialCodigoSap}, CorredorVendedor: {orden.CorredorVendedor}, CorredorVendedorCuil: {orden.CorredorVendedorCuil}, CorredorVendedorCodigoSap: {orden.CorredorVendedorCodigoSap}, CorredorVendedorSecundario: {orden.CorredorVendedorSecundario}, CorredorVendedorSecundarioCuil: {orden.CorredorVendedorSecundarioCuil}, CorredorVendedorSecundarioCodigoSap: {orden.CorredorVendedorSecundarioCodigoSap}");
                orden.PagadorFleteCuil = Constantes.ValoresPorDefecto.CuitMOA.ToString();
                orden.PagadorFlete = Constantes.ValoresPorDefecto.RazonSocialMOA.ToUpper();
                orden.PagadorFleteId = proveedorMOA != null ? proveedorMOA.Id : 0;
                orden.TipoCategoriaId = categoria.Id;
                orden.TipoCategoria = categoria.Clasificacion;
                orden.KmRecorrer = int.TryParse(orden.KmARecorrer, out var kmARecorrer) ? kmARecorrer : 0;
                orden.FechaCP = DateTime.Now;   
                logger.Debug($"Datos de la carta de porte: ProvinciaCodigoSap: {orden.ProvinciaCodigoSap}, ProcedenciaCodigoSap: {orden.ProcedenciaCodigoSap}, DestinoLocalidadCodigoAfip: {orden.DestinoLocalidadCodigoAfip}, DestinoProvinciaCodigoAfip: {orden.DestinoProvinciaCodigoAfip}, DestinoPlantaAfip: {orden.DestinoPlantaAfip}, TipoComercialId: {orden.TipoComercialId}, ProcedenciaId: {orden.ProcedenciaId}, DestinoId: {orden.DestinoId}");
                logger.Debug($"Km a recorrer: {orden.KmARecorrer}, Tarifa: {orden.TarifaTonelada}, Cupo: {orden.Cupo}");


                var resultadoCartaPorte = servicioComandos.Ejecutar(new Dominio.Comandos.CrearCartaPorteByPass
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

                var ordenDto = srvRepositorio.ObtenerCartaPorte(resultado.Id);
                var recorridoEgreso = srvRepositorio.ObtenerRecorridoPorNumeroDocumento(ordenDto.NroCartaPorte).Where(c => c.TipoComercial.Id == orden.TipoComercialId).First();

                if (ordenDto != null)
                {
                    ordenDto.VehiculoDemorado = orden.VehiculoDemorado;
                    CartaPorte.Set(context, ordenDto);
                    FechaInicio.Set(context, DateTime.Now);
                    NumeroCartaPorte.Set(context, ordenDto.NroCartaPorte);
                    TipoDocumentoIngreso.Set(context, Dominio.Enums.TipoDocumentoIngreso.CartaPorte);
                    VehiculoDemorado.Set(context, ordenDto.VehiculoDemorado);
                    PesoTara.Set(context, orden?.Vehiculos?.FirstOrDefault()?.PesoTaraOrigen);
                    PesoBruto.Set(context, orden?.Vehiculos?.FirstOrDefault()?.PesoBrutoOrigen);
                }

                if (resultadoCartaPorte.HayErrores)
                {
                    resultado.Errores.Add("ResultadoCartaPorte", string.Join(",", resultadoCartaPorte.HayErrores));
                }

                if (ordenDto.Id > 0)
                {
                   
                    var resultadoCaracteristicasAnalizadas = servicioComandos.Ejecutar(new Dominio.Comandos.CrearCaracteristicasAnalizadas
                    {
                        IdRecorridoIngreso = orden.CartaPorteByPass.RecorridoIdIngreso,
                        IdRecorridoEgreso = recorridoEgreso.Id
                    }) as ResultadoCrear;

                    if (resultadoCaracteristicasAnalizadas.HayErrores)
                    {
                        resultado.Errores.Add("ResultadoCaracteristicas", string.Join(",", resultadoCaracteristicasAnalizadas.HayErrores));
                    }
                }
                
            }
            catch (Exception)
            {
                resultado.Errores.Add("ErrorCrearCartaPorte", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }


    }
}