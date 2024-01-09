using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCartaPorte : ProcesadorComando<CrearCartaPorte>
    {
        public ProcesadorCrearCartaPorte(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearCartaPorte comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorCrearCartaPorte para la instancia de workflow = {0}", comando.InstanciaWorkflowId);
                var workflow = Repositorio.Obtener<Workflow>(f => f.Codigo == comando.NombreWorkflow);

                if (workflow != null)
                {
                    var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
                    var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
                    var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
                    var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId ?? 0);
                    var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                    var titular = Repositorio.Obtener<Proveedor>(comando.Orden.TitularCartaPorteId);
                    var procedencia = Repositorio.Obtener<Localidad>(comando.Orden.ProcedenciaId);
                    var agente = Repositorio.Obtener<Proveedor>(comando.Orden.AgenteComprasId);
                    var prestador = Repositorio.Obtener<Proveedor>(comando.Orden.PrestadorId);
                    var bocaDestino = Repositorio.Obtener<BocaDestino>(comando.Orden.BocaDestinoId);
                    var intermediario = Repositorio.Obtener<Proveedor>(comando.Orden.IntermediarioId);
                    var rtteComercial = Repositorio.Obtener<Proveedor>(comando.Orden.RtteComercialId);
                    var corredor = Repositorio.Obtener<Proveedor>(comando.Orden.CorredorId);
                    var corredorVendedor = Repositorio.Obtener<Proveedor>(comando.Orden.CorredorVendedorId);
                    var intermediarioFlete = Repositorio.Obtener<Proveedor>(comando.Orden.IntermediarioFleteId);
                    var categoria = Repositorio.Obtener<Categoria>(comando.Orden.TipoCategoriaId);
                    var entregador = Repositorio.Obtener<Entregador>(comando.Orden.EntregadorId);
                    var workflowDefinicion = Repositorio.Obtener<WorkflowDefinicion>(comando.WorkflowDefinicionId);
                    var tecnologia = Repositorio.Obtener<Tecnologia>(comando.Orden.TecnologiaId ?? 0);
                    var rtteComercialVentaSecundaria = Repositorio.Obtener<Proveedor>(comando.Orden.RtteComercialVentaSecundarioId);
                    var rtteComercialProductor = Repositorio.Obtener<Proveedor>(comando.Orden.RtteComercialProductorId);
                    var corredorVendedorSecundario = Repositorio.Obtener<Proveedor>(comando.Orden.CorredorVendedorSecundarioId);
                    var rtteComercialVentaSecundaria2 = Repositorio.Obtener<Proveedor>(comando.Orden.RtteComercialVentaSecundario2Id);
                    var ramalFerroviario = Repositorio.Obtener<RamalFerroviario>(comando.Orden.CodigoRamalId);
                    var pagadorFlete = Repositorio.Obtener<Proveedor>(comando.Orden.PagadorFleteId ?? 0);
                    var representanteRecibidor = Repositorio.Obtener<Entregador>(comando.Orden.RepresentanteRecibidorId ?? 0);
                    var tipoVariedad = Repositorio.Obtener<TipoVariedad>(comando.TipoVariedadId ?? 0);

                    Centro destino = null;
                    Proveedor destinatario = null;
                    Cliente destinatarioCliente = null;
                    Cliente destinoCliente = null;
                    CartaPorte cartaPorteFerroviario = null;
                    Transportista transportistaTramo2 = null;
                    if (comando.Orden.EsClienteDestinatario)
                    {
                        destinatarioCliente = Repositorio.Obtener<Cliente>(comando.Orden.DestinatarioId);
                        destinoCliente = Repositorio.Obtener<Cliente>(comando.Orden.DestinoId);
                    }
                    else
                    {
                        destinatario = Repositorio.Obtener<Proveedor>(comando.Orden.DestinatarioId);
                        destino = Repositorio.Obtener<Centro>(comando.Orden.DestinoId);
                    }
                    CartaPorte cartaPorte;
                    if (comando.Orden.Cpe && comando.Vehiculo.TipoVehiculo == TipoVehiculo.Tren)
                    {
                        transportistaTramo2 = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaTramo2Id ?? 0);
                        var ctgFerroviario = string.IsNullOrEmpty(comando?.Vehiculo?.NumCTG) ? comando.Orden.NroCartaPorte : comando?.Vehiculo?.NumCTG;
                        cartaPorteFerroviario =
                        Repositorio.ObtenerMayor<Recorrido, DateTime, CartaPorte>(
                            x =>
                            x.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte && x.NumeroDocumentoIngreso == ctgFerroviario &&
                            x.Centro.Id != comando.Orden.DestinoId && x.Workflow.TipoDeWorkflow != workflow.TipoDeWorkflow && !x.Rechazado &&
                            x.Terminado, x => x.FechaInicio, x => x.Vehiculo.CartaPorte);
                    }
                    if (comando.Vehiculo.Primero || comando.Orden.Cpe && comando.Vehiculo.TipoVehiculo == TipoVehiculo.Tren)
                    {
                        var vehiculos = (from vehiculosObj in comando.Orden.Vehiculos select Conversor.Convertir<VehiculoDto, Vehiculo>(vehiculosObj)).ToList();

                        if (string.IsNullOrEmpty(comando.Orden.FotoRutaDestino))
                        {
                            comando.Orden.FotoRutaDestino = Repositorio.ObtenerProyeccion<CartaPorte, string>(x => x.NroCartaPorte == comando.Orden.NroCartaPorte && x.FotoRutaDestino != null, x => x.FotoRutaDestino);
                        }

                        cartaPorte = new CartaPorte
                        {
                            TipoVehiculo = comando.Orden.TipoVehiculo,
                            NroCartaPorte = comando.Orden.Cpe && comando.Vehiculo.TipoVehiculo == TipoVehiculo.Tren ? (string.IsNullOrEmpty(comando?.Vehiculo?.NumCTG) ? comando.Orden.NroCartaPorte : comando?.Vehiculo?.NumCTG) : comando.Orden.NroCartaPorte,
                            CTG = comando.Orden.Cpe && comando.Vehiculo.TipoVehiculo == TipoVehiculo.Tren ? comando?.Vehiculo?.NumOrden ?? string.Empty : comando.Orden.CTG,
                            FechaCP = comando.Orden.FechaCP,
                            TipoComercial = tipoComercial,
                            CEE = comando.Orden.CEE,
                            FechaEmision = comando.Orden.FechaEmision,
                            FechaVto = comando.Orden.FechaVto,
                            TitularCartaPorte = titular,
                            Destinatario = destinatario,
                            Transportista = transportista,
                            Chofer = chofer,
                            Cosecha = comando.Orden.Cosecha,
                            Procedencia = procedencia,
                            OrigenVehiculo = comando.Orden.OrigenVehiculo,
                            KmRecorrer = comando.Orden.KmRecorrer,
                            TarifaTonelada = comando.Orden.TarifaTonelada,
                            FleteAPagar = comando.Orden.FleteAPagar,
                            CentroDestino = destino,
                            Material = material,
                            AgenteCompras = agente,
                            Prestador = prestador,
                            BocaDestino = bocaDestino,
                            CodEstab = comando.Orden.CodEstab,
                            DestinatarioCliente = destinatarioCliente,
                            ClienteDestino = destinoCliente,
                            Variedad = comando.Orden.Variedad,
                            FletePagado = comando.Orden.FletePagado,
                            AcuerdoMarco = comando.Orden.AcuerdoMarco,
                            Caratula = comando.Orden.Caratula,
                            Intermediario = intermediario,
                            RtteComercial = rtteComercial,
                            Entregador = entregador,
                            Corredor = corredor,
                            Desvio = comando.Orden.Desvio,
                            TarifaReferencia = comando.Orden.TarifaReferencia,
                            CodigoAnexo = comando.Orden.CodigoAnexo,
                            Vehiculos = comando.Vehiculo.TipoVehiculo == TipoVehiculo.Tren && comando.Orden.Cpe ? vehiculos.Where(w => w.Patente == comando.Vehiculo.Patente).ToList() : vehiculos,
                            Tecnologia = tecnologia,
                            Cupo = comando.Orden.Cupo != null ? comando.Orden.Cupo.ToUpper() : comando.Orden.Cupo,
                            CorredorVendedor = corredorVendedor,
                            Categoria = categoria,
                            IntermediarioFlete = intermediarioFlete,
                            TrigoEspecial = comando.Orden.TrigoEspecial,
                            NumeroAduana = comando.Orden.NumeroAduana,
                            EsExtranjero = comando.Orden.EsExtranjero,
                            FotoRutaDestino = comando.Orden.FotoRutaDestino,
                            FotoRutaDestinoDetalle = comando.Orden.FotoRutaDestinoDetalle,
                            Cpe = comando.Orden.Cpe,
                            Sucursal = comando.Orden.Cpe && comando.Vehiculo.TipoVehiculo == TipoVehiculo.Tren ? Convert.ToInt32(comando?.Vehiculo?.Sucural) : comando.Orden.Sucursal,
                            RtteComercialVentaSecundaria = rtteComercialVentaSecundaria,
                            RtteComercialProductor = rtteComercialProductor,
                            CorredorVendedorSecundario = corredorVendedorSecundario,
                            RtteComercialVentaSecundaria2 = rtteComercialVentaSecundaria2,
                            Observacion = cartaPorteFerroviario is null ? comando?.Orden?.Observacion : cartaPorteFerroviario?.Observacion,
                            NumeroOperativo = comando.Orden.NumeroOperativo,
                            RamalFerroviario = ramalFerroviario,
                            NumeroPrecinto = cartaPorteFerroviario is null ? comando?.Orden?.NumeroPrecinto : cartaPorteFerroviario?.NumeroPrecinto,
                            TransportistaTramo2 = comando.Orden.Cpe && comando.Vehiculo.TipoVehiculo == TipoVehiculo.Tren ? transportistaTramo2 : null,
                            PagadorFlete = pagadorFlete,
                            RepresentanteRecibidor = representanteRecibidor,
                            FotoRutaSustentable = comando.Orden.FotoRutaSustentable,
                        };
                        cartaPorte.NroCartaPorte = cartaPorte.NroCartaPorte.Trim();
                        foreach (var vehiculo in vehiculos)
                        {
                            vehiculo.CartaPorte = cartaPorte;
                        }
                    }
                    else
                    {
                        cartaPorte = Repositorio.Obtener<CartaPorte>(comando.Orden.Id);
                    }
                    var recorrido = Repositorio.Obtener<Recorrido>(f => f.InstanciaWorkflow == comando.InstanciaWorkflowId);

                    if (recorrido == null)
                    {
                        recorrido = new Recorrido
                        {
                            InstanciaWorkflow = comando.InstanciaWorkflowId,
                            Usuario = comando.Usuario,
                            FechaInicio = DateTime.Now,
                            Workflow = workflow,
                            Chofer = chofer,
                            Centro = centro,
                            Patente = comando.Vehiculo.Patente,
                            Transportista = transportista,
                            TipoComercial = tipoComercial,
                            TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                            Material = material,
                            PesoBrutoOrigen = comando.Vehiculo.PesoBrutoOrigen,
                            PesoTaraOrigen = comando.Vehiculo.PesoTaraOrigen,
                            NumeroDocumentoIngreso = comando.Orden.Cpe && comando.Vehiculo.TipoVehiculo == TipoVehiculo.Tren ? (string.IsNullOrEmpty(comando?.Vehiculo?.NumCTG) ? comando.Orden.NroCartaPorte : comando?.Vehiculo?.NumCTG) : comando.Orden.NroCartaPorte.ToString(CultureInfo.InvariantCulture),
                            Vehiculo = cartaPorte.Vehiculos.FirstOrDefault(t => t.Patente == comando.Vehiculo.Patente),
                            WorkflowDefinicion = workflowDefinicion,
                            TipoVehiculo = comando.Orden.TipoVehiculo,
                            VehiculoDemorado = comando.Orden.VehiculoDemorado,
                            MotivoDemora = comando.Orden.MotivoDemora,
                            TipoVariedad = tipoVariedad,
                        };
                        Repositorio.Agregar(recorrido);
                    }
                    else
                    {
                        recorrido.Usuario = comando.Usuario;
                        recorrido.FechaInicio = DateTime.Now;
                        recorrido.Workflow = workflow;
                        recorrido.Chofer = chofer;
                        recorrido.Centro = centro;
                        recorrido.Patente = comando.Vehiculo.Patente;
                        recorrido.Transportista = transportista;
                        recorrido.TipoComercial = tipoComercial;
                        recorrido.TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte;
                        recorrido.Material = material;
                        recorrido.PesoBrutoOrigen = comando.Vehiculo.PesoBrutoOrigen;
                        recorrido.PesoTaraOrigen = comando.Vehiculo.PesoTaraOrigen;
                        recorrido.NumeroDocumentoIngreso = comando.Orden.Cpe && comando.Vehiculo.TipoVehiculo == TipoVehiculo.Tren ? (string.IsNullOrEmpty(comando?.Vehiculo?.NumCTG) ? comando.Orden.NroCartaPorte : comando?.Vehiculo?.NumCTG) : comando.Orden.NroCartaPorte.ToString(CultureInfo.InvariantCulture);
                        recorrido.Vehiculo = cartaPorte.Vehiculos.FirstOrDefault(t => t.Patente == comando.Vehiculo.Patente);
                        recorrido.WorkflowDefinicion = workflowDefinicion;
                        recorrido.VehiculoDemorado = comando.Orden.VehiculoDemorado;
                        recorrido.MotivoDemora = comando.Orden.MotivoDemora;
                        recorrido.TipoVariedad = tipoVariedad;
                    }

                    var vehiculoEntity = cartaPorte.Vehiculos.FirstOrDefault(t => t.Patente == comando.Vehiculo.Patente);
                    var categoriaVehiculo = Repositorio.Obtener<CategoriaVehiculo>(
                        f => f.Patente == vehiculoEntity.Patente
                        && (f.PatenteAcoplado == vehiculoEntity.PatenteAcoplado
                        || comando.Vehiculo.PatenteAcoplado == null)
                        && (f.PatenteAcoplado2 == comando.Vehiculo.PatenteAcoplado2
                        || vehiculoEntity.PatenteAcoplado2 == null)
                    );
                    if (categoriaVehiculo == null)
                    {
                        categoriaVehiculo = new CategoriaVehiculo
                        {
                            Id = -1,
                            Patente = vehiculoEntity.Patente,
                            PatenteAcoplado = vehiculoEntity.PatenteAcoplado,
                            PatenteAcoplado2 = vehiculoEntity.PatenteAcoplado2,
                            TipoVehiculo = (int)vehiculoEntity.TipoVehiculo
                        };
                        Repositorio.Agregar(categoriaVehiculo);
                    }

                    Log.Info("Se procederá a crear el recorrido para el workflow {0}", comando.NombreWorkflow);
                    Log.Info("Se creó exitosamente el recorrido para el workflow {0}", comando.NombreWorkflow);
                    if (comando.Vehiculo.Primero || comando.Orden.Cpe && comando.Vehiculo.TipoVehiculo == TipoVehiculo.Tren)
                    {
                        Repositorio.Agregar(cartaPorte);
                        Repositorio.GuardarCambios();
                        Log.Info("Se creó exitosamente la carta de porte para el workflow {0}", comando.NombreWorkflow);
                        resultado.Id = (int)cartaPorte.GetType().GetProperty("Id").GetValue(cartaPorte, null);
                    }
                    else
                    {
                        Repositorio.GuardarCambios();
                        resultado.Id = comando.Orden.Id != 0 ? comando.Orden.Id : cartaPorte.Id;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar crear la carta de porte para la instancia de workflow {0}", comando.InstanciaWorkflowId);
                resultado.Error("", Textos.CartaDePorte_Error);
            }
            return resultado;
        }
    }
}