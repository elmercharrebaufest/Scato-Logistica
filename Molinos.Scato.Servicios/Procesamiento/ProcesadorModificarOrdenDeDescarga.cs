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
    public class ProcesadorModificarOrdenDeDescarga : ProcesadorModificar<ModificarOrdenDeDescarga>
    {
        public ProcesadorModificarOrdenDeDescarga(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarOrdenDeDescarga comando)
        {
            var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
            var proveedor = Repositorio.Obtener<Proveedor>(comando.Orden.ProveedorId);
            var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
            var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
            var recorrido = Repositorio.Obtener<Recorrido>(comando.Orden.RecorridoId);
            var ordenDeDescarga = Repositorio.Obtener<OrdenDeDescarga>(comando.Orden.Id);

            var listaCampos = new List<LogModificacionDocumentoIngresoCampo>();
            var logModificacionDocumento =
                Repositorio.Obtener<LogModificacionDocumentoIngreso>(
                    q =>
                    q.Numero == comando.Orden.Numero && q.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenDeDescarga);
            if (logModificacionDocumento == null)
            {
                logModificacionDocumento = new LogModificacionDocumentoIngreso
                {
                    NombreUsuarioUltimaModificacion = comando.NombreUsuario,
                    FechaUltimaModificacion = DateTime.Now,
                    Numero = comando.Orden.Numero,
                    TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenDeDescarga
                };
                Repositorio.Agregar(logModificacionDocumento);
            }
            if (ordenDeDescarga.Chofer != chofer)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Chofer",
                    ValorOriginal = ordenDeDescarga.Chofer.Nombre + " " + ordenDeDescarga.Chofer.Apellido,
                    ValorNuevo = chofer.Nombre + " " + chofer.Apellido,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeDescarga.Chofer = chofer;
            }
            if (ordenDeDescarga.Proveedor != proveedor)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Proveedor",
                    ValorOriginal = ordenDeDescarga.Proveedor.Descripcion,
                    ValorNuevo = proveedor.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeDescarga.Proveedor = proveedor;
            }
            if (ordenDeDescarga.TipoComercial != tipoComercial)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Tipo Comercial",
                    ValorOriginal = ordenDeDescarga.TipoComercial.Descripcion,
                    ValorNuevo = tipoComercial.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeDescarga.TipoComercial = tipoComercial;
            }
            if (ordenDeDescarga.Transportista != transportista)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Transportista",
                    ValorOriginal = ordenDeDescarga.Transportista.RazonSocial,
                    ValorNuevo = transportista.RazonSocial,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeDescarga.Transportista = transportista;
            }
            if (ordenDeDescarga.PatenteCamion != comando.Orden.PatenteCamion)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Patente Camion",
                    ValorOriginal = ordenDeDescarga.PatenteCamion,
                    ValorNuevo = comando.Orden.PatenteCamion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeDescarga.PatenteCamion = comando.Orden.PatenteCamion;
            }
            if (ordenDeDescarga.PatenteAcoplado != comando.Orden.PatenteAcoplado)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Patente Acoplado",
                    ValorOriginal = ordenDeDescarga.PatenteAcoplado,
                    ValorNuevo = comando.Orden.PatenteAcoplado,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeDescarga.PatenteAcoplado = comando.Orden.PatenteAcoplado;
            }

            recorrido.Patente = comando.Orden.PatenteCamion;
            recorrido.Chofer = chofer;
            recorrido.Transportista = transportista;
            recorrido.TipoComercial = tipoComercial;
            recorrido.TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenDeDescarga;
            recorrido.NumeroDocumentoIngreso = comando.Orden.Numero;

            foreach (var campo in listaCampos)
            {
                Repositorio.Agregar(campo);
            }
        }

        protected override void Validar(ModificarOrdenDeDescarga comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Transportista>(x => x.Id == comando.Orden.TransportistaId))
            {
                resultado.Error("Transportista", string.Format(Textos.Error_Requerido, Textos.Transportista));
            }
        }
    }
}
