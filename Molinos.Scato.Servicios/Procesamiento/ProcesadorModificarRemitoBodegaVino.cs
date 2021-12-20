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
    public class ProcesadorModificarRemitoBodegaVino : ProcesadorModificar<ModificarRemitoBodegaVino>
    {
        public ProcesadorModificarRemitoBodegaVino(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarRemitoBodegaVino comando)
        {
            var chofer = Repositorio.Obtener<Chofer>(comando.Orden.Chofer.Id);
            var material = Repositorio.Obtener<Material>(comando.Orden.MaterialId);
            var transportista = Repositorio.Obtener<Transportista>(comando.Orden.TransportistaId);
            var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Orden.TipoComercialId);
            var recorrido = Repositorio.Obtener<Recorrido>(comando.Orden.RecorridoId);

            recorrido.Patente = comando.Orden.Patente;
            recorrido.Chofer = chofer;
            recorrido.Material = material;
            recorrido.Transportista = transportista;
            recorrido.TipoComercial = tipoComercial;
            recorrido.NumeroDocumentoIngreso = comando.Orden.NroRemito;
            
            var remitoBodegaVino = Repositorio.Obtener<RemitoBodegaVino>(comando.Orden.Id);
            var listaCampos = new List<LogModificacionDocumentoIngresoCampo>();
            var logModificacionDocumento =
                Repositorio.Obtener<LogModificacionDocumentoIngreso>(
                    q => q.TipoDocumentoIngreso == TipoDocumentoIngreso.RemitoBodegaVino && q.Numero == comando.Orden.NroRemito);

            if (logModificacionDocumento == null)
            {
                logModificacionDocumento = new LogModificacionDocumentoIngreso
                    {
                        FechaUltimaModificacion = DateTime.Now,
                        NombreUsuarioUltimaModificacion = comando.NombreUsuario,
                        Numero = comando.Orden.NroRemito,
                        TipoDocumentoIngreso = TipoDocumentoIngreso.RemitoBodegaVino
                    };
                Repositorio.Agregar(logModificacionDocumento);
            }

            if (remitoBodegaVino.Chofer != chofer)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                    {
                        Nombre = "Chofer",
                        LogModificacionDocumentoIngreso = logModificacionDocumento,
                        ValorOriginal = remitoBodegaVino.Chofer.Nombre + " " + remitoBodegaVino.Chofer.Apellido,
                        ValorNuevo = chofer.Nombre + " " + chofer.Apellido,
                        NombreUsuario = comando.NombreUsuario,
                        Fecha = DateTime.Now
                        
                    });
                remitoBodegaVino.Chofer = chofer;
            }

            if (remitoBodegaVino.Material != material)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                    {
                        Nombre = "Material",
                        LogModificacionDocumentoIngreso = logModificacionDocumento,
                        ValorOriginal = remitoBodegaVino.Material.Descripcion,
                        ValorNuevo = material.Descripcion,
                        Fecha = DateTime.Now,
                        NombreUsuario = comando.NombreUsuario
                    });
                remitoBodegaVino.Material = material;
            }

            if (remitoBodegaVino.TipoComercial != tipoComercial)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                    {
                        Nombre = "Tipo Comercial",
                        LogModificacionDocumentoIngreso = logModificacionDocumento,
                        ValorOriginal = remitoBodegaVino.TipoComercial.Descripcion,
                        ValorNuevo = tipoComercial.Descripcion,
                        Fecha = DateTime.Now,
                        NombreUsuario = comando.NombreUsuario
                    });
                remitoBodegaVino.TipoComercial = tipoComercial;
            }

            if (remitoBodegaVino.Transportista != transportista)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                    {
                        Nombre = "Transportista",
                        LogModificacionDocumentoIngreso = logModificacionDocumento,
                        ValorOriginal = remitoBodegaVino.Transportista.RazonSocial,
                        ValorNuevo = transportista.RazonSocial,
                        Fecha = DateTime.Now,
                        NombreUsuario = comando.NombreUsuario
                    });
                remitoBodegaVino.Transportista = transportista;
            }

            if (remitoBodegaVino.Patente != comando.Orden.Patente)
            {
                listaCampos.Add(new LogModificacionDocumentoIngresoCampo
                    {
                        Nombre = "Patente",
                        LogModificacionDocumentoIngreso = logModificacionDocumento,
                        ValorOriginal = remitoBodegaVino.Patente,
                        ValorNuevo = comando.Orden.Patente,
                        Fecha = DateTime.Now,
                        NombreUsuario = comando.NombreUsuario
                    });
                remitoBodegaVino.Patente = comando.Orden.Patente;
            }

            foreach (var campo in listaCampos)
            {
                Repositorio.Agregar(campo);
            }

            remitoBodegaVino.PesoBrutoOrigen = comando.Orden.PesoBrutoOrigen ?? 0;
            remitoBodegaVino.PesoTaraOrigen = comando.Orden.PesoTaraOrigen ?? 0;
            remitoBodegaVino.PesoNetoOrigen = comando.Orden.PesoNetoOrigen ?? 0;
        }

        protected override void Validar(ModificarRemitoBodegaVino comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Chofer>(x => x.Id == comando.Orden.Chofer.Id))
            {
                resultado.Error("Chofer",String.Format(Textos.Error_Requerido,Textos.Chofer));
            }
            if (!Repositorio.Existe<Material>(x => x.Id == comando.Orden.MaterialId))
            {
                resultado.Error("MaterialId", String.Format(Textos.Error_Requerido, Textos.Material));
            }
            if (!Repositorio.Existe<Transportista>(x => x.Id == comando.Orden.TransportistaId))
            {
                resultado.Error("Transportista", String.Format(Textos.Error_Requerido, Textos.Transportista));
            }
            if (!Repositorio.Existe<TipoComercial>(x => x.Id == comando.Orden.TipoComercialId))
            {
              resultado.Error("TipoComercialId", String.Format(Textos.Error_Requerido,Textos.TipoComercial));
            }
        }
    }
}
