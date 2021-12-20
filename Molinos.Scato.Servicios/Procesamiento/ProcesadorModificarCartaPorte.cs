using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCartaPorte : ProcesadorModificar<ModificarCartaPorte>
    {
        public ProcesadorModificarCartaPorte(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCartaPorte comando)
        {
            var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
            var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
            var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
            var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId ?? 0);
            var titular = Repositorio.Obtener<Proveedor>(comando.Orden.TitularCartaPorteId);
            var agente = Repositorio.Obtener<Proveedor>(comando.Orden.AgenteComprasId);
            var intermediario = Repositorio.Obtener<Proveedor>(comando.Orden.IntermediarioId);
            var rtteComercial = Repositorio.Obtener<Proveedor>(comando.Orden.RtteComercialId);
            var corredor = Repositorio.Obtener<Proveedor>(comando.Orden.CorredorId);
            var entregador = Repositorio.Obtener<Entregador>(comando.Orden.EntregadorId);

            var corredorVendedor = Repositorio.Obtener<Proveedor>(comando.Orden.CorredorVendedorId);
            var intermediarioFlete = Repositorio.Obtener<Proveedor>(comando.Orden.IntermediarioFleteId);
            var categoria = Repositorio.Obtener<Categoria>(comando.Orden.TipoCategoriaId);

            var procedencia = Repositorio.Obtener<Localidad>(comando.Orden.ProcedenciaId);

            Centro centroDestino = null;
            Proveedor destinatario = null;
            Cliente destinatarioCliente = null;
            Cliente destinoCliente = null;
            if (comando.Orden.EsClienteDestinatario)
            {
                destinatarioCliente = Repositorio.Obtener<Cliente>(comando.Orden.DestinatarioId);
                destinoCliente = Repositorio.Obtener<Cliente>(comando.Orden.DestinoId);
            }
            else
            {
                destinatario = Repositorio.Obtener<Proveedor>(comando.Orden.DestinatarioId);
                centroDestino = Repositorio.Obtener<Centro>(comando.Orden.DestinoId);
            }
            var listaCampos = new List<LogModificacionDocumentoIngresoCampo>();
            var logModificacionDocumento =
                Repositorio.Obtener<LogModificacionDocumentoIngreso>(
                    q =>
                    q.Numero == comando.Orden.NroCartaPorte && q.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte);
            if (logModificacionDocumento == null)
            {
                logModificacionDocumento = new LogModificacionDocumentoIngreso
                {
                    NombreUsuarioUltimaModificacion = comando.NombreUsuario,
                    FechaUltimaModificacion = DateTime.Now,
                    Numero = comando.Orden.NroCartaPorte,
                    TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte
                };
                Repositorio.Agregar(logModificacionDocumento);
            }
            var cartaPorte = Repositorio.Obtener<CartaPorte>(comando.Orden.Id);

            if (comando.Orden.EsClienteDestinatario && cartaPorte.DestinatarioCliente != destinatarioCliente)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Destinatario",
                    ValorOriginal = cartaPorte.DestinatarioCliente != null ? cartaPorte.DestinatarioCliente.Descripcion : null,
                    ValorNuevo = destinatarioCliente != null ? destinatarioCliente.Descripcion : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.DestinatarioCliente = destinatarioCliente;
            }

            if (cartaPorte.FechaCP != comando.Orden.FechaCP)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "FechaCP",
                    ValorOriginal = cartaPorte.FechaCP.ToString("dd/MM/yyyy"),
                    ValorNuevo = comando.Orden.FechaCP.ToString("dd/MM/yyyy"),
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.FechaCP = comando.Orden.FechaCP;
            }

            if (cartaPorte.FechaVto != comando.Orden.FechaVto)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "FechaCP",
                    ValorOriginal = cartaPorte.FechaVto.ToString("dd/MM/yyyy"),
                    ValorNuevo = comando.Orden.FechaVto.ToString("dd/MM/yyyy"),
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.FechaVto = comando.Orden.FechaVto;
            }

            if (!comando.Orden.EsClienteDestinatario && cartaPorte.Destinatario != destinatario)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Destinatario",
                    ValorOriginal = cartaPorte.Destinatario != null ? cartaPorte.Destinatario.Descripcion : null,
                    ValorNuevo = destinatario != null ? destinatario.RazonSocial : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.Destinatario = destinatario;
            }
            if (comando.Orden.EsClienteDestinatario && cartaPorte.ClienteDestino != destinoCliente)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Destino",
                    ValorOriginal = cartaPorte.ClienteDestino != null ? cartaPorte.ClienteDestino.Descripcion : null,
                    ValorNuevo = destinoCliente != null ? destinoCliente.Descripcion : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.ClienteDestino = destinoCliente;
            }
            if (!comando.Orden.EsClienteDestinatario && cartaPorte.CentroDestino != centroDestino)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Destino",
                    ValorOriginal = cartaPorte.CentroDestino != null ? cartaPorte.CentroDestino.Descripcion : null,
                    ValorNuevo = centroDestino != null ? centroDestino.Descripcion : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.CentroDestino = centroDestino;
            }
            if (cartaPorte.TipoComercial != tipoComercial)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                    {
                        Nombre = "Tipo Comercial",
                        ValorOriginal = cartaPorte.TipoComercial != null ? cartaPorte.TipoComercial.Descripcion : null,
                        ValorNuevo = tipoComercial != null ? tipoComercial.Descripcion : null,
                        LogModificacionDocumentoIngreso = logModificacionDocumento,
                        Fecha = DateTime.Now,
                        NombreUsuario = comando.NombreUsuario
                    });
                cartaPorte.TipoComercial = tipoComercial;
            }
            if (cartaPorte.TitularCartaPorte != titular)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Titular Carta de Porte",
                    ValorOriginal = cartaPorte.TitularCartaPorte != null ? cartaPorte.TitularCartaPorte.RazonSocial : null,
                    ValorNuevo = titular != null ? titular.RazonSocial : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.TitularCartaPorte = titular;
            }
            if (cartaPorte.Procedencia != procedencia)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Procedencia",
                    ValorOriginal = cartaPorte.Procedencia != null ? cartaPorte.Procedencia.Descripcion : null,
                    ValorNuevo = procedencia != null ? procedencia.Descripcion : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.Procedencia = procedencia;
            }
            if (cartaPorte.Transportista != transportista)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Transportista",
                    ValorOriginal = cartaPorte.Transportista != null ? cartaPorte.Transportista.RazonSocial : null,
                    ValorNuevo = transportista != null ? transportista.RazonSocial : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.Transportista = transportista;
            }
            if (cartaPorte.Chofer != chofer)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Chofer",
                    ValorOriginal = cartaPorte.Chofer.Nombre + " " + cartaPorte.Chofer.Apellido,
                    ValorNuevo = chofer.Nombre + " " + chofer.Apellido,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.Chofer = chofer;
            }
            if (cartaPorte.Cosecha != comando.Orden.Cosecha)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Cosecha",
                    ValorOriginal = cartaPorte.Cosecha,
                    ValorNuevo = comando.Orden.Cosecha,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.Cosecha = comando.Orden.Cosecha;
            }
            if (cartaPorte.OrigenVehiculo != comando.Orden.OrigenVehiculo)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Origen Vehiculo",
                    ValorOriginal = cartaPorte.OrigenVehiculo.ToString(),
                    ValorNuevo = comando.Orden.OrigenVehiculo.ToString(),
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.OrigenVehiculo = comando.Orden.OrigenVehiculo;
            }
            if (cartaPorte.KmRecorrer != comando.Orden.KmRecorrer)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Km Recorrer",
                    ValorOriginal = cartaPorte.KmRecorrer.HasValue ? cartaPorte.KmRecorrer.ToString() : null,
                    ValorNuevo = comando.Orden.KmRecorrer.HasValue ? comando.Orden.KmRecorrer.ToString() : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.KmRecorrer = comando.Orden.KmRecorrer;
            }
            if (cartaPorte.TarifaTonelada != comando.Orden.TarifaTonelada)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Tarifa Tonelada",
                    ValorOriginal = cartaPorte.TarifaTonelada.HasValue ? cartaPorte.TarifaTonelada.ToString() : null,
                    ValorNuevo = comando.Orden.TarifaTonelada.HasValue ? comando.Orden.TarifaTonelada.ToString() : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.TarifaTonelada = comando.Orden.TarifaTonelada;
            }
            if (cartaPorte.Material != material)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Material",
                    ValorOriginal = cartaPorte.Material.Descripcion,
                    ValorNuevo = material.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.Material = material;
            }
            if (cartaPorte.AgenteCompras != agente)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Agente Compras",
                    ValorOriginal = cartaPorte.AgenteCompras != null ? cartaPorte.AgenteCompras.RazonSocial : null,
                    ValorNuevo = agente != null ? agente.RazonSocial : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.AgenteCompras = agente;
            }
            if (cartaPorte.Intermediario != intermediario)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Intermediario",
                    ValorOriginal = cartaPorte.Intermediario != null ? cartaPorte.Intermediario.RazonSocial : null,
                    ValorNuevo = intermediario != null ? intermediario.RazonSocial : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.Intermediario = intermediario;
            }
            if (cartaPorte.RtteComercial != rtteComercial)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Rtte Comercial",
                    ValorOriginal = cartaPorte.RtteComercial != null ? cartaPorte.RtteComercial.RazonSocial : null,
                    ValorNuevo = rtteComercial != null ? rtteComercial.RazonSocial : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.RtteComercial = rtteComercial;
            }
            if (cartaPorte.Entregador != entregador)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Entregador",
                    ValorOriginal = cartaPorte.Entregador != null ? cartaPorte.Entregador.RazonSocial : null,
                    ValorNuevo = entregador != null ? entregador.RazonSocial : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.Entregador = entregador;
            }
            if (cartaPorte.Corredor != corredor)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Corredor",
                    ValorOriginal = cartaPorte.Corredor != null ? cartaPorte.Corredor.RazonSocial : null,
                    ValorNuevo = corredor != null ? corredor.RazonSocial : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.Corredor = corredor;
            }
            if (cartaPorte.CorredorVendedor != corredorVendedor)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "CorredorVendedor",
                    ValorOriginal = cartaPorte.CorredorVendedor != null ? cartaPorte.CorredorVendedor.RazonSocial : null,
                    ValorNuevo = corredorVendedor != null ? corredorVendedor.RazonSocial : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.CorredorVendedor = corredorVendedor;
            }
            if (cartaPorte.IntermediarioFlete != intermediarioFlete)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "IntermediarioFlete",
                    ValorOriginal = cartaPorte.IntermediarioFlete != null ? cartaPorte.IntermediarioFlete.RazonSocial : null,
                    ValorNuevo = intermediarioFlete != null ? intermediarioFlete.RazonSocial : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.IntermediarioFlete = intermediarioFlete;
            }
            if (cartaPorte.Categoria != categoria)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Categoria",
                    ValorOriginal = cartaPorte.Categoria != null ? cartaPorte.Categoria.Clasificacion : null,
                    ValorNuevo = categoria != null ? categoria.Clasificacion : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.Categoria = categoria;
            }
            if (cartaPorte.TarifaReferencia != comando.Orden.TarifaReferencia)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Tarifa Referencia",
                    ValorOriginal = cartaPorte.TarifaReferencia.HasValue ? cartaPorte.TarifaReferencia.ToString() : null,
                    ValorNuevo = comando.Orden.TarifaReferencia.HasValue ? comando.Orden.TarifaReferencia.ToString() : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.TarifaReferencia = comando.Orden.TarifaReferencia;
            }

            if (cartaPorte.CodigoAnexo != comando.Orden.CodigoAnexo)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Código Anexo",
                    ValorOriginal = cartaPorte.CodigoAnexo,
                    ValorNuevo = comando.Orden.CodigoAnexo,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.CodigoAnexo = comando.Orden.CodigoAnexo;
            }

            if (cartaPorte.Cupo != comando.Orden.Cupo)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Cupo",
                    ValorOriginal = cartaPorte.Cupo,
                    ValorNuevo = comando.Orden.Cupo,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                cartaPorte.Cupo = comando.Orden.Cupo;

            }

            foreach (var vehiculo in comando.Orden.Vehiculos)
            {
                var vOriginal = cartaPorte.Vehiculos.FirstOrDefault(x => x.Id == vehiculo.Id);
                if (vOriginal.TipoVehiculo != vehiculo.TipoVehiculo)
                {
                    listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                    {
                        Nombre = "Patente",
                        ValorOriginal = vOriginal.TipoVehiculo.ToString(),
                        ValorNuevo = vehiculo.TipoVehiculo.ToString(),
                        LogModificacionDocumentoIngreso = logModificacionDocumento,
                        Fecha = DateTime.Now,
                        NombreUsuario = comando.NombreUsuario
                    });
                    vOriginal.TipoVehiculo = vehiculo.TipoVehiculo;
                    cartaPorte.TipoVehiculo = comando.Orden.TipoVehiculo;
                }
                if (vOriginal.Patente != vehiculo.Patente)
                {
                    listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                    {
                        Nombre = "Patente",
                        ValorOriginal = vOriginal.Patente,
                        ValorNuevo = vehiculo.Patente,
                        LogModificacionDocumentoIngreso = logModificacionDocumento,
                        Fecha = DateTime.Now,
                        NombreUsuario = comando.NombreUsuario
                    });
                    vOriginal.Patente = vehiculo.Patente;
                }
                if (vOriginal.PatenteAcoplado != vehiculo.PatenteAcoplado)
                {
                    listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                    {
                        Nombre = "Patente Acoplado",
                        ValorOriginal = vOriginal.PatenteAcoplado,
                        ValorNuevo = vehiculo.PatenteAcoplado,
                        LogModificacionDocumentoIngreso = logModificacionDocumento,
                        Fecha = DateTime.Now,
                        NombreUsuario = comando.NombreUsuario
                    });
                    vOriginal.PatenteAcoplado = vehiculo.PatenteAcoplado;
                }
                if (vOriginal.PesoBrutoOrigen != (vehiculo.PesoBrutoOrigen ?? vOriginal.PesoBrutoOrigen))
                {
                    listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                    {
                        Nombre = "Peso Bruto Origen",
                        ValorOriginal = vOriginal.PesoBrutoOrigen.ToString(CultureInfo.InvariantCulture),
                        ValorNuevo = (vehiculo.PesoBrutoOrigen ?? vOriginal.PesoBrutoOrigen).ToString(CultureInfo.InvariantCulture),
                        LogModificacionDocumentoIngreso = logModificacionDocumento,
                        Fecha = DateTime.Now,
                        NombreUsuario = comando.NombreUsuario
                    });
                    vOriginal.PesoBrutoOrigen = vehiculo.PesoBrutoOrigen ?? vOriginal.PesoBrutoOrigen;
                }
                if (vOriginal.PesoNetoOrigen != (vehiculo.PesoNetoOrigen ?? vOriginal.PesoNetoOrigen))
                {
                    listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                    {
                        Nombre = "Peso Neto Origen",
                        ValorOriginal = vOriginal.PesoNetoOrigen.ToString(CultureInfo.InvariantCulture),
                        ValorNuevo = (vehiculo.PesoNetoOrigen ?? vOriginal.PesoNetoOrigen).ToString(CultureInfo.InvariantCulture),
                        LogModificacionDocumentoIngreso = logModificacionDocumento,
                        Fecha = DateTime.Now,
                        NombreUsuario = comando.NombreUsuario
                    });
                    vOriginal.PesoNetoOrigen = vehiculo.PesoNetoOrigen ?? vOriginal.PesoNetoOrigen;
                }
                if (vOriginal.PesoTaraOrigen != (vehiculo.PesoTaraOrigen ?? vOriginal.PesoTaraOrigen))
                {
                    listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                    {
                        Nombre = "Peso Tara Origen",
                        ValorOriginal = vOriginal.PesoTaraOrigen.ToString(CultureInfo.InvariantCulture),
                        ValorNuevo = (vehiculo.PesoTaraOrigen ?? vOriginal.PesoTaraOrigen).ToString(CultureInfo.InvariantCulture),
                        LogModificacionDocumentoIngreso = logModificacionDocumento,
                        Fecha = DateTime.Now,
                        NombreUsuario = comando.NombreUsuario
                    });
                    vOriginal.PesoTaraOrigen = vehiculo.PesoTaraOrigen ?? vOriginal.PesoTaraOrigen;
                }
            }
            var recorridos = Repositorio.Listar<Recorrido>(x => x.Vehiculo.CartaPorte.Id == comando.Orden.Id);
            foreach (var recorrido in recorridos)
            {
                var vehiculo = recorrido.Vehiculo;
                if (vehiculo != null)
                {
                    recorrido.Patente = vehiculo.Patente;
                    recorrido.PesoBrutoOrigen = vehiculo.PesoBrutoOrigen;
                    recorrido.PesoTaraOrigen = vehiculo.PesoTaraOrigen;
                    recorrido.Vehiculo = vehiculo;
                }
                recorrido.Chofer = chofer;
                recorrido.Transportista = transportista;
                recorrido.TipoComercial = tipoComercial;
                recorrido.TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte;
                recorrido.Material = material;
                recorrido.TipoVehiculo = comando.Orden.TipoVehiculo;
            }
            foreach (var campo in listaCampos)
            {
                Repositorio.Agregar(campo);
            }
        }

        protected override void Validar(ModificarCartaPorte comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Transportista>(x => x.Id == comando.Orden.TransportistaId))
            {
                resultado.Error("Transportista", string.Format(Textos.Error_Requerido, Textos.Transportista));
            }
        }
    }
}
