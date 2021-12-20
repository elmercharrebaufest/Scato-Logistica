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
    public class ProcesadorModificarInhabilitacionCamion : ProcesadorModificar<ModificarInhabilitacionCamion>
    {
        public ProcesadorModificarInhabilitacionCamion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarInhabilitacionCamion comando)
        {
            var inhabilitacionCamion = Repositorio.Obtener<InhabilitacionCamion>(comando.Dto.Id);
            Repositorio.Agregar(new HistoricoInhabilitacionCamion
            {

                Comentario = comando.Dto.Comentario,
                Fecha = DateTime.Now,
                InhabilitacionCamion = inhabilitacionCamion,
                FechaDesde = inhabilitacionCamion.FechaDesde,
                FechaHasta = inhabilitacionCamion.FechaHasta,
                Motivo = inhabilitacionCamion.Motivo,
                NombreUsuarioResponsable = inhabilitacionCamion.NombreUsuarioResponsable,
                NombreUsuarioCambio = comando.Dto.NombreUsuarioResponsable
            });
            inhabilitacionCamion.Centro = inhabilitacionCamion.Centro;
            inhabilitacionCamion.FechaDesde = comando.Dto.FechaDesde;
            inhabilitacionCamion.FechaHasta = comando.Dto.FechaHasta;
            inhabilitacionCamion.Motivo = comando.Dto.Motivo;
            inhabilitacionCamion.Patente = comando.Dto.Patente;
            inhabilitacionCamion.NombreUsuarioResponsable = comando.Dto.NombreUsuarioResponsable;

            var adjuntosAnteriores =  inhabilitacionCamion.Adjuntos.Where(x => comando.Dto.Adjuntos.Any(t => t.Id == x.Id)).ToList();

            foreach(var archivoBorrado in Repositorio.Listar<Adjunto>(x => x.InhabilitacionCamion == null && x.InhabilitacionCamion == null))
            {
                Repositorio.Remover(archivoBorrado);
            }

            inhabilitacionCamion.Adjuntos.Clear();
            
            foreach(var archivo in adjuntosAnteriores)
            {
                inhabilitacionCamion.Adjuntos.Add(archivo);
            }

            foreach (var archivo in comando.Dto.Adjuntos.Where( x => !string.IsNullOrEmpty(x.Archivo) && x.Id == 0))
            {
                inhabilitacionCamion.Adjuntos.Add(Conversor.Convertir<AdjuntoDto, Adjunto>(archivo));
            }
        }

        protected override void Validar(ModificarInhabilitacionCamion comando, Resultado resultado)
        {
        }
    }
}
