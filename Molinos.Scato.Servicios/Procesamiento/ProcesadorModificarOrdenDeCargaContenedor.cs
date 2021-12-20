using System;
using System.Collections.Generic;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarOrdenDeCargaContenedor : ProcesadorModificar<ModificarOrdenDeCargaContenedor>
    {
        public ProcesadorModificarOrdenDeCargaContenedor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarOrdenDeCargaContenedor comando)
        {
            var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
            var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
            var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
            var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
            var recorrido = Repositorio.Obtener<Recorrido>(comando.Orden.RecorridoId);
            var destino = Repositorio.Obtener<Cliente>(comando.Orden.DestinoId);
            var contenedorEntrada = Repositorio.Obtener<TaraContenedor>(comando.Orden.ContenedorEntradaId);
            var contenedorSalida = Repositorio.Obtener<TaraContenedor>(comando.Orden.ContenedorSalidaId);

            recorrido.Patente = comando.Orden.PatenteCamion;
            recorrido.Chofer = chofer;
            recorrido.Transportista = transportista;
            recorrido.TipoComercial = tipoComercial;
            recorrido.Material = material;
            recorrido.NumeroDocumentoIngreso = comando.Orden.OrdenDeCargaContenedor;

            var ordenDeCargaContenedor = Repositorio.Obtener<OrdenDeCargaContenedor>(comando.Orden.Id);

            var listaCampos = new List<LogModificacionDocumentoIngresoCampo>();
            var logModificacionDocumento =
                Repositorio.Obtener<LogModificacionDocumentoIngreso>(
                    q =>
                    q.Numero == comando.Orden.OrdenDeCargaContenedor && q.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenDeCargaContenedor);
            if (logModificacionDocumento == null)
            {
                logModificacionDocumento = new LogModificacionDocumentoIngreso
                {
                    NombreUsuarioUltimaModificacion = comando.NombreUsuario,
                    FechaUltimaModificacion = DateTime.Now,
                    Numero = comando.Orden.OrdenDeCargaContenedor,
                    TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenDeCargaContenedor
                };
                Repositorio.Agregar(logModificacionDocumento);
            }
            if (ordenDeCargaContenedor.Chofer != chofer)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Chofer",
                    ValorOriginal = ordenDeCargaContenedor.Chofer.Nombre + " " + ordenDeCargaContenedor.Chofer.Apellido,
                    ValorNuevo = chofer.Nombre + " " + chofer.Apellido,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeCargaContenedor.Chofer = chofer;
            }

            if (ordenDeCargaContenedor.Material != material)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Material",
                    ValorOriginal = ordenDeCargaContenedor.Material.Descripcion,
                    ValorNuevo = material.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeCargaContenedor.Material = material;
            }
            if (ordenDeCargaContenedor.TipoComercial != tipoComercial)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Tipo Comercial",
                    ValorOriginal = ordenDeCargaContenedor.TipoComercial.Descripcion,
                    ValorNuevo = tipoComercial.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeCargaContenedor.TipoComercial = tipoComercial;
            }
            if (ordenDeCargaContenedor.Transportista != transportista)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Transportista",
                    ValorOriginal = ordenDeCargaContenedor.Transportista.RazonSocial,
                    ValorNuevo = transportista.RazonSocial,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeCargaContenedor.Transportista = transportista;
            }
            if (ordenDeCargaContenedor.Destino != destino)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Destino",
                    ValorOriginal = ordenDeCargaContenedor.Destino.Descripcion,
                    ValorNuevo = destino.Descripcion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeCargaContenedor.Destino = destino;
            }
            if (ordenDeCargaContenedor.PatenteCamion != comando.Orden.PatenteCamion)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Patente Camion",
                    ValorOriginal = ordenDeCargaContenedor.PatenteCamion,
                    ValorNuevo = comando.Orden.PatenteCamion,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeCargaContenedor.PatenteCamion = comando.Orden.PatenteCamion;
            }
            if (ordenDeCargaContenedor.PatenteAcoplado != comando.Orden.PatenteAcoplado)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Patente Acoplado",
                    ValorOriginal = ordenDeCargaContenedor.PatenteAcoplado,
                    ValorNuevo = comando.Orden.PatenteAcoplado,
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeCargaContenedor.PatenteAcoplado = comando.Orden.PatenteAcoplado;
            }
            if (ordenDeCargaContenedor.ContenedorEntrada.Id != contenedorEntrada.Id)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Contenedor Entrada",
                    ValorOriginal = ordenDeCargaContenedor.ContenedorEntrada.Id.ToString(CultureInfo.InvariantCulture),
                    ValorNuevo = contenedorEntrada.Id.ToString(CultureInfo.InvariantCulture),
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeCargaContenedor.ContenedorEntrada = contenedorEntrada;
            }
            if (ordenDeCargaContenedor.ContenedorSalida.Id != contenedorSalida.Id)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                {
                    Nombre = "Contenedor Salida",
                    ValorOriginal = ordenDeCargaContenedor.ContenedorSalida.Id.ToString(CultureInfo.InvariantCulture),
                    ValorNuevo = contenedorSalida.Id.ToString(CultureInfo.InvariantCulture),
                    LogModificacionDocumentoIngreso = logModificacionDocumento,
                    Fecha = DateTime.Now,
                    NombreUsuario = comando.NombreUsuario
                });
                ordenDeCargaContenedor.ContenedorSalida = contenedorSalida;
            }
            ordenDeCargaContenedor.Recorrido = recorrido;
            foreach (var campo in listaCampos)
            {
                Repositorio.Agregar(campo);
            }
        }

        protected override void Validar(ModificarOrdenDeCargaContenedor comando, Resultado resultado)
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
            if (!Repositorio.Existe<Cliente>(x => x.Id == comando.Orden.DestinoId))
            {
                resultado.Error("Destino", string.Format(Textos.Error_Requerido, Textos.Destino));
            }
        }
    }
}
