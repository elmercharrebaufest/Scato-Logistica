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
    public class ProcesadorModificarOrdenCargaInterna : ProcesadorModificar<ModificarOrdenCargaInterna>
    {
        public ProcesadorModificarOrdenCargaInterna(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarOrdenCargaInterna comando)
        {
            var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
            var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
            var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
            var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
            var recorrido = Repositorio.Obtener<Recorrido>(comando.Orden.RecorridoId);
            var destino = Repositorio.Obtener<Cliente>(comando.Orden.DestinoId);
            var localidadDestino = Repositorio.Obtener<Localidad>(comando.Orden.LocalidadDestinoId);

            recorrido.Patente = comando.Orden.PatenteCamion;
            recorrido.Chofer = chofer;
            recorrido.Transportista = transportista;
            recorrido.TipoComercial = tipoComercial;
            recorrido.Material = material;
            recorrido.NumeroDocumentoIngreso = comando.Orden.NumeroOrden;

            var ordenCargaInterna = Repositorio.Obtener<OrdenCargaInterna>(comando.Orden.Id);

            var listaCampos = new List<LogModificacionDocumentoIngresoCampo>();
            var logModificacionDocumento =
                Repositorio.Obtener<LogModificacionDocumentoIngreso>(
                    q =>
                    q.Numero == comando.Orden.NumeroOrden && q.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenCargaInterna);
            if (logModificacionDocumento == null)
            {
                logModificacionDocumento = new LogModificacionDocumentoIngreso
                {
                    NombreUsuarioUltimaModificacion = comando.NombreUsuario,
                    FechaUltimaModificacion = DateTime.Now,
                    Numero = comando.Orden.NumeroOrden,
                    TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna
                };
                Repositorio.Agregar(logModificacionDocumento);
            }
            if (ordenCargaInterna.Chofer != chofer)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Chofer",
                    ValorOriginal = ordenCargaInterna.Chofer.Nombre + " " + ordenCargaInterna.Chofer.Apellido,
                    ValorNuevo = chofer.Nombre + " " + chofer.Apellido,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInterna.Chofer = chofer;
            }
            if (ordenCargaInterna.Material != material)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Material",
                    ValorOriginal = ordenCargaInterna.Material.Descripcion,
                    ValorNuevo = material.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInterna.Material = material;
            }
            if (ordenCargaInterna.TipoComercial != tipoComercial)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Tipo Comercial",
                    ValorOriginal = ordenCargaInterna.TipoComercial.Descripcion,
                    ValorNuevo = tipoComercial.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInterna.TipoComercial = tipoComercial;
            }
            if (ordenCargaInterna.Transportista != transportista)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Transportista",
                    ValorOriginal = ordenCargaInterna.Transportista.RazonSocial,
                    ValorNuevo = transportista.RazonSocial,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInterna.Transportista = transportista;
            }
            if (ordenCargaInterna.PatenteCamion != comando.Orden.PatenteCamion)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Patente Camion",
                    ValorOriginal = ordenCargaInterna.PatenteCamion,
                    ValorNuevo = comando.Orden.PatenteCamion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInterna.PatenteCamion = comando.Orden.PatenteCamion;
            }
            if (ordenCargaInterna.PatenteAcoplado != comando.Orden.PatenteAcoplado)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Patente Acoplado",
                    ValorOriginal = ordenCargaInterna.PatenteAcoplado,
                    ValorNuevo = comando.Orden.PatenteAcoplado,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInterna.PatenteAcoplado = comando.Orden.PatenteAcoplado;
            }
            if (ordenCargaInterna.KmRecorrer != comando.Orden.KmARecorrer)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Km a Recorrer",
                    ValorOriginal = ordenCargaInterna.KmRecorrer,
                    ValorNuevo = comando.Orden.KmARecorrer,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInterna.KmRecorrer = comando.Orden.KmARecorrer;
            }
            if (ordenCargaInterna.LocalidadDestino != localidadDestino)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Localidad Destino",
                    ValorOriginal = ordenCargaInterna.LocalidadDestino.Descripcion,
                    ValorNuevo = localidadDestino.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInterna.LocalidadDestino = localidadDestino;
            }
            ordenCargaInterna.Recorrido = recorrido;
            foreach (var campo in listaCampos)
            {
                Repositorio.Agregar(campo);
            }
        }

        protected override void Validar(ModificarOrdenCargaInterna comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Chofer>(x => x.Id == comando.Orden.Chofer.Id))
            {
                resultado.Error("Chofer", string.Format(Textos.Error_Requerido, Textos.Chofer));
            }
            if (!Repositorio.Existe<Material>(x => x.Id == comando.Orden.MaterialId))
            {
                resultado.Error("MaterialId", string.Format(Textos.Error_Requerido, Textos.Material));
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
