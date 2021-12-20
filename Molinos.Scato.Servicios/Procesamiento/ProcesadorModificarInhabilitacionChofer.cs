using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarInhabilitacionChofer : ProcesadorModificar<ModificarInhabilitacionChofer>
    {
        public ProcesadorModificarInhabilitacionChofer(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarInhabilitacionChofer comando)
        {
            var chofer = Repositorio.Obtener<Chofer>(x => x.NumeroDeDocumento == comando.Dto.NumeroDeDocumento &&
                                                            x.TipoDocumentoIdentidad.Id == comando.Dto.TipoDocumentoIdentidadId);
            var inhabilitacionChofer = Repositorio.Obtener<InhabilitacionChofer>(comando.Dto.Id);

            Repositorio.Agregar(new HistoricoInhabilitacionChofer            {

                Comentario = comando.Dto.Comentario,
                Fecha = DateTime.Now,
                InhabilitacionChofer = inhabilitacionChofer,
                FechaDesde = inhabilitacionChofer.FechaDesde,
                FechaHasta = inhabilitacionChofer.FechaHasta,
                Motivo = inhabilitacionChofer.Motivo,
                NombreUsuarioResponsable = inhabilitacionChofer.NombreUsuarioResponsable,
                NombreUsuarioCambio = comando.Dto.NombreUsuarioResponsable
            });

            inhabilitacionChofer.Chofer = chofer;
            inhabilitacionChofer.FechaDesde = comando.Dto.FechaDesde;
            inhabilitacionChofer.FechaHasta = comando.Dto.FechaHasta;
            inhabilitacionChofer.Motivo = comando.Dto.Motivo;
            inhabilitacionChofer.Centro = inhabilitacionChofer.Centro;
            inhabilitacionChofer.NombreUsuarioResponsable = comando.Dto.NombreUsuarioResponsable;

            foreach (var archivoBorrado in Repositorio.Listar<Adjunto>(x => x.InhabilitacionCamion == null && x.InhabilitacionChofer == null))
            {
                Repositorio.Remover(archivoBorrado);
            }

            if (inhabilitacionChofer.Chofer.TipoDocumentoIdentidad.Id != comando.Dto.TipoDocumentoIdentidadId)
            {
                inhabilitacionChofer.Chofer.TipoDocumentoIdentidad =
                    Repositorio.Obtener<TipoDocumentoIdentidad>(comando.Dto.TipoDocumentoIdentidadId);
            }

            var adjuntosAnteriores = inhabilitacionChofer.Adjuntos.Where(x => comando.Dto.Adjuntos.Any(t => t.Id == x.Id)).ToList();
            inhabilitacionChofer.Adjuntos.Clear();
            foreach (var archivo in adjuntosAnteriores)
            {
                inhabilitacionChofer.Adjuntos.Add(archivo);
            }

            foreach (var archivo in comando.Dto.Adjuntos.Where(x => !string.IsNullOrEmpty(x.Archivo) && x.Id == 0))
            {
                inhabilitacionChofer.Adjuntos.Add(Conversor.Convertir<AdjuntoDto, Adjunto>(archivo));
            }
        }

        protected override void Validar(ModificarInhabilitacionChofer comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Chofer>(x => x.NumeroDeDocumento == comando.Dto.NumeroDeDocumento && x.TipoDocumentoIdentidad.Id == comando.Dto.TipoDocumentoIdentidadId))
            {
                resultado.Error("NumeroDeDocumento", Textos.InhabilitacionChofer_ChoferNoExiste);
            }
        }
    }
}
