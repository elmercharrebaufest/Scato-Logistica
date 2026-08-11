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
            ILogger logger = null;

            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                logger = context.GetExtension<ILogger>();

                if (servicioComandos == null || srvRepositorio == null)
                {
                    resultado.Errores.Add("ErrorCrearCartaPorte", Textos.Error_ActualizarGenerico);
                    return resultado;
                }

                var tipoMaterialPorVariedad = srvRepositorio.ObtenerTipoVariedadRecorridoAnterior(orden.NroCartaPorte, centroId);
                var centroDto = srvRepositorio.ObtenerCentro(orden.DestinoId);

                var planta = srvRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass, Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Planta);
                var localidad = srvRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass, Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Localidad);
                var provincia = srvRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass, Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Provincia);
                var centro = srvRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass, Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.Centro);
                var tipocomercialSap = srvRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.CrearCartaPorteByPass, Constantes.ConfiguracionGeneral.CrearCartaPorteByPass.TipoComercialEgreso);
                var proveedorMOA = srvRepositorio.ObtenerProveedorPorCuit(Constantes.Proveedores.CuitMolinos, new TiposProveedor { PR = true });
                var categoria = srvRepositorio.ObtenerCategoriaPorClasificacion(Constantes.ClasificacionCategorias.OPERADOR);
                var entregadorSinEntrega = srvRepositorio.BuscarEntregador("Sin Entrega") ?? srvRepositorio.BuscarEntregador("Sin Entregador");
                var tipoComercial = tipocomercialSap == null ? null : srvRepositorio.ObtenerTipoComercialPorCodigoSap(tipocomercialSap.Valor);

                int destinoLocalidadCodigoAfip;
                int destinoProvinciaCodigoAfip;
                int destinoPlantaAfip;
                int destinoCentroId;
                if (localidad == null ||
                    provincia == null ||
                    planta == null ||
                    centro == null ||
                    !int.TryParse(localidad.Valor, out destinoLocalidadCodigoAfip) ||
                    !int.TryParse(provincia.Valor, out destinoProvinciaCodigoAfip) ||
                    !int.TryParse(planta.Valor, out destinoPlantaAfip) ||
                    !int.TryParse(centro.Valor, out destinoCentroId))
                {
                    logger?.Error("No se pudo resolver la configuración obligatoria para CrearCartaPorteByPass en la instancia {0}", instanciaWorkflow);
                    resultado.Errores.Add("ErrorConfiguracionCrearCartaPorteByPass", Textos.Error_ActualizarGenerico);
                    return resultado;
                }

                if (tipoComercial == null || categoria == null || proveedorMOA == null || centroDto == null)
                {
                    logger?.Error("No se pudieron resolver entidades obligatorias para CrearCartaPorteByPass en la instancia {0}", instanciaWorkflow);
                    resultado.Errores.Add("ErrorDatosCrearCartaPorteByPass", Textos.Error_ActualizarGenerico);
                    return resultado;
                }

                orden.ProvinciaCodigoSap = orden.DestinoProvincia;
                orden.ProcedenciaCodigoSap = orden.DestinoLocalidadCodigoSap;
                orden.DestinoLocalidadCodigoAfip = destinoLocalidadCodigoAfip;
                orden.DestinoProvinciaCodigoAfip = destinoProvinciaCodigoAfip;
                orden.DestinoPlantaAfip = destinoPlantaAfip;
                orden.TipoComercialId = tipoComercial.Id ?? 0;
                orden.ProcedenciaId = centroDto.LocalidadId ?? 0;
                orden.DestinoId = destinoCentroId;
                orden.CodEstab = centroDto.CodigoEstablecimiento;
                logger.Info($"Creando carta de porte by pass para el centro {centro.Valor} con localidad {localidad.Valor} y provincia {provincia.Valor}");

                AplicarBlanqueoIntervinientesByPass(orden, proveedorMOA, entregadorSinEntrega);

                logger.Debug($"Datos para la carta de porte: TarifaReferencia: {orden.TarifaReferencia}, TitularCartaPorteId: {orden.TitularCartaPorteId}, RtteComercial: {orden.RtteComercial}, RtteComercialCuit: {orden.RtteComercialCuit}, RtteComercialCodigoSap: {orden.RtteComercialCodigoSap}, CorredorVendedor: {orden.CorredorVendedor}, CorredorVendedorCuil: {orden.CorredorVendedorCuil}, CorredorVendedorCodigoSap: {orden.CorredorVendedorCodigoSap}, CorredorVendedorSecundario: {orden.CorredorVendedorSecundario}, CorredorVendedorSecundarioCuil: {orden.CorredorVendedorSecundarioCuil}, CorredorVendedorSecundarioCodigoSap: {orden.CorredorVendedorSecundarioCodigoSap}");
                orden.PagadorFleteCuil = Constantes.ValoresPorDefecto.CuitMOA.ToString();
                orden.PagadorFlete = Constantes.ValoresPorDefecto.RazonSocialMOA.ToUpper();
                orden.PagadorFleteId = proveedorMOA.Id;
                orden.TipoCategoriaId = categoria.Id;
                orden.TipoCategoria = categoria.Clasificacion;
                orden.KmRecorrer = int.TryParse(orden.KmARecorrer, out var kmARecorrer) ? kmARecorrer : 0;
                orden.FechaCP = DateTime.Now;
                orden.Observacion = string.Empty;
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
                if (resultadoCartaPorte == null)
                {
                    resultado.Errores.Add("ResultadoCartaPorteNulo", Textos.Error_ActualizarGenerico);
                    return resultado;
                }

                resultado.Id = resultadoCartaPorte.Id;
                if (resultadoCartaPorte.HayErrores)
                {
                    var detalleErrores = resultadoCartaPorte.Errores == null || !resultadoCartaPorte.Errores.Any()
                        ? Textos.Error_ActualizarGenerico
                        : string.Join(",", resultadoCartaPorte.Errores.Values);
                    resultado.Errores.Add("ResultadoCartaPorte", detalleErrores);
                    return resultado;
                }

                var ordenDto = srvRepositorio.ObtenerCartaPorte(resultado.Id);
                if (ordenDto == null)
                {
                    resultado.Errores.Add("OrdenDtoNoEncontrada", Textos.Error_ActualizarGenerico);
                    return resultado;
                }

                var recorridoEgreso = srvRepositorio.ObtenerRecorridoPorNumeroDocumento(ordenDto.NroCartaPorte).FirstOrDefault(c => c.TipoComercial.Id == orden.TipoComercialId);
                if (recorridoEgreso == null)
                {
                    resultado.Errores.Add("RecorridoEgresoNoEncontrado", Textos.Error_ActualizarGenerico);
                    return resultado;
                }

                ordenDto.VehiculoDemorado = orden.VehiculoDemorado;
                CartaPorte.Set(context, ordenDto);
                FechaInicio.Set(context, DateTime.Now);
                NumeroCartaPorte.Set(context, ordenDto.NroCartaPorte);
                TipoDocumentoIngreso.Set(context, Dominio.Enums.TipoDocumentoIngreso.CartaPorte);
                VehiculoDemorado.Set(context, ordenDto.VehiculoDemorado);
                var vehiculoPrimario = orden?.Vehiculos?.FirstOrDefault();
                PesoTara.Set(context, vehiculoPrimario?.PesoTaraOrigen);
                PesoBruto.Set(context, vehiculoPrimario?.PesoBrutoOrigen);

                if (ordenDto.Id > 0)
                {
                    if (orden.CartaPorteByPass == null)
                    {
                        resultado.Errores.Add("CartaPorteByPassNula", Textos.Error_ActualizarGenerico);
                        return resultado;
                    }
                    
                    var resultadoCaracteristicasAnalizadas = servicioComandos.Ejecutar(new Dominio.Comandos.CrearCaracteristicasAnalizadas
                    {
                        IdRecorridoIngreso = orden.CartaPorteByPass.RecorridoIdIngreso,
                        IdRecorridoEgreso = recorridoEgreso.Id
                    }) as ResultadoCrear;

                    if (resultadoCaracteristicasAnalizadas == null)
                    {
                        resultado.Errores.Add("ResultadoCaracteristicasNulo", Textos.Error_ActualizarGenerico);
                        return resultado;
                    }

                    if (resultadoCaracteristicasAnalizadas.HayErrores)
                    {
                        var detalleErrores = resultadoCaracteristicasAnalizadas.Errores == null || !resultadoCaracteristicasAnalizadas.Errores.Any()
                            ? Textos.Error_ActualizarGenerico
                            : string.Join(",", resultadoCaracteristicasAnalizadas.Errores.Values);
                        resultado.Errores.Add("ResultadoCaracteristicas", detalleErrores);
                    }
                }
                
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error al crear carta de porte by pass. InstanciaWorkflowId: {0}", instanciaWorkflow);
                resultado.Errores.Add("ErrorCrearCartaPorte", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }

        private static void AplicarBlanqueoIntervinientesByPass(CartaPorteDto orden, ProveedorDto proveedorMOA, EntregadorDto entregadorSinEntrega)
        {
            orden.TarifaReferencia = null;
            orden.FotoRutaDestino = null;
            orden.TitularCartaPorteId = proveedorMOA.Id;

            orden.RtteComercialProductor = string.Empty;
            orden.RtteComercialProductorCuil = string.Empty;
            orden.RtteComercialProductorCodigoSap = string.Empty;
            orden.RtteComercialProductorId = 0;

            orden.RtteComercial = string.Empty;
            orden.RtteComercialCuit = string.Empty;
            orden.RtteComercialCodigoSap = string.Empty;
            orden.RtteComercialId = 0;

            orden.RtteComercialVentaSecundario = string.Empty;
            orden.RtteComercialVentaSecundarioCuil = string.Empty;
            orden.RtteComercialVentaSecundarioCodigoSap = string.Empty;
            orden.RtteComercialVentaSecundarioId = 0;

            orden.RtteComercialVentaSecundario2 = string.Empty;
            orden.RtteComercialVentaSecundario2Cuil = string.Empty;
            orden.RtteComercialVentaSecundario2CodigoSap = string.Empty;
            orden.RtteComercialVentaSecundario2Id = 0;

            orden.Corredor = string.Empty;
            orden.CorredorCuil = string.Empty;
            orden.CorredorCodigoSap = string.Empty;
            orden.CorredorId = 0;

            orden.CorredorVendedor = string.Empty;
            orden.CorredorVendedorCuil = string.Empty;
            orden.CorredorVendedorCodigoSap = string.Empty;
            orden.CorredorVendedorId = 0;

            orden.CorredorVendedorSecundario = string.Empty;
            orden.CorredorVendedorSecundarioCuil = string.Empty;
            orden.CorredorVendedorSecundarioCodigoSap = string.Empty;
            orden.CorredorVendedorSecundarioId = 0;

            orden.AgenteCompras = string.Empty;
            orden.AgenteComprasCuil = string.Empty;
            orden.AgenteComprasCodigoSap = string.Empty;
            orden.AgenteComprasId = 0;

            orden.RepresentanteRecibidor = string.Empty;
            orden.RepresentanteRecibidorCuil = string.Empty;
            orden.RepresentanteRecibidorId = null;

            orden.Entregador = "Sin Entrega";
            orden.EntregadorCuit = entregadorSinEntrega?.Cuil ?? string.Empty;
            orden.EntregadorId = entregadorSinEntrega?.Id ?? 0;

            orden.Destinatario = "MOLINOS AGRO S.A.";
            orden.DestinatarioCuil = Constantes.ValoresPorDefecto.CuitMOA.ToString();
            orden.DestinatarioCodigoSap = proveedorMOA.CodigoSap ?? string.Empty;
            orden.DestinatarioId = proveedorMOA.Id;

            orden.Intermediario = string.Empty;
            orden.IntermediarioId = null;
            orden.IntermediarioCodigoSap = string.Empty;
        }


    }
}