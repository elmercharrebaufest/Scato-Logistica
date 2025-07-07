using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
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
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarOrdenesNoGranosCargaDeCupo : ProcesadorComando<ConsultarOrdenesNoGranosCargaDeCupo>
    {
        private readonly IServicioOperaciones servicioOperaciones;
        private readonly IServicioComandos servicioComandos;
        private readonly ZSDWS_SCATO servicioSap;
        private readonly IServicioRepositorio servicioRepositorio;

        public ProcesadorConsultarOrdenesNoGranosCargaDeCupo(
            IRepositorio repositorio, 
            IConversor conversor, 
            ILogger log, 
            IServicioOperaciones servicioOperaciones, 
            IServicioComandos servicioComandos, 
            ZSDWS_SCATO servicioSap, 
            IServicioRepositorio servicioRepositorio)
            : base(repositorio, conversor, log)
        {
            this.servicioOperaciones = servicioOperaciones;
            this.servicioComandos = servicioComandos;
            this.servicioSap = servicioSap;            
            this.servicioRepositorio = servicioRepositorio;
        }

        public override Resultado Ejecutar(ConsultarOrdenesNoGranosCargaDeCupo comando)
        {
            var resultado = new ResultadoConsultarOrdenesNoGranosCargaDeCupo();
            var comparador = new OrdenNoGranosCargaDeCupoComparer();

            try
            {
                var ordenesFason = servicioOperaciones.ObtenerOrdenesDeCarga(comando.Patente);
                var ordenesInsumos = servicioOperaciones.ObtenerOrdenesResiduos(comando.Patente);
                var centro = Repositorio.Obtener<Centro>(c=> c.Id == comando.CentroId);
                var orden = new OrdenDeCargaSapDto(centro.CodigoSAP, comando.Patente, Constantes.WorkFlow.workflowVentaFas);
                var ordenesFas = servicioComandos.Ejecutar(new ConsultarOrdenCargaFas { Orden = orden }) as ResultadoConsultaOrdenCargaFas;

                var ordenesCargaDeCupo = ProcesarOrdenes(ordenesFason, ordenesInsumos, ordenesFas.Orden);
                resultado.Ordenes.AddRange(ordenesCargaDeCupo.Distinct(comparador));
            }
            catch (WebException ex)
            {
                Log.Error(ex, "Error con Endpoint MOA Operaciones");
                resultado.Error("Error", "Falló la comunicación con la API Operaciones");
            }
            catch (ClienteDuplicadoException ex)
            {
                resultado.Error("ClienteDuplicado", ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al Consultar Ordenes Operaciones en Carga de Cupo");
                resultado.Error("Error", ex.Message);
            }

            try
            {
                if (!resultado.Ordenes.Any())
                {
                    var ordenesCargaDeCupo = ConsultarOrdenesDeCargaDesdeSap(comando);
                    resultado.Ordenes.AddRange(ordenesCargaDeCupo.Distinct(comparador));
                }
            }
            catch (EndpointNotFoundException ex)
            {
                Log.Error(ex, "Error con Endpoint SAP");
                resultado.Error("Error", "Falló la comunicación con el servicio SAP");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al Consultar Ordenes SAP en Carga de Cupo");
                resultado.Error("Error", ex.Message);
            }

            return resultado;
        }

        private IEnumerable<OrdenNoGranosCargaDeCupoDto> ProcesarOrdenes(
            IEnumerable<OrdenDeCargaDto> ordenesFason,
            IEnumerable<OrdenResiduosDto> ordenesInsumos,
            IEnumerable<OrdenCargaFasDto> ordenesFas)
        {
            var ordenesCargaDeCupo = new List<OrdenNoGranosCargaDeCupoDto>();

            foreach (var orden in ordenesFason)
            {
                if (OrdenUtilizadaFason(orden.Id.ToString()))
                    continue;

                var clienteCuit = FormatterHelper.ConvertirCuilConGuiones(orden.CUITCliente);
                if (Repositorio.Contar<Cliente>(x => x.Cuit.Equals(clienteCuit) && x.Activo) > 1)
                    throw new ClienteDuplicadoException("Existe más de una entidad SAP para el CUIT ingresado. No se puede continuar con la carga.");

                var materialId = int.Parse(orden.CodigoProducto);
                var ordenCarga = GenerarOrdenCarga(materialId, orden.DescripcionProducto, orden.FleteMOA ? TipoOrdenCargaNoGranos.FasonConFlete : TipoOrdenCargaNoGranos.FasonSinFlete, orden.PatenteAcoplado);
                ordenesCargaDeCupo.Add(ordenCarga);
            }

            foreach (var orden in ordenesInsumos)
            {
                if (OrdenUtilizada(orden.Id.ToString()))
                    continue;

                var ordenCarga = GenerarOrdenCarga(orden.CodigoProducto, orden.DescripcionProducto, TipoOrdenCargaNoGranos.Insumos, orden.PatenteAcoplado);
                ordenesCargaDeCupo.Add(ordenCarga);
            }

            if(ordenesFas != null)
            {
                foreach (var orden in ordenesFas)
                {
                    if (OrdenUtilizadaFas(orden.Id))
                        continue;
                    var materialId = orden.MaterialId;
                    var ordenCarga = GenerarOrdenCarga(materialId, orden.MaterialDesc, TipoOrdenCargaNoGranos.Fas, orden.PatenteAcoplado);
                    ordenesCargaDeCupo.Add(ordenCarga);
                }
            }

            return ordenesCargaDeCupo;
        }

        private bool OrdenUtilizadaFason(string ordenOperacionesId)
        {
            return Repositorio.Existe<OrdenCargaInternaFason>(x => x.NumeroOrdenExterno == ordenOperacionesId && (x.Recorrido.Rechazado == false || x.Recorrido.Terminado == false));
        }

        private bool OrdenUtilizada(string ordenOperacionesId)
        {
            return Repositorio.Existe<OrdenCargaInterna>(x => x.Id_operaciones == ordenOperacionesId && (x.Recorrido.Rechazado == false || x.Recorrido.Terminado == false));
        }

        private bool OrdenUtilizadaFas(int ordenId)
        {
            return Repositorio.Existe<OrdenCargaFas>(x => x.Id == ordenId && (x.Recorrido.Rechazado == false || x.Recorrido.Terminado == false));
        }

        private OrdenNoGranosCargaDeCupoDto GenerarOrdenCarga(int materialId, string materialDescripcion, TipoOrdenCargaNoGranos tipoOrden, string patenteAcoplado)
        {
            if ((tipoOrden == TipoOrdenCargaNoGranos.FasonConFlete || tipoOrden == TipoOrdenCargaNoGranos.FasonSinFlete))
            {
                var materialCodigoSap = materialId.ToString();
                var material = Repositorio.Obtener<Material>(x => x.CodigoSAP == materialCodigoSap);
                if (material == null)
                    throw new Exception($"No se encontró el material {materialDescripcion} de la Orden {tipoOrden}");
                else
                    materialId = material.Id;
            } 
            else
            {
                if (!Repositorio.Existe<Material>(x => x.Id == materialId))
                    throw new Exception($"No se encontró el material {materialDescripcion} de la Orden {tipoOrden}");
            }

            var ordenCarga = new OrdenNoGranosCargaDeCupoDto
            {
                MaterialId = materialId,
                MaterialDescripcion = materialDescripcion,
                TipoOrden = tipoOrden,
                PatenteAcoplado = patenteAcoplado
            };

            return ordenCarga;
        }

        private IEnumerable<OrdenNoGranosCargaDeCupoDto> ConsultarOrdenesDeCargaDesdeSap(ConsultarOrdenesNoGranosCargaDeCupo comando)
        {
            var ordenesCargaDeCupo = new List<OrdenNoGranosCargaDeCupoDto>();

            var centroCodigoSap = Repositorio.ObtenerProyeccion<Centro, string>(x => x.Id == comando.CentroId, x => x.CodigoSAP);
            var consultaOrdenDeCarga = new ConsultaOrdenDeCarga
            {
                Centro = centroCodigoSap,
                Patente = comando.Patente
            };
            var request = new ConsultaOrdenDeCargaRequest
            {
                ConsultaOrdenDeCarga = consultaOrdenDeCarga
            };

            GenerarServicioSap(out ZSDWS_SCATO servicio);
            var respuestaSap = servicio.ConsultaOrdenDeCarga(request);

            if (respuestaSap.ConsultaOrdenDeCargaResponse.Salida.Any())
            {
                var ordenCargaFas = respuestaSap.ConsultaOrdenDeCargaResponse.Salida;

                foreach (var orden in ordenCargaFas)
                {
                    var materialCodigoSAP = orden.MATNR.TrimStart(new[] { '0' });
                    var material = Repositorio.Obtener<Material>(x => x.CodigoSAP == materialCodigoSAP);
                    if (material == null)
                        throw new Exception(string.Format(Textos.OrdenCargaFAS_MaterialInexistente, orden.MATNR));

                    var ordenCarga = new OrdenNoGranosCargaDeCupoDto
                    {
                        MaterialId = material.Id,
                        MaterialDescripcion = material.Descripcion,
                        TipoOrden = TipoOrdenCargaNoGranos.Ninguno,
                        PatenteAcoplado = orden.ACOPL.Trim()
                    };
                    ordenesCargaDeCupo.Add(ordenCarga);
                }
            }

            return ordenesCargaDeCupo;
        }

        private void GenerarServicioSap(out ZSDWS_SCATO servicio)
        {
            Log.Info("Generando servicio SAP");
            this.servicioRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.ServicioSap, Constantes.ConfiguracionGeneral.Servicios.SapDummy);
            var confiSapDummy = servicioRepositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.ServicioSap, Constantes.ConfiguracionGeneral.Servicios.SapDummy);
            bool usarMock = bool.Parse(confiSapDummy.Valor);
            if (usarMock)
            {
                servicio = new ServicioSapMock(Repositorio, Conversor, Log); // Usa el mock en lugar del servicio real
                Log.Info("Usando servicio mock");
            }
            else
            {
                servicio = servicioSap; // Usa el servicio real
            }
        }
    }

    public class ClienteDuplicadoException : Exception
    {
        public ClienteDuplicadoException(string message) : base(message)
        {
        }
    }
}
