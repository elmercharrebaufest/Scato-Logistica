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
    public class ProcesadorModificarOrdenEntrePlantas : ProcesadorModificar<ModificarOrdenEntrePlantas>
    {
        public ProcesadorModificarOrdenEntrePlantas(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }
        protected override void ModificarEntidad(ModificarOrdenEntrePlantas comando)
        {
            var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
            var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
            var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
            var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
            var centroDestino = Repositorio.Obtener<Centro>(comando.Orden.CentroDestinoId);
            var recorrido = Repositorio.Obtener<Recorrido>(comando.Orden.RecorridoId);
            var ordenEntrePlantas = Repositorio.Obtener<OrdenEntrePlantas>(comando.Orden.Id);
            var listaCampos = new List<LogModificacionDocumentoIngresoCampo>();
            var logModificacionDocumento =
                Repositorio.Obtener<LogModificacionDocumentoIngreso>(
                    q =>
                    q.Numero == comando.Orden.Numero && q.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenEntrePlantas);
            if (logModificacionDocumento == null)
            {
                logModificacionDocumento = new LogModificacionDocumentoIngreso
                {
                    NombreUsuarioUltimaModificacion = comando.NombreUsuario,
                    FechaUltimaModificacion = DateTime.Now,
                    Numero = comando.Orden.Numero,
                    TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenEntrePlantas
                };
                Repositorio.Agregar(logModificacionDocumento);
            }
            if (ordenEntrePlantas.Chofer != chofer)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Chofer",
                    ValorOriginal = ordenEntrePlantas.Chofer.Nombre + " " + ordenEntrePlantas.Chofer.Apellido,
                    ValorNuevo = chofer.Nombre + " " + chofer.Apellido,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenEntrePlantas.Chofer = chofer;
            }
            if (ordenEntrePlantas.TipoComercial != tipoComercial)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Tipo Comercial",
                    ValorOriginal = ordenEntrePlantas.TipoComercial.Descripcion,
                    ValorNuevo = tipoComercial.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenEntrePlantas.TipoComercial = tipoComercial;
            }
            if (ordenEntrePlantas.Material != material)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Material",
                    ValorOriginal = ordenEntrePlantas.Material.Descripcion,
                    ValorNuevo = material.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenEntrePlantas.Material = material;
            }
            if (ordenEntrePlantas.Transportista != transportista)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Transportista",
                    ValorOriginal = ordenEntrePlantas.Transportista.RazonSocial,
                    ValorNuevo = transportista.RazonSocial,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenEntrePlantas.Transportista = transportista;
            }
            if (ordenEntrePlantas.PatenteCamion != comando.Orden.PatenteCamion)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Patente Camion",
                    ValorOriginal = ordenEntrePlantas.PatenteCamion,
                    ValorNuevo = comando.Orden.PatenteCamion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenEntrePlantas.PatenteCamion = comando.Orden.PatenteCamion;
            }
            if (ordenEntrePlantas.PatenteAcoplado != comando.Orden.PatenteAcoplado)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Patente Acoplado",
                    ValorOriginal = ordenEntrePlantas.PatenteAcoplado,
                    ValorNuevo = comando.Orden.PatenteAcoplado,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenEntrePlantas.PatenteAcoplado = comando.Orden.PatenteAcoplado;
            }
            if (ordenEntrePlantas.CentroDestino != centroDestino)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Centro Destino",
                    ValorOriginal = ordenEntrePlantas.CentroDestino.Descripcion,
                    ValorNuevo = centroDestino.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenEntrePlantas.CentroDestino = centroDestino;
            }
            if (ordenEntrePlantas.CodigoAnexo != comando.Orden.CodigoAnexo)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Código Anexo",
                    ValorOriginal = ordenEntrePlantas.CodigoAnexo,
                    ValorNuevo = comando.Orden.CodigoAnexo,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenEntrePlantas.CodigoAnexo = comando.Orden.CodigoAnexo;
            }
            if (ordenEntrePlantas.KmRecorrer != comando.Orden.KmRecorrer)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "KM a recorrer",
                    ValorOriginal = ordenEntrePlantas.KmRecorrer.ToString(),
                    ValorNuevo = comando.Orden.KmRecorrer.ToString(),
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenEntrePlantas.KmRecorrer = comando.Orden.KmRecorrer;
            }
            recorrido.Patente = comando.Orden.PatenteCamion;
            recorrido.Chofer = chofer;
            recorrido.Transportista = transportista;
            recorrido.TipoComercial = tipoComercial;
            recorrido.Material = material;
            foreach (var campo in listaCampos)
            {
                Repositorio.Agregar(campo);
            }
        }
        protected override void Validar(ModificarOrdenEntrePlantas comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Transportista>(x => x.Id == comando.Orden.TransportistaId))
            {
                resultado.Error("Transportista", Textos.Error_Requerido);
            }
        }
    }
}
