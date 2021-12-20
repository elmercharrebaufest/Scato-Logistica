using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearInhabilitacionCamion : ProcesadorCrear<CrearInhabilitacionCamion, InhabilitacionCamion>
    {
        public ProcesadorCrearInhabilitacionCamion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override InhabilitacionCamion CrearEntidad(CrearInhabilitacionCamion comando)
        {
            comando.Dto.FechaHasta = comando.Dto.FechaHasta.Add(new TimeSpan(0, 23, 59, 59));




            var camion = new InhabilitacionCamion
            {
                Patente = comando.Dto.Patente.ToUpper(),
                FechaDesde = comando.Dto.FechaDesde,
                FechaHasta = comando.Dto.FechaHasta,
                Motivo = comando.Dto.Motivo,
                Centro = Repositorio.Obtener<Centro>(x => x.Id == comando.Dto.CentroId),
                NombreUsuarioResponsable = comando.Dto.NombreUsuarioResponsable
            };
            camion.Adjuntos = new List<Adjunto>();
            if(comando.Dto.Adjuntos != null)
            {
                foreach (var archivo in comando.Dto.Adjuntos)
                {
                    camion.Adjuntos.Add(Conversor.Convertir<AdjuntoDto, Adjunto>(archivo));
                }
            }
            return camion;
        }

        protected override void Validar(CrearInhabilitacionCamion comando, Resultado resultado)
        {
        }
    }
}
