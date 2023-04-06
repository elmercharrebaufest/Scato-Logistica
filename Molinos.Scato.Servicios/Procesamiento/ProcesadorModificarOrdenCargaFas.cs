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
    public class ProcesadorModificarOrdenCargaFas : ProcesadorModificar<ModificarOrdenCargaFas>
    {
        public ProcesadorModificarOrdenCargaFas(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarOrdenCargaFas comando)
        {
            var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
            var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
            var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
            var cliente = Repositorio.Obtener<Cliente>(comando.Orden.ClienteId);
            var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
            var recorrido = Repositorio.Obtener<Recorrido>(comando.Orden.RecorridoId);
            var localidadDestino = Repositorio.Obtener<Localidad>(comando.Orden.LocalidadDestinoId);
            var pagadorFlete = Repositorio.Obtener<Cliente>(comando.Orden.PagadorFleteId);
            var corredor = Repositorio.Obtener<Proveedor>(comando.Orden.CorredorId);
            var comisionista = Repositorio.Obtener<Cliente>(comando.Orden.ComisionistaId);
            var remitente = Repositorio.Obtener<Cliente>(comando.Orden.RemitenteId);

            var ordenCargaFas = Repositorio.Obtener<OrdenCargaFas>(comando.Orden.Id);

            var listaCampos = new List<LogModificacionDocumentoIngresoCampo>();
            var logModificacionDocumento =
                Repositorio.Obtener<LogModificacionDocumentoIngreso>(
                    q =>
                    q.Numero == comando.Orden.NumeroOrden && q.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenCargaFas);
            if (logModificacionDocumento == null)
            {
                logModificacionDocumento = new LogModificacionDocumentoIngreso
                {
                    NombreUsuarioUltimaModificacion = comando.NombreUsuario,
                    FechaUltimaModificacion = DateTime.Now,
                    Numero = comando.Orden.NumeroOrden,
                    TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaFas
                };
                Repositorio.Agregar(logModificacionDocumento);
            }

            if (ordenCargaFas.TipoComercial != tipoComercial)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Tipo Comercial",
                    ValorOriginal = ordenCargaFas.TipoComercial?.Descripcion,
                    ValorNuevo = tipoComercial.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaFas.TipoComercial = tipoComercial;
            }
            if (ordenCargaFas.Material != material)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Material",
                    ValorOriginal = ordenCargaFas.Material?.Descripcion,
                    ValorNuevo = material.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaFas.Material = material;
            }
            if (ordenCargaFas.Transportista != transportista)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Transportista",
                    ValorOriginal = ordenCargaFas.Transportista?.RazonSocial,
                    ValorNuevo = transportista.RazonSocial,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaFas.Transportista = transportista;
            }
            if (ordenCargaFas.Chofer != chofer)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Chofer",
                    ValorOriginal = ordenCargaFas.Chofer != null ? ordenCargaFas.Chofer.Nombre + " " + ordenCargaFas.Chofer.Apellido : "",
                    ValorNuevo = chofer.Nombre + " " + chofer.Apellido,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaFas.Chofer = chofer;
            }
            if (ordenCargaFas.PatenteCamion != comando.Orden.PatenteCamion)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Patente Camion",
                    ValorOriginal = ordenCargaFas.PatenteCamion,
                    ValorNuevo = comando.Orden.PatenteCamion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaFas.PatenteCamion = comando.Orden.PatenteCamion;
            }
            if (ordenCargaFas.PatenteAcoplado != comando.Orden.PatenteAcoplado)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Patente Acoplado",
                    ValorOriginal = ordenCargaFas.PatenteAcoplado,
                    ValorNuevo = comando.Orden.PatenteAcoplado,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenCargaFas.PatenteAcoplado = comando.Orden.PatenteAcoplado;
            }
            if (ordenCargaFas.KmRecorrer != comando.Orden.KmARecorrer)
            {
                ordenCargaFas.KmRecorrer = comando.Orden.KmARecorrer;
            }
            if (ordenCargaFas.LocalidadDestino != localidadDestino)
            {
                ordenCargaFas.LocalidadDestino = localidadDestino;
            }
            if (ordenCargaFas.Cliente != cliente)
            {
                ordenCargaFas.Cliente = cliente;
            }
            if (ordenCargaFas.NumeroOrden != comando.Orden.NumeroOrden)
            {
                ordenCargaFas.NumeroOrden = comando.Orden.NumeroOrden;
                recorrido.NumeroDocumentoIngreso = comando.Orden.NumeroOrden;
            }

            #region Campos para Derivado Granario
            ordenCargaFas.PlantaDGDestino = comando.Orden.PlantaDGDestino;
            ordenCargaFas.OrdenDomicilioDestino = comando.Orden.OrdenDomicilioDestino;
            ordenCargaFas.TipoDomicilioDestino = comando.Orden.TipoDomicilioDestino;
            ordenCargaFas.PagadorFlete = pagadorFlete;
            ordenCargaFas.CuitDestinatario = comando.Orden.CuitDestinatario;
            ordenCargaFas.Corredor = corredor;
            ordenCargaFas.Comisionista = comisionista;
            ordenCargaFas.Remitente = remitente;
            ordenCargaFas.DerivadoGranarioHabilitado = comando.Orden.DerivadoGranarioHabilitado;
            #endregion

            recorrido.Patente = comando.Orden.PatenteCamion;
            recorrido.Chofer = chofer;
            recorrido.Transportista = transportista;
            recorrido.TipoComercial = tipoComercial;
            recorrido.Material = material;
            recorrido.TipoVehiculo = comando.Orden.TipoVehiculo;

        }

        protected override void Validar(ModificarOrdenCargaFas comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Transportista>(x => x.Id == comando.Orden.TransportistaId))
            {
                resultado.Error("Transportista", string.Format(Textos.Error_Requerido, Textos.Transportista));
            }
        }
    }
}
