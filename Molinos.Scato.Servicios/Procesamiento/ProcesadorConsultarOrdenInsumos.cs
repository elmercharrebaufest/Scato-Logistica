using System;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.Consultas;
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
    public class ProcesadorConsultarOrdenInsumos : ProcesadorComando<ConsultarOrdenInsumos>
    {
        private readonly IServicioOperaciones servicioOperaciones;
        private readonly IServicioComandos servicioComandos;

        public ProcesadorConsultarOrdenInsumos(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOperaciones servicioOperaciones, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.servicioOperaciones = servicioOperaciones;
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(ConsultarOrdenInsumos comando)
        {
            var resultado = new ResultadoConsultarOrdenInsumos();

            try
            {
                var ordenOperaciones = ObtenerOrdenOperaciones(comando.Patente, comando.MaterialId);
                Validar(
                    comando.CentroId,
                    resultado,
                    ordenOperaciones,
                    out Material material,
                    out Almacen almacen,
                    out Chofer chofer,
                    out Transportista transportista,
                    out Cliente clienteDestino,
                    out TipoComercial tipoComercial,
                    out TipoVehiculo tipoVehiculo,
                    out Cliente clientePagadorFlete);
                resultado.Dto = CrearOrdenCargaInternaDto(comando,
                    ordenOperaciones, 
                    material, 
                    almacen, 
                    chofer, 
                    transportista,
                    clienteDestino, 
                    tipoComercial, 
                    tipoVehiculo,
                    clientePagadorFlete);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al Consultar Orden Insumos Pendiente No Granos");
                resultado.Error("Error", ex.Message);
            }

            return resultado;
        }

        private OrdenResiduosDto ObtenerOrdenOperaciones(string patente, int materialId)
        {
            var ordenesOperaciones = servicioOperaciones.ObtenerOrdenesResiduos(patente).Where(x => x.CodigoProducto == materialId);
            if (!ordenesOperaciones.Any())
                throw new Exception($"No se encontraron Ordenes de Residuos en Operaciones con la patente {patente}");

            var ordenOperacionesMasAntigua = ordenesOperaciones.OrderBy(x => x.Id).FirstOrDefault();
            return ordenOperacionesMasAntigua;
        }

        private void Validar(
            int centroId,
            ResultadoConsultarOrdenInsumos resultado,
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
                resultado.Error(nameof(OrdenCargaInternaDto.MaterialId), string.Format(Textos.Error_Requerido, Textos.Material));

            var configMaterialExcepcionFastPass = Repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.CargaDeCupo && x.Nombre == Constantes.ConfiguracionGeneral.CargaDeCupo.MaterialExcepcionFastPass && x.CentroId == centroId);
            if (configMaterialExcepcionFastPass != null && !string.IsNullOrEmpty(configMaterialExcepcionFastPass.Valor))
            {
                var materialesIdConExcepcion = configMaterialExcepcionFastPass.Valor.Split(';');
                if (materialesIdConExcepcion.Contains(material.Id.ToString()))
                    resultado.Error(nameof(OrdenCargaInternaDto.Almacen_Id), "El Material tiene excepcion al Fast Pass. Seleccionar el almacen manualmente.");
            }

            var idOrdenOperaciones = ordenOperaciones.Id.ToString();
            if (Repositorio.Existe<OrdenCargaInterna>(x => x.Id_operaciones == idOrdenOperaciones && (x.Recorrido.Rechazado == false || x.Recorrido.Terminado == false)))
                resultado.Error(nameof(OrdenCargaInternaDto.Id_operaciones),Textos.IdOperacionesYaUtilizado);

            if (Repositorio.Existe<Recorrido>(x => !x.Terminado && x.Patente == ordenOperaciones.PatenteChasis))
                resultado.Error(nameof(OrdenCargaInternaDto.PatenteCamion), Textos.PatenteEnOtroWorkflow);

            almacen = Repositorio.Obtener<Almacen>(x => x.Id == ordenOperaciones.AlmacenId);
            if (material != null && material.Descripcion == "RESIDUOS ORGANICOS" && almacen == null)
                resultado.Error(nameof(OrdenCargaInternaDto.Almacen_Id), Textos.OrdenInterna_AlmacenRequerido);

            var cuitChofer = FormatterHelper.ConvertirCuilConGuiones(ordenOperaciones.CUILChofer);
            chofer = ObtenerChofer(resultado, cuitChofer, ordenOperaciones.ChoferApellido, ordenOperaciones.ChoferNombre);

            var cuitClienteDestino = FormatterHelper.ConvertirCuilConGuiones(ordenOperaciones.CUITCliente);
            clienteDestino = Repositorio.Obtener<Cliente>(x => x.Cuit == cuitClienteDestino);
            if (clienteDestino == null)
                resultado.Error(nameof(OrdenCargaInternaDto.DestinoId), string.Format(Textos.Error_Requerido, Textos.Destino));
            
            tipoComercial = ObtenerTipoComercial(material.Id);

            var cuitTransportista = FormatterHelper.ConvertirCuilConGuiones(ordenOperaciones.CUITTransporte);
            transportista = ObtenerTransportista(resultado, tipoComercial, cuitTransportista);

            var comandoCNRT = GenerarComandoConsultarCNRT(ordenOperaciones.PatenteChasis, ordenOperaciones.PatenteAcoplado);
            var resultadoCategoriaCamion = servicioComandos.Ejecutar(comandoCNRT) as ResultadoEscalables;
            if (resultadoCategoriaCamion.HayErrores)
            {
                resultado.Error(nameof(OrdenCargaInternaDto.TipoVehiculo), resultadoCategoriaCamion.Errores.FirstOrDefault().Value);
                tipoVehiculo = TipoVehiculo.Camión;
            }
            else if (!resultadoCategoriaCamion.Categoria.HasValue)
            {
                resultado.Error(nameof(OrdenCargaInternaDto.TipoVehiculo), Textos.CategoriaEscalable_Nula);
                tipoVehiculo = TipoVehiculo.Camión;
            }
            else 
                tipoVehiculo = resultadoCategoriaCamion.Categoria.Value;
            
            if (material.EsDerivadoGranario)
            {
                if (string.IsNullOrEmpty(ordenOperaciones.PlantaCodigo))
                    resultado.Error(nameof(OrdenCargaInternaDto.PlantaDGDestino), string.Format(Textos.Error_Requerido, Textos.OrdenCarga_PlantaDGDestino));

                if (string.IsNullOrEmpty(ordenOperaciones.DomicilioTipo))
                    resultado.Error(nameof(OrdenCargaInternaDto.TipoYOrdenDestino), string.Format(Textos.Error_Requerido, Textos.OrdenCarga_TipoYOrdenDestino));

                if (ordenOperaciones.DomicilioOrden <= 0)
                    resultado.Error(nameof(OrdenCargaInternaDto.TipoYOrdenDestino), string.Format(Textos.Error_Requerido, Textos.OrdenCarga_TipoYOrdenDestino));

                if (string.IsNullOrEmpty(ordenOperaciones.KmARecorrer))
                    resultado.Error(nameof(OrdenCargaInternaDto.KmARecorrer), string.Format(Textos.Error_Requerido, Textos.CartaPorte_KmRecorrer));

                var cuitClientePagadorFlete = FormatterHelper.ConvertirCuilConGuiones(ordenOperaciones.PagadorFlete);
                clientePagadorFlete = Repositorio.Obtener<Cliente>(x => x.Cuit == cuitClientePagadorFlete);
                if (clientePagadorFlete == null)
                    resultado.Error(nameof(OrdenCargaInternaDto.PagadorFleteId), Textos.OrdenCargaFAS_PagadorFleteInexistente);
            }
        }

        private Transportista ObtenerTransportista(ResultadoConsultarOrdenInsumos resultado, TipoComercial tipoComercial, string cuitTransportista)
        {
            if (tipoComercial.TransportistaEsProveedor)
            {
                var proveedor = Repositorio.Obtener<Proveedor>(x => x.Cuil == cuitTransportista && x.Activo == true && x.PR == true && x.CM == false && x.AM == false);
                if (proveedor == null)
                {
                    resultado.Error(nameof(OrdenCargaInternaDto.TransportistaId), string.Format(Textos.Error_Requerido, Textos.Transportista));
                    return null;
                }

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
                    {
                        resultado.Error(nameof(OrdenCargaInternaDto.TransportistaId), resultadoTransportista.Errores.Values.First());
                        return null;
                    }
                    
                    return Repositorio.Obtener<Transportista>(resultadoTransportista.Id);
                }

                return transportista;
            } else
            {
                var transportista = Repositorio.Obtener<Transportista>(x => x.Cuit == cuitTransportista);
                if (transportista == null)
                {
                    resultado.Error(nameof(OrdenCargaInternaDto.TransportistaId), string.Format(Textos.Error_Requerido, Textos.Transportista));
                    return null;
                }

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

        private Chofer ObtenerChofer(ResultadoConsultarOrdenInsumos resultado, string cuitChofer, string choferApellido, string nombreChofer)
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
                {
                    resultado.Error(nameof(OrdenCargaInternaDto.Chofer), resultadoChofer.Errores.Values.First());
                    return null;
                }

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

        private OrdenCargaInternaDto CrearOrdenCargaInternaDto(ConsultarOrdenInsumos comando, OrdenResiduosDto ordenOperaciones, Material material, Almacen almacen, Chofer chofer, Transportista transportista, Cliente cliente, TipoComercial tipoComercial, TipoVehiculo tipoVehiculo, Cliente pagadorFlete)
        {
            var orden = new OrdenCargaInterna
            {
                Chofer = chofer,
                Destino = cliente,
                FechaEmision = DateTime.Now,
                Id_operaciones = ordenOperaciones.Id.ToString(),
                KmRecorrer = ordenOperaciones.KmARecorrer,
                LocalidadDestino = Repositorio.Obtener<Localidad>(x => x.Id == ordenOperaciones.LocalidadId),
                Material = material,
                NumeroOrden = comando.NumeroOrden,
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
            ordenDto.EsTransportista = !tipoComercial.TransportistaEsProveedor;

            return ordenDto;
        }
    }
}
