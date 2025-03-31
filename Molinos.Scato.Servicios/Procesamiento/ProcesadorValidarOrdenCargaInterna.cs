using System;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorValidarOrdenCargaInterna : ProcesadorComando<ValidarOrdenCargaInterna>
    {
        private readonly IServicioOperaciones servicioOperaciones;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicioRepositorio;

        public ProcesadorValidarOrdenCargaInterna(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOperaciones servicioOperaciones, IServicioComandos servicioComandos, IServicioRepositorio servicioRepositorio)
            : base(repositorio, conversor, log)
        {
            this.servicioOperaciones = servicioOperaciones;
            this.servicioComandos = servicioComandos;
            this.servicioRepositorio = servicioRepositorio;
        }

        public override Resultado Ejecutar(ValidarOrdenCargaInterna comando)
        {
            var resultado = new ResultadoValidarOrdenCargaInterna();

            try
            {
                var ordenOperaciones = ObtenerOrdenOperaciones(comando.Dto.PatenteCamion, comando.Dto.MaterialId);
                Validar(
                    comando.CentroId,
                    ordenOperaciones,
                    out Material material,
                    out Almacen almacen,
                    out Chofer chofer,
                    out Transportista transportista,
                    out Cliente clienteDestino,
                    out TipoComercial tipoComercial,
                    out TipoVehiculo tipoVehiculo,
                    out Cliente clientePagadorFlete);
                resultado.Dto = CrearOrdenCargaInternaDto(ordenOperaciones, 
                    material, 
                    almacen, 
                    chofer, 
                    transportista,
                    clienteDestino, 
                    tipoComercial, 
                    tipoVehiculo,
                    clientePagadorFlete);
                resultado.ControlRecorrido = GenerarControlRecorrido(comando.PuestoDeTrabajoId, comando.Usuario, comando.AplicaFastPass);
            }
            catch (OrdenCargaInternaException ex)
            {
                Log.Info(ex, "Error de Lógica al Validar Fast Pass");
                resultado.Error(ex.KeyError, ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error de Aplicación al Validar Fast Pass");
                resultado.Error("", ex.Message);
            }

            return resultado;
        }

        private OrdenResiduosDto ObtenerOrdenOperaciones(string patente, int materialId)
        {
            var ordenesOperaciones = servicioOperaciones.ObtenerOrdenesResiduos(patente);

            // Filtrar por el materialId (ya es el código directo en este caso)
            var ordenesFiltradas = ordenesOperaciones.Where(x => x.CodigoProducto == materialId).ToList();

            if (!ordenesFiltradas.Any())
                throw new Exception(($"No se encontraron Ordenes de Residuos en Operaciones con la patente {patente}"));

            // Verificar si hay múltiples clientes para este material
            var clientesDistintos = ordenesFiltradas.Select(o => o.CUITCliente).Distinct().ToList();
            if (clientesDistintos.Count > 1)
            {
                throw new Exception($"Existen múltiples órdenes de diferentes clientes para la patente: {patente}  y un mismo material.");
            }

            // Si hay un solo cliente, seleccionar la orden más antigua
            var ordenOperacionesMasAntigua = ordenesFiltradas.OrderBy(x => x.Id).FirstOrDefault();
            return ordenOperacionesMasAntigua;
        }

        private void Validar(
            int centroId,
            OrdenResiduosDto ordenOperaciones, 
            out Material material, 
            out Almacen almacen, 
            out Chofer chofer, 
            out Transportista transportista, 
            out Cliente clienteDestino, 
            out TipoComercial tipoComercial, 
            out TipoVehiculo tipoVehiculo, 
            out Cliente clientePagadorFlete)
        {
            clientePagadorFlete = null;
            
            material = Repositorio.Obtener<Material>(x => x.Id == ordenOperaciones.CodigoProducto);
            if (material == null)
                throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.MaterialId), string.Format(Textos.Error_Requerido, Textos.Material));

            var configMaterialExcepcionFastPass = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.CargaDeCupo && x.Nombre == Constantes.ConfiguracionGeneral.CargaDeCupo.MaterialExcepcionFastPass && x.CentroId == centroId);
            if (configMaterialExcepcionFastPass != null && !string.IsNullOrEmpty(configMaterialExcepcionFastPass.Valor))
            {
                var materialesIdConExcepcion = configMaterialExcepcionFastPass.Valor.Split(';');
                if (materialesIdConExcepcion.Contains(material.Id.ToString()))
                    throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.Almacen_Id), "El Material tiene excepcion al Fast Pass. Seleccionar el almacen manualmente.");
            }

            var idOrdenOperaciones = ordenOperaciones.Id.ToString();
            if (Repositorio.Existe<OrdenCargaInterna>(x => x.Id_operaciones == idOrdenOperaciones && (x.Recorrido.Rechazado == false || x.Recorrido.Terminado == false)))
                throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.Id_operaciones),Textos.IdOperacionesYaUtilizado);

            if (Repositorio.Existe<Recorrido>(x => !x.Terminado && x.Patente == ordenOperaciones.PatenteChasis))
                throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.PatenteCamion), Textos.PatenteEnOtroWorkflow);

            almacen = Repositorio.Obtener<Almacen>(x => x.Id == ordenOperaciones.AlmacenId);
            if (material.Descripcion == "RESIDUOS ORGANICOS" && almacen == null)
                throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.Almacen_Id), Textos.OrdenInterna_AlmacenRequerido);

            var cuitChofer = FormatterHelper.ConvertirCuilConGuiones(ordenOperaciones.CUILChofer);
            chofer = ObtenerChofer(cuitChofer, ordenOperaciones.ChoferApellido, ordenOperaciones.ChoferNombre);

            var cuitClienteDestino = FormatterHelper.ConvertirCuilConGuiones(ordenOperaciones.CUITCliente);
            clienteDestino = Repositorio.Obtener<Cliente>(x => x.Cuit == cuitClienteDestino);
            if (clienteDestino == null)
                throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.DestinoId), string.Format(Textos.Error_Requerido, Textos.Destino));
            
            tipoComercial = ObtenerTipoComercial(material.Id);

            var cuitTransportista = FormatterHelper.ConvertirCuilConGuiones(ordenOperaciones.CUITTransporte);
            transportista = ObtenerTransportista(tipoComercial, cuitTransportista);

            var comandoCNRT = GenerarComandoConsultarCNRT(ordenOperaciones.PatenteChasis, ordenOperaciones.PatenteAcoplado);
            var resultadoCategoriaCamion = servicioComandos.Ejecutar(comandoCNRT) as ResultadoEscalables;
            if (resultadoCategoriaCamion.HayErrores)
                throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.TipoVehiculo), resultadoCategoriaCamion.Errores.FirstOrDefault().Value);
            else if (!resultadoCategoriaCamion.Categoria.HasValue)
                throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.TipoVehiculo), "No se obtuvo el tipo de vehiculo en el CNRT");
            else 
                tipoVehiculo = resultadoCategoriaCamion.Categoria.Value;
            
            if (material.EsDerivadoGranario)
            {
                if (string.IsNullOrEmpty(ordenOperaciones.PlantaCodigo))
                    throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.PlantaDGDestino), string.Format(Textos.Error_Requerido, Textos.OrdenCarga_PlantaDGDestino));

                if (string.IsNullOrEmpty(ordenOperaciones.DomicilioTipo))
                    throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.TipoYOrdenDestino), string.Format(Textos.Error_Requerido, Textos.OrdenCarga_TipoYOrdenDestino));

                if (ordenOperaciones.DomicilioOrden <= 0)
                    throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.TipoYOrdenDestino), string.Format(Textos.Error_Requerido, Textos.OrdenCarga_TipoYOrdenDestino));

                if (string.IsNullOrEmpty(ordenOperaciones.KmARecorrer) || ordenOperaciones.KmARecorrer.Length > 4 || !int.TryParse(ordenOperaciones.KmARecorrer, out int km) || km <= 0)
                    throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.KmARecorrer), string.Format(Textos.Error_Requerido, Textos.CartaPorte_KmRecorrer));

                var cuitClientePagadorFlete = FormatterHelper.ConvertirCuilConGuiones(ordenOperaciones.PagadorFlete);
                clientePagadorFlete = Repositorio.Obtener<Cliente>(x => x.Cuit == cuitClientePagadorFlete);
                if (clientePagadorFlete == null)
                    throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.PagadorFleteId), Textos.OrdenCargaFAS_PagadorFleteInexistente);

                ValidarAltaCPEProvisoria(centroId, ordenOperaciones, tipoVehiculo, clienteDestino.Id, transportista.Id, clientePagadorFlete.Id);
            }

        }

        private Transportista ObtenerTransportista(TipoComercial tipoComercial, string cuitTransportista)
        {
            if (tipoComercial.TransportistaEsProveedor)
            {
                var proveedor = Repositorio.Obtener<Proveedor>(x => x.Cuil == cuitTransportista && x.Activo == true && x.PR == true && x.CM == false && x.AM == false);
                if (proveedor == null)
                    throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.TransportistaId), string.Format(Textos.Error_Requerido, Textos.Transportista));

                var transportista = Repositorio.Obtener<Transportista>(x => x.Cuit == cuitTransportista);
                if (transportista == null)
                {
                    var resultadoTransportista = servicioComandos.Ejecutar(new CrearTransportista
                    {
                        Dto = new TransportistaDto
                        {
                            Cuit = proveedor.Cuil,
                            Domicilio = proveedor.Domicilio,
                            LocalidadId = proveedor.Localidad.Id,
                            ProvinciaId = proveedor.Provincia.Id,
                            RazonSocial = proveedor.RazonSocial
                        }
                    }) as ResultadoCrear;
                    if (resultadoTransportista.HayErrores)
                        throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.TransportistaId), resultadoTransportista.Errores.Values.First());
                    
                    return Repositorio.Obtener<Transportista>(resultadoTransportista.Id);
                }

                return transportista;
            } else
            {
                var transportista = Repositorio.Obtener<Transportista>(x => x.Cuit == cuitTransportista);
                if (transportista == null)
                    throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.TransportistaId), string.Format(Textos.Error_Requerido, Textos.Transportista));

                return transportista;
            }
        }

        private TipoComercial ObtenerTipoComercial(int materialId)
        {
            var materialIdGoma = 64207;
            var materialIdRecicableNoPeligroso = 64207;
            var tipoComercialIdVentaNoFas = 6;
            var tipoComercialIdEgresoResiduosIndustriales = 2;
            if (materialId == materialIdGoma || materialId == materialIdRecicableNoPeligroso)
                return Repositorio.Obtener<TipoComercial>(x => x.Id == tipoComercialIdVentaNoFas);
            else
                return Repositorio.Obtener<TipoComercial>(x => x.Id == tipoComercialIdEgresoResiduosIndustriales);
        }

        private Chofer ObtenerChofer(string cuitChofer, string choferApellido, string nombreChofer)
        {
            var chofer = Repositorio.Obtener<Chofer>(x => x.Cuil == cuitChofer);
            if (chofer == null)
            {
                var documento = FormatterHelper.ObtenerDocumentoDesdeCuilConGuiones(cuitChofer);
                var resultadoChofer = servicioComandos.Ejecutar(new CrearChofer 
                { 
                    Dto = new ChoferDto
                    {
                        Nombre = nombreChofer,
                        Apellido = choferApellido,
                        Cuil = cuitChofer,
                        TipoDocumentoIdentidadId = 1, // DNI
                        NumeroDeDocumento = documento,
                    }
                }) as ResultadoCrear;
                if (resultadoChofer.HayErrores)
                    throw new OrdenCargaInternaException(nameof(OrdenCargaInternaDto.Chofer), resultadoChofer.Errores.Values.First());

                return Repositorio.Obtener<Chofer>(resultadoChofer.Id);
            }

            return chofer;
        }

        private Comando GenerarComandoConsultarCNRT(string patente, string acoplado)
        {
            var configCNRTDummy = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.IngresarOrdenCargaInternaFason && x.Nombre == Constantes.ConfiguracionGeneral.CNRT.CNRTDummy && x.CentroId == null);
            if (configCNRTDummy != null && bool.TryParse(configCNRTDummy.Valor, out bool configCNRTDummyActive) && configCNRTDummyActive)
            {
                return new ConsultarEscalablesDummy
                {
                    Patente = patente,
                    Acoplado = acoplado,
                };
            }
            else
            {
                return new ConsultarEscalables
                {
                    Patente = patente,
                    Acoplado = acoplado,
                };
            }
        }

        private void ValidarAltaCPEProvisoria(int centroId, OrdenResiduosDto ordenOperaciones, TipoVehiculo tipoVehiculo, int clienteDestinoId, int transportistaId, int clientePagadorFleteId)
        {
            servicioComandos.Ejecutar(new ConfirmarCTGVencidas
            {
                CentroId = centroId,
                TipoPerfil = TipoPerfil.Solicitante,
            });

            var resultadoAltaDummy = servicioComandos.Ejecutar(new AutorizarCpeDGDummy
            {
                TipoVehiculo = tipoVehiculo,
                CentroId = centroId,
                MaterialId = ordenOperaciones.CodigoProducto,
                DestinoId = clienteDestinoId,
                DestinoPlanta = int.Parse(ordenOperaciones.PlantaCodigo),
                DestinoDomicilioTipo = int.Parse(ordenOperaciones.DomicilioTipo),
                DestinoDomicilioOrden = ordenOperaciones.DomicilioOrden,
                TransportistaId = transportistaId,
                Dominios = new[] { ordenOperaciones.PatenteChasis, ordenOperaciones.PatenteAcoplado },
                KmRecorrer = int.Parse(ordenOperaciones.KmARecorrer),
                ChoferCuit = ordenOperaciones.CUILChofer,
                PagadorFleteId = clientePagadorFleteId,

            }) as ResultadoCartaPorteElectronicaDummy;
            if (resultadoAltaDummy.HayErrores)
                throw new OrdenCargaInternaException("ErrorAFIP", resultadoAltaDummy.Errores.Values.First());

            var resultadoAnulacionDummy = servicioComandos.Ejecutar(new AnularCPEDGDummy
            {
                CentroId = centroId,
                NroOrden = (int)resultadoAltaDummy.NroOrden,
                Sucursal = resultadoAltaDummy.Sucursal,
                TipoCPE = (short)resultadoAltaDummy.TipoCPE,
            });
            if (resultadoAnulacionDummy.HayErrores)
                throw new OrdenCargaInternaException("ErrorAFIP", resultadoAnulacionDummy.Errores.Values.First());
        }

        private OrdenCargaInternaDto CrearOrdenCargaInternaDto(OrdenResiduosDto ordenOperaciones, Material material, Almacen almacen, Chofer chofer, Transportista transportista, Cliente cliente, TipoComercial tipoComercial, TipoVehiculo tipoVehiculo, Cliente pagadorFlete)
        {
            var numeroOrden = servicioRepositorio.ObtenerNumeroDocumentoGenerado().ToString(CultureInfo.InvariantCulture).PadLeft(8, '0');
            var orden = new OrdenCargaInterna
            {
                Chofer = chofer,
                Destino = cliente,
                FechaEmision = DateTime.Now,
                Id_operaciones = ordenOperaciones.Id.ToString(),
                KmRecorrer = ordenOperaciones.KmARecorrer,
                LocalidadDestino = Repositorio.Obtener<Localidad>(x => x.Id == ordenOperaciones.LocalidadId),
                Material = material,
                NumeroOrden = numeroOrden,
                PatenteAcoplado = ordenOperaciones.PatenteAcoplado,
                PatenteCamion = ordenOperaciones.PatenteChasis,
                TipoComercial = tipoComercial,
                Transportista = transportista,
            };

            if (material.EsDerivadoGranario)
            {
                orden.DerivadoGranarioHabilitado = true;
                orden.PlantaDGDestino = int.Parse(ordenOperaciones.PlantaCodigo);
                orden.TipoDomicilioDestino = int.Parse(ordenOperaciones.DomicilioTipo);
                orden.OrdenDomicilioDestino = ordenOperaciones.DomicilioOrden;
                orden.PagadorFlete = pagadorFlete;
            }

            var ordenDto = Conversor.Convertir<OrdenCargaInterna, OrdenCargaInternaDto>(orden);
            ordenDto.TipoVehiculo = tipoVehiculo;
            ordenDto.Almacen_Id = almacen?.Id;
            ordenDto.Calle_Id = 3; // Se le asigna siempre la Calle 3
            ordenDto.TipoYOrdenDestino = ordenDto.TipoDomicilioDestino + "-" + ordenDto.OrdenDomicilioDestino;
            ordenDto.EsTransportista = !tipoComercial.TransportistaEsProveedor;

            return ordenDto;
        }

        private ControlRecorridoDto GenerarControlRecorrido(int puestoId, string usuario, bool esFastPass)
        {
            return new ControlRecorridoDto
            {
                Actividad = Textos.ActIngresarOrdenCargaInterna,
                ActividadXaml = "IngresarOrdenCargaInterna",
                PuestoDeTrabajoId = puestoId,
                NombreUsuario = usuario,
                Comentario = esFastPass ? "Avanzó por Fast Pass" : string.Empty,
            };
        }
    }

    public class OrdenCargaInternaException : Exception
    {
        public string KeyError { get; set; }
        public OrdenCargaInternaException(string key, string message) : base(message)
        {
            KeyError = key;
        }
    }
}
