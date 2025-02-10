using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using static Molinos.Scato.Dominio.Constantes;

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
            DateTime fecha = DateTime.Now;

            var logModificacionDocumento =
                Repositorio.Obtener<LogModificacionDocumentoIngreso>(q =>
                    q.Numero == comando.Orden.NumeroOrden && 
                    q.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenCargaInternaFason);
            
            if (logModificacionDocumento == null)
            {
                logModificacionDocumento = new LogModificacionDocumentoIngreso
                {
                    NombreUsuarioUltimaModificacion = comando.NombreUsuario,
                    FechaUltimaModificacion = fecha,
                    Numero = comando.Orden.NumeroOrden,
                    TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInternaFason
                };
                Repositorio.Agregar(logModificacionDocumento);
            }
            
            var ordenCargaInternaFason = Repositorio.Obtener<OrdenCargaInternaFason>(comando.Orden.Id);

            var listaCampos = new List<LogModificacionDocumentoIngresoCampo>();

            if (ordenCargaInternaFason.NumeroOrden != comando.Orden.NumeroOrden)
            {
                listaCampos.Add(
                    this.CrearLogModificacionDocumentoIngresoCampo(
                        logModificacionDocumento,
                        comando,
                        "Número Orden",
                        ordenCargaInternaFason.NumeroOrden,
                        comando.Orden.NumeroOrden,
                        fecha));

                ordenCargaInternaFason.NumeroOrden = comando.Orden.NumeroOrden;
            }

            var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
            if (ordenCargaInternaFason.Chofer != chofer)
            {
                listaCampos.Add(
                    this.CrearLogModificacionDocumentoIngresoCampo(
                        logModificacionDocumento,
                        comando,
                        "Chofer",
                        ordenCargaInternaFason.Chofer != null ? ordenCargaInternaFason.Chofer.Nombre + " " + ordenCargaInternaFason.Chofer.Apellido : null,
                        chofer != null ? chofer.Nombre + " " + chofer.Apellido : null, 
                        fecha));
                
                ordenCargaInternaFason.Chofer = chofer;
            }
            
            if(ordenCargaInternaFason.FechaEmision != comando.Orden.FechaEmision)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                        logModificacionDocumento,
                        comando,
                        "Fecha Emisión",
                        ordenCargaInternaFason.FechaEmision.ToString(),
                        comando.Orden.FechaEmision.ToString(),
                        fecha));

                ordenCargaInternaFason.FechaEmision = comando.Orden.FechaEmision;
            }

            var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
            if (ordenCargaInternaFason.Material != material)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Material",
                    ordenCargaInternaFason.Material?.Descripcion,
                    material?.Descripcion,
                    fecha));

                ordenCargaInternaFason.Material = material;
            }

            if (ordenCargaInternaFason.PatenteAcoplado != comando.Orden.PatenteAcoplado)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Patente Acoplado",
                    ordenCargaInternaFason.PatenteAcoplado,
                    comando.Orden.PatenteAcoplado,
                    fecha));

                ordenCargaInternaFason.PatenteAcoplado = comando.Orden.PatenteAcoplado;
            }

            if (ordenCargaInternaFason.PatenteCamion != comando.Orden.PatenteCamion)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando, 
                    "Patente Camion",
                    ordenCargaInternaFason.PatenteCamion,
                    comando.Orden.PatenteCamion,
                    fecha));

                ordenCargaInternaFason.PatenteCamion = comando.Orden.PatenteCamion;
            }

            var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
            if (ordenCargaInternaFason.TipoComercial != tipoComercial)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Tipo Comercial",
                    ordenCargaInternaFason.TipoComercial?.Descripcion,
                    tipoComercial?.Descripcion,
                    fecha));

                ordenCargaInternaFason.TipoComercial = tipoComercial;
            }

            var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
            if (ordenCargaInternaFason.Transportista != transportista)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                   logModificacionDocumento,
                    comando,
                    "Transportista",
                    ordenCargaInternaFason.Transportista != null ? ordenCargaInternaFason.Transportista.RazonSocial : null,
                    transportista != null ? transportista.RazonSocial : null,
                    fecha));

                ordenCargaInternaFason.Transportista = transportista;
            }

            var cliente = Repositorio.Obtener<Cliente>(comando.Orden.ClienteId);
            if (ordenCargaInternaFason.Cliente != cliente)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Cliente",
                    ordenCargaInternaFason.Cliente?.Descripcion,
                    cliente?.Descripcion,
                    fecha));

                ordenCargaInternaFason.Cliente = cliente;
            }

            if (ordenCargaInternaFason.KmRecorrer != comando.Orden.KmARecorrer)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Km a recorrer",
                    ordenCargaInternaFason.KmRecorrer?.ToString(),
                    comando.Orden.KmARecorrer?.ToString(),
                    fecha));

                ordenCargaInternaFason.KmRecorrer = comando.Orden.KmARecorrer;
            }

            var localidadDestino = Repositorio.Obtener<Localidad>(comando.Orden.LocalidadDestinoId);
            if (ordenCargaInternaFason.LocalidadDestino != localidadDestino)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Localidad Destino",
                    ordenCargaInternaFason.LocalidadDestino?.Descripcion,
                    localidadDestino?.Descripcion,
                    fecha));

                ordenCargaInternaFason.LocalidadDestino = localidadDestino;
            }

            if (ordenCargaInternaFason.DerivadoGranarioHabilitado != comando.Orden.DerivadoGranarioHabilitado)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Derivado Granario Habilitado",
                    ordenCargaInternaFason.DerivadoGranarioHabilitado.ToString(),
                    comando.Orden.DerivadoGranarioHabilitado.ToString(),
                    fecha));
            
                ordenCargaInternaFason.DerivadoGranarioHabilitado = comando.Orden.DerivadoGranarioHabilitado;
            }

            if (ordenCargaInternaFason.PlantaDGDestino != comando.Orden.PlantaDGDestino)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Planta DG Destino",
                    ordenCargaInternaFason.PlantaDGDestino?.ToString(),
                    comando.Orden.PlantaDGDestino?.ToString(),
                    fecha));

                ordenCargaInternaFason.PlantaDGDestino = comando.Orden.PlantaDGDestino;
            }

            if (ordenCargaInternaFason.OrdenDomicilioDestino != comando.Orden.OrdenDomicilioDestino)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Orden Domicilio Destino",
                    ordenCargaInternaFason.OrdenDomicilioDestino?.ToString(),
                    comando.Orden.OrdenDomicilioDestino?.ToString(),
                    fecha));

                ordenCargaInternaFason.OrdenDomicilioDestino = comando.Orden.OrdenDomicilioDestino;
            }

            var pagadorFlete = Repositorio.Obtener<Cliente>(comando.Orden.PagadorFleteId);
            if (ordenCargaInternaFason.PagadorFlete != pagadorFlete)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Pagador Flete",
                    ordenCargaInternaFason.PagadorFlete?.Descripcion,
                    pagadorFlete?.Descripcion,
                    fecha));

                ordenCargaInternaFason.PagadorFlete = pagadorFlete;
            }

            var corredor = Repositorio.Obtener<Proveedor>(comando.Orden.CorredorId);
            if (ordenCargaInternaFason.Corredor != corredor)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Corredor",
                    ordenCargaInternaFason.Corredor?.RazonSocial,
                    corredor?.RazonSocial,
                    fecha));

                ordenCargaInternaFason.Corredor = corredor;
            }

            var comisionista = Repositorio.Obtener<Cliente>(comando.Orden.ComisionistaId);
            if (ordenCargaInternaFason.Comisionista != comisionista)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Comisionista",
                    ordenCargaInternaFason.Comisionista?.Descripcion,
                    comisionista?.Descripcion,
                    fecha));

                ordenCargaInternaFason.Comisionista = comisionista;
            }

            var remitente = Repositorio.Obtener<Cliente>(comando.Orden.RemitenteId);
            if (ordenCargaInternaFason.Remitente != remitente)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Remitente",
                    ordenCargaInternaFason.Remitente?.Descripcion,
                    remitente?.Descripcion,
                    fecha));

                ordenCargaInternaFason.Remitente = remitente;
            }

            var intermediarioFlete = Repositorio.Obtener<Proveedor>(comando.Orden.IntermediarioFleteId);
            if (ordenCargaInternaFason.IntermediarioFlete != intermediarioFlete)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Intermediario Flete",
                    ordenCargaInternaFason.IntermediarioFlete?.RazonSocial,
                    intermediarioFlete?.RazonSocial,
                    fecha));

                ordenCargaInternaFason.IntermediarioFlete = intermediarioFlete;
            }

            if (ordenCargaInternaFason.TipoDomicilioDestino != comando.Orden.TipoDomicilioDestino)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Tipo Domicilio Destino",
                    ordenCargaInternaFason.TipoDomicilioDestino?.ToString(),
                    comando.Orden.TipoDomicilioDestino?.ToString(),
                    fecha));

                ordenCargaInternaFason.TipoDomicilioDestino = comando.Orden.TipoDomicilioDestino;
            }

            var destinatario = Repositorio.Obtener<Cliente>(comando.Orden.DestinatarioId);
            if (ordenCargaInternaFason.Destinatario != destinatario)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Destinatario",
                    ordenCargaInternaFason.Destinatario.Cuit,
                    destinatario.Cuit,
                    fecha));

                ordenCargaInternaFason.Destinatario = destinatario;
            }

            if (ordenCargaInternaFason.NumeroOrdenExterno != comando.Orden.NumeroOrdenExterno)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Número Orden Externo",
                    ordenCargaInternaFason.NumeroOrdenExterno,
                    comando.Orden.NumeroOrdenExterno,
                    fecha));

                ordenCargaInternaFason.NumeroOrdenExterno = comando.Orden.NumeroOrdenExterno;
            }

            if(ordenCargaInternaFason.Observaciones != comando.Orden.Observaciones)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Observaciones",
                    ordenCargaInternaFason.Observaciones,
                    comando.Orden.Observaciones,
                    fecha));

                ordenCargaInternaFason.Observaciones = comando.Orden.Observaciones;
            }

            if (ordenCargaInternaFason.DestinoMercaderia != comando.Orden.DestinoMercaderia)
            {
                listaCampos.Add(this.CrearLogModificacionDocumentoIngresoCampo(
                    logModificacionDocumento,
                    comando,
                    "Destino Mercadería",
                    ordenCargaInternaFason.DestinoMercaderia,
                    comando.Orden.DestinoMercaderia,
                    fecha));

                ordenCargaInternaFason.DestinoMercaderia = comando.Orden.DestinoMercaderia;
            }

            foreach (var campo in listaCampos)
                Repositorio.Agregar(campo);

            // Actualización de campos de recorrido
            var recorrido = Repositorio.Obtener<Recorrido>(comando.Orden.RecorridoId);
            recorrido.Patente = comando.Orden.PatenteCamion;
            recorrido.Chofer = chofer;
            recorrido.Transportista = transportista;
            recorrido.TipoComercial = tipoComercial;
            recorrido.Material = material;
            recorrido.NumeroDocumentoIngreso = comando.Orden.NumeroOrden;
            recorrido.TipoVehiculo = comando.Orden.TipoVehiculo;
            // El resto de los campos no hacen falta ser actualizados porque o no deben ser alterados o se alteran en etapas más adelante en el workflow:
            // (VehiculoDemorado, MotivoDemora, Rechazado, InstanciaWorkflow, Usuario, Workflow, Centro, TipoDocumentoIngreso, FechaInicio, FechaFin, WorkflowDefinicion)

            ordenCargaInternaFason.Recorrido = recorrido;
        }
        private LogModificacionDocumentoIngresoCampo CrearLogModificacionDocumentoIngresoCampo(
        LogModificacionDocumentoIngreso logModificacionDocumentoIngreso,
        ModificarOrdenCargaInternaFason comando,
        string nombre,
        string valorOriginal,
        string valorNuevo, 
            DateTime fecha)
        {
            return new LogModificacionDocumentoIngresoCampo
            {
                Nombre = nombre,
                ValorOriginal = valorOriginal,
                ValorNuevo = valorNuevo,
                Fecha = fecha,
                NombreUsuario = comando.NombreUsuario,
                LogModificacionDocumentoIngreso = logModificacionDocumentoIngreso
            };
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
