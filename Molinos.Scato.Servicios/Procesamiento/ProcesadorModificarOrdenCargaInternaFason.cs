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
    public class ProcesadorModificarOrdenCargaInternaFason : ProcesadorModificar<ModificarOrdenCargaInternaFason>
    {
        public ProcesadorModificarOrdenCargaInternaFason(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarOrdenCargaInternaFason comando)
        {
            var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
            var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
            var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
            var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
            var recorrido = Repositorio.Obtener<Recorrido>(comando.Orden.RecorridoId);
            var cliente = Repositorio.Obtener<Cliente>(comando.Orden.ClienteId);
            var localidadDestino = Repositorio.Obtener<Localidad>(comando.Orden.LocalidadDestinoId);

            recorrido.Patente = comando.Orden.PatenteCamion;
            recorrido.Chofer = chofer;
            recorrido.Transportista = transportista;
            recorrido.TipoComercial = tipoComercial;
            recorrido.Material = material;
            recorrido.NumeroDocumentoIngreso = comando.Orden.NumeroOrden;

            var listaCampos = new List<LogModificacionDocumentoIngresoCampo>();
            var logModificacionDocumento =
                Repositorio.Obtener<LogModificacionDocumentoIngreso>(
                    q =>
                    q.Numero == comando.Orden.NumeroOrden && q.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenCargaInternaFason);
            if (logModificacionDocumento == null)
            {
                logModificacionDocumento = new LogModificacionDocumentoIngreso
                {
                    NombreUsuarioUltimaModificacion = comando.NombreUsuario,
                    FechaUltimaModificacion = DateTime.Now,
                    Numero = comando.Orden.NumeroOrden,
                    TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInternaFason
                };
                Repositorio.Agregar(logModificacionDocumento);
            }
            var ordenCargaInternaFason = Repositorio.Obtener<OrdenCargaInternaFason>(comando.Orden.Id);
            if (ordenCargaInternaFason.Chofer != chofer)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Chofer",
                    ValorOriginal = ordenCargaInternaFason.Chofer.Nombre + " " + ordenCargaInternaFason.Chofer.Apellido,
                    ValorNuevo = chofer.Nombre + " " + chofer.Apellido,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInternaFason.Chofer = chofer;
            }
            if (ordenCargaInternaFason.Cliente != cliente)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Cliente",
                    ValorOriginal = ordenCargaInternaFason.Cliente.Descripcion,
                    ValorNuevo = cliente.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInternaFason.Cliente = cliente;
            }
            if (ordenCargaInternaFason.TipoComercial != tipoComercial)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Tipo Comercial",
                    ValorOriginal = ordenCargaInternaFason.TipoComercial.Descripcion,
                    ValorNuevo = tipoComercial.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInternaFason.TipoComercial = tipoComercial;
            }
            if (ordenCargaInternaFason.Material != material)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Material",
                    ValorOriginal = ordenCargaInternaFason.Material.Descripcion,
                    ValorNuevo = material.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInternaFason.Material = material;
            }
            if (ordenCargaInternaFason.Transportista != transportista)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Transportista",
                    ValorOriginal = ordenCargaInternaFason.Transportista != null ? ordenCargaInternaFason.Transportista.RazonSocial : null,
                    ValorNuevo = transportista != null ? transportista.RazonSocial : null,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInternaFason.Transportista = transportista;
            }
            if (ordenCargaInternaFason.PatenteCamion != comando.Orden.PatenteCamion)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Patente Camion",
                    ValorOriginal = ordenCargaInternaFason.PatenteCamion,
                    ValorNuevo = comando.Orden.PatenteCamion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInternaFason.PatenteCamion = comando.Orden.PatenteCamion;
            }
            if (ordenCargaInternaFason.PatenteAcoplado != comando.Orden.PatenteAcoplado)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Patente Acoplado",
                    ValorOriginal = ordenCargaInternaFason.PatenteAcoplado,
                    ValorNuevo = comando.Orden.PatenteAcoplado,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaInternaFason.PatenteAcoplado = comando.Orden.PatenteAcoplado;
            }
            foreach (var campo in listaCampos)
            {
                Repositorio.Agregar(campo);
            }
            ordenCargaInternaFason.Recorrido = recorrido;
            if (ordenCargaInternaFason.LocalidadDestino != localidadDestino)
            {
                ordenCargaInternaFason.LocalidadDestino = localidadDestino;
            }
            if (ordenCargaInternaFason.KmRecorrer != comando.Orden.KmARecorrer)
            {
                ordenCargaInternaFason.KmRecorrer = comando.Orden.KmARecorrer;
            }
        }

        protected override void Validar(ModificarOrdenCargaInternaFason comando, Resultado resultado)
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
