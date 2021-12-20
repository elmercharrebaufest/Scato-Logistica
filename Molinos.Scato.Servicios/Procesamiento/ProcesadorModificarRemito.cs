using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRemito : ProcesadorModificar<ModificarRemito>
    {
        public ProcesadorModificarRemito(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarRemito comando)
        {
            var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
            var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
            var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
            var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
            var recorrido = Repositorio.Obtener<Recorrido>(comando.Orden.RecorridoId);
            var procedencia = Repositorio.Obtener<Localidad>(comando.Orden.ProcedenciaId);
            recorrido.Patente = comando.Orden.PatenteCamion;
            recorrido.Chofer = chofer;
            recorrido.Transportista = transportista;
            recorrido.TipoComercial = tipoComercial;
            recorrido.Material = material;
            recorrido.NumeroDocumentoIngreso = comando.Orden.OrdenDeDescarga;

            var remito = Repositorio.Obtener<Remito>(comando.Orden.Id);
            var listaCampos = new List<LogModificacionDocumentoIngresoCampo>();
            var logModificacionDocumento =
                Repositorio.Obtener<LogModificacionDocumentoIngreso>(
                    q =>
                    q.Numero == comando.Orden.OrdenDeDescarga && q.TipoDocumentoIngreso == TipoDocumentoIngreso.Remito);
            if (logModificacionDocumento == null)
            {
                logModificacionDocumento = new LogModificacionDocumentoIngreso
                {
                    NombreUsuarioUltimaModificacion = comando.NombreUsuario,
                    FechaUltimaModificacion = DateTime.Now,
                    Numero = comando.Orden.OrdenDeDescarga,
                    TipoDocumentoIngreso = TipoDocumentoIngreso.Remito
                };
                Repositorio.Agregar(logModificacionDocumento);
            }
            if (remito.Chofer != chofer)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Chofer",
                    ValorOriginal = remito.Chofer.Nombre + " " + remito.Chofer.Apellido,
                    ValorNuevo = chofer.Nombre + " " + chofer.Apellido,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                remito.Chofer = chofer;
            }
            if (remito.Material != material)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Material",
                    ValorOriginal = remito.Material.Descripcion,
                    ValorNuevo = material.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                remito.Material = material;
            }
            if (comando.Orden.EsRemitoProveedor)
            {
                var proveedor = Repositorio.Obtener<Proveedor>(comando.Orden.OrigenId);
                if (remito.ProveedorOrigen != proveedor)
                {
                    listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                        {
                            Nombre = "Proveedor",
                            ValorOriginal = remito.ProveedorOrigen.RazonSocial,
                            ValorNuevo = proveedor.RazonSocial,
                            LogModificacionDocumentoIngreso = logModificacionDocumento,
                            Fecha = DateTime.Now,
                            NombreUsuario = comando.NombreUsuario
                        });
                    remito.ProveedorOrigen = proveedor;
                }
            }
            else
            {
                var centro = Repositorio.Obtener<Centro>(comando.Orden.OrigenId);
                if (remito.CentroOrigen != centro)
                {
                    listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                    {
                        Nombre = "Centro",
                        ValorOriginal = remito.CentroOrigen.Descripcion,
                        ValorNuevo = centro.Descripcion,
                        LogModificacionDocumentoIngreso = logModificacionDocumento,
                        Fecha = DateTime.Now,
                        NombreUsuario = comando.NombreUsuario
                    });
                    remito.CentroOrigen = centro;
                }
            }
            if (remito.TipoComercial != tipoComercial)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Tipo Comercial",
                    ValorOriginal = remito.TipoComercial.Descripcion,
                    ValorNuevo = tipoComercial.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                remito.TipoComercial = tipoComercial;
            }
            if (remito.Transportista != transportista)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Transportista",
                    ValorOriginal = remito.Transportista.RazonSocial,
                    ValorNuevo = transportista.RazonSocial,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                remito.Transportista = transportista;
            }
            if (remito.PatenteCamion != comando.Orden.PatenteCamion)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Patente Camion",
                    ValorOriginal = remito.PatenteCamion,
                    ValorNuevo = comando.Orden.PatenteCamion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                remito.PatenteCamion = comando.Orden.PatenteCamion;
            }
            if (remito.PatenteAcoplado != comando.Orden.PatenteAcoplado)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Patente Acoplado",
                    ValorOriginal = remito.PatenteAcoplado,
                    ValorNuevo = comando.Orden.PatenteAcoplado,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                remito.PatenteAcoplado = comando.Orden.PatenteAcoplado;
            }
            foreach (var campo in listaCampos)
            {
                Repositorio.Agregar(campo);
            }
            remito.PesoNetoOrigen = comando.Orden.PesoNetoOrigen ?? 0;
            remito.PesoBrutoOrigen = comando.Orden.PesoBrutoOrigen ?? 0;
            remito.PesoTaraOrigen = comando.Orden.PesoTaraOrigen ?? 0;
            remito.KmRecorrer = comando.Orden.KmRecorrer;
            remito.Procedencia = procedencia;
            remito.Cosecha = comando.Orden.Cosecha;
            remito.CodEstab = comando.Orden.CodEstab;
        }

        protected override void Validar(ModificarRemito comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Chofer>(x => x.Id == comando.Orden.Chofer.Id))
            {
                resultado.Error("Chofer", string.Format(Textos.Error_Requerido, Textos.Chofer));
            }
            if (!Repositorio.Existe<Material>(x => x.Id == comando.Orden.MaterialId))
            {
                resultado.Error("MaterialId", string.Format(Textos.Error_Requerido, Textos.Material));
            }
            if (comando.Orden.EsRemitoProveedor && !Repositorio.Existe<Proveedor>(x => x.Id == comando.Orden.OrigenId))
            {
                resultado.Error("Origen", string.Format(Textos.Error_Requerido, Textos.Proveedor));
            }
            if (!comando.Orden.EsRemitoProveedor && !Repositorio.Existe<Centro>(x => x.Id == comando.Orden.OrigenId))
            {
                resultado.Error("Origen", string.Format(Textos.Error_Requerido, Textos.Centro));
            }
            if (!Repositorio.Existe<TipoComercial>(x => x.Id == comando.Orden.TipoComercialId))
            {
                resultado.Error("TipoComercialId", string.Format(Textos.Error_Requerido, Textos.TipoComercial));
            }
            if (!Repositorio.Existe<Transportista>(x => x.Id == comando.Orden.TransportistaId))
            {
                resultado.Error("Transportista", string.Format(Textos.Error_Requerido, Textos.Transportista));
            }
        }
    }
}
